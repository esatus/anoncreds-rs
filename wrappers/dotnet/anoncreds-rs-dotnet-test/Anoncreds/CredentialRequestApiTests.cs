using anoncreds_rs_dotnet.Anoncreds;
using anoncreds_rs_dotnet.Models;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Threading.Tasks;

namespace anoncreds_rs_dotnet_test.Anoncreds
{
    public class CredentialRequestApiTests
    {
        #region Tests for CreateCredentialRequestAsync
        [Test, TestCase(TestName = "CreateCredentialRequestAsync() with all Arguments set returns a request and metadata.")]
        public async Task CreateCredentialRequestAsyncWorks()
        {
            //Arrange
            CredentialDefinition mockCredDef = (await MockDataProvider.MockCredDef()).Item1;
            CredentialOffer mockCredOffer = await MockDataProvider.MockCredOffer();
            string mockEntropy = "mockEntropy";
            string mockMasterSecretName = "mockMasterSecretName";
            MasterSecret mockMasterSecret = await MasterSecretApi.CreateMasterSecretAsync();

            //Act
            (CredentialRequest request, CredentialRequestMetadata metaData) = await CredentialRequestApi.CreateCredentialRequestAsync(mockEntropy, mockCredDef, mockMasterSecret, mockMasterSecretName, mockCredOffer);

            //Assert
            _ = request.Should().NotBeNull();
            _ = request.Should().BeOfType(typeof(CredentialRequest));
            _ = metaData.Should().NotBeNull();
            _ = metaData.Should().BeOfType(typeof(CredentialRequestMetadata));
        }

        [Test, TestCase(TestName = "CreateCredentialRequestJsonAsync() with all Arguments set returns a request and metadata.")]
        public async Task CreateCredentialRequestJsonAsyncWorks()
        {
            //Arrange
            CredentialDefinition mockCredDef = (await MockDataProvider.MockCredDef()).Item1;
            CredentialOffer mockCredOffer = await MockDataProvider.MockCredOffer();
            string mockEntropy = "mockEntropy";
            string mockMasterSecretName = "mockMasterSecretName";
            MasterSecret mockMasterSecret = await MasterSecretApi.CreateMasterSecretAsync();

            //Act
            (string request, string metaData) = await CredentialRequestApi.CreateCredentialRequestJsonAsync(mockEntropy, JsonConvert.SerializeObject(mockCredDef), JsonConvert.SerializeObject(mockMasterSecret), mockMasterSecretName, JsonConvert.SerializeObject(mockCredOffer));

            //Assert
            _ = request.Should().NotBeNullOrEmpty();
            _ = metaData.Should().NotBeNullOrEmpty();
        }
        #endregion
    }
}