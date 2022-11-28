using System;

namespace anoncreds_rs_dotnet.Models
{
    public class MasterSecret
    {
        public SecretValue Value { get; set; }
        public IntPtr Handle { get; set; }
        public string JsonString { get; set; }
    }
}