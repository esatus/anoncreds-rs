using System;

namespace anoncreds_rs_dotnet.Models
{
    public class CredentialDefinitionPrivate
    {
        public IntPtr Handle { get; set; }
        public string JsonString { get; set; }
        public CredDefPvtValue Value { get; set; }
    }
}