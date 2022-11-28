using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class RevocationRegistryDefinitionPrivateValue
    {
        [JsonProperty("gamma")]
        public string Gamma { get; set; }
    }
}