using anoncreds_rs_dotnet.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using static anoncreds_rs_dotnet.Models.Structures;

namespace anoncreds_rs_dotnet.Anoncreds
{
    public static class CredentialRequestApi
    {
        /// <summary>
        /// Creates a new <see cref="CredentialRequest"/> and the <see cref="CredentialRequestMetadata"/> to a given <see cref="CredentialDefinition"/>.
        /// </summary>
        /// <param name="proverDid">Prover DID.</param>
        /// <param name="credentialDefinition">Credential definition.</param>
        /// <param name="masterSecretId">Id of master secret.</param>
        /// <param name="masterSecret">New master secret.</param>
        /// <param name="credentialOffer">Credential offer.</param>
        /// <exception cref="AnoncredsRsException">Throws if any argument is invalid.</exception>
        /// <returns>New <see cref="CredentialRequest"/> and its <see cref="CredentialRequestMetadata"/>.</returns>
        public static async Task<(CredentialRequest, CredentialRequestMetadata)> CreateCredentialRequestAsync(
            string entropy,
            CredentialDefinition credentialDefinition,
            MasterSecret masterSecret,
            string masterSecretId,
            CredentialOffer credentialOffer,
            string proverDid = null)
        {
            IntPtr requestHandle = new IntPtr();
            IntPtr metadataHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_create_credential_request(
                FfiStr.Create(entropy),
                FfiStr.Create(proverDid),
                credentialDefinition.Handle,
                masterSecret.Handle,
                FfiStr.Create(masterSecretId),
                credentialOffer.Handle,
                ref requestHandle,
                ref metadataHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }
            CredentialRequest requestObject = await CreateCredentialRequestObject(requestHandle);
            CredentialRequestMetadata metadataObject = await CreateCredentialRequestMetadataObject(metadataHandle);
            return (requestObject, metadataObject);
        }

        /// <summary>
        /// Creates a new <see cref="CredentialRequest"/> and the <see cref="CredentialRequestMetadata"/> as JSON strings to a given <see cref="CredentialDefinition"/>.
        /// </summary>
        /// <param name="proverDid">Prover DID.</param>
        /// <param name="credentialDefinitionJson">Credential definition as JSON string.</param>
        /// <param name="masterSecretJson">Master secret as JSON string.</param>
        /// <param name="masterSecretId">Id of master secret.</param>
        /// <param name="credentialOfferJson">Credential offer as JSON string.</param>
        /// <exception cref="AnoncredsRsException">Throws if any argument is invalid.</exception>
        /// <returns>New <see cref="CredentialRequest"/> and its <see cref="CredentialRequestMetadata"/> as JSON strings.</returns>
        public static async Task<(string, string)> CreateCredentialRequestJsonAsync(
            string entropy,
            string credentialDefinitionJson,
            string masterSecretJson,
            string masterSecretId,
            string credentialOfferJson,
            string proverDid = null)
        {
            IntPtr credDefObjectHandle = new IntPtr();
            IntPtr masterSecretObjectHandle = new IntPtr();
            IntPtr credentialOfferObjectHandle = new IntPtr();

            _ = NativeMethods.anoncreds_credential_definition_from_json(ByteBuffer.Create(credentialDefinitionJson), ref credDefObjectHandle);
            _ = NativeMethods.anoncreds_master_secret_from_json(ByteBuffer.Create(masterSecretJson), ref masterSecretObjectHandle);
            _ = NativeMethods.anoncreds_credential_offer_from_json(ByteBuffer.Create(credentialOfferJson), ref credentialOfferObjectHandle);

            IntPtr requestHandle = new IntPtr();
            IntPtr metadataHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_create_credential_request(
                FfiStr.Create(entropy),
                FfiStr.Create(proverDid),
                credDefObjectHandle,
                masterSecretObjectHandle,
                FfiStr.Create(masterSecretId),
                credentialOfferObjectHandle,
                ref requestHandle,
                ref metadataHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }
            string requestJson = await ObjectApi.ToJsonAsync(requestHandle);
            string metadataJson = await ObjectApi.ToJsonAsync(metadataHandle);

            return (requestJson, metadataJson);
        }

        #region private methods
        /// <summary>
        /// Creates a <see cref="CredentialRequest"/> to a given handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a credential request.</param>
        /// <returns>A <see cref="CredentialRequest"/>.</returns>
        private static async Task<CredentialRequest> CreateCredentialRequestObject(IntPtr objectHandle)
        {
            string credReqJson = await ObjectApi.ToJsonAsync(objectHandle);
            CredentialRequest requestObject = JsonConvert.DeserializeObject<CredentialRequest>(credReqJson, Settings.JsonSettings);
            requestObject.JsonString = credReqJson;
            requestObject.Handle = objectHandle;
            return await Task.FromResult(requestObject);
        }

        /// <summary>
        /// Creates a <see cref="CredentialRequestMetadata"/> to a given handle.
        /// </summary>
        /// <param name="objectHandle">Handle of credential request metadata.</param>
        /// <returns>A <see cref="CredentialRequestMetadata"/>.</returns>
        private static async Task<CredentialRequestMetadata> CreateCredentialRequestMetadataObject(IntPtr objectHandle)
        {
            string credMetadataJson = await ObjectApi.ToJsonAsync(objectHandle);
            CredentialRequestMetadata requestObject = JsonConvert.DeserializeObject<CredentialRequestMetadata>(credMetadataJson, Settings.JsonSettings);
            requestObject.JsonString = credMetadataJson;
            requestObject.Handle = objectHandle;
            return await Task.FromResult(requestObject);
        }
        #endregion
    }
}