using anoncreds_rs_dotnet.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using static anoncreds_rs_dotnet.Models.Structures;

namespace anoncreds_rs_dotnet.Anoncreds
{
    public static class MasterSecretApi
    {
        /// <summary>
        /// Creates a new <see cref="MasterSecret"/>.
        /// </summary>
        /// <exception cref="AnoncredsRsException">Throws when <see cref="MasterSecret"/> can't be created.</exception>
        /// <returns>New <see cref="MasterSecret"/>.</returns>
        public static async Task<MasterSecret> CreateMasterSecretAsync()
        {
            IntPtr result = new IntPtr();
            int errorCode = NativeMethods.anoncreds_create_master_secret(ref result);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            string masterSecretJson = await ObjectApi.ToJsonAsync(result);
            MasterSecret msObject = JsonConvert.DeserializeObject<MasterSecret>(masterSecretJson, Settings.JsonSettings);
            msObject.JsonString = masterSecretJson;
            msObject.Handle = result;
            return await Task.FromResult(msObject);
        }
        /// <summary>
        /// Creates a new <see cref="MasterSecret"/> as JSON string.
        /// </summary>
        /// <exception cref="AnoncredsRsException">Throws when <see cref="MasterSecret"/> can't be created.</exception>
        /// <returns>New <see cref="MasterSecret"/> as JSON string.</returns>
        public static async Task<string> CreateMasterSecretJsonAsync()
        {
            IntPtr result = new IntPtr();
            int errorCode = NativeMethods.anoncreds_create_master_secret(ref result);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            string masterSecretJson = await ObjectApi.ToJsonAsync(result);

            return await Task.FromResult(masterSecretJson);
        }
    }
}