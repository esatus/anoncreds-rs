using Newtonsoft.Json;
using System.Collections.Generic;

namespace anoncreds_rs_dotnet.Models
{
    public class PredicateInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("p_type")]
        public PredicateTypes PredicateType { get; set; }
        [JsonProperty("p_value")]
        public int PredicateValue { get; set; }
        [JsonProperty("restrictions")]
        public List<AttributeFilter> Restrictions { get; set; }
        [JsonProperty("non_revoked")]
        public NonRevokedInterval NonRevoked { get; set; }
    }
}