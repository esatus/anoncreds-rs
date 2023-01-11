using System.Threading.Tasks;

namespace anoncreds_rs_dotnet.Anoncreds
{
    public static class ErrorApi
    {
        /// <summary>
        /// Returns a JSON string of the last thrown native error.
        /// </summary>
        /// <returns>Error JSON in form of <c>{"code":[int],"message":[string]}</c>.</returns>
        public static Task<string> GetCurrentErrorAsync()
        {
            string result = "";
            _ = NativeMethods.anoncreds_get_current_error(ref result);
            return Task.FromResult(result);
        }
    }
}