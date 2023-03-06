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
            List<string> attrNames = new() { "name", "age", "sex" };
            List<string> attrNamesRaw = new() { "Alex", "20", "male" };
            List<string> attrNamesEnc = await CredentialApi.EncodeCredentialAttributesAsync(attrNamesRaw);
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string proverDid = "VsKV7grR1BUE29mG2Fm2kX";
            string schemaName = "gvt";
            string schemaVersion = "1.0";

            MasterSecret masterSecretObject = await MasterSecretApi.CreateMasterSecretAsync();

            Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (CredentialDefinition credDefObject, CredentialDefinitionPrivate credDefPvtObject, CredentialKeyCorrectnessProof keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = schemaObject.IssuerId;
            CredentialOffer credOfferObject = await CredentialOfferApi.CreateCredentialOfferAsync(schemaId, credDefObject.CredentialDefinitionId, keyProofObject);

            (CredentialRequest credRequestObject, _) =
                await CredentialRequestApi.CreateCredentialRequestAsync(proverDid, credDefObject, masterSecretObject, "testMasterSecretName", credOfferObject);

            //Act
            Credential actual = await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject, attrNames, attrNamesRaw, attrNamesEnc);

            //Assert
            _ = actual.Should().NotBeNull();
        }

        [Test, TestCase(TestName = "CreateCredentialAsync() creates a credential and revocation registry object.")]
        public async Task CreateCredentialAsyncWorksWithRevocation()
        {
            //Arrange
            List<string> attrNames = new() { "name", "age", "sex" };
            List<string> attrNamesRaw = new() { "Alex", "20", "male" };
            List<string> attrNamesEnc = await CredentialApi.EncodeCredentialAttributesAsync(attrNamesRaw);
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string proverDid = "VsKV7grR1BUE29mG2Fm2kX";
            string schemaName = "gvt";
            string schemaVersion = "1.0";
            string testTailsPathForRevocation = null;

            MasterSecret masterSecretObject = await MasterSecretApi.CreateMasterSecretAsync();

            Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (CredentialDefinition credDefObject, CredentialDefinitionPrivate credDefPvtObject, CredentialKeyCorrectnessProof keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = schemaObject.IssuerId;
            CredentialOffer credOfferObject = await CredentialOfferApi.CreateCredentialOfferAsync(schemaId, credDefObject.CredentialDefinitionId, keyProofObject);

            (CredentialRequest credRequestObject, _) =
                await CredentialRequestApi.CreateCredentialRequestAsync(proverDid, credDefObject, masterSecretObject, "testMasterSecretName", credOfferObject);

            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            (RevocationRegistryDefinition revRegDefObject, RevocationRegistryDefinitionPrivate revRegDefPvtObject) = await RevocationApi.CreateRevocationRegistryDefinitionAsync(issuerDid, credDefObject, "test_tag", RegistryType.CL_ACCUM, 99, testTailsPathForRevocation);
            RevocationStatusList revStatusList = await RevocationApi.CreateRevocationStatusListAsync("NcYxiDXkpYi6ov5FcYDi1e:4:NcYxiDXkpYi6ov5FcYDi1e:3:CL:NcYxiDXkpYi6ov5FcYDi1e:2:gvt:1.0:tag:CL_ACCUM:test_tag", revRegDefObject, timestamp, IssuerType.ISSUANCE_BY_DEFAULT);

            //Act
            Credential actual = await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject, attrNames, attrNamesRaw, attrNamesEnc, revStatusList, null, revRegDefObject, revRegDefPvtObject, 1);

            //Assert
            _ = actual.Should().NotBeNull();
        }

        [Test, TestCase(TestName = "CreateCredentialAsync() throws AnoncredsRsException when attribute names do not match their values.")]
        public async Task CreateCredentialAsyncWrongAttributes()
        {
            //Arrange
            List<string> attrNames = new() { "name", "age", "sex" };
            List<string> attrNamesRaw = new() { "Alex" };
            List<string> attrNamesEnc = await CredentialApi.EncodeCredentialAttributesAsync(attrNamesRaw);
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string proverDid = "VsKV7grR1BUE29mG2Fm2kX";
            string schemaName = "gvt";
            string schemaVersion = "1.0";

            MasterSecret masterSecretObject = await MasterSecretApi.CreateMasterSecretAsync();

            Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (CredentialDefinition credDefObject, CredentialDefinitionPrivate credDefPvtObject, CredentialKeyCorrectnessProof keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = schemaObject.IssuerId;
            CredentialOffer credOfferObject = await CredentialOfferApi.CreateCredentialOfferAsync(schemaId, credDefObject.CredentialDefinitionId, keyProofObject);

            (CredentialRequest credRequestObject, CredentialRequestMetadata metaDataObject) =
                await CredentialRequestApi.CreateCredentialRequestAsync(proverDid, credDefObject, masterSecretObject, "testMasterSecretName", credOfferObject);

            //Act
            Func<Task> act = async () => await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject, attrNames, attrNamesRaw, attrNamesEnc);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }

        [Test, TestCase(TestName = "CreateCredentialAsync() with JSON input creates a credential without revocation.")]
        public async Task CreateCredentialAsyncJsonWorks()
        {
            //Arrange
            List<string> attrNames = new() { "name", "age", "sex" };
            List<string> attrNamesRaw = new() { "Alex", "20", "male" };
            List<string> attrNamesEnc = await CredentialApi.EncodeCredentialAttributesAsync(attrNamesRaw);
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string proverDid = "VsKV7grR1BUE29mG2Fm2kX";
            string schemaName = "gvt";
            string schemaVersion = "1.0";

            string masterSecretObjectJson = await MasterSecretApi.CreateMasterSecretJsonAsync();

            string schemaObjectJson = await SchemaApi.CreateSchemaJsonAsync(issuerDid, schemaName, schemaVersion, attrNames);

            (string credDefObjectJson, string credDefPvtObjectJson, string keyProofObjectJson) =
                await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(issuerDid, schemaObjectJson, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = issuerDid;
            string credDefId = issuerDid;
            string credOfferObjectJson = await CredentialOfferApi.CreateCredentialOfferJsonAsync(schemaId, credDefId, keyProofObjectJson);
            (string credRequestObjectJson, _) =
                await CredentialRequestApi.CreateCredentialRequestJsonAsync(proverDid, credDefObjectJson, masterSecretObjectJson, "testMasterSecretName", credOfferObjectJson);

            //Act
            string actual = await CredentialApi.CreateCredentialAsync(credDefObjectJson, credDefPvtObjectJson, credOfferObjectJson, credRequestObjectJson, attrNames, attrNamesRaw, attrNamesEnc);

            //Assert
            _ = actual.Should().NotBeNullOrEmpty();
        }

        [Test, TestCase(TestName = "CreateCredentialAsync() with JSON input creates a credential and revocation registry object.")]
        public async Task CreateCredentialAsyncJsonWorksWithRevocation()
        {
            //Arrange
            List<string> attrNames = new() { "name", "age", "sex" };
            List<string> attrNamesRaw = new() { "Alex", "20", "male" };
            List<string> attrNamesEnc = await CredentialApi.EncodeCredentialAttributesAsync(attrNamesRaw);
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string proverDid = "VsKV7grR1BUE29mG2Fm2kX";
            string schemaName = "gvt";
            string schemaVersion = "1.0";
            string testTailsPathForRevocation = null;

            string masterSecretObjectJson = await MasterSecretApi.CreateMasterSecretJsonAsync();

            string schemaObjectJson = await SchemaApi.CreateSchemaJsonAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (string credDefObjectJson, string credDefPvtObjectJson, string keyProofObjectJson) =
                await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(issuerDid, schemaObjectJson, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = issuerDid;
            string credDefId = issuerDid;
            string credOfferObjectJson = await CredentialOfferApi.CreateCredentialOfferJsonAsync(schemaId, credDefId, keyProofObjectJson);
            (string credRequestObjectJson, _) =
                await CredentialRequestApi.CreateCredentialRequestJsonAsync(proverDid, credDefObjectJson, masterSecretObjectJson, "testMasterSecretName", credOfferObjectJson);
            (string revRegDefObjectJson, string revRegDefPvtObjectJson) =
                await RevocationApi.CreateRevocationRegistryDefinitionJsonAsync(issuerDid, credDefObjectJson, "test_tag", RegistryType.CL_ACCUM, 99, testTailsPathForRevocation);

            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            string revStatusListJson = await RevocationApi.CreateRevocationStatusListJsonAsync("NcYxiDXkpYi6ov5FcYDi1e:4:NcYxiDXkpYi6ov5FcYDi1e:3:CL:NcYxiDXkpYi6ov5FcYDi1e:2:gvt:1.0:tag:CL_ACCUM:test_tag", revRegDefObjectJson, timestamp, IssuerType.ISSUANCE_BY_DEFAULT);

            //Act
            string actual = await CredentialApi.CreateCredentialAsync(credDefObjectJson, credDefPvtObjectJson, credOfferObjectJson, credRequestObjectJson,
                attrNames, attrNamesRaw, attrNamesEnc, revStatusListJson, null, revRegDefObjectJson, revRegDefPvtObjectJson, 1);

            //Assert
            _ = actual.Should().NotBeNullOrEmpty();
        }

        [Test, TestCase(TestName = "CreateCredentialAsync() throws AnoncredsRsException when revocation data is incomplete.")]
        public async Task CreateCredentialAsyncJsonIncompleteRevocation()
        {
            //Arrange
            List<string> attrNames = new() { "name", "age", "sex" };
            List<string> attrNamesRaw = new() { "Alex", "20", "male" };
            List<string> attrNamesEnc = await CredentialApi.EncodeCredentialAttributesAsync(attrNamesRaw);
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string proverDid = "VsKV7grR1BUE29mG2Fm2kX";
            string schemaName = "gvt";
            string schemaVersion = "1.0";
            string testTailsPathForRevocation = null;

            string masterSecretObjectJson = await MasterSecretApi.CreateMasterSecretJsonAsync();

            string schemaObjectJson = await SchemaApi.CreateSchemaJsonAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (string credDefObjectJson, string credDefPvtObjectJson, string keyProofObjectJson) =
                await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(issuerDid, schemaObjectJson, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = issuerDid;
            string credDefId = issuerDid;
            string credOfferObjectJson = await CredentialOfferApi.CreateCredentialOfferJsonAsync(schemaId, credDefId, keyProofObjectJson);

            (string credRequestObjectJson, string metaDataObjectJson) =
                await CredentialRequestApi.CreateCredentialRequestJsonAsync(proverDid, credDefObjectJson, masterSecretObjectJson, "testMasterSecretName", credOfferObjectJson);

            (string revRegDefObjectJson, string revRegDefPvtObjectJson) =
                await RevocationApi.CreateRevocationRegistryDefinitionJsonAsync(issuerDid, credDefObjectJson, "test_tag", RegistryType.CL_ACCUM, 99, testTailsPathForRevocation);
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            string revStatusListJson = await RevocationApi.CreateRevocationStatusListJsonAsync("NcYxiDXkpYi6ov5FcYDi1e:4:NcYxiDXkpYi6ov5FcYDi1e:3:CL:NcYxiDXkpYi6ov5FcYDi1e:2:gvt:1.0:tag:CL_ACCUM:test_tag", revRegDefObjectJson, timestamp, IssuerType.ISSUANCE_BY_DEFAULT);

            //Act
            Func<Task> act = async () => await CredentialApi.CreateCredentialAsync(credDefObjectJson, credDefPvtObjectJson, credOfferObjectJson, credRequestObjectJson,
                attrNames, attrNamesRaw, attrNamesEnc, revStatusListJson, null, revRegDefObjectJson, "", 1);

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
            List<string> attrNames = new() { "name", "age", "sex" };
            List<string> attrNamesRaw = new() { "Alex", "20", "male" };
            List<string> attrNamesEnc = await CredentialApi.EncodeCredentialAttributesAsync(attrNamesRaw);
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string proverDid = "VsKV7grR1BUE29mG2Fm2kX";
            string schemaName = "gvt";
            string schemaVersion = "1.0";
            MasterSecret masterSecretObject = await MasterSecretApi.CreateMasterSecretAsync();

            Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (CredentialDefinition credDefObject, CredentialDefinitionPrivate credDefPvtObject, CredentialKeyCorrectnessProof keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = schemaObject.IssuerId;
            CredentialOffer credOfferObject = await CredentialOfferApi.CreateCredentialOfferAsync(schemaId, credDefObject.CredentialDefinitionId, keyProofObject);

            (CredentialRequest credRequestObject, CredentialRequestMetadata metaDataObject) =
                await CredentialRequestApi.CreateCredentialRequestAsync(proverDid, credDefObject, masterSecretObject, "testMasterSecretName", credOfferObject);

            Credential credObject = await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject, attrNames, attrNamesRaw, attrNamesEnc);

            //Act
            Credential credObjectProcessed = await CredentialApi.ProcessCredentialAsync(credObject, metaDataObject, masterSecretObject, credDefObject);

            //Assert
            _ = credObjectProcessed.Should().BeOfType(typeof(Credential));
        }

        [Test, TestCase(TestName = "ProcessCredentialAsync() with JSON input creates a credential object.")]
        public async Task ProcessCredentialAsyncJson()
        {
            //Arrange
            List<string> attrNames = new() { "name", "age", "sex" };
            List<string> attrNamesRaw = new() { "Alex", "20", "male" };
            List<string> attrNamesEnc = await CredentialApi.EncodeCredentialAttributesAsync(attrNamesRaw);
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string proverDid = "VsKV7grR1BUE29mG2Fm2kX";
            string schemaName = "gvt";
            string schemaVersion = "1.0";

            string masterSecretObjectJson = await MasterSecretApi.CreateMasterSecretJsonAsync();

            string schemaObjectJson = await SchemaApi.CreateSchemaJsonAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (string credDefObjectJson, string credDefPvtObjectJson, string keyProofObjectJson) =
                await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(issuerDid, schemaObjectJson, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = issuerDid;
            string credDefId = issuerDid;
            string credOfferObjectJson = await CredentialOfferApi.CreateCredentialOfferJsonAsync(schemaId, credDefId, keyProofObjectJson);

            (string credRequestObjectJson, string metaDataObjectJson) =
                await CredentialRequestApi.CreateCredentialRequestJsonAsync(proverDid, credDefObjectJson, masterSecretObjectJson, "testMasterSecretName", credOfferObjectJson);

            string credObjectJson = await CredentialApi.CreateCredentialAsync(credDefObjectJson, credDefPvtObjectJson, credOfferObjectJson, credRequestObjectJson, attrNames, attrNamesRaw, attrNamesEnc);

            //Act
            string credObjectProcessedJson = await CredentialApi.ProcessCredentialAsync(credObjectJson, metaDataObjectJson, masterSecretObjectJson, credDefObjectJson);

            //Assert
            _ = credObjectProcessedJson.Should().NotBeNullOrEmpty();
        }

        [Test, TestCase(TestName = "ProcessCredentialAsync() throws AnoncredsRsException when maste secret does not match credential.")]
        public async Task ProcessCredentialAsyncThrowsException()
        {
            //Arrange
            List<string> attrNames = new() { "name", "age", "sex" };
            List<string> attrNamesRaw = new() { "Alex", "20", "male" };
            List<string> attrNamesEnc = await CredentialApi.EncodeCredentialAttributesAsync(attrNamesRaw);
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string proverDid = "VsKV7grR1BUE29mG2Fm2kX";
            string schemaName = "gvt";
            string schemaVersion = "1.0";

            MasterSecret masterSecretObject = await MasterSecretApi.CreateMasterSecretAsync();
            MasterSecret masterSecretObject2 = await MasterSecretApi.CreateMasterSecretAsync();

            Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (CredentialDefinition credDefObject, CredentialDefinitionPrivate credDefPvtObject, CredentialKeyCorrectnessProof keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = schemaObject.IssuerId;
            CredentialOffer credOfferObject = await CredentialOfferApi.CreateCredentialOfferAsync(schemaId, credDefObject.CredentialDefinitionId, keyProofObject);

            (CredentialRequest credRequestObject, CredentialRequestMetadata metaDataObject) =
                await CredentialRequestApi.CreateCredentialRequestAsync(proverDid, credDefObject, masterSecretObject, "testMasterSecretName", credOfferObject);

            Credential credObject = await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject, attrNames, attrNamesRaw, attrNamesEnc);

            //Act
            Func<Task> act = async () => await CredentialApi.ProcessCredentialAsync(credObject, metaDataObject, masterSecretObject2, credDefObject);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }
        #endregion

        #region Tests for GetCredentialAttributeAsync
        [Test, TestCase(TestName = "GetCredentialAttributeAsync() works for attribute names: schema_id, cred_def_id, rev_reg_id.")]
        public async Task GetCredentialAttributeAsync()
        {
            //Arrange
            List<string> attrNames = new() { "name", "age", "sex" };
            List<string> attrNamesRaw = new() { "Alex", "20", "male" };
            List<string> attrNamesEnc = await CredentialApi.EncodeCredentialAttributesAsync(attrNamesRaw);
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string proverDid = "VsKV7grR1BUE29mG2Fm2kX";
            string schemaName = "gvt";
            string schemaVersion = "1.0";
            MasterSecret masterSecretObject = await MasterSecretApi.CreateMasterSecretAsync();
            string testTailsPathForRevocation = null;

            Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (CredentialDefinition credDefObject, CredentialDefinitionPrivate credDefPvtObject, CredentialKeyCorrectnessProof keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = schemaObject.IssuerId;
            CredentialOffer credOfferObject = await CredentialOfferApi.CreateCredentialOfferAsync(schemaId, credDefObject.CredentialDefinitionId, keyProofObject);
            (CredentialRequest credRequestObject, _) =
                await CredentialRequestApi.CreateCredentialRequestAsync(proverDid, credDefObject, masterSecretObject, "testMasterSecretName", credOfferObject);

            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            (RevocationRegistryDefinition revRegDefObject, RevocationRegistryDefinitionPrivate revRegDefPvtObject) = await RevocationApi.CreateRevocationRegistryDefinitionAsync(issuerDid, credDefObject, "test_tag", RegistryType.CL_ACCUM, 99, testTailsPathForRevocation);
            RevocationStatusList revStatusList = await RevocationApi.CreateRevocationStatusListAsync("NcYxiDXkpYi6ov5FcYDi1e:4:NcYxiDXkpYi6ov5FcYDi1e:3:CL:NcYxiDXkpYi6ov5FcYDi1e:2:gvt:1.0:tag:CL_ACCUM:test_tag", revRegDefObject, timestamp, IssuerType.ISSUANCE_BY_DEFAULT);

            Credential credObject = await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject, attrNames, attrNamesRaw, attrNamesEnc, revStatusList, null, revRegDefObject, revRegDefPvtObject, 1);

            //Act
            //note: only attribute "schema_id", "cred_def_id", "rev_reg_id", "rev_reg_index" supported so far.
            string attrSchemaId = await CredentialApi.GetCredentialAttributeAsync(credObject, "schema_id");
            string attrCredDefId = await CredentialApi.GetCredentialAttributeAsync(credObject, "cred_def_id");
            string attrRevRegId = await CredentialApi.GetCredentialAttributeAsync(credObject, "rev_reg_id");
            string attrRevRegIndex = await CredentialApi.GetCredentialAttributeAsync(credObject, "rev_reg_index");
            //string attrDefault = await CredentialApi.GetCredentialAttributeAsync(credObject, "default");

            //Assert
            _ = attrSchemaId.Should().Be(credObject.SchemaId);
            _ = attrCredDefId.Should().Be(credObject.CredentialDefinitionId);
            _ = attrRevRegId.Should().Be(credObject.RevocationRegistryId);
            _ = attrRevRegIndex.Should().Be(credObject.Signature.RCredential.I.ToString());
            //attrDefault.Should().Be("");
        }

        [Test, TestCase(TestName = "GetCredentialAttributeAsync() with JSON inputs works for attribute names: schema_id, cred_def_id, rev_reg_id.")]
        public async Task GetCredentialAttributeJsonAsync()
        {
            //Arrange
            List<string> attrNames = new() { "name", "age", "sex" };
            List<string> attrNamesRaw = new() { "Alex", "20", "male" };
            List<string> attrNamesEnc = await CredentialApi.EncodeCredentialAttributesAsync(attrNamesRaw);
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string proverDid = "VsKV7grR1BUE29mG2Fm2kX";
            string schemaName = "gvt";
            string schemaVersion = "1.0";
            string masterSecretObject = await MasterSecretApi.CreateMasterSecretJsonAsync();
            string testTailsPathForRevocation = null;

            string schemaObject = await SchemaApi.CreateSchemaJsonAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (string credDefObjectJson, string credDefPvtObjectJson, string keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(issuerDid, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = issuerDid;
            string credDefId = issuerDid;
            string credOfferObjectJson = await CredentialOfferApi.CreateCredentialOfferJsonAsync(schemaId, credDefId, keyProofObject);
            (string credRequestObjectJson, _) =
                await CredentialRequestApi.CreateCredentialRequestJsonAsync(proverDid, credDefObjectJson, masterSecretObject, "testMasterSecretName", credOfferObjectJson);

            (string revRegDefObjectJson, string revRegDefPvtObjectJson) =
                await RevocationApi.CreateRevocationRegistryDefinitionJsonAsync(issuerDid, credDefObjectJson, "test_tag", RegistryType.CL_ACCUM, 99, testTailsPathForRevocation);

            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            string revStatusListJson = await RevocationApi.CreateRevocationStatusListJsonAsync("NcYxiDXkpYi6ov5FcYDi1e:4:NcYxiDXkpYi6ov5FcYDi1e:3:CL:NcYxiDXkpYi6ov5FcYDi1e:2:gvt:1.0:tag:CL_ACCUM:test_tag", revRegDefObjectJson, timestamp, IssuerType.ISSUANCE_BY_DEFAULT);


            string credObject = await CredentialApi.CreateCredentialAsync(credDefObjectJson, credDefPvtObjectJson, credOfferObjectJson, credRequestObjectJson,
                attrNames, attrNamesRaw, attrNamesEnc, revStatusListJson, null, revRegDefObjectJson, revRegDefPvtObjectJson, 1);

            //Act
            //note: only attribute "schema_id", "cred_def_id", "rev_reg_id", "rev_reg_index" supported so far.
            string attrSchemaId = await CredentialApi.GetCredentialAttributeAsync(credObject, "schema_id");
            string attrCredDefId = await CredentialApi.GetCredentialAttributeAsync(credObject, "cred_def_id");
            string attrRevRegIndex = await CredentialApi.GetCredentialAttributeAsync(credObject, "rev_reg_index");

            //Assert
            _ = attrSchemaId.Should().Be(JObject.Parse(credObject)["schema_id"].ToString());
            _ = attrCredDefId.Should().Be(JObject.Parse(credObject)["cred_def_id"].ToString());
            _ = attrRevRegIndex.Should().Be(JObject.Parse(JObject.Parse(JObject.Parse(credObject)["signature"].ToString())["r_credential"].ToString())["i"].ToString());

        }

        [Test, TestCase(TestName = "GetCredentialAttributeAsync() throws AnoncredsRsException when given empty attribute name.")]
        public async Task GetCredentialAttributeAsyncThrowsException()
        {
            //Arrange
            string attributeName = "";
            List<string> attrNames = new() { "name", "age", "sex" };
            List<string> attrNamesRaw = new() { "Alex", "20", "male" };
            List<string> attrNamesEnc = await CredentialApi.EncodeCredentialAttributesAsync(attrNamesRaw);
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string proverDid = "VsKV7grR1BUE29mG2Fm2kX";
            string schemaName = "gvt";
            string schemaVersion = "1.0";
            MasterSecret masterSecretObject = await MasterSecretApi.CreateMasterSecretAsync();

            Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (CredentialDefinition credDefObject, CredentialDefinitionPrivate credDefPvtObject, CredentialKeyCorrectnessProof keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = schemaObject.IssuerId;
            CredentialOffer credOfferObject = await CredentialOfferApi.CreateCredentialOfferAsync(schemaId, credDefObject.CredentialDefinitionId, keyProofObject);

            (CredentialRequest credRequestObject, CredentialRequestMetadata metaDataObject) =
                await CredentialRequestApi.CreateCredentialRequestAsync(proverDid, credDefObject, masterSecretObject, "testMasterSecretName", credOfferObject);

            Credential credObject = await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject, attrNames, attrNamesRaw, attrNamesEnc);

            //Act
            Func<Task> act = async () => await CredentialApi.GetCredentialAttributeAsync(credObject, attributeName);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }

        [Test, TestCase(TestName = "GetCredentialAttributeAsync() with JSON inputs throws AnoncredsRsException when given empty attribute name.")]
        public async Task GetCredentialAttributeJsonAsyncThrowsException()
        {
            //Arrange
            string attributeName = "";

            List<string> attrNames = new() { "name", "age", "sex" };
            List<string> attrNamesRaw = new() { "Alex", "20", "male" };
            List<string> attrNamesEnc = await CredentialApi.EncodeCredentialAttributesAsync(attrNamesRaw);
            string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
            string proverDid = "VsKV7grR1BUE29mG2Fm2kX";
            string schemaName = "gvt";
            string schemaVersion = "1.0";
            string masterSecretObject = await MasterSecretApi.CreateMasterSecretJsonAsync();

            string schemaObject = await SchemaApi.CreateSchemaJsonAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (string credDefObjectJson, string credDefPvtObjectJson, string keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(issuerDid, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = issuerDid;
            string credDefId = issuerDid;
            string credOfferObjectJson = await CredentialOfferApi.CreateCredentialOfferJsonAsync(schemaId, credDefId, keyProofObject);

            (string credRequestObjectJson, string metaDataObject) =
                await CredentialRequestApi.CreateCredentialRequestJsonAsync(proverDid, credDefObjectJson, masterSecretObject, "testMasterSecretName", credOfferObjectJson);

            string credObject = await CredentialApi.CreateCredentialAsync(credDefObjectJson, credDefPvtObjectJson, credOfferObjectJson, credRequestObjectJson, attrNames, attrNamesRaw, attrNamesEnc);

            //Act
            Func<Task> act = async () => await CredentialApi.GetCredentialAttributeAsync(credObject, attributeName);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();

        }
        #endregion
    }
}