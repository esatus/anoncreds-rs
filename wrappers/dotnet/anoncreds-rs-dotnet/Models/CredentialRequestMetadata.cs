using Newtonsoft.Json;
using System;

namespace anoncreds_rs_dotnet.Models
{
    public class CredentialRequestMetadata
    {
        [JsonProperty("master_secret_blinding_data")]
        public MasterSecretBlindingData MsBlindingData { get; set; }
        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("master_secret_name")]
        public string MsName { get; set; }
        public IntPtr Handle { get; set; }
        public string JsonString { get; set; }
    }
}