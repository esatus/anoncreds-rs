using System;

namespace anoncreds_rs_dotnet.Models
{
    public class RevocationRegistryEntry
    {
        public long DefEntryIdx { get; set; }
        public IntPtr Entry { get; set; }
        public long Timestamp { get; set; }
    }
}