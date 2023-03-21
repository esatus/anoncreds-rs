using anoncreds_rs_dotnet.Anoncreds;
using anoncreds_rs_dotnet.Models;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace anoncreds_rs_dotnet_test.Anoncreds
{
    internal class SchemaApiTests
    {
        #region Tests for CreateSchemaAsync
        private static IEnumerable<TestCaseData> CreateCasesCreateSchemaWorks()
        {
            yield return new TestCaseData("NcYxiDXkpYi6ov5FcYDi1e")
                .SetName("CreateSchemaAsync() creates a valid schema object with legacy issuerId.");
            yield return new TestCaseData("did:uri:new")
                .SetName("CreateSchemaAsync() creates a valid schema object with uri issuerId.");
            yield return new TestCaseData("did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK")
                .SetName("CreateSchemaAsync() creates a valid schema object with another uri issuerId.");
        }

        [Test, TestCaseSource(nameof(CreateCasesCreateSchemaWorks))]
        public async Task CreateSchemaWorks(string issuerId)
        {
            //Arrange
            List<string> attrNames = new() { "gender", "age", "sex" };
            string schemaName = "gvt";
            string schemaVersion = "1.0";

            //Act
            Schema testObject = await SchemaApi.CreateSchemaAsync(issuerId, schemaName, schemaVersion, attrNames);

            //Assert
            _ = testObject.Should().BeOfType(typeof(Schema));
        }

        private static IEnumerable<TestCaseData> CreateCasesCreateSchemaInvalidIssuerId()
        {
            yield return new TestCaseData("")
                .SetName("CreateSchemaAsync() throws a AnoncredsRsException if no issuerId is provided.");
            yield return new TestCaseData(":::")
                .SetName("CreateSchemaAsync() throws a AnoncredsRsException if invalid issuerId is provided.");
            yield return new TestCaseData("IIIIIIIIIIIIIIIIIIIIII")
                .SetName("CreateSchemaAsync() throws a AnoncredsRsException if another invalid issuerId is provided.");
        }

        [Test, TestCaseSource(nameof(CreateCasesCreateSchemaInvalidIssuerId))]
        public async Task CreateSchemaThrowsExceptionForInvalidIssuerId(string issuerId)
        {
            //Arrange
            List<string> attrNames = new() { "gender", "age", "sex" };
            string schemaName = "gvt";
            string schemaVersion = "1.0";

            //Act
            Func<Task> act = async () => await SchemaApi.CreateSchemaAsync(issuerId, schemaName, schemaVersion, attrNames);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }

        private static IEnumerable<TestCaseData> CreateCasesCreateSchemaInvalidAttributeNames()
        {
            List<string> attrNamesEmpty = new() { };

            yield return new TestCaseData(attrNamesEmpty)
                .SetName("CreateSchemaAsync() throws a AnoncredsRsException if no attribute names are provided.");
        }
        [Test, TestCaseSource(nameof(CreateCasesCreateSchemaInvalidAttributeNames))]
        public async Task CreateSchemaThrowsExceptionForInvalidAttributeNames(List<string> attrNames)
        {
            //Arrange
            string issuerId = "NcYxiDXkpYi6ov5FcYDi1e";
            string schemaName = "gvt";
            string schemaVersion = "1.0";

            //Act
            Func<Task> act = async () => await SchemaApi.CreateSchemaAsync(issuerId, schemaName, schemaVersion, attrNames);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }
        #endregion

        #region Tests for CreateSchemaJsonAsync
        private static IEnumerable<TestCaseData> CreateCasesCreateSchemaJsonWorks()
        {
            yield return new TestCaseData("NcYxiDXkpYi6ov5FcYDi1e")
                .SetName("CreateSchemaJsonAsync() creates a valid schema object with legacy issuerId.");
            yield return new TestCaseData("did:uri:new")
                .SetName("CreateSchemaJsonAsync() creates a valid schema object with uri issuerId.");
            yield return new TestCaseData("did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK")
                .SetName("CreateSchemaJsonAsync() creates a valid schema object with another uri issuerId.");
        }

        [Test, TestCaseSource(nameof(CreateCasesCreateSchemaJsonWorks))]
        public async Task CreateSchemaJsonWorks(string issuerId)
        {
            //Arrange
            List<string> attrNames = new() { "gender", "age", "sex" };
            string schemaName = "gvt";
            string schemaVersion = "1.0";

            //Act
            string testObject = await SchemaApi.CreateSchemaJsonAsync(issuerId, schemaName, schemaVersion, attrNames);

            //Assert
            _ = testObject.Should().BeOfType(typeof(string));
            _ = testObject.Should().NotBeNullOrEmpty();
        }

        private static IEnumerable<TestCaseData> CreateCasesCreateSchemaJsonInvalidIssuerId()
        {
            yield return new TestCaseData("")
                .SetName("CreateSchemaJsonAsync() throws a AnoncredsRsException if no issuerId is provided.");
            yield return new TestCaseData(":::")
                .SetName("CreateSchemaJsonAsync() throws a AnoncredsRsException if invalid issuerId is provided.");
            yield return new TestCaseData("IIIIIIIIIIIIIIIIIIIIII")
                .SetName("CreateSchemaJsonAsync() throws a AnoncredsRsException if another invalid issuerId is provided.");
        }

        [Test, TestCaseSource(nameof(CreateCasesCreateSchemaJsonInvalidIssuerId))]
        public async Task CreateSchemaJsonThrowsExceptionForInvalidIssuerId(string issuerId)
        {
            //Arrange
            List<string> attrNames = new() { "gender", "age", "sex" };
            string schemaName = "gvt";
            string schemaVersion = "1.0";

            //Act
            Func<Task> act = async () => await SchemaApi.CreateSchemaJsonAsync(issuerId, schemaName, schemaVersion, attrNames);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }


        private static IEnumerable<TestCaseData> CreateCasesCreateSchemaJsonInvalidAttributeNames()
        {
            List<string> attrNamesEmpty = new() { };

            yield return new TestCaseData(attrNamesEmpty)
                .SetName("CreateSchemaJsonAsync() throws a AnoncredsRsException if no attribute names are provided.");
        }
        [Test, TestCaseSource(nameof(CreateCasesCreateSchemaJsonInvalidAttributeNames))]
        public async Task CreateSchemaJsonThrowsExceptionForInvalidAttributeNames(List<string> attrNames)
        {
            //Arrange
            string issuerId = "NcYxiDXkpYi6ov5FcYDi1e";
            string schemaName = "gvt";
            string schemaVersion = "1.0";

            //Act
            Func<Task> act = async () => await SchemaApi.CreateSchemaJsonAsync(issuerId, schemaName, schemaVersion, attrNames);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }
        #endregion

        #region Tests for CreateSchemaFromJsonAsync
        [Test, TestCase(TestName = "CreateSchemaFromJsonAsync() returns a Schema object if a valid json string is provided.")]
        public async Task CreateSchemaFromJsonAsyncWorks()
        {
            //Arrange
            string schemaJson = await MockDataProvider.MockSchemaJson();

            //Act
            Schema actual = await SchemaApi.CreateSchemaFromJsonAsync(schemaJson);

            //Assert
            _ = actual.Should().BeOfType<Schema>();
            _ = actual.IssuerId.Should().Be("mock:SchemaIssuerUri");
        }

        [Test, TestCase(TestName = "CreateSchemaFromJsonAsync() throws a AnoncredsRsException if an empty json string is provided.")]
        public async Task CreateSchemaFromJsonAsyncThrowsExceptionForEmptyString()
        {
            //Arrange
            string schemaJson = "";

            //Act
            Func<Task> act = async () => await SchemaApi.CreateSchemaFromJsonAsync(schemaJson);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }

        [Test, TestCase(TestName = "CreateSchemaFromJsonAsync() throws a AnoncredsRsException if an invalid json string is provided.")]
        public async Task CreateSchemaFromJsonAsyncThrowsExceptionForInvalidString()
        {
            //Arrange
            string schemaJson = "{}";

            //Act
            Func<Task> act = async () => await SchemaApi.CreateSchemaFromJsonAsync(schemaJson);

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }
        #endregion
    }
}