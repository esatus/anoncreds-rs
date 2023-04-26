﻿using anoncreds_rs_dotnet.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static anoncreds_rs_dotnet.Models.Structures;

namespace anoncreds_rs_dotnet.Anoncreds
{
    public static class CredentialOfferApi
    {
        /// <summary>
        /// Creates a <see cref="CredentialOffer"/> to a given <see cref="CredentialDefinition"/>.
        /// </summary>
        /// <param name="schemaId">Id of the corresponding schema.</param>
        /// <param name="credDefObject">Credential definition.</param>
        /// <param name="keyProofObject">Key correctness proof.</param>
        /// <exception cref="AnoncredsRsException">Throws if any parameter is invalid.</exception>
        /// <returns>A new <see cref="CredentialOffer"/>.</returns>
        public static async Task<CredentialOffer> CreateCredentialOfferAsync(
            string schemaId,
            string credDefId,
            CredentialKeyCorrectnessProof keyProofObject)
        {
            IntPtr credOfferObjectHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_create_credential_offer(FfiStr.Create(schemaId), FfiStr.Create(credDefId), keyProofObject.Handle, ref credOfferObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            CredentialOffer credOfferObject = await CreateCredentialOfferObject(credOfferObjectHandle);
            return await Task.FromResult(credOfferObject);
        }

        /// <summary>
        /// Creates a <see cref="CredentialOffer"/> to a given handle of a <see cref="CredentialDefinition"/>.
        /// </summary>
        /// <param name="schemaId">Id of the corresponding schema.</param>
        /// <param name="credDefObjectJson">Credential definition as JSON string.</param>
        /// <param name="keyProofObjectJson">Key correctness proof as JSON string.</param>
        /// <exception cref="AnoncredsRsException">Throws if any parameter is invalid.</exception>
        /// <returns>A new <see cref="CredentialOffer"/> as JSON string.</returns>
        public static async Task<string> CreateCredentialOfferJsonAsync(
            string schemaId,
            string credDefId,
            string keyProofObjectJson)
        {
            IntPtr keyProofObjecthandle = new IntPtr();
            _ = NativeMethods.anoncreds_key_correctness_proof_from_json(ByteBuffer.Create(keyProofObjectJson), ref keyProofObjecthandle);

            IntPtr credOfferObjectHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_create_credential_offer(FfiStr.Create(schemaId), FfiStr.Create(credDefId), keyProofObjecthandle, ref credOfferObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            string credOfferJson = await ObjectApi.ToJsonAsync(credOfferObjectHandle);
            return await Task.FromResult(credOfferJson);
        }

        #region private methods
        /// <summary>
        /// Creates a <see cref="CredentialOffer"/> to a given handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a credential offer.</param>
        /// <returns>A <see cref="CredentialOffer"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        private static async Task<CredentialOffer> CreateCredentialOfferObject(IntPtr objectHandle)
        {
            string credOfferJson = await ObjectApi.ToJsonAsync(objectHandle);
            CredentialOffer credOfferObject = JsonConvert.DeserializeObject<CredentialOffer>(credOfferJson, Settings.JsonSettings);
            credOfferObject.JsonString = credOfferJson;
            credOfferObject.Handle = objectHandle;

            try
            {
                JObject jObj = JObject.Parse(credOfferJson);
                credOfferObject.KeyCorrectnessProof.XrCap = new List<KeyProofAttributeValue>();
                foreach (JToken ele in jObj["key_correctness_proof"]["xr_cap"])
                {
                    KeyProofAttributeValue attribute = new KeyProofAttributeValue(ele.First.ToString(), ele.Last.ToString());
                    credOfferObject.KeyCorrectnessProof.XrCap.Add(attribute);
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("Could not find field xr_cap.", e);
            }

            return await Task.FromResult(credOfferObject);
        }
        #endregion
    }
}