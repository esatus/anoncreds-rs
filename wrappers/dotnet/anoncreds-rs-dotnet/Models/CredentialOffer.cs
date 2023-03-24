using Newtonsoft.Json;
using System;

namespace anoncreds_rs_dotnet.Models
{
    public class CredentialOffer
    {
        [JsonProperty("schema_id")]
        public string SchemaId { get; set; }

        [JsonProperty("cred_def_id")]
        public string CredentialDefinitionId { get; set; }

        [JsonProperty("key_correctness_proof")]
        public CredentialKeyCorrectnessProof KeyCorrectnessProof { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("method_name")]
        public string MethodName { get; set; }

        public string JsonString { get; set; }
        public IntPtr Handle { get; set; }
    }
}