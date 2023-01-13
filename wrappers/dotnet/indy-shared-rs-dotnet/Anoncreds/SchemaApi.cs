using anoncreds_rs_dotnet.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static anoncreds_rs_dotnet.Models.Structures;

namespace anoncreds_rs_dotnet.Anoncreds
{
    public class SchemaApi
    {
        /// <summary>
        /// Creates a new <see cref="Schema"/> object.
        /// </summary>
        /// <param name="originDid">Did of issuer.</param>
        /// <param name="schemaName">Schema name.</param>
        /// <param name="schemaVersion">Version of schema.</param>
        /// <param name="attrNames">Names of the schema attributes.</param>
        /// <param name="seqNo">Sequence number.</param>
        /// <exception cref="AnoncredsRsException">Throws when any parameter is invalid.</exception>
        /// <exception cref="System.InvalidOperationException">Throws when <paramref name="attrNames"/> are empty.</exception>
        /// <returns>A new <see cref="Schema"/> object.</returns>
        public static async Task<Schema> CreateSchemaAsync(
            string originDid, 
            string schemaName, 
            string schemaVersion, 
            List<string> attrNames 
            )
        {
            IntPtr schemaObjectHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_create_schema(FfiStr.Create(schemaName), FfiStr.Create(schemaVersion), FfiStr.Create(originDid), FfiStrList.Create(attrNames), ref schemaObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            Schema schemaObject = await CreateSchemaObjectAsync(schemaObjectHandle);
            return await Task.FromResult(schemaObject);
        }

        /// <summary>
        /// Creates a new <see cref="Schema"/> as JSON string.
        /// </summary>
        /// <param name="originDid">Did of issuer.</param>
        /// <param name="schemaName">Schema name.</param>
        /// <param name="schemaVersion">Version of schema.</param>
        /// <param name="attrNames">Names of the schema attributes.</param>
        /// <param name="seqNo">Sequence number.</param>
        /// <exception cref="AnoncredsRsException">Throws when any parameter is invalid.</exception>
        /// <exception cref="InvalidOperationException">Throws when <paramref name="attrNames"/> are empty.</exception>
        /// <returns>A new <see cref="Schema"/> as JSON string.</returns>
        public static async Task<string> CreateSchemaJsonAsync(
            string originDid, 
            string schemaName, 
            string schemaVersion, 
            List<string> attrNames
            )
        {
            IntPtr schemaObjectHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_create_schema(FfiStr.Create(schemaName), FfiStr.Create(schemaVersion), FfiStr.Create(originDid), FfiStrList.Create(attrNames), ref schemaObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            return await ObjectApi.ToJsonAsync(schemaObjectHandle);
        }

        /// <summary>
        /// Creates a new <see cref="Schema"/> object from a JSON string.
        /// </summary>
        /// <param name="schemaJson">A <see cref="Schema"/>  as JSON string.</param>
        /// <exception cref="AnoncredsRsException">Throws when provided <paramref name="schemaJson"/> is invalid.</exception>
        /// <exception cref="System.IndexOutOfRangeException">Throws when <paramref name="schemaJson"/> is empty.</exception>
        /// <returns>A new <see cref="Schema"/> object.</returns>
        public static async Task<Schema> CreateSchemaFromJsonAsync(string schemaJson)
        {
            IntPtr schemaObjectHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_schema_from_json(ByteBuffer.Create(schemaJson), ref schemaObjectHandle);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            Schema schemaObject = await CreateSchemaObjectAsync(schemaObjectHandle);
            return await Task.FromResult(schemaObject);
        }

        /// <summary>
        /// Create a <see cref="Schema"/> to a handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a schema.</param>
        /// <returns>A new <see cref="Schema"/>.</returns>
        private static async Task<Schema> CreateSchemaObjectAsync(IntPtr objectHandle)
        {
            string schemaJson = await ObjectApi.ToJsonAsync(objectHandle);
            Schema schemaObject = JsonConvert.DeserializeObject<Schema>(schemaJson, Settings.JsonSettings);
            schemaObject.JsonString = schemaJson;
            schemaObject.Handle = objectHandle;
            return await Task.FromResult(schemaObject);
        }
    }
}