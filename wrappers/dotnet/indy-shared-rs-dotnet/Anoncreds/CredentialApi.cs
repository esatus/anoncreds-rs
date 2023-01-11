using anoncreds_rs_dotnet.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static anoncreds_rs_dotnet.Models.Structures;

namespace anoncreds_rs_dotnet.Anoncreds
{
    public static class CredentialApi
    {
        /// <summary>
        /// Creates a new tuple of <see cref="Credential"/>, <see cref="RevocationRegistry"/> and <see cref="RevocationRegistryDelta"/> objects.
        /// <para>
        /// Note: Either all of the optional parameters (<paramref name="revocationRegistryDefinition"/>, <paramref name="revocationRegistryDefinitionPrivate"/>, <paramref name="revocationRegistry"/>, 
        /// <paramref name="regIdx"/> and <paramref name="regUsed"/>) have to be provided or none.
        /// </para>
        /// </summary>
        /// <param name="credDefObject">Definition of the credential.</param>
        /// <param name="credDefPvtObject">Private key params of the credential.</param>
        /// <param name="credOfferObject">Credential offer.</param>
        /// <param name="credReqObject">Credential request.</param>
        /// <param name="attributeNames">Attribute names.</param>
        /// <param name="attributeRawValues">Raw values of the attributes.</param>
        /// <param name="attributeEncodedValues">Encoded values of the attributes.</param>
        /// <param name="revocationRegistryDefinition">Definition of the RevocationRegistry.</param>
        /// <param name="revocationRegistryDefinitionPrivate">Private key params of the RevocationRegistry.</param>
        /// <param name="revocationRegistry">RevocationRegistry</param>
        /// <param name="regIdx">Credential revocation index.</param>
        /// <param name="regUsed">List of revoked credential indices.</param>
        /// <returns>A new <see cref="Credential"/>, <see cref="RevocationRegistry"/> and <see cref="RevocationRegistryDelta"/>.</returns>
        /// <exception cref="AnoncredsRsException">Throws if any parameters are invalid.</exception>
        public static async Task<(Credential, RevocationRegistry, RevocationRegistryDelta)> CreateCredentialAsync(
            CredentialDefinition credDefObject,
            CredentialDefinitionPrivate credDefPvtObject,
            CredentialOffer credOfferObject,
            CredentialRequest credReqObject,
            List<string> attributeNames,
            List<string> attributeRawValues,
            List<string> attributeEncodedValues,
            RevocationRegistryDefinition revocationRegistryDefinition = null,
            RevocationRegistryDefinitionPrivate revocationRegistryDefinitionPrivate = null,
            RevocationRegistry revocationRegistry = null,
            long regIdx = -1,
            List<long> regUsed = null)
        {
            IntPtr credObjectHandle = new IntPtr();
            IntPtr revRegObjectHandle = new IntPtr();
            IntPtr revDeltaObjectHandle = new IntPtr();
            int errorCode;
            if (revocationRegistryDefinition == null
                && revocationRegistryDefinitionPrivate == null
                && revocationRegistry == null
                && regIdx == -1
                && regUsed == null)
            {
                errorCode = NativeMethods.anoncreds_create_credential(
                credDefObject.Handle,
                credDefPvtObject.Handle,
                credOfferObject.Handle,
                credReqObject.Handle,
                FfiStrList.Create(attributeNames),
                FfiStrList.Create(attributeRawValues),
                FfiStrList.Create(attributeEncodedValues),
                new IntPtr(),
                ref credObjectHandle,
                ref revRegObjectHandle,
                ref revDeltaObjectHandle);
            }
            else if (revocationRegistryDefinition != null
                && revocationRegistryDefinitionPrivate != null
                && revocationRegistry != null
                && regIdx != -1)
            {
                CredentialRevocationConfig credRevInfo = new CredentialRevocationConfig()
                {
                    RevRegDefObjectHandle = revocationRegistryDefinition.Handle,
                    RevRegDefPvtObjectHandle = revocationRegistryDefinitionPrivate.Handle,
                    RevRegObjectHandle = revocationRegistry.Handle,
                    TailsPath = revocationRegistryDefinition.Value.TailsLocation,
                    RegIdx = regIdx,
                    RegUsed = regUsed
                };

                errorCode = NativeMethods.anoncreds_create_credential(
                credDefObject.Handle,
                credDefPvtObject.Handle,
                credOfferObject.Handle,
                credReqObject.Handle,
                FfiStrList.Create(attributeNames),
                FfiStrList.Create(attributeRawValues),
                FfiStrList.Create(attributeEncodedValues),
                FfiCredRevInfo.Create(credRevInfo),
                ref credObjectHandle,
                ref revRegObjectHandle,
                ref revDeltaObjectHandle);
            }
            else
            {
                throw new AnoncredsRsException("Revocation data incomplete.", ErrorCode.Input);
            }

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            Credential credObject = await CreateCredentialObjectAsync(credObjectHandle);

            RevocationRegistry revRegObject = null;
            if (!revRegObjectHandle.Equals(new IntPtr()))
            {
                revRegObject = await CreateRevocationRegistryObjectAsync(revRegObjectHandle);
            }

            RevocationRegistryDelta revDeltaObject = null;
            if (!revDeltaObjectHandle.Equals(new IntPtr()))
            {
                revDeltaObject = await CreateRevocationRegistryDeltaObjectAsync(revDeltaObjectHandle);
            }

            return await Task.FromResult((credObject, revRegObject, revDeltaObject));
        }

