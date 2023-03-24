﻿using Newtonsoft.Json;
using System;

namespace anoncreds_rs_dotnet.Models
{
    public class CredentialRequest
    {
        [JsonProperty("prover_did")]
        public string ProverDid { get; set; }

        [JsonProperty("cred_def_id")]
        public string CredentialDefinitionId { get; set; }

        [JsonProperty("blinded_ms")]
        public BlindedMs BlindedMs { get; set; }

        [JsonProperty("blinded_ms_correctness_proof")]
        public BlindingMsCorrectnessProof BlindedMsCorrectnessProof { get; set; }
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        public IntPtr Handle { get; set; }
        public string JsonString { get; set; }
    }
}