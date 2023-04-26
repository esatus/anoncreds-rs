using anoncreds_rs_dotnet.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using static anoncreds_rs_dotnet.Models.Structures;

namespace anoncreds_rs_dotnet.Anoncreds
{
    public static class LinkSecretApi
    {
        /// <summary>
        /// Creates a new <see cref="LinkSecret"/>.
        /// </summary>
        /// <exception cref="AnoncredsRsException">Throws when <see cref="LinkSecret"/> can't be created.</exception>
        /// <returns>New <see cref="LinkSecret"/>.</returns>
        public static async Task<LinkSecret> CreateLinkSecretAsync()
        {
            string result = "";
            int errorCode = NativeMethods.anoncreds_create_link_secret(ref result);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            //string linkSecret = await ObjectApi.ToJsonAsync(result.data);
            LinkSecret msObject = JsonConvert.DeserializeObject<LinkSecret>(result, Settings.JsonSettings);
            msObject.JsonString = result;
            //msObject.Handle = result.data;
            return await Task.FromResult(msObject);
        }
        /// <summary>
        /// Creates a new <see cref="LinkSecret"/> as JSON string.
        /// </summary>
        /// <exception cref="AnoncredsRsException">Throws when <see cref="LinkSecret"/> can't be created.</exception>
        /// <returns>New <see cref="LinkSecret"/> as JSON string.</returns>
        public static async Task<string> CreateLinkSecretJsonAsync()
        {
            string result = "";
            int errorCode = NativeMethods.anoncreds_create_link_secret(ref result);

            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }

            string linkSecretJson = result;

            return await Task.FromResult(linkSecretJson);
        }
    }
}