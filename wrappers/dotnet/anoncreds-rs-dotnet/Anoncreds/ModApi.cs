using anoncreds_rs_dotnet.Models;
using System.Threading.Tasks;
using static anoncreds_rs_dotnet.Models.Structures;

namespace anoncreds_rs_dotnet.Anoncreds
{
    public static class ModApi
    { 
        public static Task SetBufferFreeAsync(ByteBuffer secretBytes)
        {
            NativeMethods.anoncreds_buffer_free(secretBytes);
            return Task.CompletedTask;
        }


        /// <summary>
        /// Sets the default logger.
        /// </summary>
        /// <exception cref="AnoncredsRsException">Throws when logger can not be set.</exception>
        public static async Task SetDefaultLoggerAsync()
        {
            int errorCode = NativeMethods.anoncreds_set_default_logger();
            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }
        }

        /// <summary>
        /// Gets the current library version.
        /// </summary>
        /// <returns>Version number as <see cref="string"/> representation.</returns>
        public static Task<string> GetVersionAsync()
        {
            string result = NativeMethods.anoncreds_version();
            return Task.FromResult(result);
        }
    }
}