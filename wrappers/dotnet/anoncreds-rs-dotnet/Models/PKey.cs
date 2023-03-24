using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class PKey
    {
        [JsonProperty("p")]
        public string P { get; set; }
        [JsonProperty("q")]
        public string Q { get; set; }
    }
}