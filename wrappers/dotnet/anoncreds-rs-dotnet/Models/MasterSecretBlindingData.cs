﻿using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class MasterSecretBlindingData
    {
        [JsonProperty("v_prime")]
        public string VPrime { get; set; }
        [JsonProperty("vr_prime")]
        public string VrPrime { get; set; }
    }
}