﻿using FluentAssertions;
using anoncreds_rs_dotnet.Anoncreds;
using NUnit.Framework;
using System.Threading.Tasks;
using System;
using anoncreds_rs_dotnet;

namespace anoncreds_rs_dotnet_test.Anoncreds
{
    public class ErrorApiTests
    {
        [Test]
        [TestCase(TestName = "GetCurrentErrorAsync returns the JSON string of an empty error.")]
        public async Task GetCurrentError()
        {
            //Arrange

            //Act
            string expected = "{\"code\":0,\"message\":null}";
            string actual = await ErrorApi.GetCurrentErrorAsync();

            //Assert
            actual.Should().Be(expected);
        }
    }
}