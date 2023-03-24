using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class Revocation
    {
        [JsonProperty("g")]
        public string G { get; set; }

        [JsonProperty("g_dash")]
        public string GDash { get; set; }
        [JsonProperty("h")]
        public string H { get; set; }
        [JsonProperty("h0")]
        public string H0 { get; set; }
        [JsonProperty("h1")]
        public string H1 { get; set; }
        [JsonProperty("h2")]
        public string H2 { get; set; }
        [JsonProperty("htilde")]
        public string HTilde { get; set; }

        [JsonProperty("h_cap")]
        public string HCap { get; set; }
        [JsonProperty("u")]
        public string U { get; set; }
        [JsonProperty("pk")]
        public string Pk { get; set; }
        [JsonProperty("y")]
        public string Y { get; set; }
    }
}