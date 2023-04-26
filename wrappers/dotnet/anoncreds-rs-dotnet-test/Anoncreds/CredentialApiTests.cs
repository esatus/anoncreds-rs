using anoncreds_rs_dotnet.Anoncreds;
using anoncreds_rs_dotnet.Models;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace anoncreds_rs_dotnet_test.Anoncreds
{
    public class CredentialApiTests
    {
        #region Tests for CreateCredentialAsync
        [Test, TestCase(TestName = "CreateCredentialAsync() creates a credential without revocation.")]
        public async Task CreateCredentialAsyncWorks()
        {
            //Arrange
            (CredentialDefinition mockCredDef, CredentialDefinitionPrivate mockCredDefPriv, _) = await MockDataProvider.MockCredDef();
            CredentialOffer mockCredOffer = await MockDataProvider.MockCredOffer();
            (CredentialRequest mockCredReq, _) = await MockDataProvider.MockCredReq();
            (List<string> names, List<string> raw, List<string> enc) = await MockDataProvider.MockAttrValues();

            //Act
            Credential actual = await CredentialApi.CreateCredentialAsync(mockCredDef, mockCredDefPriv, mockCredOffer, mockCredReq, names, raw, enc);

            //Assert
            _ = actual.Should().NotBeNull();
        }

        [Test, TestCase(TestName = "CreateCredentialAsync() creates a credential and revocation registry object.")]
        public async Task CreateCredentialAsyncWorksWithRevocation()
        {
            //Arrange
            (CredentialDefinition mockCredDef, CredentialDefinitionPrivate mockCredDefPriv, _) = await MockDataProvider.MockCredDef();
            CredentialOffer mockCredOffer = await MockDataProvider.MockCredOffer();
            (CredentialRequest mockCredReq, _) = await MockDataProvider.MockCredReq();
            (List<string> names, List<string> raw, List<string> enc) = await MockDataProvider.MockAttrValues();

            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            (RevocationRegistryDefinition revRegDefObject, RevocationRegistryDefinitionPrivate revRegDefPvtObject) = await MockDataProvider.MockRevRegDef(mockCredDef);
            RevocationStatusList revStatusList = await MockDataProvider.MockRevStatusList(revRegDefObject, mockCredDef.IssuerId, timestamp);

            //Act
            Credential actual = await CredentialApi.CreateCredentialAsync(mockCredDef, mockCredDefPriv, mockCredOffer, mockCredReq, names, raw, enc, revStatusList, null, revRegDefObject, revRegDefPvtObject, 1);

            //Assert
            _ = actual.Should().NotBeNull();
        }

        [Test, TestCase(TestName = "CreateCredentialAsync() throws AnoncredsRsException when attribute names do not match their values.")]
        public async Task CreateCredentialAsyncWrongAttributes()
        {
            //Arrange
            List<string> names = new() { "attribute1", "attribute2", "attribute3" };
            List<string> raw = new() { "value1" };
            List<string> enc = await CredentialApi.EncodeCredentialAttributesAsync(raw);
            (CredentialDefinition mockCredDef, CredentialDefinitionPrivate mockCredDefPriv, _) = await MockDataProvider.MockCredDef();
            CredentialOffer mockCredOffer = await MockDataProvider.MockCredOffer();
            (CredentialRequest mockCredReq, _) = await MockDataProvider.MockCredReq();

            //Act
            Func<Task> act = async () => await CredentialApi.CreateCredentialAsync(mockCredDef, mockCredDefPriv, mockCredOffer, mockCredReq, names, raw, enc);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }

        [Test, TestCase(TestName = "CreateCredentialAsync() with JSON input creates a credential without revocation.")]
        public async Task CreateCredentialAsyncJsonWorks()
        {
            //Arrange
            (string mockCredDefJson, string mockCredDefPrivJson, _) = await MockDataProvider.MockCredDefJson();
            string mockCredOfferJson = await MockDataProvider.MockCredOfferJson();
            (string mockCredReqJson, _) = await MockDataProvider.MockCredReqJson();
            (List<string> names, List<string> raw, List<string> enc) = await MockDataProvider.MockAttrValues();

            //Act
            string actual = await CredentialApi.CreateCredentialAsync(mockCredDefJson, mockCredDefPrivJson, mockCredOfferJson, mockCredReqJson, names, raw, enc);

            //Assert
            _ = actual.Should().NotBeNullOrEmpty();
        }

        [Test, TestCase(TestName = "CreateCredentialAsync() with JSON input creates a credential and revocation registry object.")]
        public async Task CreateCredentialAsyncJsonWorksWithRevocation()
        {
            //Arrange
            (string mockCredDefJson, string mockCredDefPrivJson, _) = await MockDataProvider.MockCredDefJson();
            (CredentialDefinition mockCredDef, _, _) = await MockDataProvider.MockCredDef();
            string mockCredOfferJson = await MockDataProvider.MockCredOfferJson();
            (string mockCredReqJson, _) = await MockDataProvider.MockCredReqJson();
            string testTailsPathForRevocation = null;

            (List<string> names, List<string> raw, List<string> enc) = await MockDataProvider.MockAttrValues();

            (string revRegDefJson, string revRegDefPvtJson) =
                await RevocationApi.CreateRevocationRegistryDefinitionJsonAsync(mockCredDef.IssuerId, mockCredDefJson, mockCredDef.Tag, RegistryType.CL_ACCUM, 99, testTailsPathForRevocation);
            (RevocationRegistryDefinition mockRevRegDef, _) = await RevocationApi.CreateRevocationRegistryDefinitionAsync(mockCredDef.IssuerId, mockCredDef, mockCredDef.Tag, RegistryType.CL_ACCUM, 99, testTailsPathForRevocation);

            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            string revStatusListJson = await RevocationApi.CreateRevocationStatusListJsonAsync(mockRevRegDef.CredentialDefinitionId, revRegDefJson, mockRevRegDef.IssuerId, timestamp, IssuerType.ISSUANCE_BY_DEFAULT);

            //Act
            string actual = await CredentialApi.CreateCredentialAsync(mockCredDefJson, mockCredDefPrivJson, mockCredOfferJson, mockCredReqJson,
                names, raw, enc, revStatusListJson, null, revRegDefJson, revRegDefPvtJson, 1);

            //Assert
            _ = actual.Should().NotBeNullOrEmpty();
        }

        [Test, TestCase(TestName = "CreateCredentialAsync() throws AnoncredsRsException when revocation data is incomplete.")]
        public async Task CreateCredentialAsyncJsonIncompleteRevocation()
        {
            //Arrange
            //Arrange
            (string mockCredDefJson, string mockCredDefPrivJson, _) = await MockDataProvider.MockCredDefJson();
            (CredentialDefinition mockCredDef, _, _) = await MockDataProvider.MockCredDef();
            string mockCredOfferJson = await MockDataProvider.MockCredOfferJson();
            (string mockCredReqJson, _) = await MockDataProvider.MockCredReqJson();
            string testTailsPathForRevocation = null;
            (List<string> names, List<string> raw, List<string> enc) = await MockDataProvider.MockAttrValues();

            (string revRegDefJson, string revRegDefPvtJson) =
                await RevocationApi.CreateRevocationRegistryDefinitionJsonAsync(mockCredDef.IssuerId, mockCredDefJson, mockCredDef.Tag, RegistryType.CL_ACCUM, 99, testTailsPathForRevocation);
            (RevocationRegistryDefinition mockRevRegDef, _) = await RevocationApi.CreateRevocationRegistryDefinitionAsync(mockCredDef.IssuerId, mockCredDef, mockCredDef.Tag, RegistryType.CL_ACCUM, 99, testTailsPathForRevocation);

            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            string revStatusListJson = await RevocationApi.CreateRevocationStatusListJsonAsync(mockRevRegDef.CredentialDefinitionId, revRegDefJson, mockRevRegDef.IssuerId, timestamp, IssuerType.ISSUANCE_BY_DEFAULT);

            //Act
            Func<Task> act = async () => await CredentialApi.CreateCredentialAsync(mockCredDefJson, mockCredDefPrivJson, mockCredOfferJson, mockCredReqJson,
                names, raw, enc, revStatusListJson, null, revRegDefJson, "", 1);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>("Revocation data incomplete.");
        }
        #endregion

        #region Tests for EncodeCredentialAttributesAsync
        [Test, TestCase(TestName = "EncodeCredentialAttributesAsync() with all arguments set returns encoded attribute.")]
        public async Task EncodeCredentialAttributesAsyncWorks()
        {
            //Arrange
            List<string> rawAttributes = new() { "test", "test2", "test3" };

            //Act
            List<string> result = await CredentialApi.EncodeCredentialAttributesAsync(rawAttributes);

            //Assert
            _ = result.Should().NotBeNull();
        }

        [Test, TestCase(TestName = "EncodeCredentialAttributesAsync() works with empty list.")]
        public async Task EncodeCredentialAttributesAsyncWorksWithEmptyList()
        {
            //Arrange
            List<string> rawAttributes = new();

            //Act
            List<string> result = await CredentialApi.EncodeCredentialAttributesAsync(rawAttributes);

            //Assert
            _ = result.Should().NotBeNull();
        }
        #endregion

        #region Tests for ProcessCredentialAsync
        [Test, TestCase(TestName = "ProcessCredentialAsync() creates a credential object.")]
        public async Task ProcessCredentialAsync()
        {
            //Arrange
            string mockLinkSecret = await LinkSecretApi.CreateLinkSecretAsync();
            (CredentialDefinition mockCredDef, CredentialDefinitionPrivate mockCredDefPriv, _) = await MockDataProvider.MockCredDef();
            CredentialOffer mockCredOffer = await MockDataProvider.MockCredOffer();
            (CredentialRequest mockCredReq, CredentialRequestMetadata mockMetaData) = await MockDataProvider.MockCredReq(linkSecret: mockLinkSecret);
            (List<string> names, List<string> raw, List<string> enc) = await MockDataProvider.MockAttrValues();

            Credential mockCredential = await CredentialApi.CreateCredentialAsync(mockCredDef, mockCredDefPriv, mockCredOffer, mockCredReq, names, raw, enc);

            //Act
            Credential credObjectProcessed = await CredentialApi.ProcessCredentialAsync(mockCredential, mockMetaData, mockLinkSecret, mockCredDef);

            //Assert
            _ = credObjectProcessed.Should().BeOfType(typeof(Credential));
        }

        [Test, TestCase(TestName = "ProcessCredentialAsync() with JSON input creates a credential object.")]
        public async Task ProcessCredentialAsyncJson()
        {
            //Arrange
            string linkSecretJson = await LinkSecretApi.CreateLinkSecretAsync();
            (string mockCredDefJson, string mockCredDefPrivJson, _) = await MockDataProvider.MockCredDefJson();
            (CredentialDefinition mockCredDef, _, _) = await MockDataProvider.MockCredDef();
            string mockCredOfferJson = await MockDataProvider.MockCredOfferJson();
            (string mockCredReqJson, string mockMetaDataJson) = await MockDataProvider.MockCredReqJson(linkSecretJson: linkSecretJson);
            string testTailsPathForRevocation = null;

            (List<string> names, List<string> raw, List<string> enc) = await MockDataProvider.MockAttrValues();

            (string revRegDefJson, string revRegDefPvtJson) =
                await RevocationApi.CreateRevocationRegistryDefinitionJsonAsync(mockCredDef.IssuerId, mockCredDefJson, mockCredDef.Tag, RegistryType.CL_ACCUM, 99, testTailsPathForRevocation);
            (RevocationRegistryDefinition mockRevRegDef, _) = await RevocationApi.CreateRevocationRegistryDefinitionAsync(mockCredDef.IssuerId, mockCredDef, mockCredDef.Tag, RegistryType.CL_ACCUM, 99, testTailsPathForRevocation);

            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            string revStatusListJson = await RevocationApi.CreateRevocationStatusListJsonAsync(mockRevRegDef.CredentialDefinitionId, revRegDefJson, mockRevRegDef.IssuerId, timestamp, IssuerType.ISSUANCE_BY_DEFAULT);

            string mockCredentialJson = await CredentialApi.CreateCredentialAsync(mockCredDefJson, mockCredDefPrivJson, mockCredOfferJson, mockCredReqJson,
                names, raw, enc, revStatusListJson, null, revRegDefJson, revRegDefPvtJson, 1);
            //Act
            string credObjectProcessedJson = await CredentialApi.ProcessCredentialAsync(mockCredentialJson, mockMetaDataJson, linkSecretJson, mockCredDefJson);

            //Assert
            _ = credObjectProcessedJson.Should().NotBeNullOrEmpty();
        }

        [Test, TestCase(TestName = "ProcessCredentialAsync() throws AnoncredsRsException when maste secret does not match credential.")]
        public async Task ProcessCredentialAsyncThrowsException()
        {
            //Arrange
            string mockWrongLinkSecret = await LinkSecretApi.CreateLinkSecretAsync();
            (CredentialDefinition mockCredDef, CredentialDefinitionPrivate mockCredDefPriv, _) = await MockDataProvider.MockCredDef();
            CredentialOffer mockCredOffer = await MockDataProvider.MockCredOffer();
            (CredentialRequest mockCredReq, CredentialRequestMetadata mockMetaData) = await MockDataProvider.MockCredReq();
            (List<string> names, List<string> raw, List<string> enc) = await MockDataProvider.MockAttrValues();

            Credential mockCredential = await CredentialApi.CreateCredentialAsync(mockCredDef, mockCredDefPriv, mockCredOffer, mockCredReq, names, raw, enc);

            //Act
            Func<Task> act = async () => await CredentialApi.ProcessCredentialAsync(mockCredential, mockMetaData, mockWrongLinkSecret, mockCredDef);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }
        #endregion

        #region Tests for GetCredentialAttributeAsync
        [Test, TestCase(TestName = "GetCredentialAttributeAsync() works for attribute names: schema_id, cred_def_id, rev_reg_id.")]
        public async Task GetCredentialAttributeAsync()
        {
            //Arrange
            (CredentialDefinition mockCredDef, CredentialDefinitionPrivate mockCredDefPriv, _) = await MockDataProvider.MockCredDef();
            CredentialOffer mockCredOffer = await MockDataProvider.MockCredOffer();
            (CredentialRequest mockCredReq, _) = await MockDataProvider.MockCredReq();
            (List<string> names, List<string> raw, List<string> enc) = await MockDataProvider.MockAttrValues();

            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            (RevocationRegistryDefinition revRegDefObject, RevocationRegistryDefinitionPrivate revRegDefPvtObject) = await MockDataProvider.MockRevRegDef(mockCredDef);
            RevocationStatusList revStatusList = await MockDataProvider.MockRevStatusList(revRegDefObject, mockCredDef.IssuerId, timestamp);

            //Act
            Credential mockCredential = await CredentialApi.CreateCredentialAsync(mockCredDef, mockCredDefPriv, mockCredOffer, mockCredReq, names, raw, enc, revStatusList, null, revRegDefObject, revRegDefPvtObject, 1);

            //Act
            //note: only attribute "schema_id", "cred_def_id", "rev_reg_id", "rev_reg_index" supported so far.
            string attrSchemaId = await CredentialApi.GetCredentialAttributeAsync(mockCredential, "schema_id");
            string attrCredDefId = await CredentialApi.GetCredentialAttributeAsync(mockCredential, "cred_def_id");
            string attrRevRegId = await CredentialApi.GetCredentialAttributeAsync(mockCredential, "rev_reg_id");
            string attrRevRegIndex = await CredentialApi.GetCredentialAttributeAsync(mockCredential, "rev_reg_index");
            //string attrDefault = await CredentialApi.GetCredentialAttributeAsync(credObject, "default");

            //Assert
            _ = attrSchemaId.Should().Be(mockCredential.SchemaId);
            _ = attrCredDefId.Should().Be(mockCredential.CredentialDefinitionId);
            _ = attrRevRegId.Should().Be(mockCredential.RevocationRegistryId);
            _ = attrRevRegIndex.Should().Be(mockCredential.Signature.RCredential.I.ToString());
            //attrDefault.Should().Be("");
        }

        [Test, TestCase(TestName = "GetCredentialAttributeAsync() with JSON inputs works for attribute names: schema_id, cred_def_id, rev_reg_id.")]
        public async Task GetCredentialAttributeJsonAsync()
        {
            //Arrange
            (string mockCredDefJson, string mockCredDefPrivJson, _) = await MockDataProvider.MockCredDefJson();
            (CredentialDefinition mockCredDef, _, _) = await MockDataProvider.MockCredDef();
            string mockCredOfferJson = await MockDataProvider.MockCredOfferJson();
            (string mockCredReqJson, _) = await MockDataProvider.MockCredReqJson();
            string testTailsPathForRevocation = null;

            (List<string> names, List<string> raw, List<string> enc) = await MockDataProvider.MockAttrValues();

            (string revRegDefJson, string revRegDefPvtJson) =
                await RevocationApi.CreateRevocationRegistryDefinitionJsonAsync(mockCredDef.IssuerId, mockCredDefJson, mockCredDef.Tag, RegistryType.CL_ACCUM, 99, testTailsPathForRevocation);
            (RevocationRegistryDefinition mockRevRegDef, _) = await RevocationApi.CreateRevocationRegistryDefinitionAsync(mockCredDef.IssuerId, mockCredDef, mockCredDef.Tag, RegistryType.CL_ACCUM, 99, testTailsPathForRevocation);

            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            string revStatusListJson = await RevocationApi.CreateRevocationStatusListJsonAsync(mockRevRegDef.CredentialDefinitionId, revRegDefJson, mockRevRegDef.IssuerId, timestamp, IssuerType.ISSUANCE_BY_DEFAULT);

            string mockCredentialJson = await CredentialApi.CreateCredentialAsync(mockCredDefJson, mockCredDefPrivJson, mockCredOfferJson, mockCredReqJson,
                names, raw, enc, revStatusListJson, null, revRegDefJson, revRegDefPvtJson, 1);

            //Act
            //note: only attribute "schema_id", "cred_def_id", "rev_reg_id", "rev_reg_index" supported so far.
            string attrSchemaId = await CredentialApi.GetCredentialAttributeAsync(mockCredentialJson, "schema_id");
            string attrCredDefId = await CredentialApi.GetCredentialAttributeAsync(mockCredentialJson, "cred_def_id");
            string attrRevRegIndex = await CredentialApi.GetCredentialAttributeAsync(mockCredentialJson, "rev_reg_index");

            //Assert
            _ = attrSchemaId.Should().Be(JObject.Parse(mockCredentialJson)["schema_id"].ToString());
            _ = attrCredDefId.Should().Be(JObject.Parse(mockCredentialJson)["cred_def_id"].ToString());
            _ = attrRevRegIndex.Should().Be(JObject.Parse(JObject.Parse(JObject.Parse(mockCredentialJson)["signature"].ToString())["r_credential"].ToString())["i"].ToString());

        }

        [Test, TestCase(TestName = "GetCredentialAttributeAsync() throws AnoncredsRsException when given empty attribute name.")]
        public async Task GetCredentialAttributeAsyncThrowsException()
        {
            //Arrange
            string attributeName = "";
            Credential mockCredential = await MockDataProvider.MockCredential();

            //Act
            Func<Task> act = async () => await CredentialApi.GetCredentialAttributeAsync(mockCredential, attributeName);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }

        [Test, TestCase(TestName = "GetCredentialAttributeAsync() with JSON inputs throws AnoncredsRsException when given empty attribute name.")]
        public async Task GetCredentialAttributeJsonAsyncThrowsException()
        {
            //Arrange
            string attributeName = "";

            string mockCredentialJson = await MockDataProvider.MockCredentialJson();

            //Act
            Func<Task> act = async () => await CredentialApi.GetCredentialAttributeAsync(mockCredentialJson, attributeName);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();

        }
        #endregion
    }
}