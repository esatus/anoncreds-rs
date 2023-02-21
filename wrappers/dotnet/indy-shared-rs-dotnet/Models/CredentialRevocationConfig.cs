using System;
using System.Collections.Generic;

namespace anoncreds_rs_dotnet.Models
{
    public class CredentialRevocationConfig
    {
        public IntPtr RevRegDefObjectHandle;
        public IntPtr RevRegDefPvtObjectHandle;
        public long RegIdx;
        public string TailsPath;
    }
}