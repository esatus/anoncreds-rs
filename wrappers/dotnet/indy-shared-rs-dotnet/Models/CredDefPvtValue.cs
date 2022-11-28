using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class CredDefPvtValue
    {
        [JsonProperty("p_key")]
        public PKey PKey { get; set; }
        [JsonProperty("r_key")]
        public RKey RKey { get; set; }
    }
}