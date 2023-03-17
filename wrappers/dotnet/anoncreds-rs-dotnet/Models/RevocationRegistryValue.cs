using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class RevocationRegistryValue
    {
        [JsonProperty("accum")]
        public string Accum { get; set; }
    }
}