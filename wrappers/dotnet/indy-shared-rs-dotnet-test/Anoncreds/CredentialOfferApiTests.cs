using anoncreds_rs_dotnet.Anoncreds;
using anoncreds_rs_dotnet.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace anoncreds_rs_dotnet_test.Anoncreds
{
    public class CredentialOfferApiTests
    {
        #region Tests for CreateCredentialOfferAsync
        [Test, TestCase(TestName = "CreateCredentialOfferAsync() returns CredentialOffer object.")]
        public async Task CreateCredentialOfferWorks()
        {
            //Arrange
            List<string> attrNames = new() { "gender", "age", "sex" };
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string schemaName = "gvt";
            string schemaVersion = "1.0";

            Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (CredentialDefinition credDef, _, CredentialKeyCorrectnessProof keyProof) =
                await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            //Act
            string schemaId = schemaObject.IssuerId;
            CredentialOffer testObject = await CredentialOfferApi.CreateCredentialOfferAsync(schemaId, credDef.CredentialDefinitionId, keyProof);

            //Assert
            _ = testObject.Should().BeOfType(typeof(CredentialOffer));
        }

        [Test, TestCase(TestName = "CreateCredentialOfferAsync() with JSON input returns CredentialOffer object.")]
        public async Task CreateCredentialOfferJsonWorks()
        {
            //Arrange
            List<string> attrNames = new() { "gender", "age", "sex" };
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string schemaName = "gvt";
            string schemaVersion = "1.0";

            string schemaObjectJson = await SchemaApi.CreateSchemaJsonAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (string credDef, _, string keyProof) =
                await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(issuerDid, schemaObjectJson, "tag", issuerDid, SignatureType.CL, true);

            //Act
            string schemaId = issuerDid;
            string credDefId = issuerDid;
            string testObject = await CredentialOfferApi.CreateCredentialOfferJsonAsync(schemaId, credDefId, keyProof);

            //Assert
            _ = testObject.Should().NotBeNullOrEmpty();
        }

        private static IEnumerable<TestCaseData> CreateCredentialOfferCases()
        {
            yield return new TestCaseData(false, false, false)
                .SetName("CreateCredentialOfferAsync() throws Exception if all arguments are null.");
            yield return new TestCaseData(false, true, true)
                .SetName("CreateCredentialOfferAsync() throws AnoncredsRsException if SchemaId is null.");
            yield return new TestCaseData(true, false, true)
                .SetName("CreateCredentialOfferAsync() throws AnoncredsRsException if CredentialDefinition is null.");
            yield return new TestCaseData(true, true, false)
                .SetName("CreateCredentialOfferAsync() throws AnoncredsRsException if CredentialKeyCorrectnessProof is null.");
        }

        [Test, TestCaseSource(nameof(CreateCredentialOfferCases))]
        public async Task CreateCredentialOfferThrowsException(bool hasSchemaId, bool hasCredDef, bool hasKeyProof)
        {
            //Arrange
            List<string> attrNames = new() { "gender", "age", "sex" };
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string schemaName = "gvt";
            string schemaVersion = "1.0";
            Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);

            string schemaId = null;
            CredentialDefinition credDef = null;
            CredentialKeyCorrectnessProof keyProof = null;

            if (hasSchemaId)
            {
                (CredentialDefinition tmpCredDef, CredentialDefinitionPrivate tmpCredDefPrivate, CredentialKeyCorrectnessProof tmpKeyProof) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag",issuerDid , SignatureType.CL, true);
                schemaId = schemaObject.IssuerId;
            }
            if (hasCredDef)
            {
                (CredentialDefinition tmpCredDef, CredentialDefinitionPrivate tmpCredDefPrivate, CredentialKeyCorrectnessProof tmpKeyProof) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag",issuerDid,  SignatureType.CL, true);
                credDef = tmpCredDef;
            }
            if (hasKeyProof)
            {
                (CredentialDefinition tmpCredDef, CredentialDefinitionPrivate tmpCredDefPrivate, CredentialKeyCorrectnessProof tmpKeyProof) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag",issuerDid, SignatureType.CL, true);
                keyProof = tmpKeyProof;
            }

            //Act
            Func<Task> act = async () => await CredentialOfferApi.CreateCredentialOfferAsync(schemaId, credDef.CredentialDefinitionId, keyProof);

            //Assert
            _ = await act.Should().ThrowAsync<Exception>();
        }
        #endregion
    }
}