using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace anoncreds_rs_dotnet.Models
{
    public class Schema
    {
        public IntPtr Handle { get; set; }
        public string JsonString { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("attrNames")]
        public HashSet<string> AttrNames { get; set; }
        [JsonProperty("issuerId")]
        public string IssuerId { get; set; }
        //workaround: 'ver' field is needed for current indy_vdr implementation, maybe updated version of indy_vdr in the future?
        [JsonProperty("ver")]
        public string Ver { get; set; }

    }
}