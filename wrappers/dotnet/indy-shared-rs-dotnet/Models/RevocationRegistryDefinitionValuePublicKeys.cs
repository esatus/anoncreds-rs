using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class RevocationRegistryDefinitionValuePublicKeys
    {
        [JsonProperty("accumKey")]
        public AccumKey AccumKey { get; set; }
    }
}