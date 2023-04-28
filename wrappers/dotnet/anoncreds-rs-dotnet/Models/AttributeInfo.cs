using Newtonsoft.Json;
using System.Collections.Generic;

namespace anoncreds_rs_dotnet.Models
{
    public class AttributeInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("names")]
        public List<string> Names { get; set; }

        [JsonProperty("restrictions")]
        public List<AttributeFilter> Restrictions { get; set; }

        [JsonProperty("non_revoked")]
        public NonRevokedInterval NonRevoked { get; set; }
    }
}