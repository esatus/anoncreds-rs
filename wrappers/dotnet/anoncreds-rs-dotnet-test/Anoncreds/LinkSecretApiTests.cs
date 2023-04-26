using anoncreds_rs_dotnet.Anoncreds;
using anoncreds_rs_dotnet.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace anoncreds_rs_dotnet_test.Anoncreds
{
    public class LinkSecretApiTests
    {
        [Test]
        [TestCase(TestName = "CreateLinkSecret does not throw an exception.")]
        public async Task CreateLinkSecretNoThrow()
        {
            //Arrange

            //Act
            Func<Task> act = async () => { _ = await LinkSecretApi.CreateLinkSecretAsync(); };

            //Assert
            _ = await act.Should().NotThrowAsync();
        }

        [Test]
        [TestCase(TestName = "CreateLinkSecret works.")]
        public async Task CreateLinkSecretWorks()
        {
            //Arrange

            //Act
            LinkSecret testObject = await LinkSecretApi.CreateLinkSecretAsync();

            //Assert
            _ = testObject.Should().BeOfType(typeof(LinkSecret));
            _ = testObject.Value.Ms.Should().NotBeNull();
        }

        [Test]
        [TestCase(TestName = "CreateLinkSecret returns a master secret as JSON string.")]
        public async Task CreateLinkSecretJsonWorks()
        {
            //Arrange

            //Act
            string testObject = await LinkSecretApi.CreateLinkSecretJsonAsync();

            //Assert
            _ = testObject.Should().NotBeNullOrEmpty();
        }
    }
}