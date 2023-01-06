#[cfg(feature = "ed25519")]
use curve25519_dalek::edwards::CompressedEdwardsY;
#[cfg(feature = "ed25519")]
use ed25519_dalek::{ExpandedSecretKey, PublicKey, SecretKey, Signature};
#[cfg(feature = "ed25519")]
use rand::{thread_rng, RngCore};
#[cfg(feature = "ed25519")]
use sha2::digest::Digest;

#[cfg(feature = "ed25519")]
use std::convert::TryFrom;

use zeroize::Zeroize;

use super::base58;
use super::error::ConversionError;
use super::{Validatable, ValidationError};

mod types;
pub use types::{KeyEncoding, KeyType};

/// Build an encoded verkey
pub fn build_full_verkey(dest: &str, key: &str) -> Result<EncodedVerKey, ConversionError> {
    EncodedVerKey::from_str_qualified(key, Some(dest), None, None)
}

/// A raw signing key used for generating transaction signatures
#[derive(Clone, Debug, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub struct PrivateKey {
    pub key: Vec<u8>,
    pub alg: KeyType,
}

impl PrivateKey {
    pub fn new<K: AsRef<[u8]>>(key: K, alg: Option<KeyType>) -> Self {
        Self {
            key: key.as_ref().to_vec(),
            alg: alg.unwrap_or_default(),
        }
    }

    #[cfg(feature = "ed25519")]
    pub fn generate(alg: Option<KeyType>) -> Result<Self, ConversionError> {
        let alg = alg.unwrap_or_default();
        match alg {
            KeyType::ED25519 => {
                let mut sk = [0u8; 32];
                thread_rng().fill_bytes(&mut sk[..]);
                Self::from_seed(&sk[..])
            }
            _ => Err("Unsupported key type".into()),
        }
    }

    #[cfg(feature = "ed25519")]
    pub fn from_seed(seed: &[u8]) -> Result<Self, ConversionError> {
        let sk = SecretKey::from_bytes(seed)
            .map_err(|err| format!("Error creating signing key: {err}"))?;
        let mut esk = [0u8; 64];
        esk[..32].copy_from_slice(sk.as_bytes());
        esk[32..].copy_from_slice(PublicKey::from(&sk).as_bytes());
        Ok(Self::new(esk, Some(KeyType::ED25519)))
    }

    pub fn public_key(&self) -> Result<VerKey, ConversionError> {
        match self.alg {
            KeyType::ED25519 => Ok(VerKey::new(&self.key[32..], Some(self.alg.clone()))),
            _ => Err("Unsupported key type".into()),
        }
    }

    pub fn key_bytes(&self) -> Vec<u8> {
        self.key.clone()
    }

    #[cfg(feature = "ed25519")]
    pub fn key_exchange(&self) -> Result<Self, ConversionError> {
        match self.alg {
            KeyType::ED25519 => {
                let mut hash = sha2::Sha512::digest(&self.key[..32]);
                let x_sk =
                    x25519_dalek::StaticSecret::from(<[u8; 32]>::try_from(&hash[..32]).unwrap());
                hash.zeroize();
                Ok(Self::new(x_sk.to_bytes(), Some(KeyType::X25519)))
            }
            _ => Err("Unsupported key format for key exchange".into()),
        }
    }

    #[cfg(feature = "ed25519")]
    pub fn sign<M: AsRef<[u8]>>(&self, message: M) -> Result<Vec<u8>, ConversionError> {
        match self.alg {
            KeyType::ED25519 => {
                let esk = ExpandedSecretKey::from(&SecretKey::from_bytes(&self.key[..32]).unwrap());
                let pk = PublicKey::from_bytes(&self.key[32..]).unwrap();
                let sig = esk.sign(message.as_ref(), &pk);
                Ok(sig.to_bytes().into())
            }
            _ => Err("Unsupported key format for signing".into()),
        }
    }
}

impl AsRef<[u8]> for PrivateKey {
    fn as_ref(&self) -> &[u8] {
        self.key.as_ref()
    }
}

impl Zeroize for PrivateKey {
    fn zeroize(&mut self) {
        self.key.zeroize();
        self.alg = KeyType::from("")
    }
}

impl Drop for PrivateKey {
    fn drop(&mut self) {
        self.zeroize()
    }
}

