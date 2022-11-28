using Newtonsoft.Json;

namespace anoncreds_rs_dotnet.Models
{
    public class MCaps
    {
        [JsonProperty("master_secret")]
        public string MasterSecret { get; set; }
    }
}