        /// <summary>
        /// Creates a new tuple of <see cref="Credential"/>, <see cref="RevocationRegistry"/> and <see cref="RevocationRegistryDelta"/> objects.
        /// </summary>
        /// <param name="credDefObjectJson"><see cref="CredentialDefinition"/> as JSON string.</param>
        /// <param name="credDefPvtObjectJson"><see cref="CredentialDefinitionPrivate"/> as JSON string.</param>
        /// <param name="credOfferObjectJson"><see cref="CredentialOffer"/> as JSON string.</param>
        /// <param name="credReqObjectJson"><see cref="CredentialRequest"/> as JSON string.</param>
        /// <param name="attributeNames">Attribute names.</param>
        /// <param name="attributeRawValues">Raw values of the attributes.</param>
        /// <param name="attributeEncodedValues">Encoded values of the attributes.</param>
        /// <param name="revocationRegistryDefinitionJson"><see cref="RevocationRegistryDefinition"/> as JSON string.</param>
        /// <param name="revocationRegistryDefinitionPrivateJson"><see cref="RevocationRegistryDefinitionPrivate"/> as JSON string.</param>
        /// <param name="revocationRegistryJson"><see cref="RevocationRegistry"/> as JSON string</param>
        /// <param name="regIdx">Credential revocation index.</param>
        /// <param name="regUsed">List of revoked credential indices.</param>
        /// <returns>A new <see cref="Credential"/>, <see cref="RevocationRegistry"/> and <see cref="RevocationRegistryDelta"/>.</returns>
        /// <exception cref="AnoncredsRsException">Throws if any parameters are invalid.</exception>
        public static async Task<(string, string, string)> CreateCredentialAsync(
            string credDefObjectJson,
            string credDefPvtObjectJson,
            string credOfferObjectJson,
            string credReqObjectJson,
            List<string> attributeNames,
            List<string> attributeRawValues,
            List<string> attributeEncodedValues,
            string revocationRegistryDefinitionJson = null,
            string revocationRegistryDefinitionPrivateJson = null,
            string revocationRegistryJson = null,
            long regIdx = -1,
            List<long> regUsed = null)
        {
            IntPtr credDefObjectHandle = new IntPtr();
            IntPtr credDefPvtObjectHandle = new IntPtr();
            IntPtr credOfferObjectHandle = new IntPtr();
            IntPtr credReqObjectHandle = new IntPtr();

            _ = NativeMethods.anoncreds_credential_definition_from_json(ByteBuffer.Create(credDefObjectJson), ref credDefObjectHandle);
            _ = NativeMethods.anoncreds_credential_definition_private_from_json(ByteBuffer.Create(credDefPvtObjectJson), ref credDefPvtObjectHandle);
            _ = NativeMethods.anoncreds_credential_offer_from_json(ByteBuffer.Create(credOfferObjectJson), ref credOfferObjectHandle);
            _ = NativeMethods.anoncreds_credential_request_from_json(ByteBuffer.Create(credReqObjectJson), ref credReqObjectHandle);

            IntPtr credObjectHandle = new IntPtr();
            IntPtr revRegObjectHandle = new IntPtr();
            IntPtr revDeltaObjectHandle = new IntPtr();
            int errorCode;
            if (revocationRegistryDefinitionJson == null
                && revocationRegistryDefinitionPrivateJson == null
                && revocationRegistryJson == null
                && regIdx == -1
                && regUsed == null)
            {
                errorCode = NativeMethods.anoncreds_create_credential(
                credDefObjectHandle,
                credDefPvtObjectHandle,
                credOfferObjectHandle,
                credReqObjectHandle,
                FfiStrList.Create(attributeNames),
                FfiStrList.Create(attributeRawValues),
                FfiStrList.Create(attributeEncodedValues),
                new IntPtr(),
                ref credObjectHandle,
                ref revRegObjectHandle,
                ref revDeltaObjectHandle);
            }
            else if (revocationRegistryDefinitionJson != null
                && revocationRegistryDefinitionPrivateJson != null
                && revocationRegistryJson != null
                && regIdx != -1)
            {
                IntPtr revocationRegistryDefinitionHandle = new IntPtr();
                IntPtr revocationRegistryDefinitionPrivateJsonHandle = new IntPtr();
                IntPtr revocationRegistryJsonHandle = new IntPtr();

                _ = NativeMethods.anoncreds_revocation_registry_definition_from_json(ByteBuffer.Create(revocationRegistryDefinitionJson), ref revocationRegistryDefinitionHandle);
                _ = NativeMethods.anoncreds_revocation_registry_definition_private_from_json(ByteBuffer.Create(revocationRegistryDefinitionPrivateJson), ref revocationRegistryDefinitionPrivateJsonHandle);
                _ = NativeMethods.anoncreds_revocation_registry_from_json(ByteBuffer.Create(revocationRegistryJson), ref revocationRegistryJsonHandle);

                string x = JObject.Parse(revocationRegistryDefinitionJson)["value"].ToString();
                string tailsLocation = JObject.Parse(x)["tailsLocation"].ToString();

                CredentialRevocationConfig credRevInfo = new CredentialRevocationConfig()
                {
                    RevRegDefObjectHandle = revocationRegistryDefinitionHandle,
                    RevRegDefPvtObjectHandle = revocationRegistryDefinitionPrivateJsonHandle,
                    RevRegObjectHandle = revocationRegistryJsonHandle,
                    TailsPath = tailsLocation,
                    RegIdx = regIdx,
                    RegUsed = regUsed
                };

                errorCode = NativeMethods.anoncreds_create_credential(
                credDefObjectHandle,
                credDefPvtObjectHandle,
                credOfferObjectHandle,
                credReqObjectHandle,
                FfiStrList.Create(attributeNames),
                FfiStrList.Create(attributeRawValues),
                FfiStrList.Create(attributeEncodedValues),
                FfiCredRevInfo.Create(credRevInfo),
                ref credObjectHandle,
                ref revRegObjectHandle,
                ref revDeltaObjectHandle);
            }
            else
            {
                throw new AnoncredsRsException("Revocation data incomplete.", ErrorCode.Input);
            }

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            string credJson = await ObjectApi.ToJsonAsync(credObjectHandle);

            string revRegJson = null;
            if (!revRegObjectHandle.Equals(new IntPtr()))
            {
                revRegJson = await ObjectApi.ToJsonAsync(revRegObjectHandle);
            }

            string revDeltaJson = null;
            if (!revDeltaObjectHandle.Equals(new IntPtr()))
            {
                revDeltaJson = await ObjectApi.ToJsonAsync(revDeltaObjectHandle);
            }

            return await Task.FromResult((credJson, revRegJson, revDeltaJson));
        }

