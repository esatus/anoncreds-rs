﻿using anoncreds_rs_dotnet.Anoncreds;
using anoncreds_rs_dotnet.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace anoncreds_rs_dotnet_test.Anoncreds
{
    internal class ObjectApiTests
    {
        #region Tests for GetTypeNameAsync
        [Test, TestCase(TestName = "GetTypeNameAsync() returns correct type name.")]
        public async Task GetTypeNameWorks()
        {
            //Arrange
            string expected = "{" +
               "\"name\":\"schema name\"," +
               "\"version\":\"schema version\"," +
               "\"attrNames\":[\"attr\"]," +
               "\"issuerId\":\"55GkHamhTU1ZbTbV2ab9DE:2:schema name:schema version\"" +
               "}";
            Schema schemaObject = await SchemaApi.CreateSchemaFromJsonAsync(expected);

            //Act
            string actual = await ObjectApi.GetTypeNameAsync(schemaObject.Handle);

            //Assert
            _ = actual.Should().Be("Schema");

        }

        [Test, TestCase(TestName = "GetTypeNameAsync() throws AnoncredsRsException if object handle is invalid.")]
        public async Task GetTypeNameThrowsException()
        {
            //Arrange

            //Act
            Func<Task> act = async () => await ObjectApi.GetTypeNameAsync(new IntPtr());

            //Assert
            _ = await act.Should().ThrowAsync<AnoncredsRsException>();
        }
        #endregion

        #region Tests for FreeObjectAsync
        [Test, TestCase(TestName = "FreeObjectAsync() works with toJsonAsync() throwing when trying to access json from an invalid handle.")]
        public async Task FreeObjectAsyncWorks()
        {
            //Arrange
            string expected = "{" +
               "\"name\":\"schema name\"," +
               "\"version\":\"schema version\"," +
               "\"attrNames\":[\"attr\"]," +
               "\"issuerId\":\"55GkHamhTU1ZbTbV2ab9DE:2:schema name:schema version\"" +
               "}";
            Schema schemaObject = await SchemaApi.CreateSchemaFromJsonAsync(expected);
            string testJson = await ObjectApi.ToJsonAsync(schemaObject.Handle);
            //Act
            await ObjectApi.FreeObjectAsync(schemaObject.Handle);
            Func<Task> actual = async () => await ObjectApi.ToJsonAsync(schemaObject.Handle);
            //Assert
            _ = testJson.Should().NotBe("");
            _ = await actual.Should().ThrowAsync<Exception>();

        }
        #endregion

        #region Tests for ToJsonAsync
        [Test, TestCase(TestName = "ToJsonAsync() converts an object handle to a correct json.")]
        public async Task ToJsonWorks()
        {
            string expected = "{" +
               "\"name\":\"schema name\"," +
               "\"version\":\"schema version\"," +
               "\"attrNames\":[\"attr\"]," +
               "\"issuerId\":\"55GkHamhTU1ZbTbV2ab9DE:2:schema name:schema version\"" +
               "}";
            Schema schemaObject = await SchemaApi.CreateSchemaFromJsonAsync(expected);

            //Act
            string actual = await ObjectApi.ToJsonAsync(schemaObject.Handle);

            //Assert
            _ = actual.Should().BeEquivalentTo(expected);
        }
        #endregion
    }
}