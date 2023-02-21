using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace anoncreds_rs_dotnet.Models
{
    public class RevocationStatusList
    {
        public IntPtr Handle { get; set; }

        public string JsonString { get; set; }

        [JsonProperty("revRegDefId")]
        public string RevocationRegistryDefinitionId { get; set; }

        [JsonProperty("revocationList")]
        public HashSet<(byte ,uint)> RevocationList { get; set; }

        [JsonProperty("registry")]
        public RevocationRegistry RevocationRegistry { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
    }
}