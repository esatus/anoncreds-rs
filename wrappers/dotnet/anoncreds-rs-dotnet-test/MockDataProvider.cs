using anoncreds_rs_dotnet.Anoncreds;
using anoncreds_rs_dotnet.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace anoncreds_rs_dotnet_test
{
    internal class MockDataProvider
    {
        static readonly JsonSerializerSettings settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

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

        public static async Task<(CredentialRequest, CredentialRequestMetadata)> MockCredReq(string entropy = null, CredentialDefinition credDef = null, string linkSecret = null, string linkSecretName = null, CredentialOffer credOffer = null)
        {
            string mockEntropy = entropy ?? "mockEntropy";
            CredentialDefinition mockCredDef = credDef ?? CredentialDefinition ??(await MockCredDef()).Item1;
            string mockLinkSecret = linkSecret ?? await LinkSecretApi.CreateLinkSecretAsync();
            string mockLinkSecretName = linkSecretName ?? "mockLinkSecretName";
            CredentialOffer mockCredOffer = credOffer ?? CredentialOffer ?? await MockCredOffer();

            (CredentialRequest, CredentialRequestMetadata) = await CredentialRequestApi.CreateCredentialRequestAsync(mockEntropy, mockCredDef, mockLinkSecret, mockLinkSecretName, mockCredOffer);
            return (CredentialRequest, CredentialRequestMetadata);
        }

        public static async Task<(string, string)> MockCredReqJson(string entropy = null, string credDefJson = null, string linkSecretJson = null, string linkSecretName = null, string credOfferJson = null)
        {
            string mockEntropy = entropy ?? "mockEntropy";
            string mockCredDefJson = credDefJson ?? CredentialDefinitionJson ?? (await MockCredDefJson()).Item1;
            string mockLinkSecretJson = linkSecretJson ?? await LinkSecretApi.CreateLinkSecretAsync();
            string mockLinkSecretName = linkSecretName ?? "mockLinkSecretName";
            string mockCredOfferJson = credOfferJson ?? CredentialOfferJson ?? await MockCredOfferJson();

            (CredentialRequestJson, CredentialRequestMetadataJson) = await CredentialRequestApi.CreateCredentialRequestJsonAsync(mockEntropy, mockCredDefJson, mockLinkSecretJson, mockLinkSecretName, mockCredOfferJson);
            return (CredentialRequestJson, CredentialRequestMetadataJson);
        }

        public static async Task<PresentationRequest> MockPresReq(string name = "ProofRequest", string version = "1.0", List<AttributeInfo> requestedAttributes = null, List<PredicateInfo> requestedPredicates = null)
        {
            string nonce = await PresentationRequestApi.GenerateNonceAsync();
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            
            Dictionary<string, AttributeInfo> attr = new();
            if (requestedAttributes != null)
            {
                int i = 1;
                foreach (AttributeInfo attribute in requestedAttributes)
                {
                    attr[$"attributeKey{i}"] = attribute;
                    i++;
                }
            }
            else
            {
                attr["attributeKey1"] = new AttributeInfo
                {
                    Name = "attribute1",
                    Names = null,
                    NonRevoked = new NonRevokedInterval { From = (ulong)timestamp, To = (ulong)timestamp },
                    Restrictions = null
                };
            }

            Dictionary<string, PredicateInfo> pred = new Dictionary<string, PredicateInfo>();
            if (requestedPredicates != null)
            {
                int i = 1;
                foreach (PredicateInfo predicate in requestedPredicates)
                {
                    pred[$"predicateKey{i}"] = predicate;
                    i++;
                }
            }

            PresentationRequest request = new()
            {
                Name = name,
                Version = version,
                Nonce = nonce,
                NonRevoked = new NonRevokedInterval { From = (ulong)timestamp, To = (ulong)timestamp },
                RequestedAttributes = attr,
                RequestedPredicates = pred,
            };

            string requestJson = JsonConvert.SerializeObject(request);

            return await PresentationRequestApi.CreatePresReqFromJsonAsync(requestJson);
        }

        public static async Task<Presentation> MockPresentation(PresentationRequest presentationRequest,
            List<CredentialEntry> credentialEntries = null,
            List<CredentialProof> credentialProofs = null,
            List<string> selfAttestNames = null,
            List<string> selfAttestValues = null,
            string linkSecret = null,
            List<Schema> schemas = null,
            List<CredentialDefinition> credentialDefinitions = null)
        {
            PresentationRequest mockPresentationRequest = presentationRequest;
            List<CredentialEntry> mockCredentialEntries = credentialEntries ?? new List<CredentialEntry>() {  };
            List<CredentialProof> mockCredentialProofs = credentialProofs ?? new List<CredentialProof>() {  };
            List<string> mockSelfAttestNames = selfAttestNames ?? new List<string>() { presentationRequest.RequestedAttributes.First().Key,  };
            List<string> mockSelfAttestValues = selfAttestValues ?? new List<string>() { "SomeValue" };
            string mockLinkSecret = linkSecret ?? await LinkSecretApi.CreateLinkSecretAsync();
            List<Schema> mockSchemas = schemas ?? new List<Schema>() { };
            List<CredentialDefinition> mockCredentialDefinitions = credentialDefinitions ?? new List<CredentialDefinition>() { };


            return await PresentationApi.CreatePresentationAsync(mockPresentationRequest, mockCredentialEntries, mockCredentialProofs,
                mockSelfAttestNames, mockSelfAttestValues, mockLinkSecret, mockSchemas, mockCredentialDefinitions);
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

        public static async Task<(RevocationRegistryDefinition, RevocationRegistryDefinitionPrivate)> MockRevRegDef(CredentialDefinition credDef = null,
            RegistryType registryType = RegistryType.CL_ACCUM,
            int maxCredNumber = 100,
            string tailsPath = null)
        {
            CredentialDefinition mockCredDef = credDef ?? (await MockCredDef()).Item1;
            string mockUri = mockCredDef.IssuerId;
            string mockCredDefTag = mockCredDef.Tag;

            return await RevocationApi.CreateRevocationRegistryDefinitionAsync(mockUri, mockCredDef, mockCredDefTag, registryType, maxCredNumber, tailsPath);
        }

        public static async Task<(string, string)> MockRevRegDefJson(string credDef = null,
            RegistryType registryType = RegistryType.CL_ACCUM,
            int maxCredNumber = 100,
            string tailsPath = null)
        {
            string mockCredDef = credDef ?? (await MockCredDefJson()).Item1;
            string mockUri = JObject.Parse(mockCredDef)["issuerId"].ToString();
            string mockCredDefTag = JObject.Parse(mockCredDef)["tag"].ToString();

            return await RevocationApi.CreateRevocationRegistryDefinitionJsonAsync(mockUri, mockCredDef, mockCredDefTag, registryType, maxCredNumber, tailsPath);
        }

        public static async Task<RevocationStatusList> MockRevStatusList(RevocationRegistryDefinition revRegDef = null,
            string issuerId = null,
            long timestamp = 0,
            IssuerType issuerType = IssuerType.ISSUANCE_BY_DEFAULT)
        {
            RevocationRegistryDefinition mockRevRegDef = revRegDef ?? (await MockRevRegDef()).Item1;
            string mockUri = issuerId ?? "mock:CredDefIssuerUri";
            long mockTimestamp = timestamp == 0 ? DateTimeOffset.Now.ToUnixTimeSeconds() : timestamp;

            return await RevocationApi.CreateRevocationStatusListAsync(revRegDef.IssuerId, mockRevRegDef, mockUri, mockTimestamp, issuerType);
        }

        public static async Task<Credential> MockCredential(CredentialDefinition credentialDefinition = null,
            CredentialDefinitionPrivate credentialDefinitionPrivate = null,
            CredentialOffer credentialOffer = null,
            CredentialRequest credentialRequest = null,
            List<string> attributeNames = null,
            List<string> valuesRaw = null,
            List<string> valuesEncoded = null,
            RevocationStatusList revocationStatusList = null,
            string revocationRegistryId = null,
            RevocationRegistryDefinition revocationRegistryDefinition = null,
            RevocationRegistryDefinitionPrivate revocationRegistryDefinitionPrivate = null,
            long regIdX = -1)
        {
            (CredentialDefinition mockCredDef, CredentialDefinitionPrivate mockCredDefPriv, _) =
                (credentialDefinition != null && credentialDefinitionPrivate != null)
                ? (credentialDefinition, credentialDefinitionPrivate, null)
                : await MockCredDef();
            CredentialOffer mockCredOffer = credentialOffer ?? await MockCredOffer();
            CredentialRequest mockCredReq = credentialRequest ?? (await MockCredReq()).Item1;

            (List<string> mockNames, List<string> mockRaws, List<string> mockEncodeds) = 
                (attributeNames != null && valuesRaw != null && valuesEncoded != null && valuesEncoded.Count == valuesRaw.Count && valuesEncoded.Count == attributeNames.Count)
                ? (attributeNames, valuesRaw, valuesEncoded)
                : await MockAttrValues();

            RevocationStatusList mockRevStatList = null;
            string mockRevRegId = null;
            RevocationRegistryDefinition mockRevRegDef = null;
            RevocationRegistryDefinitionPrivate mockRevRegDefPriv = null;
            long mockRegId = -1;

            if(revocationStatusList != null && regIdX != -1
                && revocationRegistryDefinition != null && revocationRegistryDefinitionPrivate != null)
            {
                mockRevStatList = revocationStatusList;
                mockRevRegDef = revocationRegistryDefinition;
                mockRevRegDefPriv= revocationRegistryDefinitionPrivate;
                mockRegId = regIdX;
            }
            else if(revocationRegistryId != null && revocationRegistryDefinition != null && revocationRegistryDefinitionPrivate != null)
            {
                mockRevRegId= revocationRegistryId;
                mockRevRegDef = revocationRegistryDefinition;
                mockRevRegDefPriv = revocationRegistryDefinitionPrivate;
            }

            return await CredentialApi.CreateCredentialAsync(mockCredDef, mockCredDefPriv, mockCredOffer, mockCredReq, mockNames, mockRaws, mockEncodeds,
                mockRevStatList, mockRevRegId, mockRevRegDef, mockRevRegDefPriv, mockRegId);
        }

        public static async Task<string> MockCredentialJson(string credentialDefinitionJson = null,
            string credentialDefinitionPrivateJson = null,
            string credentialOfferJson = null,
            string credentialRequestJson = null,
            List<string> attributeNames = null,
            List<string> valuesRaw = null,
            List<string> valuesEncoded = null,
            string revocationStatusListJson = null,
            string revocationRegistryId = null,
            string revocationRegistryDefinitionJson = null,
            string revocationRegistryDefinitionPrivateJson = null,
            long regIdX = -1)
        {
            (string mockCredDef, string mockCredDefPriv, _) =
                (credentialDefinitionJson != null && credentialDefinitionPrivateJson != null)
                ? (credentialDefinitionJson, credentialDefinitionPrivateJson, null)
                : await MockCredDefJson();
            string mockCredOffer = credentialOfferJson ?? await MockCredOfferJson();
            string mockCredReq = credentialRequestJson ?? (await MockCredReqJson()).Item1;

            (List<string> mockNames, List<string> mockRaws, List<string> mockEncodeds) =
                (attributeNames != null && valuesRaw != null && valuesEncoded != null && valuesEncoded.Count == valuesRaw.Count && valuesEncoded.Count == attributeNames.Count)
                ? (attributeNames, valuesRaw, valuesEncoded)
                : await MockAttrValues();

            string mockRevStatList = null;
            string mockRevRegId = null;
            string mockRevRegDef = null;
            string mockRevRegDefPriv = null;
            long mockRegId = -1;

            if (revocationStatusListJson != null && revocationRegistryId != null && regIdX != -1
                && revocationRegistryDefinitionJson != null && revocationRegistryDefinitionPrivateJson != null)
            {
                mockRevStatList = revocationStatusListJson;
                mockRevRegId = revocationRegistryId;
                mockRevRegDef = revocationRegistryDefinitionJson;
                mockRevRegDefPriv = revocationRegistryDefinitionPrivateJson;
                mockRegId = regIdX;
            }

            return await CredentialApi.CreateCredentialAsync(mockCredDef, mockCredDefPriv, mockCredOffer, mockCredReq, mockNames, mockRaws, mockEncodeds,
                mockRevStatList, mockRevRegId, mockRevRegDef, mockRevRegDefPriv, mockRegId);
        }
    }
}
