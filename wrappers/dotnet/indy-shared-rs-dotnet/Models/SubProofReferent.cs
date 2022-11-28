using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class SubProofReferent
    {
        [JsonProperty("sub_proof_index")]
        public uint SubProofIndex { get; set; }
    }
}