impl Validatable for PrivateKey {
    fn validate(&self) -> Result<(), ValidationError> {
        if self.alg == KeyType::ED25519 {
            if self.key.len() == 64 {
                Ok(())
            } else {
                Err("Invalid signing key length".into())
            }
        } else {
            Err("Unsupported signing key type".into())
        }
    }
}

/// A raw verkey used in verifying signatures
#[derive(Clone, Debug, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub struct VerKey {
    pub key: Vec<u8>,
    pub alg: KeyType,
}

impl VerKey {
    pub fn new<K: AsRef<[u8]>>(key: K, alg: Option<KeyType>) -> Self {
        let alg = alg.unwrap_or_default();
        Self {
            key: key.as_ref().to_vec(),
            alg,
        }
    }

    pub fn as_base58(&self) -> Result<EncodedVerKey, ConversionError> {
        self.encode(&KeyEncoding::BASE58)
    }

    pub fn encode(&self, enc: &KeyEncoding) -> Result<EncodedVerKey, ConversionError> {
        match enc {
            KeyEncoding::BASE58 => {
                let key = base58::encode(&self.key);
                Ok(EncodedVerKey::new(
                    key.as_str(),
                    Some(self.alg.clone()),
                    Some(enc.clone()),
                ))
            }
            _ => Err("Unsupported key encoding".into()),
        }
    }

    pub fn key_bytes(&self) -> Vec<u8> {
        self.key.clone()
    }

    #[cfg(feature = "ed25519")]
    pub fn key_exchange(&self) -> Result<Self, ConversionError> {
        match self.alg {
            KeyType::ED25519 => {
                let vky = CompressedEdwardsY::from_slice(&self.key[..]);
                if let Some(x_vk) = vky.decompress() {
                    Ok(Self::new(
                        x_vk.to_montgomery().as_bytes(),
                        Some(KeyType::X25519),
                    ))
                } else {
                    Err("Error converting to x25519 key".into())
                }
            }
            _ => Err("Unsupported verkey type".into()),
        }
    }

    #[cfg(feature = "ed25519")]
    pub fn verify_signature<M: AsRef<[u8]>, S: AsRef<[u8]>>(
        &self,
        message: M,
        signature: S,
    ) -> Result<bool, ConversionError> {
        match self.alg {
            KeyType::ED25519 => {
                let vk = PublicKey::from_bytes(&self.key[..]).unwrap();
                if let Ok(sig) = Signature::try_from(signature.as_ref()) {
                    Ok(vk.verify_strict(message.as_ref(), &sig).is_ok())
                } else {
                    Err("Error validating message signature".into())
                }
            }
            _ => Err("Unsupported verkey type".into()),
        }
    }
}

impl AsRef<[u8]> for VerKey {
    fn as_ref(&self) -> &[u8] {
        self.key.as_ref()
    }
}

impl std::fmt::Display for VerKey {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match &self.as_base58() {
            Ok(k) => k.fmt(f),
            Err(err) => write!(f, "<Error encoding key: {err}>"),
        }
    }
}

impl Validatable for VerKey {
    fn validate(&self) -> Result<(), ValidationError> {
        if self.alg == KeyType::ED25519 {
            let bytes = self.key_bytes();
            if bytes.len() == 32 {
                Ok(())
            } else {
                Err("Invalid verkey length".into())
            }
        } else {
            Err("Unsupported verkey type".into())
        }
    }
}

impl Zeroize for VerKey {
    fn zeroize(&mut self) {
        self.key.zeroize();
        self.alg = KeyType::from("");
    }
}

impl Drop for VerKey {
    fn drop(&mut self) {
        self.zeroize()
    }
}

/// An encoded verkey appropriate for storing and transmitting
#[derive(Clone, Debug, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub struct EncodedVerKey {
    pub key: String,
    pub alg: KeyType,
    pub enc: KeyEncoding,
}

impl EncodedVerKey {
    pub fn new<K: AsRef<str>>(key: K, alg: Option<KeyType>, enc: Option<KeyEncoding>) -> Self {
        let alg = alg.unwrap_or_default();
        let enc = enc.unwrap_or_default();
        Self {
            key: key.as_ref().to_owned(),
            alg,
            enc,
        }
    }

