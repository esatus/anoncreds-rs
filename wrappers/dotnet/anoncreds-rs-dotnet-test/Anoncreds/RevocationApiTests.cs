using anoncreds_rs_dotnet.Anoncreds;
using anoncreds_rs_dotnet.Models;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace anoncreds_rs_dotnet_test.Anoncreds
{
    public class RevocationApiTests
    {
        #region Tests for CreateRevocationStatusList
        [Test, TestCase(TestName = "CreateRevocationStatusListAsync() returns a RevocationStatusList object.")]
        public async Task CreateRevocationStatusListAsyncWorks()
        {
            //Arrange
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            CredentialDefinition mockCredDef = (await MockDataProvider.MockCredDef()).Item1;
            RevocationRegistryDefinition mockRevRegDef = (await MockDataProvider.MockRevRegDef(credDef: mockCredDef)).Item1;

            //Act
            RevocationStatusList actual = await RevocationApi.CreateRevocationStatusListAsync(mockRevRegDef.IssuerId, mockRevRegDef, mockCredDef.IssuerId, timestamp, IssuerType.ISSUANCE_BY_DEFAULT);

            //Assert
            _ = actual.Should().BeOfType<RevocationStatusList>();
        }

        [Test, TestCase(TestName = "CreateRevocationStatusListJsonAsync() returns a RevocationStatusList JSON.")]
        public async Task CreateRevocationStatusListJsonAsyncWorks()
        {
            //Arrange
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            string mockCredDef = (await MockDataProvider.MockCredDefJson()).Item1;
            string mockRevRegDef = (await MockDataProvider.MockRevRegDefJson(credDef: mockCredDef)).Item1;

            //Act
            string actual = await RevocationApi.CreateRevocationStatusListJsonAsync(JObject.Parse(mockRevRegDef)["issuerId"].ToString(), mockRevRegDef, JObject.Parse(mockCredDef)["issuerId"].ToString(), timestamp, IssuerType.ISSUANCE_BY_DEFAULT);

            //Assert
            _ = actual.Should().NotBeNullOrEmpty();
        }
        #endregion

        #region Tests for CreateRevocationRegistryFromJson
        [Test, TestCase(TestName = "CreateRevocationRegistryFromJsonAsync() works.")]
        public async Task CreateRevocationRegistryFromJsonAsyncWorks()
        {
            //Arrange
            string revRegJson = "{" +
                "\"ver\":\"1.0\"," +
                "\"value\":{" +
                    "\"accum\":\"21 121B0EBFF6EFDCD588B810E6ACF0394EC5D0EC1F44CA0987BFED06D73658566FF 21 1229800B78BFC8C1DF553C48D4A32F13D116A85494146D8727B0ABFD0C249F06E 6 8372FF1535B689AD7F4DB98C8A0FFCE387074208FE55DC9A94628A4FF0E921EE 4 364D0DF6297A4C8B1AF4B0B33EB80528D6D1559ED633D2106F3930152C7FE700 6 74AE0C197DD8E241B5F77DFD4CDF219A80091C4CFB869D72F8EDC22C4562E0F8 4 2BA80EE5693D063BBBA422DCCCB0C6CE4354378C9B81FCAE92C609F0A01C1D1E\"" +
                    "}" +
                "}";

            //Act
            RevocationRegistry expected = await RevocationApi.CreateRevocationRegistryFromJsonAsync(revRegJson);

            //Assert
            _ = expected.Should().BeOfType<RevocationRegistry>();
        }

        [Test, TestCase(TestName = "CreateRevocationRegistryFromJsonAsync() throws AnoncredsRsException when provided with an empty json string ")]
        public async Task CreateRevocationRegistryFromJsonAsyncThrowsExceptionForEmptyString()
        {
            //Arrange
            string revRegJson = "";

            //Act
            Func<Task> act = async () => await RevocationApi.CreateRevocationRegistryFromJsonAsync(revRegJson);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }

        [Test, TestCase(TestName = "CreateRevocationRegistryFromJsonAsync() throws AnoncredsRsException when provided with invalid json string.")]
        public async Task CreateRevocationRegistryFromJsonAsyncThrowsExceptionForInvalidString()
        {
            //Arrange
            string revRegJson = "{}";

            //Act
            Func<Task> act = async () => await RevocationApi.CreateRevocationRegistryFromJsonAsync(revRegJson);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }
        #endregion

        #region Tests for CreateRevocationRegistryDefinitionFromJson
        [Test, TestCase(TestName = "CreateRevocationRegistryDefinitionFromJsonAsync() creates a new revocation registry definition.")]
        public async Task CreateRevocationRegistryDefinitionFromJsonAsyncWorks()
        {
            //Arrange
            string mockRevRegDefJson = (await MockDataProvider.MockRevRegDefJson()).Item1;

            //Act
            RevocationRegistryDefinition expected = await RevocationApi.CreateRevocationRegistryDefinitionFromJsonAsync(mockRevRegDefJson);

            //Assert
            _ = expected.Should().BeOfType<RevocationRegistryDefinition>();
        }

        [Test, TestCase(TestName = "CreateRevocationRegistryDefinitionFromJsonAsync() throws AnoncredsRsException when provided with empty json string.")]
        public async Task CreateRevocationRegistryDefinitionFromJsonAsyncThrowsExceptionForEmptyString()
        {
            //Arrange
            string revRegDefJson = "";

            //Act
            Func<Task> act = async () => await RevocationApi.CreateRevocationRegistryDefinitionFromJsonAsync(revRegDefJson);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }

        [Test, TestCase(TestName = "CreateRevocationRegistryDefinitionFromJsonAsync() throws AnoncredsRsException when provided with invalid json string.")]
        public async Task CreateRevocationRegistryDefinitionFromJsonAsyncThrowsExceptionForInvalidString()
        {
            //Arrange
            string revRegDefJson = "{}";

            //Act
            Func<Task> act = async () => await RevocationApi.CreateRevocationRegistryDefinitionFromJsonAsync(revRegDefJson);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }
        #endregion

        #region Tests for UpdateRevocationStatusListAsync
        //[Test, TestCase(TestName = "UpdateRevocationStatusListAsync() works.")]
        //public async Task UpdateRevocationStatusListAsyncWorks()
        //{
        //    //Arrange
        //    long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        //    (CredentialDefinition mockCredDef, CredentialDefinitionPrivate mockCredDefPriv, _) = await MockDataProvider.MockCredDef();
        //    CredentialOffer mockCredOffer = await MockDataProvider.MockCredOffer();
        //    (CredentialRequest mockCredReq, _) = await MockDataProvider.MockCredReq();
        //    (List<string> names, List<string> raw, List<string> encoded) = await MockDataProvider.MockAttrValues();
        //    (RevocationRegistryDefinition mockRevRegDef, RevocationRegistryDefinitionPrivate mockRegRevDefPriv) = await MockDataProvider.MockRevRegDef(credDef: mockCredDef);
        //    RevocationStatusList mockRevRegStatList = await RevocationApi.CreateRevocationStatusListAsync(mockRevRegDef.IssuerId, mockRevRegDef, mockCredDef.IssuerId, timestamp, IssuerType.ISSUANCE_BY_DEFAULT);
        //    Credential mockCred = await MockDataProvider.MockCredential(mockCredDef, mockCredDefPriv, mockCredOffer, mockCredReq, names, raw, encoded,
        //        mockRevRegStatList, null, mockRevRegDef, mockRegRevDefPriv, 0);

        //    //Act
        //    RevocationStatusList actual = await RevocationApi.UpdateRevocationStatusListAsync(timestamp, new List<long>() { 1 }, new List<long>() { 1 }, mockRevRegDef, mockRevRegStatList);

        //    //Assert
        //    _ = actual.RevocationList[0].Should().BeTrue();
        //}

        //[Test, TestCase(TestName = "UpdateRevocationRegistryAsync() throws AnoncredsRsException when revocation registry is invalid.")]
        //public async Task UpdateRevocationRegistryThrowsException()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (CredentialDefinition credDef, _, _) =
        //        await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (RevocationRegistryDefinition revRegDefObject, _, RevocationRegistry tmpRevRegObject, _) =
        //        await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    List<long> issuedList = new() { 0 };
        //    List<long> revokedList = new() { 0 };

        //    //Act
        //    Func<Task> act = async () => await RevocationApi.UpdateRevocationRegistryAsync(
        //            revRegDefObject,
        //            new(),
        //            issuedList,
        //            revokedList,
        //            revRegDefObject.Value.TailsLocation
        //            );

        //    //Assert
        //    _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        //}

        //[Test, TestCase(TestName = "UpdateRevocationRegistryAsync() works.")]
        //public async Task UpdateRevocationRegistryWorksWithJson()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    string schemaObject = await SchemaApi.CreateSchemaJsonAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (string credDef, _, _) = await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(issuerDid, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (string revRegDefJson, _, string tmpRevRegJson, _) =
        //        await RevocationApi.CreateRevocationRegistryJsonAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);
        //    RevocationRegistryDefinition revRegDefObject = await RevocationApi.CreateRevocationRegistryDefinitionFromJsonAsync(revRegDefJson);
        //    List<long> issuedList = new() { 0 };
        //    List<long> revokedList = new() { 0 };

        //    //Act
        //    (string revRegJson, string revRegDeltaJson) =
        //        await RevocationApi.UpdateRevocationRegistryAsync(
        //            revRegDefJson,
        //            tmpRevRegJson,
        //            issuedList,
        //            revokedList,
        //            revRegDefObject.Value.TailsLocation);


        //    //Assert
        //    _ = revRegJson.Should().NotBeEmpty();
        //    _ = revRegDeltaJson.Should().NotBeEmpty();
        //}

        //[Test, TestCase(TestName = "UpdateRevocationRegistryAsync() throws AnoncredsRsException when revocation registry is invalid.")]
        //public async Task UpdateRevocationRegistryThrowsExceptionWithJson()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    string schemaObject = await SchemaApi.CreateSchemaJsonAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (string credDef, _, _) =
        //        await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(issuerDid, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (string revRegDefObject, _, string tmpRevRegObject, _) =
        //        await RevocationApi.CreateRevocationRegistryJsonAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    List<long> issuedList = new() { 0 };
        //    List<long> revokedList = new() { 0 };

        //    //Act
        //    Func<Task> act = async () => await RevocationApi.UpdateRevocationRegistryAsync(
        //            revRegDefObject,
        //            "",
        //            issuedList,
        //            revokedList,
        //            ""
        //            );

        //    //Assert
        //    _ = await act.Should().ThrowAsync<Exception>();
        //}
        #endregion

        //#region Tests for RevokeCredentialAsync
        //[Test, TestCase(TestName = "RevokeCredentialAsync() works.")]
        //public async Task RevokeCredentialWorks()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);
        //    (CredentialDefinition credDef,
        //        _,
        //        _) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (RevocationRegistryDefinition revRegDefObject,
        //        _,
        //        RevocationRegistry tmpRevRegObject,
        //        _) = await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    //Act
        //    (_, RevocationRegistryDelta actual) =
        //        await RevocationApi.RevokeCredentialAsync(
        //            revRegDefObject,
        //            tmpRevRegObject,
        //            0,
        //            revRegDefObject.Value.TailsLocation
        //            );

        //    //Assert
        //    _ = actual.Value.PrevAccum.Should().NotBeNull();
        //    _ = actual.Value.Revoked.Should().HaveCount(1);
        //}

        //[Test, TestCase(TestName = "RevokeCredential() throws AnoncredsRsException when revocation registry is invalid.")]
        //public async Task RevokeCredentialThrowsException()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);
        //    (CredentialDefinition credDef,
        //        _,
        //        _) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (RevocationRegistryDefinition revRegDefObject,
        //        _,
        //        RevocationRegistry tmpRevRegObject,
        //        _) = await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    //Act
        //    Func<Task> act = async () => await RevocationApi.RevokeCredentialAsync(
        //            revRegDefObject,
        //            new(),
        //            0,
        //            revRegDefObject.Value.TailsLocation
        //            );

        //    //Assert
        //    _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        //}

        //[Test, TestCase(TestName = "RevokeCredentialAsync() works.")]
        //public async Task RevokeCredentialWorksWithJson()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    string schemaObject = await SchemaApi.CreateSchemaJsonAsync(issuerDid, schemaName, schemaVersion, attrNames);
        //    (string credDef,
        //        _,
        //        _) = await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(issuerDid, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (string revRegDefJson,
        //        _,
        //        string tmpRevRegJson,
        //        _) = await RevocationApi.CreateRevocationRegistryJsonAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    RevocationRegistryDefinition revRegDefObject = await RevocationApi.CreateRevocationRegistryDefinitionFromJsonAsync(revRegDefJson);

        //    //Act
        //    (_, string deltaJson) =
        //        await RevocationApi.RevokeCredentialAsync(
        //            revRegDefJson,
        //            tmpRevRegJson,
        //            0,
        //            revRegDefObject.Value.TailsLocation
        //            );

        //    RevocationRegistryDelta actual = JsonConvert.DeserializeObject<RevocationRegistryDelta>(deltaJson);

        //    //Assert
        //    _ = actual.Value.PrevAccum.Should().NotBeNull();
        //    _ = actual.Value.Revoked.Should().HaveCount(1);
        //}

        //[Test, TestCase(TestName = "RevokeCredential() throws AnoncredsRsException when revocation registry is invalid.")]
        //public async Task RevokeCredentialThrowsExceptionWithJson()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    string schemaObject = await SchemaApi.CreateSchemaJsonAsync(issuerDid, schemaName, schemaVersion, attrNames);
        //    (string credDef,
        //        _,
        //        _) = await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(issuerDid, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (string revRegDefObject,
        //        _,
        //        string tmpRevRegObject,
        //        _) = await RevocationApi.CreateRevocationRegistryJsonAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    //Act
        //    Func<Task> act = async () => await RevocationApi.RevokeCredentialAsync(
        //            revRegDefObject,
        //            "",
        //            0,
        //            ""
        //            );

        //    //Assert
        //    _ = await act.Should().ThrowAsync<Exception>();
        //}
        //#endregion

        //#region Tests for MergeRevocationRegistryDeltas
        //[Test, TestCase(TestName = "MergeRevocationRegistryAsync() works.")]
        //public async Task MergeRevocationRegistryDeltasWorks()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (CredentialDefinition credDef, _, _) =
        //        await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (RevocationRegistryDefinition revRegDefObject, _, RevocationRegistry revRegObject, _) =
        //        await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    List<long> issuedList1 = new() { 0, 2 };
        //    List<long> issuedList2 = new() { 0, 3 };
        //    List<long> revokedList1 = new() { 0, 2 };
        //    List<long> revokedList2 = new() { 0, 3 };

        //    (_, RevocationRegistryDelta delta1) = await RevocationApi.UpdateRevocationRegistryAsync(
        //        revRegDefObject,
        //        revRegObject,
        //        issuedList1,
        //        revokedList1,
        //        revRegDefObject.Value.TailsLocation
        //        );
        //    (_, RevocationRegistryDelta delta2) = await RevocationApi.UpdateRevocationRegistryAsync(
        //        revRegDefObject,
        //        revRegObject,
        //        issuedList2,
        //        revokedList2,
        //        revRegDefObject.Value.TailsLocation
        //        );

        //    //Act
        //    RevocationRegistryDelta actual = await RevocationApi.MergeRevocationRegistryDeltasAsync(delta1, delta2);

        //    //Assert
        //    _ = actual.Value.Revoked.Should().HaveCount(1);
        //    _ = actual.Value.Revoked.Contains(2).Should().BeTrue();
        //}

        //[Test, TestCase(TestName = "MergeRevocationRegistryDeltaAsync() throws AnoncredsRsException when one delta is invalid.")]
        //public async Task MergeRevocationRegistryDeltasASyncThrowsException()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (CredentialDefinition credDef, _, _) =
        //        await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (RevocationRegistryDefinition revRegDefObject, _, RevocationRegistry revRegObject, _) =
        //        await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    List<long> issuedList1 = new() { 0, 2 };
        //    List<long> issuedList2 = new() { 0, 3 };
        //    List<long> revokedList1 = new() { 0, 2 };
        //    List<long> revokedList2 = new() { 0, 3 };

        //    (_, RevocationRegistryDelta delta1) = await RevocationApi.UpdateRevocationRegistryAsync(
        //        revRegDefObject,
        //        revRegObject,
        //        issuedList1,
        //        revokedList1,
        //        revRegDefObject.Value.TailsLocation
        //        );
        //    (_, RevocationRegistryDelta delta2) = await RevocationApi.UpdateRevocationRegistryAsync(
        //        revRegDefObject,
        //        revRegObject,
        //        issuedList2,
        //        revokedList2,
        //        revRegDefObject.Value.TailsLocation
        //        );

        //    //Act
        //    Func<Task> act = async () => await RevocationApi.MergeRevocationRegistryDeltasAsync(delta1, new());

        //    //Assert
        //    _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        //}

        //[Test, TestCase(TestName = "MergeRevocationRegistryAsync() works.")]
        //public async Task MergeRevocationRegistryDeltasWorksWithJson()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    string schemaObject = await SchemaApi.CreateSchemaJsonAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (string credDef, _, _) =
        //        await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(issuerDid, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (string revRegDefJson, _, string revRegJson, _) =
        //        await RevocationApi.CreateRevocationRegistryJsonAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    RevocationRegistryDefinition revRegDefObject = await RevocationApi.CreateRevocationRegistryDefinitionFromJsonAsync(revRegDefJson);

        //    List<long> issuedList1 = new() { 0, 2 };
        //    List<long> issuedList2 = new() { 0, 3 };
        //    List<long> revokedList1 = new() { 0, 2 };
        //    List<long> revokedList2 = new() { 0, 3 };

        //    (_, string delta1) = await RevocationApi.UpdateRevocationRegistryAsync(
        //        revRegDefJson,
        //        revRegJson,
        //        issuedList1,
        //        revokedList1,
        //        revRegDefObject.Value.TailsLocation
        //        );
        //    (_, string delta2) = await RevocationApi.UpdateRevocationRegistryAsync(
        //        revRegDefJson,
        //        revRegJson,
        //        issuedList2,
        //        revokedList2,
        //        revRegDefObject.Value.TailsLocation
        //        );

        //    //Act
        //    string deltaJson = await RevocationApi.MergeRevocationRegistryDeltasAsync(delta1, delta2);

        //    RevocationRegistryDelta actual = JsonConvert.DeserializeObject<RevocationRegistryDelta>(deltaJson);

        //    //Assert
        //    _ = actual.Value.Revoked.Should().HaveCount(1);
        //    _ = actual.Value.Revoked.Contains(2).Should().BeTrue();
        //}

        //[Test, TestCase(TestName = "MergeRevocationRegistryDeltaAsync() throws AnoncredsRsException when one delta is invalid.")]
        //public async Task MergeRevocationRegistryDeltasASyncThrowsExceptionWithJson()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    string schemaObject = await SchemaApi.CreateSchemaJsonAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (string credDef, _, _) =
        //        await CredentialDefinitionApi.CreateCredentialDefinitionJsonAsync(issuerDid, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (string revRegDefJson, _, string revRegJson, _) =
        //        await RevocationApi.CreateRevocationRegistryJsonAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    RevocationRegistryDefinition revRegDefObject = await RevocationApi.CreateRevocationRegistryDefinitionFromJsonAsync(revRegDefJson);

        //    List<long> issuedList1 = new() { 0, 2 };
        //    List<long> issuedList2 = new() { 0, 3 };
        //    List<long> revokedList1 = new() { 0, 2 };
        //    List<long> revokedList2 = new() { 0, 3 };

        //    (_, string delta1) = await RevocationApi.UpdateRevocationRegistryAsync(
        //        revRegDefJson,
        //        revRegJson,
        //        issuedList1,
        //        revokedList1,
        //        revRegDefObject.Value.TailsLocation
        //        );

        //    //Act
        //    Func<Task> act = async () => await RevocationApi.MergeRevocationRegistryDeltasAsync(delta1, "{}");

        //    //Assert
        //    _ = await act.Should().ThrowAsync<Exception>();
        //}
        //#endregion

        //#region Tests for CreateOrUpdateRevocationState
        //[Test, TestCase(TestName = "CreateOrUpdateRevocationStateAsync() works.")]
        //public async Task CreateOrUpdateRevocationStateAsyncWorks()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (CredentialDefinition credDef,
        //        _,
        //        _) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);


        //    (RevocationRegistryDefinition revRegDefObject, _, _, RevocationRegistryDelta revRegDeltaObject) =
        //        await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    CredentialRevocationState init = await RevocationApi.CreateOrUpdateRevocationStateAsync(
        //        revRegDefObject,
        //        revRegDeltaObject,
        //        1,
        //        100,
        //        revRegDefObject.Value.TailsLocation,
        //        null);

        //    //Act
        //    CredentialRevocationState actual = await RevocationApi.CreateOrUpdateRevocationStateAsync(
        //        revRegDefObject,
        //        revRegDeltaObject,
        //        2,
        //        400,
        //        revRegDefObject.Value.TailsLocation,
        //        init);

        //    //Assert
        //    _ = actual.Should().NotBeNull();
        //}

        //[Test, TestCase(TestName = "CreateOrUpdateRevocationStateAsync() works with rev state equals null.")]
        //public async Task CreateOrUpdateRevocationStateNullAsyncWorks()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (CredentialDefinition credDef,
        //        _,
        //        _) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);


        //    (RevocationRegistryDefinition revRegDefObject, _, _, RevocationRegistryDelta revRegDeltaObject) =
        //        await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    //Act
        //    CredentialRevocationState actual = await RevocationApi.CreateOrUpdateRevocationStateAsync(
        //        revRegDefObject,
        //        revRegDeltaObject,
        //        1,
        //        200,
        //        revRegDefObject.Value.TailsLocation,
        //        null);

        //    //Assert
        //    _ = actual.Should().NotBeNull();
        //}

        //[Test, TestCase(TestName = "CreateOrUpdateRevocationStateAsync() throws AnoncredsRsException when revocation registry delta is invalid.")]
        //public async Task CreateOrUpdateRevocationStateAsyncThrowsException()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (CredentialDefinition credDef,
        //        _,
        //        _) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);


        //    (RevocationRegistryDefinition revRegDefObject, _, _, RevocationRegistryDelta revRegDeltaObject) =
        //        await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    CredentialRevocationState revState = new();

        //    //Act
        //    Func<Task> act = async () => await RevocationApi.CreateOrUpdateRevocationStateAsync(
        //        revRegDefObject,
        //        new(),
        //        0,
        //        0,
        //        revRegDefObject.Value.TailsLocation,
        //        revState);

        //    //Assert
        //    _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        //}

        //[Test, TestCase(TestName = "CreateOrUpdateRevocationStateJsonAsync() works.")]
        //public async Task CreateOrUpdateRevocationStateAsyncWorksWithJson()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (CredentialDefinition credDef,
        //        _,
        //        _) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);


        //    (RevocationRegistryDefinition revRegDefObject, _, _, RevocationRegistryDelta revRegDeltaObject) =
        //        await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    string init = await RevocationApi.CreateOrUpdateRevocationStateAsync(
        //        revRegDefObject.JsonString,
        //        revRegDeltaObject.JsonString,
        //        1,
        //        200,
        //        revRegDefObject.Value.TailsLocation,
        //        null);

        //    //Act
        //    string actual = await RevocationApi.CreateOrUpdateRevocationStateAsync(
        //        revRegDefObject.JsonString,
        //        revRegDeltaObject.JsonString,
        //        2,
        //        400,
        //        revRegDefObject.Value.TailsLocation,
        //        init);

        //    //Assert
        //    _ = actual.Should().NotBeNull();
        //}

        //[Test, TestCase(TestName = "CreateOrUpdateRevocationStateJsonAsync() works with rev state equals null.")]
        //public async Task CreateOrUpdateRevocationStateNullAsyncWorksWithJson()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (CredentialDefinition credDef,
        //        _,
        //        _) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);


        //    (RevocationRegistryDefinition revRegDefObject, _, _, RevocationRegistryDelta revRegDeltaObject) =
        //        await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    //Act
        //    string actual = await RevocationApi.CreateOrUpdateRevocationStateAsync(
        //        revRegDefObject.JsonString,
        //        revRegDeltaObject.JsonString,
        //        1,
        //        200,
        //        revRegDefObject.Value.TailsLocation,
        //        null);

        //    //Assert
        //    _ = actual.Should().NotBeNull();
        //}

        //[Test, TestCase(TestName = "CreateOrUpdateRevocationStateJsonAsync() throws AnoncredsRsException when revocation registry delta is invalid.")]
        //public async Task CreateOrUpdateRevocationStateAsyncThrowsExceptionWithJson()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (CredentialDefinition credDef,
        //        _,
        //        _) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);


        //    (RevocationRegistryDefinition revRegDefObject, _, _, RevocationRegistryDelta revRegDeltaObject) =
        //        await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    //Act
        //    Func<Task> act = async () => await RevocationApi.CreateOrUpdateRevocationStateAsync(
        //        revRegDefObject.JsonString,
        //        "",
        //        1,
        //        200,
        //        revRegDefObject.Value.TailsLocation,
        //        null
        //        );

        //    //Assert
        //    _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        //}
        //#endregion

        #region Tests for CreateRevocationStateFromJson
        [Test, TestCase(TestName = "CreateRevocationStateFromJsonAsync() returns a CredentialRevocationState object if provided with a valid json string.")]
        public async Task CreateRevocationStateFromJsonAsyncWorks()
        {
            //Arrange
            string revStateJson = "{" +
                "\"Handle\":9," +
                "\"witness\":{" +
                    "\"omega\":\"21 12CD27F6902B0C605053D21C277B62B2625802AECB16B41C45113CD3DA8A03A0D 21 13AEF810B5457092EC814EB84ECE38DD159A36D224551B051312410497A55A134 6 77FD09EE7F36E02EE33F475F95A06D2F13B8C1B3FDB5AD135CFD92E67CCB5EB2 4 37976F8859E86691D601415504DD4473F969C27FDB655787BCCA778FEC2F9C13 6 6D551EC893C94FB1347556ECA88226446184C48D97EE99B9437238E4687C0C2A 4 16917F5C8DE3FB1855737C204E57B1ED23AC27E238751BF182F5D47A78841884\"" +
                    "}," +
                    "\"rev_reg\":{" +
                        "\"accum\":\"21 12CD27F6902B0C605053D21C277B62B2625802AECB16B41C45113CD3DA8A03A0D 21 13AEF810B5457092EC814EB84ECE38DD159A36D224551B051312410497A55A134 6 77FD09EE7F36E02EE33F475F95A06D2F13B8C1B3FDB5AD135CFD92E67CCB5EB2 4 37976F8859E86691D601415504DD4473F969C27FDB655787BCCA778FEC2F9C13 6 6D551EC893C94FB1347556ECA88226446184C48D97EE99B9437238E4687C0C2A 4 16917F5C8DE3FB1855737C204E57B1ED23AC27E238751BF182F5D47A78841884\"" +
                    "}," +
                    "\"timestamp\":0" +
                "}";


            //Act
            CredentialRevocationState actual = await RevocationApi.CreateRevocationStateFromJsonAsync(revStateJson);

            //Assert
            _ = actual.Should().BeOfType<CredentialRevocationState>();
        }

        [Test, TestCase(TestName = "CreateRevocationStateFromJsonAsync() throws AnoncredsRsException when provided with an empty json string.")]
        public async Task CreateRevocationStateFromJsonAsyncThrowsExceptionsForEmptyString()
        {
            //Arrange
            string revStateJson = "";

            //Act
            Func<Task> act = async () => await RevocationApi.CreateRevocationStateFromJsonAsync(revStateJson);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }

        [Test, TestCase(TestName = "CreateRevocationStateFromJsonAsync() throws AnoncredsRsException when provided with an invalid json string.")]
        public async Task CreateRevocationStateFromJsonAsyncThrowsExceptionsForInvalidString()
        {
            //Arrange
            string revStateJson = "{}";

            //Act
            Func<Task> act = async () => await RevocationApi.CreateRevocationStateFromJsonAsync(revStateJson);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }
        #endregion

        //#region Tests for GetRevocationRegistryDefinitionAttribute
        //[Test, TestCase(TestName = "GetRevocationRegistryDefinitionAttributeAsync() works for attribute name 'id'.")]
        //public async Task GetRevocationRegistryDefinitionAttributeAsyncIdWorks()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (CredentialDefinition credDef,
        //        _,
        //        _) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (RevocationRegistryDefinition revRegDefObject,
        //        _,
        //        _,
        //        _) = await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    //Act
        //    string actual = await RevocationApi.GetRevocationRegistryDefinitionAttributeAsync(
        //        revRegDefObject,
        //        "id"
        //        );

        //    //Assert
        //    _ = actual.Should().NotBeNull();
        //}

        //[Test, TestCase(TestName = "GetRevocationRegistryDefinitionAttributeAsync() works for attribute name 'max_cred_num'.")]
        //public async Task GetRevocationRegistryDefinitionAttributeAsyncMaxCredNumWorks()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (CredentialDefinition credDef,
        //        _,
        //        _) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (RevocationRegistryDefinition revRegDefObject,
        //        _,
        //        _,
        //        _) = await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    //Act
        //    string actual = await RevocationApi.GetRevocationRegistryDefinitionAttributeAsync(
        //        revRegDefObject,
        //        "max_cred_num"
        //        );

        //    //Assert
        //    _ = actual.Should().NotBeNull();
        //}

        //[Test, TestCase(TestName = "GetRevocationRegistryDefinitionAttributeAsync() works for attribute name 'tails_hash'.")]
        //public async Task GetRevocationRegistryDefinitionAttributeAsyncTailsHashWorks()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (CredentialDefinition credDef,
        //        _,
        //        _) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (RevocationRegistryDefinition revRegDefObject,
        //        _,
        //        _,
        //        _) = await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    //Act
        //    string actual = await RevocationApi.GetRevocationRegistryDefinitionAttributeAsync(
        //        revRegDefObject,
        //        "tails_hash"
        //        );

        //    //Assert
        //    _ = actual.Should().NotBeNull();
        //}

        //[Test, TestCase(TestName = "GetRevocationRegistryDefinitionAttributeAsync() works for attribute name 'tails_location'.")]
        //public async Task GetRevocationRegistryDefinitionAttributeAsyncTailsLocationWorks()
        //{
        //    //Arrange
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (CredentialDefinition credDef,
        //        _,
        //        _) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (RevocationRegistryDefinition revRegDefObject,
        //        _,
        //        _,
        //        _) = await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    //Act
        //    string actual = await RevocationApi.GetRevocationRegistryDefinitionAttributeAsync(
        //        revRegDefObject,
        //        "tails_location"
        //        );

        //    //Assert
        //    _ = actual.Should().NotBeNull();
        //}

        //[Test, TestCase(TestName = "GetRevocationRegistryDefinitionAttributeAsync() throws AnoncredsRsException for invalid attribute name.")]
        //public async Task GetRevocationDefinitionAttributeAsyncThrowsException()
        //{
        //    //Arrange
        //    string attributeName = "test";
        //    List<string> attrNames = new() { "gender", "age", "sex" };
        //    string issuerDid = "NcYxiDXkpYi6ov5FcYDi1e";
        //    string schemaName = "gvt";
        //    string schemaVersion = "1.0";
        //    string testTailsPath = null;

        //    Schema schemaObject = await SchemaApi.CreateSchemaAsync(issuerDid, schemaName, schemaVersion, attrNames);

        //    (CredentialDefinition credDef,
        //        _,
        //        _) = await CredentialDefinitionApi.CreateCredentialDefinitionAsync(schemaObject.IssuerId, schemaObject, "tag", issuerDid, SignatureType.CL, true);

        //    (RevocationRegistryDefinition revRegDefObject,
        //        _,
        //        _,
        //        _) = await RevocationApi.CreateRevocationRegistryAsync(issuerDid, credDef, "test_tag", RegistryType.CL_ACCUM, IssuerType.ISSUANCE_BY_DEFAULT, 99, testTailsPath);

        //    //Act
        //    Func<Task> act = async () => await RevocationApi.GetRevocationRegistryDefinitionAttributeAsync(revRegDefObject, attributeName);

        //    //Assert
        //    _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        //}
        //#endregion
    }
}
