using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class SecretValue
    {
        [JsonProperty("ms")]
        public string Ms { get; set; }
    }
}