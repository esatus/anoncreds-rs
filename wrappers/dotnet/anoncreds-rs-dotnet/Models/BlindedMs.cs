using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace anoncreds_rs_dotnet.Models
{
    public class BlindedMs
    {
        [JsonProperty("u")]
        public string U { get; set; }
        [JsonProperty("ur")]
        public string Ur { get; set; }

        [JsonProperty("hidden_attributes")]
        public List<string> HiddenAttributes { get; set; }

        [JsonProperty("committed_attributes")]
        public JObject ComittedAttributes { get; set; }
    }
}