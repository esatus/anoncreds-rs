using anoncreds_rs_dotnet.Anoncreds;
using anoncreds_rs_dotnet.Models;
using Newtonsoft.Json;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace anoncreds_rs_dotnet_test
{
    internal class MockDataProvider
    {
        public static Schema Schema { get; set; }
        public static string SchemaJson { get; set; }
        public static CredentialDefinition CredentialDefinition { get; set; }
        public static CredentialDefinitionPrivate CredentialDefinitionPrivate { get; set; }
        public static CredentialKeyCorrectnessProof CredentialKeyCorrectnessProof { get; set; }
        public static string CredentialDefinitionJson { get; set; }
        public static string CredentialDefinitionPrivateJson { get; set; }
        public static string CredentialKeyCorrectnessProofJson { get; set; }
        public static CredentialOffer CredentialOffer { get; set; }
        public static string CredentialOfferJson { get; set; }
        public static CredentialRequest CredentialRequest { get; set; }
        public static CredentialRequestMetadata CredentialRequestMetadata { get; set; }
        public static string CredentialRequestJson { get; set; }
        public static string CredentialRequestMetadataJson { get; set; }

        public static async Task<Schema> MockSchema(string issuerUri = null, string schemaName = null, string schemaVersion = null, List<string> attrNames = null)
        {
            string mockSchemaIssuerUri = issuerUri ?? "mock:SchemaIssuerUri";
            string mockSchemaName = schemaName ?? "mockSchemaName";
            string mockSchemaVersion = schemaVersion ?? "mockSchemaVersion";
            List<string> mockSchemaAttrNames = attrNames ?? new() { "attribute1", "attribute2", "attribute3" };

            Schema = await SchemaApi.CreateSchemaAsync(mockSchemaIssuerUri, mockSchemaName, mockSchemaVersion, mockSchemaAttrNames);
            return Schema;
        }

        public static async Task<string> MockSchemaJson(string issuerUri = null, string schemaName = null, string schemaVersion = null, List<string> attrNames = null)
        {
            string mockSchemaIssuerUri = issuerUri ?? "mock:SchemaIssuerUri";
            string mockSchemaName = schemaName ?? "mockSchemaName";
            string mockSchemaVersion = schemaVersion ?? "mockSchemaVersion";
            List<string> mockSchemaAttrNames = attrNames ?? new() { "attribute1", "attribute2", "attribute3" };

            SchemaJson = await SchemaApi.CreateSchemaJsonAsync(mockSchemaIssuerUri, mockSchemaName, mockSchemaVersion, mockSchemaAttrNames);
            return SchemaJson;
        }

        public static async Task<(CredentialDefinition, CredentialDefinitionPrivate, CredentialKeyCorrectnessProof)> MockCredDef(
            Schema schema = null,
            string tag = null,
            string issuerUri = null,
            SignatureType signatureType = SignatureType.CL,
            bool supportRevocation = true)
        {
            string mockSchemaIssuerUri = "mock:SchemaIssuerUri";
            string mockSchemaName = "mockSchemaName";
            string mockSchemaVersion = "mockSchemaVersion";
            List<string> mockSchemaAttrNames = new() { "attribute1", "attribute2", "attribute3" };
            if (schema != null)
            {
                mockSchemaIssuerUri = schema.IssuerId;
                mockSchemaName = schema.Name;
                mockSchemaVersion = schema.Version;
                mockSchemaAttrNames = schema.AttrNames.ToList();
            }
            else if (schema == null && Schema != null)
            {
                mockSchemaIssuerUri = Schema.IssuerId;
                mockSchemaName = Schema.Name;
                mockSchemaVersion = Schema.Version;
                mockSchemaAttrNames = Schema.AttrNames.ToList();
            }

            Schema mockSchema = await SchemaApi.CreateSchemaAsync(mockSchemaIssuerUri, mockSchemaName, mockSchemaVersion, mockSchemaAttrNames);

            string mockCredDefTag = tag ?? "mockCredDefTag";
            string mockCredDefIssuerUri = issuerUri ?? "mock:CredDefIssuerUri";

            (CredentialDefinition, CredentialDefinitionPrivate, CredentialKeyCorrectnessProof) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(mockSchema.IssuerId, mockSchema, mockCredDefTag, mockCredDefIssuerUri, signatureType, supportRevocation);
            return (CredentialDefinition, CredentialDefinitionPrivate, CredentialKeyCorrectnessProof);
        }

        public static async Task<(string, string, string)> MockCredDefJson(
            Schema schema = null,
            string tag = null,
            string issuerUri = null,
            SignatureType signatureType = SignatureType.CL,
            bool supportRevocation = true)
        {
            string mockSchemaIssuerUri = "mock:SchemaIssuerUri";
            string mockSchemaName = "mockSchemaName";
            string mockSchemaVersion = "mockSchemaVersion";
            List<string> mockSchemaAttrNames = new() { "attribute1", "attribute2", "attribute3" };
            if (schema != null)
            {
                mockSchemaIssuerUri = schema.IssuerId;
                mockSchemaName = schema.Name;
                mockSchemaVersion = schema.Version;
                mockSchemaAttrNames = schema.AttrNames.ToList();
            }
            else if (schema == null && Schema != null)
            {
                mockSchemaIssuerUri = Schema.IssuerId;
                mockSchemaName = Schema.Name;
                mockSchemaVersion = Schema.Version;
                mockSchemaAttrNames = Schema.AttrNames.ToList();
            }

            string mockSchemaJson = await SchemaApi.CreateSchemaJsonAsync(mockSchemaIssuerUri, mockSchemaName, mockSchemaVersion, mockSchemaAttrNames);

            string mockCredDefTag = tag ?? "mockCredDefTag";
            string mockCredDefIssuerUri = issuerUri ?? "mock:CredDefIssuerUri";

            (CredentialDefinitionJson, CredentialDefinitionPrivateJson, CredentialKeyCorrectnessProofJson) = await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(mockSchemaIssuerUri, mockSchemaJson, mockCredDefTag, mockCredDefIssuerUri, signatureType, supportRevocation);
            return (CredentialDefinitionJson, CredentialDefinitionPrivateJson, CredentialKeyCorrectnessProofJson);
        }

        public static async Task<CredentialOffer> MockCredOffer(string schemaIssuerUri = null, string credDefIssuerUri = null, CredentialKeyCorrectnessProof credentialKeyCorrectnessProof = null)
        {
            string mockSchemaIssuerUri = schemaIssuerUri ?? "mock:SchemaIssuerUri";
            string mockCredDefIssuerUri = credDefIssuerUri ?? "mock:CredDefIssuerUri";
            CredentialKeyCorrectnessProof mockKCP = credentialKeyCorrectnessProof ?? CredentialKeyCorrectnessProof ?? (await MockCredDef()).Item3;

            CredentialOffer = await CredentialOfferApi.CreateCredentialOfferAsync(mockSchemaIssuerUri, mockCredDefIssuerUri, mockKCP);
            return CredentialOffer;
        }

        public static async Task<string> MockCredOfferJson(string schemaIssuerUri = null, string credDefIssuerUri = null, string credentialKeyCorrectnessProofJson = null)
        {
            string mockSchemaIssuerUri = schemaIssuerUri ?? "mock:SchemaIssuerUri";
            string mockCredDefIssuerUri = credDefIssuerUri ?? "mock:CredDefIssuerUri";
            string mockKCPJson = credentialKeyCorrectnessProofJson ?? CredentialKeyCorrectnessProofJson ?? (await MockCredDefJson()).Item3;

            CredentialOfferJson = await CredentialOfferApi.CreateCredentialOfferJsonAsync(mockSchemaIssuerUri, mockCredDefIssuerUri, mockKCPJson);
            return CredentialOfferJson;
        }

        public static async Task<(CredentialRequest, CredentialRequestMetadata)> MockCredReq(string entropy = null, CredentialDefinition credDef = null, MasterSecret masterSecret = null, string masterSecretName = null, CredentialOffer credOffer = null)
        {
            string mockEntropy = entropy ?? "mockEntropy";
            CredentialDefinition mockCredDef = credDef ?? CredentialDefinition ??(await MockCredDef()).Item1;
            MasterSecret mockMasterSecret = masterSecret ?? await MasterSecretApi.CreateMasterSecretAsync();
            string mockMasterSecretName = masterSecretName ?? "mockMasterSecretName";
            CredentialOffer mockCredOffer = credOffer ?? CredentialOffer ?? await MockCredOffer();

            (CredentialRequest, CredentialRequestMetadata) = await CredentialRequestApi.CreateCredentialRequestAsync(mockEntropy, mockCredDef, mockMasterSecret, mockMasterSecretName, mockCredOffer);
            return (CredentialRequest, CredentialRequestMetadata);
        }

        public static async Task<(string, string)> MockCredReqJson(string entropy = null, string credDefJson = null, string masterSecretJson = null, string masterSecretName = null, string credOfferJson = null)
        {
            string mockEntropy = entropy ?? "mockEntropy";
            string mockCredDefJson = credDefJson ?? CredentialDefinitionJson ?? (await MockCredDefJson()).Item1;
            string mockMasterSecretJson = masterSecretJson ?? await MasterSecretApi.CreateMasterSecretJsonAsync();
            string mockMasterSecretName = masterSecretName ?? "mockMasterSecretName";
            string mockCredOfferJson = credOfferJson ?? CredentialOfferJson ?? await MockCredOfferJson();

            (CredentialRequestJson, CredentialRequestMetadataJson) = await CredentialRequestApi.CreateCredentialRequestJsonAsync(mockEntropy, mockCredDefJson, mockMasterSecretJson, mockMasterSecretName, mockCredOfferJson);
            return (CredentialRequestJson, CredentialRequestMetadataJson);
        }

        public static async Task<PresentationRequest> MockPresReq(string name = "ProofRequest", string version = "1.0", List<AttributeInfo> requestedAttributes = null, List<PredicateInfo> requestedPredicates = null)
        {
            string nonce = await PresentationRequestApi.GenerateNonceAsync();
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            
            string requestedAttributesString =
                "\"attributeKey1\": " +
                    "{" +
                        "\"name\":\"attribute1\"," +
                        "\"value\":\"value1\"," +
                        "\"names\": [], " +
                        "\"non_revoked\":" +
                        "{ " +
                            $"\"from\": {timestamp}, " +
                            $"\"to\": {timestamp}" +
                        "}" +
                    "}";
            if (requestedAttributes != null)
            {
                requestedAttributesString = "";
                int i = 1;
                foreach (var attribute in requestedAttributes)
                {
                    if (i > 1)
                    {
                        requestedAttributesString += ", ";
                    }
                    requestedAttributesString += $"\"attributeKey{i}\": ";
                    requestedAttributesString += JsonConvert.SerializeObject(attribute);
                    i += 1;
                }
            }

            string requestedPredicatesString;
            if (requestedPredicates != null)
            {
                requestedPredicatesString = "";
                int i = 1;
                foreach (var predicate in requestedPredicates)
                {
                    if (i > 1)
                    {
                        requestedPredicatesString += ", ";
                    }
                    requestedPredicatesString += $"\"predicateKey{i}\": ";
                    requestedPredicatesString += JsonConvert.SerializeObject(predicate);
                    i += 1;
                }
            }
            string presReqJson = "{" +
                $"\"name\": \"{name}\", " +
                $"\"version\": \"{version}\", " +
                $"\"nonce\": \"{nonce}\", " +
                "\"requested_attributes\": {" +
                $"{requestedAttributesString}" +
                "}, \"revealed_attrs_groups\": {}," +
                "\"requested_predicates\": {}, " +
                "\"non_revoked\": " +
                "{ " +
                    $"\"from\": {timestamp}," +
                    $"\"to\": {timestamp}" +
                "}," +
                "\"ver\": \"1.0\"" +
                "}";
            return await PresentationRequestApi.CreatePresReqFromJsonAsync(presReqJson);
        }

        public static async Task<Presentation> MockPresentation(PresentationRequest presentationRequest,
            List<CredentialEntry> credentialEntries = null,
            List<CredentialProof> credentialProofs = null,
            List<string> selfAttestNames = null,
            List<string> selfAttestValues = null,
            MasterSecret masterSecret = null,
            List<Schema> schemas = null,
            List<CredentialDefinition> credentialDefinitions = null)
        {
            PresentationRequest mockPresentationRequest = presentationRequest;
            List<CredentialEntry> mockCredentialEntries = credentialEntries ?? new List<CredentialEntry>() {  };
            List<CredentialProof> mockCredentialProofs = credentialProofs ?? new List<CredentialProof>() {  };
            List<string> mockSelfAttestNames = selfAttestNames ?? new List<string>() { presentationRequest.RequestedAttributes.First().Key,  };
            List<string> mockSelfAttestValues = selfAttestValues ?? new List<string>() { "SomeValue" };
            MasterSecret mockMasterSecret = masterSecret ?? await MasterSecretApi.CreateMasterSecretAsync();
            List<Schema> mockSchemas = schemas ?? new List<Schema>() { };
            List<CredentialDefinition> mockCredentialDefinitions = credentialDefinitions ?? new List<CredentialDefinition>() { };


            return await PresentationApi.CreatePresentationAsync(mockPresentationRequest, mockCredentialEntries, mockCredentialProofs,
                mockSelfAttestNames, mockSelfAttestValues, mockMasterSecret, mockSchemas, mockCredentialDefinitions);
        }

        public static async Task<(List<string>, List<string>, List<string>)> MockAttrValues(Schema schema = null)
        {
            Schema mockSchema = schema ?? await MockSchema();

            List<string> attrNames = new List<string>();
            List<string> attrValuesRaw = new List<string>();

            int i = 1;
            foreach (string attr in mockSchema.AttrNames)
            {
                attrNames.Add(attr);
                attrValuesRaw.Add($"value{i}");
                i += 1;
            }

            List<string> attrValuesEncoded = await CredentialApi.EncodeCredentialAttributesAsync(attrValuesRaw);

            return(attrNames, attrValuesRaw, attrValuesEncoded);
        }
    }
}
