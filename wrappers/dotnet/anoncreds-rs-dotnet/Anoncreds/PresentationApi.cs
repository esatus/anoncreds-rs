using anoncreds_rs_dotnet.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static anoncreds_rs_dotnet.Models.Structures;

namespace anoncreds_rs_dotnet.Anoncreds
{
    public static class PresentationApi
    {
        /// <summary>
        /// Creates a new <see cref="Presentation"/> object.
        /// </summary>
        /// <param name="presentationRequest">Presentation request.</param>
        /// <param name="credentialEntries">Credential entries.</param>
        /// <param name="credentialProofs">Credential proofs.</param>
        /// <param name="selfAttestNames">Names of self attested attributes.</param>
        /// <param name="selfAttestValues">Values of self attested attributes.</param>
        /// <param name="masterSecret">Master secret.</param>
        /// <param name="schemas">Corresponding schemas.</param>
        /// <param name="credDefs">Credential definitions.</param>
        /// <exception cref="AnoncredsRsException">Throws when any parameters are invalid.</exception>
        /// <returns>New <see cref="Presentation"/> object.</returns>
        public static async Task<Presentation> CreatePresentationAsync(
            PresentationRequest presentationRequest,
            List<CredentialEntry> credentialEntries,
            List<CredentialProof> credentialProofs,
            List<string> selfAttestNames,
            List<string> selfAttestValues,
            MasterSecret masterSecret,
            List<Schema> schemas,
            List<CredentialDefinition> credDefs)
        {
            IntPtr presentationObjectHandle = new IntPtr();
            List<IntPtr> schemaHandles = (from schema in schemas
                                        select schema.Handle).ToList();
            List<string> schemaIds = (from schema in schemas
                                          select schema.IssuerId).ToList();
            List<IntPtr> credDefHandles = (from credDef in credDefs
                                         select credDef.Handle).ToList();
            List<string> credDefIds = (from credDef in credDefs
                                           select credDef.IssuerId).ToList();

            int errorCode = NativeMethods.anoncreds_create_presentation(
                presentationRequest.Handle,
                FfiCredentialEntryList.Create(credentialEntries),
                FfiCredentialProveList.Create(credentialProofs),
                FfiStrList.Create(selfAttestNames),
                FfiStrList.Create(selfAttestValues),
                masterSecret.Handle,
                FfiUIntList.Create(schemaHandles),
                FfiStrList.Create(schemaIds),
                FfiUIntList.Create(credDefHandles),
                FfiStrList.Create(credDefIds),
                ref presentationObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            Presentation presentationObject = await CreatePresentationObject(presentationObjectHandle);
            return await Task.FromResult(presentationObject);
        }

        /// <summary>
        /// Creates a new <see cref="Presentation"/> as JSON string.
        /// </summary>
        /// <param name="presentationRequestJson">Presentation request as JSON string.</param>
        /// <param name="credentialEntryJsons">Credential entry as JSON strings.</param>
        /// <param name="credentialProofJsons">Credential proof as JSON strings.</param>
        /// <param name="selfAttestNames">Names of self attested attributes.</param>
        /// <param name="selfAttestValues">Values of self attested attributes.</param>
        /// <param name="masterSecretJson">Master secret as JSON string.</param>
        /// <param name="schemaJsons">Corresponding schema as JSON strings.</param>
        /// <param name="credDefJsons">Credential definition as JSON strings.</param>
        /// <exception cref="AnoncredsRsException">Throws when any parameters are invalid.</exception>
        /// <returns>New <see cref="Presentation"/> as JSON string.</returns>
        public static async Task<string> CreatePresentationAsync(
            string presentationRequestJson,
            List<string> credentialEntryJsons,
            List<string> credentialProofJsons,
            List<string> selfAttestNames,
            List<string> selfAttestValues,
            string masterSecretJson,
            List<string> schemaJsons,
            List<string> credDefJsons)
        {
            IntPtr presentationObjectHandle = new IntPtr();
            List<CredentialEntry> credentialEntries = new List<CredentialEntry>();
            List<CredentialProof> credentialProofs = new List<CredentialProof>();
            IntPtr presentationRequestHandle = new IntPtr();
            IntPtr masterSecretHandle = new IntPtr();
            List<IntPtr> schemaHandles = new List<IntPtr>();
            List<IntPtr> credDefHandles = new List<IntPtr>();
            List<string> schemaIds = new List<string>();
            List<string> credDefIds = new List<string>();

            foreach (string credentialEntryJson in credentialEntryJsons)
            {
                credentialEntries.Add(JsonConvert.DeserializeObject<CredentialEntry>(credentialEntryJson));
            }
            foreach (string credentialProofJson in credentialProofJsons)
            {
                credentialProofs.Add(JsonConvert.DeserializeObject<CredentialProof>(credentialProofJson));
            }
            _ = NativeMethods.anoncreds_presentation_request_from_json(ByteBuffer.Create(presentationRequestJson), ref presentationRequestHandle);
            _ = NativeMethods.anoncreds_master_secret_from_json(ByteBuffer.Create(masterSecretJson), ref masterSecretHandle);
            foreach(string schemaJson in schemaJsons)
            {
                IntPtr newSchemaHandle = new IntPtr();
                _ = NativeMethods.anoncreds_schema_from_json(ByteBuffer.Create(schemaJson), ref newSchemaHandle);
                schemaHandles.Add(newSchemaHandle);

                Schema newSchema = JsonConvert.DeserializeObject<Schema>(schemaJson,Settings.JsonSettings);
                schemaIds.Add(newSchema.IssuerId);
            }
            foreach (string credDefJson in credDefJsons)
            {
                IntPtr newCredDefHandle = new IntPtr();
                _ = NativeMethods.anoncreds_credential_definition_from_json(ByteBuffer.Create(credDefJson), ref newCredDefHandle);
                credDefHandles.Add(newCredDefHandle);

                CredentialDefinition newCredDef = JsonConvert.DeserializeObject<CredentialDefinition>(credDefJson, Settings.JsonSettings);
                credDefIds.Add(newCredDef.IssuerId);
            }
            
            int errorCode = NativeMethods.anoncreds_create_presentation(
                presentationRequestHandle,
                FfiCredentialEntryList.Create(credentialEntries),
                FfiCredentialProveList.Create(credentialProofs),
                FfiStrList.Create(selfAttestNames),
                FfiStrList.Create(selfAttestValues),
                masterSecretHandle,
                FfiUIntList.Create(schemaHandles),
                FfiStrList.Create(schemaIds),
                FfiUIntList.Create(credDefHandles),
                FfiStrList.Create(credDefIds),
                ref presentationObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            string presentationJson = await ObjectApi.ToJsonAsync(presentationObjectHandle);
            return await Task.FromResult(presentationJson);
        }

        /// <summary>
        /// Creates a <see cref="Presentation"/> object from JSON.
        /// </summary>
        /// <param name="presentationJson">JSON string of presentation object.</param>
        /// <exception cref="IndexOutOfRangeException">Throws when <paramref name="presentationJson"/> is empty.</exception>
        /// <exception cref="AnoncredsRsException">Throws if <paramref name="presentationJson"/> is an invalid json object.</exception>
        /// <returns>New <see cref="Presentation"/> object.</returns>
        public static async Task<Presentation> CreatePresentationFromJsonAsync(string presentationJson)
        {
            IntPtr presentationObjectHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_presentation_from_json(ByteBuffer.Create(presentationJson), ref presentationObjectHandle);
            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }
            Presentation presentationObject = await CreatePresentationObject(presentationObjectHandle);
            return await Task.FromResult(presentationObject);
        }

        /// <summary>
        /// Verifies that a presentation matches its request.
        /// </summary>
        /// <param name="presentation">Presentation to verify.</param>
        /// <param name="presentationRequest">Request to verify the <paramref name="presentation"/> object with.</param>
        /// <param name="schemas">Corresponding schemas.</param>
        /// <param name="credentialDefinitions">Corresponding credential definitions.</param>
        /// <param name="revocationRegistryDefinitions">Corresponding revocation registry definitions.</param>
        /// <param name="revocationStatusList">Corresponding revocation status lists.</param>
        /// <param name="nonrevokedIntervalOverrides">Corresponding NonRevokedIntervalOverride object list.</param>
        /// <exception cref="AnoncredsRsException">Throws if any parameter is invalid.</exception>
        /// <returns>TRUE if provided <see cref="Presentation"/> can be verified, otherwise FALSE.</returns>
        public static async Task<bool> VerifyPresentationAsync(
            Presentation presentation,
            PresentationRequest presentationRequest,
            List<Schema> schemas,
            List<CredentialDefinition> credentialDefinitions,
            List<RevocationRegistryDefinition> revocationRegistryDefinitions,
            List<RevocationStatusList> revocationStatusList,
            List<NonrevokedIntervalOverride> nonrevokedIntervalOverrides = null)
        {
            byte verify = 0;
            List<IntPtr> schemaHandles =
                (from schema in schemas select schema.Handle).ToList();
            List<string> schemaIds =
                (from schema in schemas select schema.IssuerId).ToList();

            List<IntPtr> credDefHandles =
                (from credDef in credentialDefinitions select credDef.Handle).ToList();
            List<string> credDefIds =
                (from credDef in credentialDefinitions select credDef.IssuerId).ToList();

            List<IntPtr> revRegDefHandles =
                (from revRegDef in revocationRegistryDefinitions select revRegDef.Handle).ToList();
            List<string> revRegDefIds =
                (from revRegDef in revocationRegistryDefinitions select revRegDef.IssuerId).ToList();
            List<IntPtr> revStatusHandles =
                (from revStatus in revocationStatusList select revStatus.Handle).ToList();

            int errorCode = NativeMethods.anoncreds_verify_presentation(
                presentation.Handle,
                presentationRequest.Handle,
                FfiUIntList.Create(schemaHandles),
                FfiStrList.Create(schemaIds),
                FfiUIntList.Create(credDefHandles),
                FfiStrList.Create(credDefIds),
                FfiUIntList.Create(revRegDefHandles),
                FfiStrList.Create(revRegDefIds),
                FfiUIntList.Create(revStatusHandles),
                FfiNonrevokedIntervalOverrideList.Create(nonrevokedIntervalOverrides),
                ref verify);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            return await Task.FromResult(Convert.ToBoolean(verify));
        }

        /// <summary>
        /// Verifies that a presentation matches its request.
        /// </summary>
        /// <param name="presentationJson">Presentation to verify as JSON string.</param>
        /// <param name="presentationRequestJson">Request to verify the <paramref name="presentationJson"/> object with as JSON string.</param>
        /// <param name="schemaJsons">Corresponding schemas as JSON strings.</param>
        /// <param name="credentialDefinitionJsons">Corresponding credential definitions as JSON strings.</param>
        /// <param name="revocationRegistryDefinitionJsons">Corresponding revocation registry definitions as JSON strings.</param>
        /// <param name="revocationStatusListJsons">Corresponding revocation status lists as JSON strings.</param>
        /// <param name="nonrevokedIntervalOverrideJsons">Corresponding NonRevokedIntervalOverride object list as JSON.</param>
        /// <exception cref="AnoncredsRsException">Throws if any parameter is invalid.</exception>
        /// <returns>TRUE if provided <see cref="Presentation"/> can be verified, otherwise FALSE.</returns>
        public static async Task<bool> VerifyPresentationAsync(
            string presentationJson,
            string presentationRequestJson,
            List<string> schemaJsons,
            List<string> credentialDefinitionJsons,
            List<string> revocationRegistryDefinitionJsons,
            List<string> revocationStatusListJsons,
            List<string> nonrevokedIntervalOverrideJsons = null)
        {
            byte verify = 0;

            IntPtr presentationHandle = new IntPtr();
            IntPtr presentationRequestHandle = new IntPtr();
            List<IntPtr> schemaHandles = new List<IntPtr>();
            List<string> schemaIds = new List<string>();
            List<IntPtr> credDefHandles = new List<IntPtr>();
            List<string> credDefIds = new List<string>();
            List<IntPtr> revRegDefHandles = new List<IntPtr>();
            List<string> revRegDefIds = new List<string>();
            List<IntPtr> revocationStatusListHandles = new List<IntPtr>();
            List<NonrevokedIntervalOverride> nonrevokedIntervalOverrides = new List<NonrevokedIntervalOverride>();

            _ = NativeMethods.anoncreds_presentation_from_json(ByteBuffer.Create(presentationJson), ref presentationHandle);
            _ = NativeMethods.anoncreds_presentation_request_from_json(ByteBuffer.Create(presentationRequestJson), ref presentationRequestHandle);
            foreach (string schemaJson in schemaJsons)
            {
                IntPtr newSchemaHandle = new IntPtr();
                _ = NativeMethods.anoncreds_schema_from_json(ByteBuffer.Create(schemaJson), ref newSchemaHandle);
                schemaHandles.Add(newSchemaHandle);

                Schema newSchema = JsonConvert.DeserializeObject<Schema>(schemaJson, Settings.JsonSettings);
                schemaIds.Add(newSchema.IssuerId);
            }
            foreach (string credentialDefinitionJson in credentialDefinitionJsons)
            {
                IntPtr newCredentialDefinitionHandle = new IntPtr();
                _ = NativeMethods.anoncreds_credential_definition_from_json(ByteBuffer.Create(credentialDefinitionJson), ref newCredentialDefinitionHandle);
                credDefHandles.Add(newCredentialDefinitionHandle);

                CredentialDefinition newCredDef = JsonConvert.DeserializeObject<CredentialDefinition>(credentialDefinitionJson, Settings.JsonSettings);
                credDefIds.Add(newCredDef.IssuerId);
            }
            foreach (string revocationRegistryDefinitionJson in revocationRegistryDefinitionJsons)
            {
                IntPtr newRevocationRegistryDefinitionHandle = new IntPtr();
                _ = NativeMethods.anoncreds_revocation_registry_definition_from_json(ByteBuffer.Create(revocationRegistryDefinitionJson), ref newRevocationRegistryDefinitionHandle);
                revRegDefHandles.Add(newRevocationRegistryDefinitionHandle);

                RevocationRegistryDefinition newRevRegDef = JsonConvert.DeserializeObject<RevocationRegistryDefinition>(revocationRegistryDefinitionJson, Settings.JsonSettings);
                revRegDefIds.Add(newRevRegDef.IssuerId);
            }
            foreach (string revocationStatusListJson in revocationStatusListJsons)
            {
                IntPtr newRevStatusListHandle = new IntPtr();
                _ = NativeMethods.anoncreds_revocation_list_from_json(ByteBuffer.Create(revocationStatusListJson), ref newRevStatusListHandle);
                revocationStatusListHandles.Add(newRevStatusListHandle);
            }
            foreach(string nonrevokedIntervalOverride in nonrevokedIntervalOverrideJsons)
            {
                nonrevokedIntervalOverrides.Add(JsonConvert.DeserializeObject<NonrevokedIntervalOverride>(nonrevokedIntervalOverride));
            }

            int errorCode = NativeMethods.anoncreds_verify_presentation(
                presentationHandle,
                presentationRequestHandle,
                FfiUIntList.Create(schemaHandles),
                FfiStrList.Create(schemaIds),
                FfiUIntList.Create(credDefHandles),
                FfiStrList.Create(credDefIds),
                FfiUIntList.Create(revRegDefHandles),
                FfiStrList.Create(revRegDefIds),
                FfiUIntList.Create(revocationStatusListHandles),
                FfiNonrevokedIntervalOverrideList.Create(nonrevokedIntervalOverrides),
                ref verify);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            return await Task.FromResult(Convert.ToBoolean(verify));
        }

        /// <summary>
        /// Creates a <see cref="Presentation"/> to a handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a presentation.</param>
        /// <returns>A <see cref="Presentation"/>.</returns>
        private static async Task<Presentation> CreatePresentationObject(IntPtr objectHandle)
        {
            string presentationJson = await ObjectApi.ToJsonAsync(objectHandle);
            Presentation presentationObject = JsonConvert.DeserializeObject<Presentation>(presentationJson, Settings.JsonSettings);

            presentationObject.JsonString = presentationJson;
            presentationObject.Handle = objectHandle;
            return await Task.FromResult(presentationObject);
        }
    }
}