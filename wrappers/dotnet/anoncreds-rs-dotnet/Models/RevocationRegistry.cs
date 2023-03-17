using Newtonsoft.Json;
using System;

namespace anoncreds_rs_dotnet.Models
{
    public class RevocationRegistry
    {
        public IntPtr Handle { get; set; }

        public string JsonString { get; set; }

        [JsonProperty("ver")]
        public string Ver { get; set; }

        [JsonProperty("value")]
        public RevocationRegistryValue Value { get; set; }
    }
}