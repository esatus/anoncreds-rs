﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace anoncreds_rs_dotnet.Models
{
    public class PrimaryProof
    {
        [JsonProperty("eq_proof")]
        public PrimaryEqualProof EqProof { get; set; }

        [JsonProperty("ne_proofs")]
        public List<PrimaryPredicateInequalityProof> NeProofs { get; set; }
    }
}