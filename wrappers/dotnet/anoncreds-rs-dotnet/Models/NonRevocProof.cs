using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class NonRevocProof
    {
        [JsonProperty("x_list")]
        public NonRevocProofXList XList { get; set; }

        [JsonProperty("c_list")]
        public NonRevocProofCList CList { get; set; }
    }
}