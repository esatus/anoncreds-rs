using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class KeyProofAttributeValue
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }

        public KeyProofAttributeValue(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}