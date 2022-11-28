using System.Threading.Tasks;

namespace indy_shared_rs_dotnet.Anoncreds
{
    public static class ErrorApi
    {
        /// <summary>
        /// Returns json <see cref="string"/> of the last thrown native error.
        /// </summary>
        /// <returns>Error json in form of <c>{"code":[int],"message":[string]}</c>.</returns>
        public static Task<string> GetCurrentErrorAsync()
        {
            string result = "";
            _ = NativeMethods.anoncreds_get_current_error(ref result);
            return Task.FromResult(result);
        }
    }
}