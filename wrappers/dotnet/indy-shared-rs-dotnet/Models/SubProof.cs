using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class SubProof
    {
        [JsonProperty("primary_proof")]
        public PrimaryProof PrimaryProof { get; set; }

        [JsonProperty("non_revoc_proof")]
        public NonRevocProof NonRevocProof { get; set; } = null;
    }
}