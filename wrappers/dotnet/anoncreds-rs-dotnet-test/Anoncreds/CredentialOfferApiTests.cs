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
            Schema mockSchema = await MockDataProvider.MockSchema();
            (CredentialDefinition mockCredDef, _, CredentialKeyCorrectnessProof mockKCP) = await MockDataProvider.MockCredDef();

            //Act
            CredentialOffer testObject = await CredentialOfferApi.CreateCredentialOfferAsync(mockSchema.IssuerId, mockCredDef.IssuerId, mockKCP);

            //Assert
            _ = testObject.Should().BeOfType(typeof(CredentialOffer));
        }

        [Test, TestCase(TestName = "CreateCredentialOfferAsync() with JSON input returns CredentialOffer object.")]
        public async Task CreateCredentialOfferJsonWorks()
        {
            //Arrange
            string mockSchemaUri = "mock:SchemaUri";
            Schema mockSchema = await MockDataProvider.MockSchema(issuerUri: mockSchemaUri);

            string mockCredDefUri = "mock:CredDefUri";
            (_, _, string mockKCPJson) = await MockDataProvider.MockCredDefJson(issuerUri: mockCredDefUri, schema: mockSchema);

            //Act
            string testObject = await CredentialOfferApi.CreateCredentialOfferJsonAsync(mockSchemaUri, mockCredDefUri, mockKCPJson);

            //Assert
            _ = testObject.Should().NotBeNullOrEmpty();
        }
        #endregion
    }
}