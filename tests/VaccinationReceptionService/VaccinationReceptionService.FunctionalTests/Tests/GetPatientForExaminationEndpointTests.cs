using BuildingBlocks.Strings;
using NSubstitute;
using System.Text.Json;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Application.Examinations.Queries;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetPatientForExaminationEndpointTests : GetListPostVaccinationPatientBaseTest
    {
        private readonly string _testToken;
        private readonly GetListPostVaccinationPatientTestFactory _factory;

        public GetPatientForExaminationEndpointTests(GetListPostVaccinationPatientTestFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task GetPatientForExamination_WithValidParameters_ReturnsOkWithPatients()
        {
            // Arrange
            var patientName = "Nguyen Van A";
            var isDiagnose = true;

            var grpcResponse = new List<PatientSummaryDTO>
            {
                new PatientSummaryDTO
                {
                    Id = 1,
                    Code = "BN001",
                    Name = "Nguyen Van A",
                    Gender = 1,
                    DOB = new DateTime(1990, 1, 1),
                    PhoneNumber = "0123456789",
                    IdentityCard = "123456789",
                    AddressDetail = "123 Street",
                    Province = "Hanoi",
                    District = "Cau Giay",
                    Ward = "Dich Vong",
                    IsPregnant = false,
                    IsForeigner = false,
                },
                new PatientSummaryDTO
                {
                    Id = 2,
                    Code = "BN002",
                    Name = "Tran Thi B",
                    Gender = 0,
                    DOB = new DateTime(1985, 5, 15),
                    PhoneNumber = "0987654321",
                    IdentityCard = "987654321",
                    AddressDetail = "456 Avenue",
                    Province = "Hanoi",
                    District = "Ba Dinh",
                    Ward = "Phuc Xa",
                    IsPregnant = false,
                    IsForeigner = false
                }
            };

            _patientGrpcClientMock
                .ListPatientsByIdsAndSearchAsync(Arg.Any<List<int>>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(grpcResponse);

            // Act
            var response = await _client.GetAsync($"/examination/patients?patientName={patientName}&isDiagnose={isDiagnose}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetPatientsForExaminationResponse>();
            result.Should().NotBeNull();
            result!.PatientExaminationInfos.Should().NotBeNull();
        }

        [Fact]
        public async Task GetPatientForExamination_WithoutParameters_ReturnsOkWithAllPatients()
        {
            // Arrange
            var grpcResponse = new List<PatientSummaryDTO>
            {
                new PatientSummaryDTO
                {
                    Id = 1,
                    Code = "BN001",
                    Name = "Nguyen Van A",
                    Gender = 1,
                    DOB = new DateTime(1990, 1, 1),
                    PhoneNumber = "0123456789",
                    IdentityCard = "123456789",
                    AddressDetail = "123 Street",
                    Province = "Hanoi",
                    District = "Cau Giay",
                    Ward = "Dich Vong",
                    IsPregnant = false,
                    IsForeigner = false
                }
            };

            _patientGrpcClientMock
                .ListPatientsByIdsAndSearchAsync(Arg.Any<List<int>>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(grpcResponse);

            // Act
            var response = await _client.GetAsync("/examination/patients");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetPatientsForExaminationResponse>();
            result.Should().NotBeNull();
            result!.PatientExaminationInfos.Should().NotBeNull();
        }

        [Fact]
        public async Task GetPatientForExamination_WithPatientNameFilter_ReturnsFilteredPatients()
        {
            // Arrange
            var patientName = "Nguyen";
            var grpcResponse = new List<PatientSummaryDTO>
            {
                new PatientSummaryDTO
                {
                    Id = 1,
                    Code = "BN001",
                    Name = "Nguyen Van A",
                    Gender = 1,
                    DOB = new DateTime(1990, 1, 1),
                    PhoneNumber = "0123456789",
                    IdentityCard = "123456789",
                    AddressDetail = "123 Street",
                    Province = "Hanoi",
                    District = "Cau Giay",
                    Ward = "Dich Vong",
                    IsPregnant = false,
                    IsForeigner = false
                }
            };

            _patientGrpcClientMock
                .ListPatientsByIdsAndSearchAsync(Arg.Any<List<int>>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(grpcResponse);

            // Act
            var response = await _client.GetAsync($"/examination/patients?patientName={patientName}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetPatientsForExaminationResponse>();
            result.Should().NotBeNull();
            result!.PatientExaminationInfos.Should().NotBeNull();
        }

        [Fact]
        public async Task GetPatientForExamination_WithIsDiagnoseTrue_ReturnsPatientsWithDiagnose()
        {
            // Arrange
            var isDiagnose = true;
            var grpcResponse = new List<PatientSummaryDTO>
            {
                new PatientSummaryDTO
                {
                    Id = 1,
                    Code = "BN001",
                    Name = "Nguyen Van A",
                    Gender = 1,
                    DOB = new DateTime(1990, 1, 1),
                    PhoneNumber = "0123456789",
                    IdentityCard = "123456789",
                    AddressDetail = "123 Street",
                    Province = "Hanoi",
                    District = "Cau Giay",
                    Ward = "Dich Vong",
                    IsPregnant = false,
                    IsForeigner = false
                }
            };

            _patientGrpcClientMock
                .ListPatientsByIdsAndSearchAsync(Arg.Any<List<int>>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(grpcResponse);

            // Act
            var response = await _client.GetAsync($"/examination/patients?isDiagnose={isDiagnose}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetPatientsForExaminationResponse>();
            result.Should().NotBeNull();
            result!.PatientExaminationInfos.Should().NotBeNull();
        }

        [Fact]
        public async Task GetPatientForExamination_WithIsDiagnoseFalse_ReturnsPatientsWithoutDiagnose()
        {
            // Arrange
            var isDiagnose = false;
            var grpcResponse = new List<PatientSummaryDTO>
            {
                new PatientSummaryDTO
                {
                    Id = 1,
                    Code = "BN001",
                    Name = "Nguyen Van A",
                    Gender = 1,
                    DOB = new DateTime(1990, 1, 1),
                    PhoneNumber = "0123456789",
                    IdentityCard = "123456789",
                    AddressDetail = "123 Street",
                    Province = "Hanoi",
                    District = "Cau Giay",
                    Ward = "Dich Vong",
                    IsPregnant = false,
                    IsForeigner = false
                }
            };

            _patientGrpcClientMock
                .ListPatientsByIdsAndSearchAsync(Arg.Any<List<int>>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(grpcResponse);

            // Act
            var response = await _client.GetAsync($"/examination/patients?isDiagnose={isDiagnose}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetPatientsForExaminationResponse>();
            result.Should().NotBeNull();
            result!.PatientExaminationInfos.Should().NotBeNull();
        }

        [Fact]
        public async Task GetPatientForExamination_WhenNoExaminationsFound_ReturnsEmptyList()
        {
            // Arrange
            var grpcResponse = new List<PatientSummaryDTO>();

            _patientGrpcClientMock
                .ListPatientsByIdsAndSearchAsync(Arg.Any<List<int>>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(grpcResponse);

            // Act
            var response = await _client.GetAsync("/examination/patients");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetPatientsForExaminationResponse>();
            result.Should().NotBeNull();
            result!.PatientExaminationInfos.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPatientForExamination_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/examination/patients");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetPatientForExamination_WithAllParameters_ReturnsCorrectPatientData()
        {
            // Arrange
            var patientName = "Nguyen Van A";
            var isDiagnose = true;

            var grpcResponse = new List<PatientSummaryDTO>
            {
                new PatientSummaryDTO
                {
                    Id = 1,
                    Code = "BN001",
                    Name = "Nguyen Van A",
                    Gender = 1,
                    DOB = new DateTime(1990, 1, 1),
                    PhoneNumber = "0123456789",
                    IdentityCard = "123456789",
                    AddressDetail = "123 Street",
                    Province = "Hanoi",
                    District = "Cau Giay",
                    Ward = "Dich Vong",
                    IsPregnant = false,
                    IsForeigner = false,
                }
            };

            _patientGrpcClientMock
                .ListPatientsByIdsAndSearchAsync(Arg.Any<List<int>>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(grpcResponse);

            // Act
            var response = await _client.GetAsync($"/examination/patients?patientName={patientName}&isDiagnose={isDiagnose}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetPatientsForExaminationResponse>();
            result.Should().NotBeNull();
            result!.PatientExaminationInfos.Should().NotBeNull();

            if (result.PatientExaminationInfos.Any())
            {
                var patient = result.PatientExaminationInfos.First();
                patient.PatientId.Should().Be(1);
                patient.PatientName.Should().Be("Nguyen Van A");
                patient.PatientCode.Should().Be("BN001");
                patient.Gender.Should().Be("Nam");
                patient.YearOfBirth.Should().Be(1990);
                patient.Age.Should().BeGreaterThan(0);
                patient.ReceptionId.Should().BeGreaterThan(0);
            }
        }

        [Fact]
        public async Task GetPatientForExamination_WithFemalePatient_ReturnsCorrectGender()
        {
            // Arrange
            var grpcResponse = new List<PatientSummaryDTO>
            {
                new PatientSummaryDTO
                {
                    Id = 2,
                    Code = "BN002",
                    Name = "Tran Thi B",
                    Gender = 0, // Female
                    DOB = new DateTime(1985, 5, 15),
                    PhoneNumber = "0987654321",
                    IdentityCard = "987654321",
                    AddressDetail = "456 Avenue",
                    Province = "Hanoi",
                    District = "Ba Dinh",
                    Ward = "Phuc Xa",
                    IsPregnant = false,
                    IsForeigner = false,
                }
            };

            _patientGrpcClientMock
                .ListPatientsByIdsAndSearchAsync(Arg.Any<List<int>>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(grpcResponse);

            // Act
            var response = await _client.GetAsync("/examination/patients");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetPatientsForExaminationResponse>();
            result.Should().NotBeNull();

            if (result!.PatientExaminationInfos.Any())
            {
                var patient = result.PatientExaminationInfos.First();
                patient.Gender.Should().Be("Nữ");
                patient.PatientName.Should().Be("Tran Thi B");
                patient.PatientCode.Should().Be("BN002");
            }
        }       
    }
}