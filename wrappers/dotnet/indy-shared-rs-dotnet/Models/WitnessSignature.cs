using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class WitnessSignature
    {
        [JsonProperty("sigma_i")]
        public string SigmaI { get; set; }

        [JsonProperty("u_i")]
        public string Ui { get; set; }

        [JsonProperty("g_i")]
        public string Gi { get; set; }
    }
}