        /// <summary>
        /// Creates a <see cref="Credential"/> from JSON.
        /// </summary>
        /// <param name="credentialJson">Credential as JSON.</param>
        /// <returns>A <see cref="Credential"/>.</returns>
        public static async Task<Credential> CreateCredentialFromJsonAsync(string credentialJson)
        {
            IntPtr credObjectHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_credential_from_json(ByteBuffer.Create(credentialJson), ref credObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            Credential result = await CreateCredentialObjectAsync(credObjectHandle);

            return result;
        }

        /// <summary>
        /// Processes a given <see cref="Credential"/>.
        /// </summary>
        /// <param name="credential">Credential to be processed.</param>
        /// <param name="credentialRequestMetadata">Metadata of the credential request.</param>
        /// <param name="masterSecret">Used master secret.</param>
        /// <param name="credentialDefinition">Credential definition of the processed credential.</param>
        /// <param name="revocationRegistryDefinition">Revocation registry definition for the processed credential.</param>
        /// <exception cref="AnoncredsRsException">Throws if any parameters are invalid.</exception>
        /// <returns>A copy of the processed <see cref="Credential"/>.</returns>
        public static async Task<Credential> ProcessCredentialAsync(
            Credential credential,
            CredentialRequestMetadata credentialRequestMetadata,
            MasterSecret masterSecret,
            CredentialDefinition credentialDefinition,
            RevocationRegistryDefinition revocationRegistryDefinition)
        {
            IntPtr credentialObjectHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_process_credential(
                credential.Handle,
                credentialRequestMetadata.Handle,
                masterSecret.Handle,
                credentialDefinition.Handle,
                revocationRegistryDefinition.Handle,
                ref credentialObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            Credential credentialObject = await CreateCredentialObjectAsync(credentialObjectHandle);

            return await Task.FromResult(credentialObject);
        }

        /// <summary>
        /// Processes a given <see cref="Credential"/>.
        /// </summary>
        /// <param name="credentialJson"><see cref="Credential"/> to be processed as JSON string.</param>
        /// <param name="credentialRequestMetadataJson"><see cref="CredentialRequestMetadata"/> as JSON string.</param>
        /// <param name="masterSecretJson">Used <see cref="MasterSecret"/> as JSON string.</param>
        /// <param name="credentialDefinitionJson"><see cref="CredentialDefinition"/> of the processed credential as JSON string.</param>
        /// <param name="revocationRegistryDefinitionJson"><see cref="RevocationRegistryDefinition"/> for the processed credential as JSON string.</param>
        /// <exception cref="AnoncredsRsException">Throws if any parameters are invalid.</exception>
        /// <returns>A copy of the processed <see cref="Credential"/> as JSON string.</returns>
        public static async Task<string> ProcessCredentialAsync(
            string credentialJson,
            string credentialRequestMetadataJson,
            string masterSecretJson,
            string credentialDefinitionJson,
            string revocationRegistryDefinitionJson)
        {
            IntPtr credObjectHandle = new IntPtr();
            IntPtr credReqMetadataObjectHandle = new IntPtr();
            IntPtr masterSecretObjectHandle = new IntPtr();
            IntPtr credDefObjectHandle = new IntPtr();
            IntPtr revRegDefObjectHandle = new IntPtr();
            _ = NativeMethods.anoncreds_credential_from_json(ByteBuffer.Create(credentialJson), ref credObjectHandle);
            _ = NativeMethods.anoncreds_credential_request_metadata_from_json(ByteBuffer.Create(credentialRequestMetadataJson), ref credReqMetadataObjectHandle);
            _ = NativeMethods.anoncreds_master_secret_from_json(ByteBuffer.Create(masterSecretJson), ref masterSecretObjectHandle);
            _ = NativeMethods.anoncreds_credential_definition_from_json(ByteBuffer.Create(credentialDefinitionJson), ref credDefObjectHandle);
            _ = NativeMethods.anoncreds_revocation_registry_definition_from_json(ByteBuffer.Create(revocationRegistryDefinitionJson), ref revRegDefObjectHandle);

            IntPtr credentialObjectHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_process_credential(
                credObjectHandle,
                credReqMetadataObjectHandle,
                masterSecretObjectHandle,
                credDefObjectHandle,
                revRegDefObjectHandle,
                ref credentialObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            Credential credentialObject = await CreateCredentialObjectAsync(credentialObjectHandle);

            return credentialObject.JsonString;
        }

        /// <summary>
        /// Encodes raw attributes to be used in a <see cref="Credential"/>.
        /// </summary>
        /// <param name="rawAttributes">Attributes to be encoded.</param>
        /// <exception cref="AnoncredsRsException">Throws when <paramref name="rawAttributes"/> are invalid.</exception>
        /// <exception cref="InvalidOperationException">Throws when <paramref name="rawAttributes"/> are empty.</exception>
        /// <returns>Returns the given <paramref name="rawAttributes"/> as encoded attributes.</returns>
        public static async Task<List<string>> EncodeCredentialAttributesAsync(List<string> rawAttributes)
        {
            string result = "";
            int errorCode = NativeMethods.anoncreds_encode_credential_attributes(FfiStrList.Create(rawAttributes), ref result);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }
            string[] abc = result.Split(',');
            return await Task.FromResult(abc.ToList());
        }

        /// <summary>
        /// Returns the attribute value of a given attribute name from a given <see cref="Credential"/>. (Currently supported attribute names: "schema_id", "cred_def_id", "rev_reg_id", "rev_reg_index").
        /// </summary>
        /// <param name="credential">The credential from which the attribute value is requested.</param>
        /// <param name="attributeName">The name of the attribute that is requested.</param>
        /// <exception cref="AnoncredsRsException">Throws when attribute name is invalid.</exception>
        /// <returns>The value of requested <paramref name="attributeName"/> from the provided <paramref name="credential"/>.</returns>
        public static async Task<string> GetCredentialAttributeAsync(Credential credential, string attributeName)
        {
            string result = "";
            int errorCode = NativeMethods.anoncreds_credential_get_attribute(credential.Handle, FfiStr.Create(attributeName), ref result);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Returns the attribute value of a given attribute name from a given <see cref="Credential"/> as JSON string. (Currently supported attribute names: "schema_id", "cred_def_id", "rev_reg_id", "rev_reg_index").
        /// </summary>
        /// <param name="credentialJson">The <see cref="Credential"/> as JSON string from which the attribute value is requested.</param>
        /// <param name="attributeName">The name of the attribute that is requested.</param>
        /// <exception cref="AnoncredsRsException">Throws when attribute name is invalid.</exception>
        /// <returns>The value of requested <paramref name="attributeName"/> from the provided <paramref name="credentialJson"/>.</returns>
        public static async Task<string> GetCredentialAttributeAsync(string credentialJson, string attributeName)
        {
            IntPtr credObjectHandle = new IntPtr();
            _ = NativeMethods.anoncreds_credential_from_json(ByteBuffer.Create(credentialJson), ref credObjectHandle);
            string result = "";
            int errorCode = NativeMethods.anoncreds_credential_get_attribute(credObjectHandle, FfiStr.Create(attributeName), ref result);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            return await Task.FromResult(result);
        }

        #region private methods
        /// <summary>
        /// Returns the <see cref="Credential"/> to a given handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a <see cref="Credential"/>.</param>
        /// <returns>A <see cref="Credential"/>.</returns>
        private static async Task<Credential> CreateCredentialObjectAsync(IntPtr objectHandle)
        {
            string credJson = await ObjectApi.ToJsonAsync(objectHandle);
            Credential credObject = JsonConvert.DeserializeObject<Credential>(credJson, Settings.JsonSettings);
            credObject.JsonString = credJson;
            credObject.Handle = objectHandle;
            return await Task.FromResult(credObject);
        }

        /// <summary>
        /// Returns the <see cref="RevocationRegistry"/> to a given handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a <see cref="RevocationRegistry"/>.</param>
        /// <returns>A <see cref="RevocationRegistry"/>.</returns>
        private static async Task<RevocationRegistry> CreateRevocationRegistryObjectAsync(IntPtr objectHandle)
        {
            string revRegJson = await ObjectApi.ToJsonAsync(objectHandle);
            RevocationRegistry revRegObject = JsonConvert.DeserializeObject<RevocationRegistry>(revRegJson, Settings.JsonSettings);
            revRegObject.JsonString = revRegJson;
            revRegObject.Handle = objectHandle;
            return await Task.FromResult(revRegObject);
        }

        /// <summary>
        /// Returns the <see cref="RevocationRegistryDelta"/> to a given handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a RevocationRegistryDelta.</param>
        /// <returns>A <see cref="RevocationRegistryDelta"/>.</returns>
        private static async Task<RevocationRegistryDelta> CreateRevocationRegistryDeltaObjectAsync(IntPtr objectHandle)
        {
            string revDeltaJson = await ObjectApi.ToJsonAsync(objectHandle);
            RevocationRegistryDelta revDeltaObject = JsonConvert.DeserializeObject<RevocationRegistryDelta>(revDeltaJson, Settings.JsonSettings);
            revDeltaObject.JsonString = revDeltaJson;
            revDeltaObject.Handle = objectHandle;
            return await Task.FromResult(revDeltaObject);
        }
        #endregion
    }
}