using Newtonsoft.Json;
using System;

namespace anoncreds_rs_dotnet.Models
{
    public class NonRevokedInterval
    {
        [JsonProperty("from")]
        public ulong From { get; set; }
        [JsonProperty("to")]
        public ulong To { get; set; }
    }
}