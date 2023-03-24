using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class RKey
    {
        [JsonProperty("x")]
        public string X { get; set; }
        [JsonProperty("sk")]
        public string Sk { get; set; }
    }
}