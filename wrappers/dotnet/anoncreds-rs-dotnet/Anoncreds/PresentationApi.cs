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
        /// <param name="linkSecret">Master secret.</param>
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
            string linkSecret,
            List<Schema> schemas,
            List<string> schemaIds,
            List<CredentialDefinition> credDefs,
            List<string> credDefIds)
        {
            IntPtr presentationObjectHandle = new IntPtr();
            List<IntPtr> schemaHandles = (from schema in schemas
                                        select schema.Handle).ToList();

            List<IntPtr> credDefHandles = (from credDef in credDefs
                                         select credDef.Handle).ToList();

            //If presentationRequestJson contains more than 1 restriction, a conversion in a format that the underlying rust code understands is needed i.e.:
            /**
            ...
            \"restrictions\":
             {\"$or\":
                 [{\"$and\":
                     [
                      {\"schema_id\":\"EU-Führerschein\"},
                      {\"cred_def_id\":\"DE-Führerschein\"}
                     ]
                 }]
             },
            ...
            **/
            string convertedPresReqJson = presentationRequest.ToQueryRequestJson();
            presentationRequest = await PresentationRequestApi.CreatePresReqFromJsonAsync(convertedPresReqJson);

            int errorCode = NativeMethods.anoncreds_create_presentation(
                presentationRequest.Handle,
                FfiCredentialEntryList.Create(credentialEntries),
                FfiCredentialProveList.Create(credentialProofs),
                FfiStrList.Create(selfAttestNames),
                FfiStrList.Create(selfAttestValues),
                FfiStr.Create(linkSecret),
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
        /// <param name="linkSecretJson">Master secret as JSON string.</param>
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
            string linkSecret,
            List<string> schemaJsons,
            List<string> schemaIds,
            List<string> credDefJsons,
            List<string> credDefIds)
        {
            IntPtr presentationObjectHandle = new IntPtr();
            List<CredentialEntry> credentialEntries = new List<CredentialEntry>();
            List<CredentialProof> credentialProofs = new List<CredentialProof>();
            IntPtr presentationRequestHandle = new IntPtr();
            List<IntPtr> schemaHandles = new List<IntPtr>();
            List<IntPtr> credDefHandles = new List<IntPtr>();

            foreach (string credentialEntryJson in credentialEntryJsons)
            {
                credentialEntries.Add(JsonConvert.DeserializeObject<CredentialEntry>(credentialEntryJson));
            }
            foreach (string credentialProofJson in credentialProofJsons)
            {
                credentialProofs.Add(JsonConvert.DeserializeObject<CredentialProof>(credentialProofJson));
            }

            //If presentationRequestJson contains more than 1 restriction, a conversion in a format that the underlying rust code understands is needed i.e.:
            /**
            ...
            \"restrictions\":
             {\"$or\":
                 [{\"$and\":
                     [
                      {\"schema_id\":\"EU-Führerschein\"},
                      {\"cred_def_id\":\"DE-Führerschein\"}
                     ]
                 }]
             },
            ...
            **/
            string convertedPresReqJson = (await PresentationRequestApi.CreatePresReqFromJsonAsync(presentationRequestJson)).ToQueryRequestJson();
            _ = NativeMethods.anoncreds_presentation_request_from_json(ByteBuffer.Create(convertedPresReqJson), ref presentationRequestHandle);

            foreach (string schemaJson in schemaJsons)
            {
                IntPtr newSchemaHandle = new IntPtr();
                _ = NativeMethods.anoncreds_schema_from_json(ByteBuffer.Create(schemaJson), ref newSchemaHandle);
                schemaHandles.Add(newSchemaHandle);
            }
            foreach (string credDefJson in credDefJsons)
            {
                IntPtr newCredDefHandle = new IntPtr();
                _ = NativeMethods.anoncreds_credential_definition_from_json(ByteBuffer.Create(credDefJson), ref newCredDefHandle);
                credDefHandles.Add(newCredDefHandle);
            }
            
            int errorCode = NativeMethods.anoncreds_create_presentation(
                presentationRequestHandle,
                FfiCredentialEntryList.Create(credentialEntries),
                FfiCredentialProveList.Create(credentialProofs),
                FfiStrList.Create(selfAttestNames),
                FfiStrList.Create(selfAttestValues),
                FfiStr.Create(linkSecret),
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
            List<string> schemaIds,
            List<CredentialDefinition> credentialDefinitions,
             List<string> credentialDefinitionIds,
            List<RevocationRegistryDefinition> revocationRegistryDefinitions = null,
             List<string> revocationRegistryDefinitionIds = null,
            List<RevocationStatusList> revocationStatusList = null,
            List<NonrevokedIntervalOverride> nonrevokedIntervalOverrides = null)
        {
            byte verify = 0;

            revocationRegistryDefinitions = revocationRegistryDefinitions == null ? new List<RevocationRegistryDefinition>() : revocationRegistryDefinitions;
            revocationStatusList = revocationStatusList == null ? new List<RevocationStatusList>() : revocationStatusList;
            revocationRegistryDefinitionIds = revocationRegistryDefinitionIds == null ? new List<string>() : revocationRegistryDefinitionIds;
            List<IntPtr> schemaHandles =
                (from schema in schemas select schema.Handle).ToList();

            List<IntPtr> credDefHandles =
                (from credDef in credentialDefinitions select credDef.Handle).ToList();

            List<IntPtr> revRegDefHandles =
                (from revRegDef in revocationRegistryDefinitions select revRegDef.Handle).ToList();

            List<IntPtr> revStatusHandles =
                (from revStatus in revocationStatusList select revStatus.Handle).ToList();

            //If presentationRequestJson contains more than 1 restriction, a conversion in a format that the underlying rust code understands is needed i.e.:
            /**
            ...
            \"restrictions\":
             {\"$or\":
                 [{\"$and\":
                     [
                      {\"schema_id\":\"EU-Führerschein\"},
                      {\"cred_def_id\":\"DE-Führerschein\"}
                     ]
                 }]
             },
            ...
            **/
            string convertedPresReqJson = presentationRequest.ToQueryRequestJson();
            presentationRequest = await PresentationRequestApi.CreatePresReqFromJsonAsync(convertedPresReqJson);

            int errorCode = NativeMethods.anoncreds_verify_presentation(
                presentation.Handle,
                presentationRequest.Handle,
                FfiUIntList.Create(schemaHandles),
                FfiStrList.Create(schemaIds),
                FfiUIntList.Create(credDefHandles),
                FfiStrList.Create(credentialDefinitionIds),
                FfiUIntList.Create(revRegDefHandles),
                FfiStrList.Create(revocationRegistryDefinitionIds),
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
            List<string> schemaIds,
            List<string> credentialDefinitionJsons,
            List<string> credentialDefinitionIds,
            List<string> revocationRegistryDefinitionJsons = null,
            List<string> revocationRegistryDefinitionIds = null,
            List<string> revocationStatusListJsons = null,
            List<string> nonrevokedIntervalOverrideJsons = null)
        {
            byte verify = 0;

            IntPtr presentationHandle = new IntPtr();
            IntPtr presentationRequestHandle = new IntPtr();
            List<IntPtr> schemaHandles = new List<IntPtr>();
            List<IntPtr> credDefHandles = new List<IntPtr>();
            revocationRegistryDefinitionJsons = revocationRegistryDefinitionJsons == null ? new List<string>() : revocationRegistryDefinitionJsons;
            List<IntPtr> revRegDefHandles = new List<IntPtr>();
            List<string> revRegDefIds = revocationRegistryDefinitionIds == null ? new List<string>(): revocationRegistryDefinitionIds;
            revocationStatusListJsons = revocationStatusListJsons == null ? new List<string>() : revocationStatusListJsons;
            List<IntPtr> revocationStatusListHandles = new List<IntPtr>();
            nonrevokedIntervalOverrideJsons = nonrevokedIntervalOverrideJsons == null? new List<string>() : nonrevokedIntervalOverrideJsons;
            List<NonrevokedIntervalOverride> nonrevokedIntervalOverrides = new List<NonrevokedIntervalOverride>();

            _ = NativeMethods.anoncreds_presentation_from_json(ByteBuffer.Create(presentationJson), ref presentationHandle);

            //If presentationRequestJson contains more than 1 restriction, a conversion in a format that the underlying rust code understands is needed i.e.:
            /**
            ...
            \"restrictions\":
             {\"$or\":
                 [{\"$and\":
                     [
                      {\"schema_id\":\"EU-Führerschein\"},
                      {\"cred_def_id\":\"DE-Führerschein\"}
                     ]
                 }]
             },
            ...
            **/
            string convertedPresReqJson = (await PresentationRequestApi.CreatePresReqFromJsonAsync(presentationRequestJson)).ToQueryRequestJson();
            _ = NativeMethods.anoncreds_presentation_request_from_json(ByteBuffer.Create(convertedPresReqJson), ref presentationRequestHandle);

            foreach (string schemaJson in schemaJsons)
            {
                IntPtr newSchemaHandle = new IntPtr();
                _ = NativeMethods.anoncreds_schema_from_json(ByteBuffer.Create(schemaJson), ref newSchemaHandle);
                schemaHandles.Add(newSchemaHandle);
            }
            foreach (string credentialDefinitionJson in credentialDefinitionJsons)
            {
                IntPtr newCredentialDefinitionHandle = new IntPtr();
                _ = NativeMethods.anoncreds_credential_definition_from_json(ByteBuffer.Create(credentialDefinitionJson), ref newCredentialDefinitionHandle);
                credDefHandles.Add(newCredentialDefinitionHandle);
            }
            foreach (string revocationRegistryDefinitionJson in revocationRegistryDefinitionJsons)
            {
                IntPtr newRevocationRegistryDefinitionHandle = new IntPtr();
                _ = NativeMethods.anoncreds_revocation_registry_definition_from_json(ByteBuffer.Create(revocationRegistryDefinitionJson), ref newRevocationRegistryDefinitionHandle);
                revRegDefHandles.Add(newRevocationRegistryDefinitionHandle);
            }
            foreach (string revocationStatusListJson in revocationStatusListJsons)
            {
                IntPtr newRevStatusListHandle = new IntPtr();
                _ = NativeMethods.anoncreds_revocation_status_list_from_json(ByteBuffer.Create(revocationStatusListJson), ref newRevStatusListHandle);
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
                FfiStrList.Create(credentialDefinitionIds),
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