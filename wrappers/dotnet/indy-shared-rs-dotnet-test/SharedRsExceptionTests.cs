using FluentAssertions;
using anoncreds_rs_dotnet.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace anoncreds_rs_dotnet_test
{
    public class AnoncredsRsExceptionTests
    {
        private static IEnumerable<TestCaseData> CreateErrorCodeCases()
        {
            yield return new TestCaseData("message matching to rust errorCode", "0", "Success")
                .SetName("AnoncredsRsExceptions contains ErrorCode 'Success' text after parsing the code to string.");
            yield return new TestCaseData("message matching to rust errorCode", "1", "Input")
                .SetName("AnoncredsRsExceptions contains ErrorCode 'Input' text after parsing the code to string.");
            yield return new TestCaseData("message matching to rust errorCode", "2", "IOError")
                .SetName("AnoncredsRsExceptions contains ErrorCode 'IOError' text after parsing the code to string.");
            yield return new TestCaseData("message matching to rust errorCode", "3", "InvalidState")
                .SetName("AnoncredsRsExceptions contains ErrorCode 'InvalidState' text after parsing the code to string.");
            yield return new TestCaseData("message matching to rust errorCode", "4", "Unexpected")
                .SetName("AnoncredsRsExceptions contains ErrorCode 'Unexpected' text after parsing the code to string.");
            yield return new TestCaseData("message matching to rust errorCode", "5", "CredentialRevoked")
                .SetName("AnoncredsRsExceptions contains ErrorCode 'CredentialRevoked' text after parsing the code to string.");
            yield return new TestCaseData("message matching to rust errorCode", "6", "InvalidUserRevocId")
                .SetName("AnoncredsRsExceptions contains ErrorCode 'InvalidUserRevocId' text after parsing the code to string.");
            yield return new TestCaseData("message matching to rust errorCode", "7", "ProofRejected")
                .SetName("AnoncredsRsExceptions contains ErrorCode 'ProofRejected' text after parsing the code to string.");
            yield return new TestCaseData("message matching to rust errorCode", "8", "RevocationRegistryFull")
                .SetName("AnoncredsRsExceptions contains ErrorCode 'RevocationRegistryFull' text after parsing the code to string.");
            yield return new TestCaseData("no message", "99", "Unknown error code")
                .SetName("AnoncredsRsExceptions contains 'Unknown error code' text after trying to parse an unknown errorCode.");
            yield return new TestCaseData("no message", "xyz", "An unknown error code was received.")
                .SetName("AnoncredsRsExceptions contains 'An unknown error code was received' text after trying to parse an non integer errorCode.");
        }

        [Test, TestCaseSource(nameof(CreateErrorCodeCases))]
        public Task AnoncredsRsExceptionsRightMessages(string testMessage, string errorCode, string expected)
        {
            //Arrange
            string testErrorMessage = $"{{\"code\":\"{errorCode}\",\"message\":\"{testMessage}\"}}";

            //Act
            AnoncredsRsException testException = AnoncredsRsException.FromSdkError(testErrorMessage);
            string actual;
            if (errorCode != "xyz")
            {
                actual = testException.Message.Substring(1, expected.Length);
            }
            else
            {
                actual = testException.Message;
            }

            //Assert
            actual.Should().Be(expected);
            return Task.CompletedTask;
        }
    }
}