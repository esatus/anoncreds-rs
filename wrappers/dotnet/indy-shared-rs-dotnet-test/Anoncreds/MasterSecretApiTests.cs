using anoncreds_rs_dotnet.Anoncreds;
using anoncreds_rs_dotnet.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace anoncreds_rs_dotnet_test.Anoncreds
{
    public class MasterSecretApiTests
    {
        [Test]
        [TestCase(TestName = "CreateMasterSecret does not throw an exception.")]
        public async Task CreateMasterSecretNoThrow()
        {
            //Arrange

            //Act
            Func<Task> act = async () => { _ = await MasterSecretApi.CreateMasterSecretAsync(); };

            //Assert
            _ = await act.Should().NotThrowAsync();
        }

        [Test]
        [TestCase(TestName = "CreateMasterSecret works.")]
        public async Task CreateMasterSecretWorks()
        {
            //Arrange

            //Act
            MasterSecret testObject = await MasterSecretApi.CreateMasterSecretAsync();

            //Assert
            _ = testObject.Should().BeOfType(typeof(MasterSecret));
            _ = testObject.Value.Ms.Should().NotBeNull();
        }

        [Test]
        [TestCase(TestName = "CreateMasterSecret returns a master secret as JSON string.")]
        public async Task CreateMasterSecretJsonWorks()
        {
            //Arrange

            //Act
            string testObject = await MasterSecretApi.CreateMasterSecretJsonAsync();

            //Assert
            _ = testObject.Should().NotBeNullOrEmpty();
        }
    }
}