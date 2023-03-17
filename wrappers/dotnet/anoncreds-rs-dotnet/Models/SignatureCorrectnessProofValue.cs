using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class SignatureCorrectnessProofValue
    {
        [JsonProperty("se")]
        public string Se { get; set; }

        [JsonProperty("c")]
        public string C { get; set; }
    }
}