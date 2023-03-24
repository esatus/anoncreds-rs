using Newtonsoft.Json;
using System;

namespace anoncreds_rs_dotnet.Models
{
    public class CredentialDefinition
    {
        public IntPtr Handle { get; set; }
        public string JsonString { get; set; }

        // TODO : IssuerId oder so lassen? sieht auch bei Schema classe
        [JsonProperty("issuerId")]
        public string IssuerId { get; set; }

        [JsonProperty("schemaId")]
        public string SchemaId { get; set; }

        [JsonProperty("type")]
        public string SignatureType { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("ver")]
        public string Ver { get; set; }
        [JsonProperty("value")]
        public CredentialDefinitionData Value { get; set; }
    }
}