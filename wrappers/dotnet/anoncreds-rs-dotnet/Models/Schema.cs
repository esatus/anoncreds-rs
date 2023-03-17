using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace anoncreds_rs_dotnet.Models
{
    public class Schema
    {
        public IntPtr Handle { get; set; }
        public string JsonString { get; set; }

        public string Name { get; set; }
        public string Version { get; set; }
        public HashSet<string> AttrNames { get; set; }
        public string IssuerId { get; set; }

        
    }
}