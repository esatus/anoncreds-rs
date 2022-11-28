using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class Witness
    {
        [JsonProperty("omega")]
        public string Omega { get; set; }
    }
}