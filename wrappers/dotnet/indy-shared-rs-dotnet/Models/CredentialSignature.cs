using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class CredentialSignature
    {
        [JsonProperty("p_credential")]
        public PrimaryCredentialSignature PCredential { get; set; }
        [JsonProperty("r_credential")]
        public NonRevocationCredentialSignature RCredential { get; set; }
    }
}