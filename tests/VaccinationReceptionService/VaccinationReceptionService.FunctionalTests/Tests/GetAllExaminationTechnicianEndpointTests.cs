using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HumanResource.Grpc;
using NSubstitute;
using VaccinationReception.Application.Examinations.Queries;
using VaccinationReceptionService.FunctionalTests.Abstractions;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetAllExaminationTechnicianEndpointTests : GetListPostVaccinationPatientBaseTest
    {
        private readonly string _testToken;
        private readonly GetListPostVaccinationPatientTestFactory _factory;

        public GetAllExaminationTechnicianEndpointTests(GetListPostVaccinationPatientTestFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task GetAllExaminationTechnician_WithValidRoleName_ReturnsOkWithTechnicians()
        {
            // Arrange
            var roleName = "Technician";

            var grpcResponse = new ListUsersByRoleWithoutPaginationResponse();
            grpcResponse.Data.Add(new ApplicationUserSummaryModel  { Id = 1, Name = "Tech One" });
            grpcResponse.Data.Add(new ApplicationUserSummaryModel { Id = 2, Name = "Tech Two" });

            _factory.ApplicationUserProtoMock
            .ListUsersByRoleNameAsync(
                Arg.Is<ListUsersByRoleNameRequest>(r => r.RoleName == roleName),
                Arg.Any<Grpc.Core.Metadata>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>())
            .Returns(CreateAsyncUnaryCall(grpcResponse));

            // Act
            var response = await _client.GetAsync($"/examination/users?roleName={roleName}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllExaminationTechnicianRespone>();
            result.Should().NotBeNull();
            result!.ExaminationTechnicians.Should().HaveCount(2);
            result.ExaminationTechnicians[0].Id.Should().Be(1);
            result.ExaminationTechnicians[0].Name.Should().Be("Tech One");
            result.ExaminationTechnicians[1].Id.Should().Be(2);
            result.ExaminationTechnicians[1].Name.Should().Be("Tech Two");
        }

        [Fact]
        public async Task GetAllExaminationTechnician_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/examination/users?roleName=Technician");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        public static Grpc.Core.AsyncUnaryCall<T> CreateAsyncUnaryCall<T>(T response)
        {
            return new Grpc.Core.AsyncUnaryCall<T>(
                Task.FromResult(response),
                Task.FromResult(new Grpc.Core.Metadata()),
                () => Status.DefaultSuccess,
                () => new Grpc.Core.Metadata(),
                () => { }
            );
        }
    }
}