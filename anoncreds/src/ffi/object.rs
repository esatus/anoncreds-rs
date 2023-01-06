use std::any::TypeId;
use std::cmp::Eq;
use std::collections::{BTreeMap, HashMap};
use std::fmt::Debug;
use std::hash::{Hash, Hasher};
use std::ops::{Deref, DerefMut};
use std::os::raw::c_char;
use std::sync::{Arc, Mutex};

use ffi_support::{rust_string_to_c, ByteBuffer};
use once_cell::sync::Lazy;
use serde::Serialize;

use super::error::{catch_error, ErrorCode};
use crate::error::Result;

pub(crate) static FFI_OBJECTS: Lazy<Mutex<BTreeMap<ObjectHandle, AnonCredsObject>>> =
    Lazy::new(|| Mutex::new(BTreeMap::new()));

indy_utils::new_handle_type!(ObjectHandle, FFI_OBJECT_COUNTER);

impl ObjectHandle {
    pub(crate) fn create<O: AnyAnonCredsObject + 'static>(value: O) -> Result<Self> {
        let handle = Self::next();
        FFI_OBJECTS
            .lock()
            .map_err(|_| err_msg!("Error locking object store"))?
            .insert(handle, AnonCredsObject::new(value));
        Ok(handle)
    }

    pub(crate) fn load(&self) -> Result<AnonCredsObject> {
        FFI_OBJECTS
            .lock()
            .map_err(|_| err_msg!("Error locking object store"))?
            .get(self)
            .cloned()
            .ok_or_else(|| err_msg!("Invalid object handle"))
    }

    pub(crate) fn opt_load(&self) -> Result<Option<AnonCredsObject>> {
        if self.0 != 0 {
            Some(
                FFI_OBJECTS
                    .lock()
                    .map_err(|_| err_msg!("Error locking object store"))?
                    .get(self)
                    .cloned()
                    .ok_or_else(|| err_msg!("Invalid object handle")),
            )
            .transpose()
        } else {
            Ok(None)
        }
    }

    pub(crate) fn remove(&self) -> Result<AnonCredsObject> {
        FFI_OBJECTS
            .lock()
            .map_err(|_| err_msg!("Error locking object store"))?
            .remove(self)
            .ok_or_else(|| err_msg!("Invalid object handle"))
    }
}

#[derive(Clone, Debug)]
#[repr(transparent)]
pub(crate) struct AnonCredsObject(Arc<dyn AnyAnonCredsObject>);

impl AnonCredsObject {
    pub fn new<O: AnyAnonCredsObject + 'static>(value: O) -> Self {
        Self(Arc::new(value))
    }

    pub fn cast_ref<O: AnyAnonCredsObject + 'static>(&self) -> Result<&O> {
        let result = unsafe { &*(&*self.0 as *const _ as *const O) };
        if self.0.type_id() == TypeId::of::<O>() {
            Ok(result)
        } else {
            Err(err_msg!(
                "Expected {} instance, received {}",
                result.type_name(),
                self.0.type_name()
            ))
        }
    }

    pub fn type_name(&self) -> &'static str {
        self.0.type_name()
    }
}

impl Hash for AnonCredsObject {
    fn hash<H: Hasher>(&self, state: &mut H) {
        std::ptr::hash(&*self.0, state);
    }
}

pub(crate) trait ToJson {
    fn to_json(&self) -> Result<Vec<u8>>;
}

impl ToJson for AnonCredsObject {
    #[inline]
    fn to_json(&self) -> Result<Vec<u8>> {
        self.0.to_json()
    }
}

impl<T> ToJson for T
where
    T: Serialize,
{
    fn to_json(&self) -> Result<Vec<u8>> {
        serde_json::to_vec(self).map_err(err_map!("Error serializing object"))
    }
}

pub(crate) trait AnyAnonCredsObject: Debug + ToJson + Send + Sync {
    fn type_name(&self) -> &'static str;

    #[doc(hidden)]
    fn type_id(&self) -> TypeId
    where
        Self: 'static,
    {
        TypeId::of::<Self>()
    }
}

macro_rules! impl_anoncreds_object {
    ($ident:path, $name:expr) => {
        impl $crate::ffi::object::AnyAnonCredsObject for $ident {
            fn type_name(&self) -> &'static str {
                $name
            }
        }
    };
}

macro_rules! impl_anoncreds_object_from_json {
    ($ident:path, $method:ident) => {
        #[no_mangle]
        pub extern "C" fn $method(
            json: ffi_support::ByteBuffer,
            result_p: *mut $crate::ffi::object::ObjectHandle,
        ) -> $crate::ffi::error::ErrorCode {
            $crate::ffi::error::catch_error(|| {
                check_useful_c_ptr!(result_p);
                let obj = serde_json::from_slice::<$ident>(json.as_slice())?;
                let handle = $crate::ffi::object::ObjectHandle::create(obj)?;
                unsafe { *result_p = handle };
                Ok(())
            })
        }
    };
}

#[no_mangle]
pub extern "C" fn anoncreds_object_get_json(
    handle: ObjectHandle,
    result_p: *mut ByteBuffer,
) -> ErrorCode {
    catch_error(|| {
        check_useful_c_ptr!(result_p);
        let obj = handle.load()?;
        let json = obj.to_json()?;
        unsafe { *result_p = ByteBuffer::from_vec(json) };
        Ok(())
    })
}

#[no_mangle]
pub extern "C" fn anoncreds_object_get_type_name(
    handle: ObjectHandle,
    result_p: *mut *const c_char,
) -> ErrorCode {
    catch_error(|| {
        check_useful_c_ptr!(result_p);
        let obj = handle.load()?;
        let name = obj.type_name();
        unsafe { *result_p = rust_string_to_c(name) };
        Ok(())
    })
}

#[no_mangle]
pub extern "C" fn anoncreds_object_free(handle: ObjectHandle) {
    handle.remove().ok();
}

pub(crate) trait AnonCredsObjectId: AnyAnonCredsObject {
    type Id: Eq + Hash;

    fn get_id(&self) -> Self::Id;
}

#[repr(transparent)]
pub(crate) struct AnonCredsObjectList(Vec<AnonCredsObject>);

impl AnonCredsObjectList {
    pub fn load(handles: &[ObjectHandle]) -> Result<Self> {
        let loaded = handles
            .iter()
            .map(ObjectHandle::load)
            .collect::<Result<_>>()?;
        Ok(Self(loaded))
    }

    #[allow(unused)]
    pub fn refs<T>(&self) -> Result<Vec<&T>>
    where
        T: AnyAnonCredsObject + 'static,
    {
        let mut refs = Vec::with_capacity(self.0.len());
        for inst in self.0.iter() {
            let inst = inst.cast_ref::<T>()?;
            refs.push(inst);
        }
        Ok(refs)
    }

    pub fn refs_map<'a, I, T>(&'a self, ids: &'a [I]) -> Result<HashMap<&I, &T>>
    where
        T: AnyAnonCredsObject + 'static,
        I: Eq + Hash,
    {
        let mut refs = HashMap::with_capacity(self.0.len());
        for (inst, id) in self.0.iter().zip(ids) {
            let inst = inst.cast_ref::<T>()?;
            refs.insert(id, inst);
        }
        Ok(refs)
    }
}

impl Deref for AnonCredsObjectList {
    type Target = Vec<AnonCredsObject>;

    fn deref(&self) -> &Self::Target {
        &self.0
    }
}

impl DerefMut for AnonCredsObjectList {
    fn deref_mut(&mut self) -> &mut Self::Target {
        &mut self.0
    }
}
