using anoncreds_rs_dotnet.Anoncreds;
using anoncreds_rs_dotnet.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace anoncreds_rs_dotnet_test
{
    internal static class MockDataProvider
    {
        public static async Task<Schema> MockSchema(string issuerUri = null, string schemaName = null, string schemaVersion = null, List<string> attrNames = null)
        {
            string mockSchemaIssuerUri = issuerUri ?? "mock:SchemaIssuerUri";
            string mockSchemaName = schemaName ?? "mockSchemaName";
            string mockSchemaVersion = schemaVersion ?? "mockSchemaVersion";
            List<string> mockSchemaAttrNames = attrNames ?? new() { "attribute1", "attribute2", "attribute3" };

            return await SchemaApi.CreateSchemaAsync(mockSchemaIssuerUri, mockSchemaName, mockSchemaVersion, mockSchemaAttrNames);
        }

        public static async Task<(CredentialDefinition, CredentialDefinitionPrivate, CredentialKeyCorrectnessProof)> MockCredDef(
            Schema schema = null,
            string tag = null,
            string issuerUri = null,
            SignatureType signatureType = SignatureType.CL,
            bool supportRevocation = true)
        {
            string mockSchemaIssuerUri = schema != null ? schema.IssuerId : "mock:SchemaIssuerUri";
            string mockSchemaName = schema != null ? schema.Name : "mockSchemaName";
            string mockSchemaVersion = schema != null ? schema.Version : "mockSchemaVersion";
            List<string> mockSchemaAttrNames = schema != null ? schema.AttrNames.ToList() : new() { "attribute1", "attribute2", "attribute3" };

            Schema mockSchema = await SchemaApi.CreateSchemaAsync(mockSchemaIssuerUri, mockSchemaName, mockSchemaVersion, mockSchemaAttrNames);

            string mockCredDefTag = tag ?? "mockCredDefTag";
            string mockCredDefIssuerUri = issuerUri ?? "mock:CredDefIssuerUri";

            return await CredentialDefinitionApi.CreateCredentialDefinitionAsync(mockSchema.IssuerId, mockSchema, mockCredDefTag, mockCredDefIssuerUri, signatureType, supportRevocation);
        }

        public static async Task<CredentialOffer> MockCredOffer(string schemaIssuerUri = null, string credDefIssuerUri = null, CredentialKeyCorrectnessProof credentialKeyCorrectnessProof = null)
        {
            string mockSchemaIssuerUri = schemaIssuerUri ?? "mock:SchemaIssuerUri";
            string mockCredDefIssuerUri = credDefIssuerUri ?? "mock:CredDefIssuerUri";
            CredentialKeyCorrectnessProof mockKCP = credentialKeyCorrectnessProof ?? (await MockCredDef()).Item3;

            return await CredentialOfferApi.CreateCredentialOfferAsync(mockSchemaIssuerUri, mockCredDefIssuerUri, mockKCP);
        }

        public static async Task<(CredentialRequest, CredentialRequestMetadata)> MockCredReq(string entropy = null, CredentialDefinition credDef = null, MasterSecret masterSecret = null, string masterSecretName = null, CredentialOffer credOffer = null)
        {
            string mockEntropy = entropy ?? "mockEntropy";
            CredentialDefinition mockCredDef = credDef ?? (await MockCredDef()).Item1;
            MasterSecret mockMasterSecret = masterSecret ?? await MasterSecretApi.CreateMasterSecretAsync();
            string mockMasterSecretName = masterSecretName ?? "mockMasterSecretName";
            CredentialOffer mockCredOffer = credOffer ?? await MockCredOffer();


            return await CredentialRequestApi.CreateCredentialRequestAsync(mockEntropy, mockCredDef, mockMasterSecret, mockMasterSecretName, mockCredOffer);
        }
    }
}
