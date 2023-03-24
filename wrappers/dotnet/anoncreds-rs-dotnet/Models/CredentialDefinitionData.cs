using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class CredentialDefinitionData
    {
        [JsonProperty("primary")]
        public Primary Primary { get; set; }
        [JsonProperty("revocation")]
        public Revocation Revocation { get; set; }
    }
}