using Newtonsoft.Json;
using System.Collections.Generic;

namespace anoncreds_rs_dotnet.Models
{
    public class Primary
    {
        [JsonProperty("n")]
        public string N { get; set; }
        [JsonProperty("s")]
        public string S { get; set; }

        [JsonProperty("r")]
        public List<KeyProofAttributeValue> R { get; set; }
        
        [JsonProperty("rctxt")]
        public string Rctxt { get; set; }
        [JsonProperty("z")]
        public string Z { get; set; }
    }
}