using anoncreds_rs_dotnet.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static anoncreds_rs_dotnet.Models.Structures;

namespace anoncreds_rs_dotnet.Anoncreds
{
    public static class CredentialDefinitionApi
    {
        /// <summary>
        /// Creates a new <see cref="CredentialDefinition"/> from <see cref="Schema"/> (only signatureType "CL" supported so far).
        /// </summary>
        /// <param name="schemaId">Schema ID.</param>
        /// <param name="schemaObject">Corresponding schema.</param>
        /// <param name="tag">Tag name.</param>
        /// <param name="issuerId">Issuer ID.</param>
        /// <param name="signatureType">Type of the sginature.</param>
        /// <param name="supportRevocation">Flag if revocation is supported or not.</param>
        /// <exception cref="AnoncredsRsException">Throws if any provided parameters are invalid.</exception>
        /// <returns>The new <see cref="CredentialDefinition"/>, <see cref="CredentialDefinitionPrivate"/> and <see cref="CredentialKeyCorrectnessProof"/>.</returns>
        public static async Task<(CredentialDefinition, CredentialDefinitionPrivate, CredentialKeyCorrectnessProof)> CreateCredentialDefinitionAsync(
            string schemaId,
            Schema schemaObject,
            string tag,
            string issuerId,
            SignatureType signatureType,
            bool supportRevocation)
        {
            IntPtr credDefHandle = new IntPtr();
            IntPtr credDefPvtHandle = new IntPtr();
            IntPtr keyProofHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_create_credential_definition(
                FfiStr.Create(schemaId),
                schemaObject.Handle,
                FfiStr.Create(tag),
                FfiStr.Create(issuerId),
                FfiStr.Create(signatureType.ToString()),
                Convert.ToByte(supportRevocation),
                ref credDefHandle,
                ref credDefPvtHandle,
                ref keyProofHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            CredentialDefinition credDefObject = await CreateCredentialDefinitonObject(credDefHandle);
            CredentialDefinitionPrivate credDefPvtObject = await CreateCredentialDefinitonPrivateObject(credDefPvtHandle);
            CredentialKeyCorrectnessProof keyProofObject = await CreateCredentialKeyProofObject(keyProofHandle);
            return await Task.FromResult((credDefObject, credDefPvtObject, keyProofObject));
        }

        /// <summary>
        /// Creates a new <see cref="CredentialDefinition"/> as JSON string from <see cref="Schema"/> (only signatureType "CL" supported so far).
        /// </summary>
        /// <param name="schemaId">Schema ID.</param>
        /// <param name="schemaObjectJson">Corresponding schema.</param>
        /// <param name="tag">Tag name.</param>
        /// <param name="issuerId">Issuer ID.</param>
        /// <param name="signatureType">Type of the sginature.</param>
        /// <param name="supportRevocation">Flag if revocation is supported or not.</param>
        /// <exception cref="AnoncredsRsException">Throws if any provided parameters are invalid.</exception>
        /// <returns>The new <see cref="CredentialDefinition"/> as JSON string, <see cref="CredentialDefinitionPrivate"/> as JSON string and <see cref="CredentialKeyCorrectnessProof"/> as JSON String.</returns>
        public static async Task<(string, string, string)> CreateCredentialDefinitionJsonAsync(
            string schemaId,
            string schemaObjectJson,
            string tag,
            string issuerId,
            SignatureType signatureType,
            bool supportRevocation)
        {
            IntPtr credDefHandle = new IntPtr();
            IntPtr credDefPvtHandle = new IntPtr();
            IntPtr keyProofHandle = new IntPtr();
            IntPtr schemaObjectHandle = new IntPtr();
            _ = NativeMethods.anoncreds_schema_from_json(ByteBuffer.Create(schemaObjectJson), ref schemaObjectHandle);

            int errorCode = NativeMethods.anoncreds_create_credential_definition(
                FfiStr.Create(schemaId),
                schemaObjectHandle,
                FfiStr.Create(tag),
                FfiStr.Create(issuerId),
                FfiStr.Create(signatureType.ToString()),
                Convert.ToByte(supportRevocation),
                ref credDefHandle,
                ref credDefPvtHandle,
                ref keyProofHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            string credDefObjectJson = await ObjectApi.ToJsonAsync(credDefHandle);
            string credDefPvtObjectJson = await ObjectApi.ToJsonAsync(credDefPvtHandle);
            string keyProofObjectJson = await ObjectApi.ToJsonAsync(keyProofHandle);

            return await Task.FromResult((credDefObjectJson, credDefPvtObjectJson, keyProofObjectJson));
        }

        #region private methods
        /// <summary>
        /// Creates a <see cref="CredentialDefinition"/> object from JSON string./>.
        /// </summary>
        /// <param name="credDefJson">Credential definition as JSON string.</param>
        /// <exception cref="AnoncredsRsException">Throws when <paramref name="credDefJson"/> is invalid.</exception>
        /// <returns>A new <see cref="CredentialDefinition"/>.</returns>
        public static async Task<CredentialDefinition> CreateCredentialDefinitionFromJsonAsync(string credDefJson)
        {
            IntPtr credDefHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_credential_definition_from_json(ByteBuffer.Create(credDefJson), ref credDefHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            CredentialDefinition result = await CreateCredentialDefinitonObject(credDefHandle);
            return await Task.FromResult(result);
        }

        /// <summary>
        /// Creates a <see cref="CredentialDefinition"/> to a handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a credential definition.</param>
        /// <returns>A new <see cref="CredentialDefinition"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        private static async Task<CredentialDefinition> CreateCredentialDefinitonObject(IntPtr objectHandle)
        {
            string credDefJson = await ObjectApi.ToJsonAsync(objectHandle);
            CredentialDefinition credDefObject = JsonConvert.DeserializeObject<CredentialDefinition>(credDefJson, Settings.JsonSettings);
            credDefObject.JsonString = credDefJson;
            credDefObject.Handle = objectHandle;

            try
            {
                JObject jObj = JObject.Parse(credDefJson);
                credDefObject.Value.Primary.R = new List<KeyProofAttributeValue>();
                foreach (JToken ele in jObj["value"]["primary"]["r"])
                {
                    string[] attrFields = ele.ToString().Split(':');
                    KeyProofAttributeValue attribute = new KeyProofAttributeValue(JsonConvert.DeserializeObject<string>(attrFields[0]), JsonConvert.DeserializeObject<string>(attrFields[1]));
                    credDefObject.Value.Primary.R.Add(attribute);
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("Could not find field r.", e);
            }
            return await Task.FromResult(credDefObject);
        }

        /// <summary>
        /// Creates a <see cref="CredentialDefinitionPrivate"/> to a handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a CredentialDefinitionPrivate.</param>
        /// <returns>A new <see cref="CredentialDefinitionPrivate"/>.</returns>
        private static async Task<CredentialDefinitionPrivate> CreateCredentialDefinitonPrivateObject(IntPtr objectHandle)
        {
            string credDefPvtJson = await ObjectApi.ToJsonAsync(objectHandle);
            CredentialDefinitionPrivate credDefPvtObject = JsonConvert.DeserializeObject<CredentialDefinitionPrivate>(credDefPvtJson, Settings.JsonSettings);
            credDefPvtObject.JsonString = credDefPvtJson;
            credDefPvtObject.Handle = objectHandle;
            return await Task.FromResult(credDefPvtObject);
        }

        /// <summary>
        /// Creates a <see cref="CredentialKeyCorrectnessProof"/> to a handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a CredentialKeyCorrectnessProof.</param>
        /// <returns>A new <see cref="CredentialKeyCorrectnessProof"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        private static async Task<CredentialKeyCorrectnessProof> CreateCredentialKeyProofObject(IntPtr objectHandle)
        {
            string keyProofJson = await ObjectApi.ToJsonAsync(objectHandle);
            CredentialKeyCorrectnessProof keyProofObject = JsonConvert.DeserializeObject<CredentialKeyCorrectnessProof>(keyProofJson, Settings.JsonSettings);
            keyProofObject.JsonString = keyProofJson;
            keyProofObject.Handle = objectHandle;

            try
            {
                JObject jObj = JObject.Parse(keyProofJson);
                keyProofObject.XrCap = new List<KeyProofAttributeValue>();
                foreach (JToken ele in jObj["xr_cap"])
                {
                    KeyProofAttributeValue attribute = new KeyProofAttributeValue(ele.First.ToString(), ele.Last.ToString());
                    keyProofObject.XrCap.Add(attribute);
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("Could not find field xr_cap.", e);
            }
            return await Task.FromResult(keyProofObject);
        }
        #endregion
    }
}