    pub fn from_did_and_verkey(did: &str, key: &str) -> Result<Self, ConversionError> {
        if let Some(stripped) = key.strip_prefix('~') {
            let mut vk_bytes = base58::decode(stripped)?;
            if vk_bytes.len() != 16 {
                return Err("Expected 16-byte abbreviated verkey".into());
            }
            let mut did_bytes = base58::decode(did)?;
            if did_bytes.len() != 16 {
                return Err("DID must be 16 bytes in length".into());
            }
            did_bytes.append(&mut vk_bytes);
            Ok(Self::new(
                base58::encode(did_bytes),
                Some(KeyType::ED25519),
                Some(KeyEncoding::BASE58),
            ))
        } else {
            Ok(Self::new(
                key,
                Some(KeyType::ED25519),
                Some(KeyEncoding::BASE58),
            ))
        }
    }

    pub fn abbreviated_for_did(&self, did: &str) -> Result<String, ConversionError> {
        let did_bytes = base58::decode(did)?;
        if did_bytes.len() != 16 {
            return Err("DID must be 16 bytes in length".into());
        }
        let vk = self.key_bytes()?;
        if vk.len() != 32 {
            return Err("Expected 32-byte verkey".into());
        }
        if &vk[..16] == did_bytes.as_slice() {
            let mut result = "~".to_string();
            result.push_str(&base58::encode(&vk[16..]));
            Ok(result)
        } else {
            Ok(base58::encode(vk))
        }
    }

    pub fn decode(&self) -> Result<VerKey, ConversionError> {
        let mut vk = self.key_bytes()?;
        let result = VerKey::new(&vk, Some(self.alg.clone()));
        vk.zeroize();
        Ok(result)
    }

    pub fn from_slice<K: AsRef<[u8]>>(key: K) -> Result<Self, ConversionError> {
        let key = std::str::from_utf8(key.as_ref())?;
        Self::from_str_qualified(key, None, None, None)
    }

    pub fn from_string(key: impl Into<String>) -> Result<Self, ConversionError> {
        Self::from_str_qualified(key.into().as_str(), None, None, None)
    }

    pub fn from_str_qualified(
        key: &str,
        dest: Option<&str>,
        alg: Option<KeyType>,
        enc: Option<KeyEncoding>,
    ) -> Result<Self, ConversionError> {
        let (key, alg) = if key.contains(':') {
            let splits: Vec<&str> = key.splitn(2, ':').collect();
            let alg = match splits[1] {
                "" => alg,
                _ => Some(splits[1].into()),
            };
            (splits[0], alg)
        } else {
            (key, alg)
        };

        if let Some(stripped) = key.strip_prefix('~') {
            let dest =
                unwrap_opt_or_return!(dest, Err("Destination required for short verkey".into()));
            let mut result = base58::decode(dest)?;
            let mut end = base58::decode(stripped)?;
            result.append(&mut end);
            Ok(Self::new(base58::encode(result), alg, enc))
        } else {
            Ok(Self::new(key, alg, enc))
        }
    }

    pub fn long_form(&self) -> String {
        let mut result = self.key.clone();
        result.push(':');
        result.push_str(&self.alg);
        result
    }

    pub fn as_base58(self) -> Result<Self, ConversionError> {
        match self.enc {
            KeyEncoding::BASE58 => Ok(self),
            _ => {
                let key = base58::encode(self.key_bytes()?);
                Ok(Self::new(
                    key.as_str(),
                    Some(self.alg.clone()),
                    Some(KeyEncoding::BASE58),
                ))
            }
        }
    }

    pub fn key_bytes(&self) -> Result<Vec<u8>, ConversionError> {
        match self.enc {
            KeyEncoding::BASE58 => Ok(base58::decode(&self.key)?),
            _ => Err("Unsupported verkey encoding".into()),
        }
    }

    pub fn encoded_key_bytes(&self) -> &[u8] {
        self.key.as_bytes()
    }

    #[cfg(feature = "ed25519")]
    pub fn key_exchange(&self) -> Result<VerKey, ConversionError> {
        let vk = self.decode()?;
        vk.key_exchange()
    }

    #[cfg(feature = "ed25519")]
    pub fn key_exchange_encoded(&self) -> Result<Self, ConversionError> {
        let x_vk = self.key_exchange()?;
        x_vk.encode(&self.enc)
    }

    #[cfg(feature = "ed25519")]
    pub fn verify_signature<M: AsRef<[u8]>, S: AsRef<[u8]>>(
        &self,
        message: M,
        signature: S,
    ) -> Result<bool, ConversionError> {
        let vk = self.decode()?;
        vk.verify_signature(message, signature)
    }
}

