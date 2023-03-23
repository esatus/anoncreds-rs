using anoncreds_rs_dotnet.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static anoncreds_rs_dotnet.Models.Structures;

namespace anoncreds_rs_dotnet.Anoncreds
{
    public static class RevocationApi
    {
        public static async Task<RevocationStatusList> CreateRevocationStatusListAsync(
            string revRegDefId,
            RevocationRegistryDefinition revRegDefObject,
            string issuerId,
            long timestamp,
            IssuerType issuanceType//byte issuanceByDefault
            )
        {
            IntPtr revStatusListObjectHandle = new IntPtr();

            int errorCode = NativeMethods.anoncreds_create_revocation_status_list(
                FfiStr.Create(revRegDefId),
                revRegDefObject.Handle,
                FfiStr.Create(issuerId),
                timestamp,
                issuanceType.Equals(IssuerType.ISSUANCE_BY_DEFAULT) ? Convert.ToByte(true) : Convert.ToByte(false),
                ref revStatusListObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            RevocationStatusList revStatusListObject = await CreateRevocationStatusListObject(revStatusListObjectHandle);
            return await Task.FromResult(revStatusListObject);
        }

        public static async Task<string> CreateRevocationStatusListJsonAsync(
            string revRegDefId,
            string revRegDefJson,
            string issuerId,
            long timestamp,
            IssuerType issuanceType//byte issuanceByDefault
            )
        {
            IntPtr revStatusListObjectHandle = new IntPtr();
            IntPtr revRegDefObjectHandle = new IntPtr();

            _ = NativeMethods.anoncreds_revocation_registry_definition_from_json(ByteBuffer.Create(revRegDefJson), ref revRegDefObjectHandle);

            int errorCode = NativeMethods.anoncreds_create_revocation_status_list(
                FfiStr.Create(revRegDefId),
                revRegDefObjectHandle,
                FfiStr.Create(issuerId),
                timestamp,
                issuanceType.Equals(IssuerType.ISSUANCE_BY_DEFAULT) ? Convert.ToByte(true) : Convert.ToByte(false),
                ref revStatusListObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            string revStatusListJson = await ObjectApi.ToJsonAsync(revStatusListObjectHandle);
            return await Task.FromResult(revStatusListJson);
        }

        public static async Task<RevocationStatusList> UpdateRevocationStatusListAsync(
            long timestamp,
            List<long> issued, //i32
            List<long> revoked,
            RevocationRegistryDefinition revRegDefObject,
            RevocationStatusList currentRevStatusListObject
            )
        {
            IntPtr newRevStatusListObjectHandle = new IntPtr();

            int errorCode = NativeMethods.anoncreds_update_revocation_status_list(
                timestamp,
                FfiLongList.Create(issued),
                FfiLongList.Create(revoked),
                revRegDefObject.Handle,
                currentRevStatusListObject.Handle,
                ref newRevStatusListObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            RevocationStatusList newRevStatusListObject = await CreateRevocationStatusListObject(newRevStatusListObjectHandle);
            return await Task.FromResult(newRevStatusListObject);
        }

        public static async Task<string> UpdateRevocationStatusListJsonAsync(
            long timestamp,
            List<long> issued,
            List<long> revoked,
            string revRegDefJson,
            string currentRevStatusListJson
            )
        {
            IntPtr revRegDefObjectHandle = new IntPtr();
            IntPtr revStatusListObjectHandle = new IntPtr();
            IntPtr newRevStatusListObjectHandle = new IntPtr();

            _ = NativeMethods.anoncreds_revocation_registry_definition_from_json(ByteBuffer.Create(revRegDefJson), ref revRegDefObjectHandle);

            _ = NativeMethods.anoncreds_revocation_list_from_json(ByteBuffer.Create(currentRevStatusListJson), ref revStatusListObjectHandle);

            int errorCode = NativeMethods.anoncreds_update_revocation_status_list(
                timestamp,
                FfiLongList.Create(issued),
                FfiLongList.Create(revoked),
                revRegDefObjectHandle,
                revStatusListObjectHandle,
                ref newRevStatusListObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            string newRevStatusListJson = await ObjectApi.ToJsonAsync(newRevStatusListObjectHandle);
            return await Task.FromResult(newRevStatusListJson);
        }

        public static async Task<RevocationStatusList> UpdateRevocationStatusListTimestampOnlyAsync(
            long timestamp,
            RevocationStatusList currentRevStatusListObject)
        {
            IntPtr newRevStatusListObjectHandle = new IntPtr();

            int errorCode = NativeMethods.anoncreds_update_revocation_status_list_timestamp_only(
                timestamp,
                currentRevStatusListObject.Handle,
                ref newRevStatusListObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            RevocationStatusList newRevStatusListObject = await CreateRevocationStatusListObject(newRevStatusListObjectHandle);
            return await Task.FromResult(newRevStatusListObject);
        }

        /// <summary>
        /// Creates a new <see cref="RevocationRegistryDefinition"/> object and its corresponding informative objects.
        /// </summary>
        /// <param name="originDid">Did of issuer.</param>
        /// <param name="credDefObject">Credential definition.</param>
        /// <param name="tag">Tag.</param>
        /// <param name="revRegType">Type of revocation registry.</param>
        /// <param name="maxCredNumber">Maximum number of credential entries.</param>
        /// <param name="tailsDirPath">Path to tails file.</param>
        /// <exception cref="AnoncredsRsException">Throws if any parameter is invalid.</exception>
        /// <returns>New <see cref="RevocationRegistryDefinition"/> and <see cref="RevocationRegistryDefinitionPrivate"/> objects.</returns>
        public static async Task<(RevocationRegistryDefinition, RevocationRegistryDefinitionPrivate)> CreateRevocationRegistryDefinitionAsync(
            string originDid,
            CredentialDefinition credDefObject,
            string tag,
            RegistryType revRegType,
            long maxCredNumber,
            string tailsDirPath)
        {
            IntPtr regDefObjectHandle = new IntPtr();
            IntPtr regDefPvtObjectHandle = new IntPtr();

            int errorCode = NativeMethods.anoncreds_create_revocation_registry_def(
                credDefObject.Handle,
                FfiStr.Create(credDefObject.IssuerId),
                FfiStr.Create(originDid),
                FfiStr.Create(tag),
                FfiStr.Create(revRegType.ToString()),
                maxCredNumber,
                FfiStr.Create(tailsDirPath),
                ref regDefObjectHandle,
                ref regDefPvtObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }
            RevocationRegistryDefinition regDefObject = await CreateRevocationRegistryDefinitionObject(regDefObjectHandle);
            RevocationRegistryDefinitionPrivate regDefPvtObject = await CreateRevocationRegistryDefinitionPrivateObject(regDefPvtObjectHandle);

            return await Task.FromResult((regDefObject, regDefPvtObject));
        }

        /// <summary>
        /// Creates a new <see cref="RevocationRegistryDefinition"/> object and its corresponding informative objects as JSON strings.
        /// </summary>
        /// <param name="originDid">Did of issuer.</param>
        /// <param name="credDefJson">Credential definition as JSON string.</param>
        /// <param name="tag">Tag.</param>
        /// <param name="revRegType">Type of revocation registry.</param>
        /// <param name="maxCredNumber">Maximum number of credential entries.</param>
        /// <param name="tailsDirPath">Path to tails file.</param>
        /// <exception cref="AnoncredsRsException">Throws if any parameter is invalid.</exception>
        /// <returns>New <see cref="RevocationRegistryDefinition"/> and <see cref="RevocationRegistryDefinitionPrivate"/> as JSON strings.</returns>
        public static async Task<(string, string)> CreateRevocationRegistryDefinitionJsonAsync(
            string originDid,
            string credDefJson,
            string tag,
            RegistryType revRegType,
            long maxCredNumber,
            string tailsDirPath)
        {
            IntPtr regDefObjectHandle = new IntPtr();
            IntPtr regDefPvtObjectHandle = new IntPtr();
            IntPtr credDefObjectHandle = new IntPtr();

            _ = NativeMethods.anoncreds_credential_definition_from_json(ByteBuffer.Create(credDefJson), ref credDefObjectHandle);
            CredentialDefinition credDefObject = 
                JsonConvert.DeserializeObject<CredentialDefinition>(credDefJson, Settings.JsonSettings);

            int errorCode = NativeMethods.anoncreds_create_revocation_registry_def(
                credDefObjectHandle,
                FfiStr.Create(credDefObject.IssuerId),
                FfiStr.Create(originDid),
                FfiStr.Create(tag),
                FfiStr.Create(revRegType.ToString()),
                maxCredNumber,
                FfiStr.Create(tailsDirPath),
                ref regDefObjectHandle,
                ref regDefPvtObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }
            string regDefJson = await ObjectApi.ToJsonAsync(regDefObjectHandle);
            string regDefPvtJson = await ObjectApi.ToJsonAsync(regDefPvtObjectHandle);

            return await Task.FromResult((regDefJson, regDefPvtJson));
        }

        /// <summary>
        /// Creates a new <see cref="RevocationRegistry"/> object from a JSON string.
        /// </summary>
        /// <param name="revRegJson">JSON string representing a RevocationRegistry.</param>
        /// <exception cref="AnoncredsRsException">Throws when provided <paramref name="revRegJson"/> is invalid.</exception>
        /// <exception cref="System.IndexOutOfRangeException">Throws when provided <paramref name="revRegJson"/> is empty.</exception>
        /// <returns>A new <see cref="RevocationRegistry"/> object.</returns>
        public static async Task<RevocationRegistry> CreateRevocationRegistryFromJsonAsync(string revRegJson)
        {
            IntPtr regEntryObjectHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_revocation_registry_from_json(ByteBuffer.Create(revRegJson), ref regEntryObjectHandle);
            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            RevocationRegistry revRegObject = await CreateRevocationRegistryObject(regEntryObjectHandle);
            return await Task.FromResult(revRegObject);
        }

        /// <summary>
        /// Creates a new <see cref="RevocationRegistryDefinition"/> object from a JSON string.
        /// </summary>
        /// <param name="revRegDefJson">JSON string representing a RevocationRegistryDefinition.</param>
        /// <exception cref="AnoncredsRsException">Throws when provided <paramref name="revRegDefJson"/> is invalid.</exception>
        /// <exception cref="System.IndexOutOfRangeException">Throws when provided <paramref name="revRegDefJson"/> is empty.</exception>
        /// <returns>A new <see cref="RevocationRegistryDefinition"/> object.</returns>
        public static async Task<RevocationRegistryDefinition> CreateRevocationRegistryDefinitionFromJsonAsync(string revRegDefJson)
        {
            IntPtr revRegDefObjectHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_revocation_registry_definition_from_json(ByteBuffer.Create(revRegDefJson), ref revRegDefObjectHandle);
            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            RevocationRegistryDefinition revRegDefObject = await CreateRevocationRegistryDefinitionObject(revRegDefObjectHandle);
            return await Task.FromResult(revRegDefObject);
        }

        /// <summary>
        /// Updates the provided <see cref="CredentialRevocationState"/> or creates a new one.
        /// </summary>
        /// <param name="revRegDef">The revocation registry definition.</param>
        /// <param name="newRevStatusList">The new revocation status list.</param>
        /// <param name="revRegIndex">The revocation registry index.</param>
        /// <param name="tailsPath">Path to the tails file.</param>
        /// <param name="revState">Revocation state to update. Default null.</param>
        /// <param name="oldRevStatusList">The old revocation status list. Default null.</param>
        /// <exception cref="AnoncredsRsException">Throws if any parameter is invalid.</exception>
        /// <returns>A <see cref="CredentialRevocationState"/> object.</returns>
        public static async Task<CredentialRevocationState> CreateOrUpdateRevocationStateAsync(
            RevocationRegistryDefinition revRegDef,
            RevocationStatusList newRevStatusList,
            long revRegIndex,
            string tailsPath,
            CredentialRevocationState revState = null,
            RevocationStatusList oldRevStatusList = null)
        {
            IntPtr credRevStateObjectHandle = new IntPtr();

            int errorCode = NativeMethods.anoncreds_create_or_update_revocation_state(
                revRegDef.Handle,
                newRevStatusList.Handle,
                revRegIndex,
                FfiStr.Create(tailsPath),
                revState == null ? new IntPtr() : revState.Handle,
                oldRevStatusList == null ? new IntPtr() : oldRevStatusList.Handle,
                ref credRevStateObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            CredentialRevocationState credRevStateObject = await CreateCredentialRevocationStateObject(credRevStateObjectHandle);

            return await Task.FromResult(credRevStateObject);
        }

        /// <summary>
        /// Updates the provided <see cref="CredentialRevocationState"/> JSON string or creates a new one.
        /// </summary>
        /// <param name="revRegDefJson">The revocation registry definition as JSON string.</param>
        /// <param name="newRevStatusListJson">The new revocation status list as JSON string.</param>
        /// <param name="revRegIndex">The revocation registry index.</param>
        /// <param name="tailsPath">Path to the tails file.</param>
        /// <param name="revStateJson">Revocation state to update as JSON string. Default null.</param>
        /// <param name="oldRevStatusListJson">The old revocation status list as JSON string. Default null.</param>
        /// <exception cref="AnoncredsRsException">Throws if any parameter is invalid.</exception>
        /// <returns>A <see cref="CredentialRevocationState"/>  as JSON string.</returns>
        public static async Task<string> CreateOrUpdateRevocationStateAsync(
            string revRegDefJson,
            string newRevStatusListJson,
            long revRegIndex,
            string tailsPath,
            string revStateJson = null,
            string oldRevStatusListJson = null)
        {
            IntPtr credRevStateObjectHandle = new IntPtr();
            IntPtr revRegDefHandle = new IntPtr();
            IntPtr newRevStatusListHandle = new IntPtr();
            IntPtr revStateHandle = new IntPtr();
            IntPtr oldRevStatusListHandle = new IntPtr();

            _ = NativeMethods.anoncreds_revocation_registry_definition_from_json(ByteBuffer.Create(revRegDefJson), ref revRegDefHandle);
            _ = NativeMethods.anoncreds_revocation_list_from_json(ByteBuffer.Create(newRevStatusListJson), ref newRevStatusListHandle);
            if (revStateJson != null)
                _ = NativeMethods.anoncreds_revocation_state_from_json(ByteBuffer.Create(revStateJson), ref revStateHandle);
            if (oldRevStatusListJson != null)
                _ = NativeMethods.anoncreds_revocation_list_from_json(ByteBuffer.Create(oldRevStatusListJson), ref oldRevStatusListHandle);

            int errorCode = NativeMethods.anoncreds_create_or_update_revocation_state(
                revRegDefHandle,
                newRevStatusListHandle,
                revRegIndex,
                FfiStr.Create(tailsPath),
                revStateHandle,
                oldRevStatusListHandle,
                ref credRevStateObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            string credRevStateJson = await ObjectApi.ToJsonAsync(credRevStateObjectHandle);

            return await Task.FromResult(credRevStateJson);
        }

        /// <summary>
        /// Creates a new <see cref="CredentialRevocationState"/> object from a JSON string.
        /// </summary>
        /// <param name="revStateJson">JSON string representing a revocation object.</param>
        /// <exception cref="AnoncredsRsException">Throws when provided <paramref name="revStateJson"/> is invalid.</exception>
        /// <exception cref="System.IndexOutOfRangeException">Throws when provided <paramref name="revStateJson"/> is empty.</exception>
        /// <returns>A new <see cref="CredentialRevocationState"/> object.</returns>
        public static async Task<CredentialRevocationState> CreateRevocationStateFromJsonAsync(string revStateJson)
        {
            IntPtr revStateObjectHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_revocation_state_from_json(ByteBuffer.Create(revStateJson), ref revStateObjectHandle);
            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }
            CredentialRevocationState revStateObject = await CreateCredentialRevocationStateObject(revStateObjectHandle);
            return await Task.FromResult(revStateObject);
        }

        /// <summary>
        /// Get the value of an <see cref="RevocationRegistryDefinition"/> attribute (Supported attribute names so far: id, max_cred_num, tails_hash or tails_location).
        /// </summary>
        /// <param name="revRegDefObject">Revocation registry definition from which the attribute is requested.</param>
        /// <param name="attributeName">Name of the requested attribute.</param>
        /// <exception cref="AnoncredsRsException">Throws when provided <paramref name="attributeName"/> or <paramref name="revRegDefObject"/> are invalid.</exception>
        /// <returns>The value of the requested <paramref name="attributeName"/> from the provided <paramref name="revRegDefObject"/>.</returns>
        public static async Task<string> GetRevocationRegistryDefinitionAttributeAsync(
            RevocationRegistryDefinition revRegDefObject,
            string attributeName)
        {
            string result = "";

            int errorCode = NativeMethods.anoncreds_revocation_registry_definition_get_attribute(
                revRegDefObject.Handle,
                FfiStr.Create(attributeName),
                ref result);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Get the value of an <see cref="RevocationRegistryDefinition"/> attribute (Supported attribute names so far: id, max_cred_num, tails_hash or tails_location).
        /// </summary>
        /// <param name="revRegDefJson">Revocation registry definition from which the attribute is requested.</param>
        /// <param name="attributeName">Name of the requested attribute. Possible values are: id, max_cred_num, tails_hash, tails_location. </param>
        /// <exception cref="AnoncredsRsException">Throws when provided <paramref name="attributeName"/> or <paramref name="revRegDefObject"/> are invalid.</exception>
        /// <returns>The value of the requested <paramref name="attributeName"/> from the provided <paramref name="revRegDefObject"/>.</returns>
        public static async Task<string> GetRevocationRegistryDefinitionAttributeAsync(
            string revRegDefJson,
            string attributeName)
        {
            string result = "";
            IntPtr revRegDefHandle = new IntPtr();

            _ = NativeMethods.anoncreds_revocation_registry_definition_from_json(ByteBuffer.Create(revRegDefJson), ref revRegDefHandle);

            int errorCode = NativeMethods.anoncreds_revocation_registry_definition_get_attribute(
                revRegDefHandle,
                FfiStr.Create(attributeName),
                ref result);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Create a <see cref="RevocationStatusList"/> to a handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a revocation registry definition.</param>
        /// <returns>A new <see cref="RevocationStatusList"/>.</returns>
        private static async Task<RevocationStatusList> CreateRevocationStatusListObject(IntPtr objectHandle)
        {
            string revStatusListJson = await ObjectApi.ToJsonAsync(objectHandle);
            RevocationStatusList revStatusListObject = JsonConvert.DeserializeObject<RevocationStatusList>(revStatusListJson, Settings.JsonSettings);
            revStatusListObject.JsonString = revStatusListJson;
            revStatusListObject.Handle = objectHandle;
            return await Task.FromResult(revStatusListObject);
        }

        /// <summary>
        /// Create a <see cref="RevocationRegistryDefinition"/> to a handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a revocation registry definition.</param>
        /// <returns>A new <see cref="RevocationRegistryDefinition"/>.</returns>
        private static async Task<RevocationRegistryDefinition> CreateRevocationRegistryDefinitionObject(IntPtr objectHandle)
        {
            string regDefJson = await ObjectApi.ToJsonAsync(objectHandle);
            RevocationRegistryDefinition regDefObject = JsonConvert.DeserializeObject<RevocationRegistryDefinition>(regDefJson, Settings.JsonSettings);
            regDefObject.JsonString = regDefJson;
            regDefObject.Handle = objectHandle;
            return await Task.FromResult(regDefObject);
        }

        /// <summary>
        /// Create a <see cref="RevocationRegistryDefinitionPrivate"/> to a handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a revocation registry definition private.</param>
        /// <returns>A new <see cref="RevocationRegistryDefinition"/>.</returns>
        private static async Task<RevocationRegistryDefinitionPrivate> CreateRevocationRegistryDefinitionPrivateObject(IntPtr objectHandle)
        {
            string regDefPvtJson = await ObjectApi.ToJsonAsync(objectHandle);
            RevocationRegistryDefinitionPrivate regDefPvtObject = JsonConvert.DeserializeObject<RevocationRegistryDefinitionPrivate>(regDefPvtJson, Settings.JsonSettings);
            regDefPvtObject.JsonString = regDefPvtJson;
            regDefPvtObject.Handle = objectHandle;
            return await Task.FromResult(regDefPvtObject);
        }

        /// <summary>
        /// Create a <see cref="RevocationRegistry"/> to a handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a revocation registry.</param>
        /// <returns>A new <see cref="RevocationRegistry"/>.</returns>
        private static async Task<RevocationRegistry> CreateRevocationRegistryObject(IntPtr objectHandle)
        {
            string revRegJson = await ObjectApi.ToJsonAsync(objectHandle);
            RevocationRegistry revRegObject = JsonConvert.DeserializeObject<RevocationRegistry>(revRegJson, Settings.JsonSettings);
            revRegObject.JsonString = revRegJson;
            revRegObject.Handle = objectHandle;
            return await Task.FromResult(revRegObject);
        }

        /// <summary>
        /// Create a <see cref="RevocationRegistryDelta"/> to a handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a revocation registry delta.</param>
        /// <returns>A new <see cref="RevocationRegistryDelta"/>.</returns>
        private static async Task<RevocationRegistryDelta> CreateRevocationRegistryDeltaObject(IntPtr objectHandle)
        {
            string revRegDeltaJson = await ObjectApi.ToJsonAsync(objectHandle);
            RevocationRegistryDelta revRegDeltaObject = JsonConvert.DeserializeObject<RevocationRegistryDelta>(revRegDeltaJson, Settings.JsonSettings);
            revRegDeltaObject.JsonString = revRegDeltaJson;
            revRegDeltaObject.Handle = objectHandle;
            return await Task.FromResult(revRegDeltaObject);
        }

        /// <summary>
        /// Create a <see cref="CredentialRevocationState"/> to a handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a credential revocation state.</param>
        /// <returns>A new <see cref="CredentialRevocationState"/>.</returns>
        private static async Task<CredentialRevocationState> CreateCredentialRevocationStateObject(IntPtr objectHandle)
        {
            string credRevStateJson = await ObjectApi.ToJsonAsync(objectHandle);
            CredentialRevocationState credRevStateObject = JsonConvert.DeserializeObject<CredentialRevocationState>(credRevStateJson, Settings.JsonSettings);
            credRevStateObject.JsonString = credRevStateJson;
            credRevStateObject.Handle = objectHandle;
            return await Task.FromResult(credRevStateObject);
        }
    }
}