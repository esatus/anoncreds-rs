using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace anoncreds_rs_dotnet.Models
{
    public class PresentationRequest
    {
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        public string JsonString { get; set; }
        [JsonProperty("requested_attributes")]
        public Dictionary<string, AttributeInfo> RequestedAttributes { get; set; }
        [JsonProperty("requested_predicates")]
        public Dictionary<string, PredicateInfo> RequestedPredicates { get; set; }
        [JsonProperty("non_revoked")]
        public NonRevokedInterval NonRevoked { get; set; }
        public IntPtr Handle { get; set; }
    }
}