impl std::fmt::Display for EncodedVerKey {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        let out = if self.alg == KeyType::default() {
            self.key.clone()
        } else {
            self.long_form()
        };
        f.write_str(out.as_str())
    }
}

impl Validatable for EncodedVerKey {
    fn validate(&self) -> Result<(), ValidationError> {
        let verkey = self.decode()?;
        verkey.validate()
    }
}

impl Zeroize for EncodedVerKey {
    fn zeroize(&mut self) {
        self.key.zeroize();
        self.alg = KeyType::from("");
        self.enc = KeyEncoding::from("")
    }
}

impl Drop for EncodedVerKey {
    fn drop(&mut self) {
        self.zeroize()
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn from_str_empty() {
        assert_eq!(
            EncodedVerKey::from_string("").unwrap(),
            EncodedVerKey::new("", Some(KeyType::default()), Some(KeyEncoding::default()))
        )
    }

    #[test]
    fn from_str_single_colon() {
        assert_eq!(
            EncodedVerKey::from_string(":").unwrap(),
            EncodedVerKey::new("", Some(KeyType::default()), Some(KeyEncoding::default()))
        )
    }

    #[test]
    fn from_str_ends_with_colon() {
        assert_eq!(
            EncodedVerKey::from_string("foo:").unwrap(),
            EncodedVerKey::new(
                "foo",
                Some(KeyType::default()),
                Some(KeyEncoding::default())
            )
        )
    }

    #[test]
    fn from_key_starts_with_colon() {
        assert_eq!(
            EncodedVerKey::from_string(":bar").unwrap(),
            EncodedVerKey::new("", Some("bar".into()), Some(KeyEncoding::default()))
        )
    }

    #[test]
    fn from_key_works() {
        assert_eq!(
            EncodedVerKey::from_string("foo:bar:baz").unwrap(),
            EncodedVerKey::new("foo", Some("bar:baz".into()), Some(KeyEncoding::default()))
        )
    }

    #[test]
    fn round_trip_verkey() {
        assert_eq!(
            EncodedVerKey::from_string("foo:bar:baz")
                .unwrap()
                .long_form(),
            "foo:bar:baz"
        )
    }

    #[cfg(feature = "ed25519")]
    #[test]
    fn key_from_seed() {
        const SEED: &[u8; 32] = b"aaaabbbbccccddddeeeeffffgggghhhh";
        let sk = PrivateKey::from_seed(&SEED[..]).unwrap();
        assert_eq!(
            &sk.as_ref()[..],
            &[
                97, 97, 97, 97, 98, 98, 98, 98, 99, 99, 99, 99, 100, 100, 100, 100, 101, 101, 101,
                101, 102, 102, 102, 102, 103, 103, 103, 103, 104, 104, 104, 104, 113, 22, 13, 44,
                71, 184, 166, 148, 196, 234, 85, 148, 234, 22, 204, 148, 187, 247, 77, 119, 250,
                28, 37, 255, 29, 31, 159, 159, 245, 68, 107, 235
            ]
        );
        let xk = sk.key_exchange().unwrap();
        assert_eq!(
            &xk.as_ref(),
            &[
                208, 235, 232, 147, 241, 214, 250, 182, 45, 157, 20, 202, 31, 184, 226, 115, 149,
                82, 210, 89, 50, 100, 22, 67, 21, 8, 124, 198, 100, 252, 237, 107
            ]
        );
    }

    #[cfg(feature = "ed25519")]
    #[test]
    fn sign_and_verify() {
        let message = b"hello there";
        let sk = PrivateKey::generate(None).unwrap();
        let sig = sk.sign(&message).unwrap();
        let vk = sk.public_key().unwrap();
        assert!(vk.verify_signature(&message, &sig).unwrap());
        assert!(vk.verify_signature(&message, &[]).is_err());
        assert!(!vk.verify_signature(&"goodbye", &sig).unwrap());
    }

    #[cfg(feature = "ed25519")]
    #[test]
    fn validate_keys() {
        let sk = PrivateKey::generate(None).unwrap();
        sk.validate().unwrap();
        let vk = sk.public_key().unwrap();
        vk.validate().unwrap();

        let sk = PrivateKey::new(b"bad key", Some(KeyType::ED25519));
        assert_eq!(sk.validate().is_ok(), false);
        let vk = VerKey::new(b"bad key", Some(KeyType::ED25519));
        assert_eq!(vk.validate().is_ok(), false);
    }
}
