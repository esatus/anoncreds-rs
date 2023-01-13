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
            (Credential credObject, RevocationRegistry revRegObjectNew, RevocationRegistryDelta revDeltaObject) =
                await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject,
                attrNames, attrNamesRaw, attrNamesEnc);

            //Assert
            _ = credObject.Should().NotBeNull();
            _ = revRegObjectNew.Should().BeNull();
            _ = revDeltaObject.Should().BeNull();
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
            (RevocationRegistryDefinition revRegDefObject, RevocationRegistryDefinitionPrivate revRegDefPvtObject, RevocationRegistry revRegObject, _) =
                await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDefObject, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPathForRevocation);


            //Act
            (Credential credObject, RevocationRegistry revRegObjectNew, RevocationRegistryDelta revDeltaObject) =
                await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject,
                attrNames, attrNamesRaw, attrNamesEnc, revRegDefObject, revRegDefPvtObject, revRegObject, 1, new List<long> { 1 });

            //Assert
            _ = credObject.Should().NotBeNull();
            _ = revRegObjectNew.Should().NotBeNull();
            _ = revDeltaObject.Should().BeNull();
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
            string testTailsPathForRevocation = null;

            MasterSecret masterSecretObject = await MasterSecretApi.CreateMasterSecretAsync();

            Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (CredentialDefinition credDefObject, CredentialDefinitionPrivate credDefPvtObject, CredentialKeyCorrectnessProof keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = schemaObject.IssuerId;
            CredentialOffer credOfferObject = await CredentialOfferApi.CreateCredentialOfferAsync(schemaId, credDefObject.CredentialDefinitionId, keyProofObject);

            (CredentialRequest credRequestObject, CredentialRequestMetadata metaDataObject) =
                await CredentialRequestApi.CreateCredentialRequestAsync(proverDid, credDefObject, masterSecretObject, "testMasterSecretName", credOfferObject);

            (RevocationRegistryDefinition revRegDefObject, RevocationRegistryDefinitionPrivate revRegDefPvtObject, RevocationRegistry revRegObject, RevocationRegistryDelta revRegDeltaObject) =
                await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDefObject, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPathForRevocation);

            //Act
            Func<Task> act = async () => await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject,
                attrNames, attrNamesRaw, attrNamesEnc, revRegDefObject, revRegDefPvtObject, revRegObject, 1, new List<long>() { 1 });

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
            (string credObjectJson, string revRegObjectNewJson, string revDeltaObjectJson) =
                await CredentialApi.CreateCredentialAsync(credDefObjectJson, credDefPvtObjectJson, credOfferObjectJson, credRequestObjectJson,
                attrNames, attrNamesRaw, attrNamesEnc);

            //Assert
            _ = credObjectJson.Should().NotBeNullOrEmpty();
            _ = revRegObjectNewJson.Should().BeNullOrEmpty();
            _ = revDeltaObjectJson.Should().BeNullOrEmpty();
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
            (string revRegDefObjectJson, string revRegDefPvtObjectJson, string revRegObjectJson, _) =
                await RevocationApi.CreateRevocationRegistryJsonAsync(issuerDid, credDefObjectJson, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPathForRevocation);

            //Act
            (string credObjectJson, string revRegObjectNewJson, string revDeltaObjectJson) =
                await CredentialApi.CreateCredentialAsync(credDefObjectJson, credDefPvtObjectJson, credOfferObjectJson, credRequestObjectJson,
                attrNames, attrNamesRaw, attrNamesEnc, revRegDefObjectJson, revRegDefPvtObjectJson, revRegObjectJson, 1, new List<long> { 1 });

            //Assert
            _ = credObjectJson.Should().NotBeNullOrEmpty();
            _ = revRegObjectNewJson.Should().NotBeNullOrEmpty();
            _ = revDeltaObjectJson.Should().BeNullOrEmpty();
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

            (string revRegDefObjectJson, string revRegDefPvtObjectJson, string revRegObjectJson, string revRegDeltaObjectJson) =
                await RevocationApi.CreateRevocationRegistryJsonAsync(issuerDid, credDefObjectJson, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPathForRevocation);

            //Act
            Func<Task> act = async () => await CredentialApi.CreateCredentialAsync(credDefObjectJson, credDefPvtObjectJson, credOfferObjectJson, credRequestObjectJson,
                attrNames, attrNamesRaw, attrNamesEnc, revRegDefObjectJson, null, revRegObjectJson, 1, new List<long> { 1 });

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
            string testTailsPathForRevocation = null;

            MasterSecret masterSecretObject = await MasterSecretApi.CreateMasterSecretAsync();

            Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (CredentialDefinition credDefObject, CredentialDefinitionPrivate credDefPvtObject, CredentialKeyCorrectnessProof keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = schemaObject.IssuerId;
            CredentialOffer credOfferObject = await CredentialOfferApi.CreateCredentialOfferAsync(schemaId, credDefObject.CredentialDefinitionId, keyProofObject);

            (CredentialRequest credRequestObject, CredentialRequestMetadata metaDataObject) =
                await CredentialRequestApi.CreateCredentialRequestAsync(proverDid, credDefObject, masterSecretObject, "testMasterSecretName", credOfferObject);

            (RevocationRegistryDefinition revRegDefObject, RevocationRegistryDefinitionPrivate revRegDefPvtObject, RevocationRegistry revRegObject, _) =
                await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDefObject, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPathForRevocation);


            (Credential credObject, _, _) =
                await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject,
                attrNames, attrNamesRaw, attrNamesEnc, revRegDefObject, revRegDefPvtObject, revRegObject, 1, new List<long>() { 1 });

            //Act
            Credential credObjectProcessed =
                await CredentialApi.ProcessCredentialAsync(credObject, metaDataObject, masterSecretObject, credDefObject, revRegDefObject);

            //Assert
            _ = credObjectProcessed.Should().BeOfType(typeof(Credential));
        }

        [Test, TestCase(TestName = "ProcessCredentialAsync() with JSON input creates a credential object.")]
        public async Task ProcessCredentialAsync2()
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

            (string revRegDefObjectJson, string revRegDefPvtObjectJson, string revRegObjectJson, _) =
                await RevocationApi.CreateRevocationRegistryJsonAsync(issuerDid, credDefObjectJson, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPathForRevocation);

            (string credObjectJson, _, _) =
                await CredentialApi.CreateCredentialAsync(credDefObjectJson, credDefPvtObjectJson, credOfferObjectJson, credRequestObjectJson,
                attrNames, attrNamesRaw, attrNamesEnc, revRegDefObjectJson, revRegDefPvtObjectJson, revRegObjectJson, 1, new List<long>() { 1 });

            //Act
            string credObjectProcessedJson =
                await CredentialApi.ProcessCredentialAsync(credObjectJson, metaDataObjectJson, masterSecretObjectJson, credDefObjectJson, revRegDefObjectJson);

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
            string testTailsPathForRevocation = null;

            MasterSecret masterSecretObject = await MasterSecretApi.CreateMasterSecretAsync();
            MasterSecret masterSecretObject2 = await MasterSecretApi.CreateMasterSecretAsync();

            Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (CredentialDefinition credDefObject, CredentialDefinitionPrivate credDefPvtObject, CredentialKeyCorrectnessProof keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = schemaObject.IssuerId;
            CredentialOffer credOfferObject = await CredentialOfferApi.CreateCredentialOfferAsync(schemaId, credDefObject.CredentialDefinitionId, keyProofObject);

            (CredentialRequest credRequestObject, CredentialRequestMetadata metaDataObject) =
                await CredentialRequestApi.CreateCredentialRequestAsync(proverDid, credDefObject, masterSecretObject, "testMasterSecretName", credOfferObject);

            (RevocationRegistryDefinition revRegDefObject, RevocationRegistryDefinitionPrivate revRegDefPvtObject, RevocationRegistry revRegObject, _) =
                await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDefObject, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPathForRevocation);

            (Credential credObject, _, _) =
                await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject,
                attrNames, attrNamesRaw, attrNamesEnc, revRegDefObject, revRegDefPvtObject, revRegObject, 1, new List<long>() { 1 });

            //Act
            Func<Task> act = async () => await CredentialApi.ProcessCredentialAsync(credObject, metaDataObject, masterSecretObject2, credDefObject, revRegDefObject);

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
            string testTailsPathForRevocation = null;
            MasterSecret masterSecretObject = await MasterSecretApi.CreateMasterSecretAsync();

            Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (CredentialDefinition credDefObject, CredentialDefinitionPrivate credDefPvtObject, CredentialKeyCorrectnessProof keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = schemaObject.IssuerId;
            CredentialOffer credOfferObject = await CredentialOfferApi.CreateCredentialOfferAsync(schemaId, credDefObject.CredentialDefinitionId, keyProofObject);
            (CredentialRequest credRequestObject, _) =
                await CredentialRequestApi.CreateCredentialRequestAsync(proverDid, credDefObject, masterSecretObject, "testMasterSecretName", credOfferObject);
            (RevocationRegistryDefinition revRegDefObject, RevocationRegistryDefinitionPrivate revRegDefPvtObject, RevocationRegistry revRegObject, _) =
                await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDefObject, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPathForRevocation);
            (Credential credObject, _, _) =
                await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject,
                attrNames, attrNamesRaw, attrNamesEnc, revRegDefObject, revRegDefPvtObject, revRegObject, 1, new List<long>() { 1 });

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
            string testTailsPathForRevocation = null;
            string masterSecretObject = await MasterSecretApi.CreateMasterSecretJsonAsync();

            string schemaObject = await SchemaApi.CreateSchemaJsonAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (string credDefObject, string credDefPvtObject, string keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(issuerDid, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = issuerDid;
            string credDefId = issuerDid;
            string credOfferObject = await CredentialOfferApi.CreateCredentialOfferJsonAsync(schemaId, credDefId, keyProofObject);
            (string credRequestObject, _) =
                await CredentialRequestApi.CreateCredentialRequestJsonAsync(proverDid, credDefObject, masterSecretObject, "testMasterSecretName", credOfferObject);
            (string revRegDefObject, string revRegDefPvtObject, string revRegObject, _) =
                await RevocationApi.CreateRevocationRegistryJsonAsync(issuerDid, credDefObject, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPathForRevocation);
            (string credObject, _, _) =
                await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject,
                attrNames, attrNamesRaw, attrNamesEnc, revRegDefObject, revRegDefPvtObject, revRegObject, 1, new List<long>() { 1 });

            //Act
            //note: only attribute "schema_id", "cred_def_id", "rev_reg_id", "rev_reg_index" supported so far.
            string attrSchemaId = await CredentialApi.GetCredentialAttributeAsync(credObject, "schema_id");
            string attrCredDefId = await CredentialApi.GetCredentialAttributeAsync(credObject, "cred_def_id");
            string attrRevRegId = await CredentialApi.GetCredentialAttributeAsync(credObject, "rev_reg_id");
            string attrRevRegIndex = await CredentialApi.GetCredentialAttributeAsync(credObject, "rev_reg_index");

            //Assert
            _ = attrSchemaId.Should().Be(JObject.Parse(credObject)["schema_id"].ToString());
            _ = attrCredDefId.Should().Be(JObject.Parse(credObject)["cred_def_id"].ToString());
            _ = attrRevRegId.Should().Be(JObject.Parse(credObject)["rev_reg_id"].ToString());
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
            string testTailsPathForRevocation = null;
            MasterSecret masterSecretObject = await MasterSecretApi.CreateMasterSecretAsync();

            Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (CredentialDefinition credDefObject, CredentialDefinitionPrivate credDefPvtObject, CredentialKeyCorrectnessProof keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag",issuerDid ,SignatureType.CL, true);

            string schemaId = schemaObject.IssuerId;
            CredentialOffer credOfferObject = await CredentialOfferApi.CreateCredentialOfferAsync(schemaId, credDefObject.CredentialDefinitionId, keyProofObject);

            (CredentialRequest credRequestObject, CredentialRequestMetadata metaDataObject) =
                await CredentialRequestApi.CreateCredentialRequestAsync(proverDid, credDefObject, masterSecretObject, "testMasterSecretName", credOfferObject);

            (RevocationRegistryDefinition revRegDefObject, RevocationRegistryDefinitionPrivate revRegDefPvtObject, RevocationRegistry revRegObject, RevocationRegistryDelta revRegDeltaObject) =
                await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDefObject, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPathForRevocation);

            (Credential credObject, RevocationRegistry revRegObjectNew, RevocationRegistryDelta revDeltaObject) =
                await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject,
                attrNames, attrNamesRaw, attrNamesEnc, revRegDefObject, revRegDefPvtObject, revRegObject, 1, new List<long>() { 1 });

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
            string testTailsPathForRevocation = null;
            string masterSecretObject = await MasterSecretApi.CreateMasterSecretJsonAsync();

            string schemaObject = await SchemaApi.CreateSchemaJsonAsync(issuerDid, schemaName, schemaVersion, attrNames);
            (string credDefObject, string credDefPvtObject, string keyProofObject) =
                await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(issuerDid, schemaObject, "tag", issuerDid, SignatureType.CL, true);

            string schemaId = issuerDid;
            string credDefId = issuerDid;
            string credOfferObject = await CredentialOfferApi.CreateCredentialOfferJsonAsync(schemaId, credDefId, keyProofObject);

            (string credRequestObject, string metaDataObject) =
                await CredentialRequestApi.CreateCredentialRequestJsonAsync(proverDid, credDefObject, masterSecretObject, "testMasterSecretName", credOfferObject);

            (string revRegDefObject, string revRegDefPvtObject, string revRegObject, string revRegDeltaObject) =
                await RevocationApi.CreateRevocationRegistryJsonAsync(issuerDid, credDefObject, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPathForRevocation);

            (string credObject, string revRegObjectNew, string revDeltaObject) =
                await CredentialApi.CreateCredentialAsync(credDefObject, credDefPvtObject, credOfferObject, credRequestObject,
                attrNames, attrNamesRaw, attrNamesEnc, revRegDefObject, revRegDefPvtObject, revRegObject, 1, new List<long>() { 1 });

            //Act
            Func<Task> act = async () => await CredentialApi.GetCredentialAttributeAsync(credObject, attributeName);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();

        }
        #endregion
    }
}