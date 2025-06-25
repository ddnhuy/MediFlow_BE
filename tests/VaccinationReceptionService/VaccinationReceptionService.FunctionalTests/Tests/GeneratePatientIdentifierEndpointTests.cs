using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GeneratePatientIdentifierEndpointTests : BaseFunctionalTest
    {
        public GeneratePatientIdentifierEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GeneratePatientIdentifier_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            // The base functional test does not add auth by default

            // Act
            var response = await _client.GetAsync("/patients/generate-identifier");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GeneratePatientIdentifier_WithToken_ReturnsOkWithValidIdentifier()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", TokenHelper.GenerateTestToken());

            // Act
            var response = await _client.GetAsync("/patients/generate-identifier");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<GeneratePatientIdentifierResponse>();
            content.Should().NotBeNull();

            // Check format: CDCDN[YY][MM][DD][HH][MM][SS][mmm]
            content!.PatientIdentifier.Should().NotBeNullOrEmpty();
            content.PatientIdentifier.Should().StartWith("CDCDN");

            // CDCDN (5) + YYMMDDHHmmssmmm (15) = 20 characters
            content.PatientIdentifier.Length.Should().Be(20);

            // More specific check with regex
            var identifierRegex = new Regex(@"^CDCDN\d{15}$");
            identifierRegex.IsMatch(content.PatientIdentifier).Should().BeTrue();
        }
    }
}
