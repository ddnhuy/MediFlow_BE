using Google.Protobuf.WellKnownTypes;
using HumanResource.Grpc;
using Management.API.Departments.Commands;
using Management.API.Departments.Queries;
using Management.API.DepartmentTypes.Queries;

namespace ManagementService.FunctionalTests.Tests;

public class DepartmentsControllerTest : BaseFunctionalTest
{
    private string _testToken;

    public DepartmentsControllerTest(FunctionalTestWebAppFactory factory) : base(factory)
    {
        _testToken = TokenHelper.GenerateTestToken();
    }

    private void SetAuthHeader()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _testToken);
    }

    [Fact]
    public async Task GetDepartments_NotAuthenticated_ReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/departments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDepartments_ReturnSuccess()
    {
        // Arrange
        SetAuthHeader();

        var grpcResponse = new HumanResource.Grpc.ListDepartmentsResponse
        {
            PageIndex = 1,
            PageSize = 100,
            Count = 10,
            Data = { new HumanResource.Grpc.DepartmentSummaryModel
            {
                Id = 1,
                Code = "TD001",
                Name = "Test Department",
                NameInEnglish = "Test Department EN",
                DepartmentTypeName = "Test Type",
                DepartmentTypeNameInEnglish = "Test Type EN",
                IsSuspended = false,
            } }
        };

        _grpcDepartmentClientMock?
            .ListDepartmentsAsync(
                Arg.Any<HumanResource.Grpc.ListDepartmentsRequest>(),
                Arg.Any<Metadata>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

        // Act
        var response = await _client.GetAsync("/departments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetDepartmentsResult>();
        result.Should().NotBeNull();
        result.Departments.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDepartments_InvalidPaginationRequest_ReturnBadRequest()
    {
        // Arrange
        SetAuthHeader();

        // Act
        var response = await _client.GetAsync("/departments?pageIndex=-1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetDepartmentById_NotAuthenticated_ReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        var departmentId = 1;

        // Act
        var response = await _client.GetAsync($"/departments/{departmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDepartmentById_ReturnSuccess()
    {
        // Arrange
        SetAuthHeader();

        var departmentId = 1;

        var grpcResponse = new HumanResource.Grpc.DepartmentDetailModel
        {
            Id = departmentId,
            Code = "TD001",
            Name = "Test Department",
            NameInEnglish = "Test Department EN",
            DepartmentTypeName = "Test Type",
            DepartmentTypeNameInEnglish = "Test Type EN",
            IsSuspended = false,
            IsCancelled = false,
            CreatedBy = 1,
            CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            LastUpdatedBy = 1,
            LastUpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            DepartmentTypeId = 1
        };

        _grpcDepartmentClientMock?
            .GetDepartmentAsync(
                Arg.Any<HumanResource.Grpc.GetDepartmentRequest>(),
                Arg.Any<Metadata>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

        // Act
        var response = await _client.GetAsync($"/departments/{departmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetDepartmentByIdResult>();
        result.Should().NotBeNull();
        result.Department.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDepartmentById_WithInvalidId_ReturnBadRequest()
    {
        // Arrange
        SetAuthHeader();

        var departmentId = -1;

        // Act
        var response = await _client.GetAsync($"/departments/{departmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateDepartment_NotAuthenticated_ReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        var request = new CreateDepartmentRequest
        {
            Code = "TD001",
            Name = "Test Department",
            NameInEnglish = "Test Department EN",
            DepartmentTypeId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/departments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateDepartment_ReturnSuccess()
    {
        // Arrange
        SetAuthHeader();

        var departmentId = 1;

        var grpcResponse = new HumanResource.Grpc.DepartmentDetailModel
        {
            Id = departmentId,
            Code = "TD001",
            Name = "Test Department",
            NameInEnglish = "Test Department EN",
            DepartmentTypeName = "Test Type",
            DepartmentTypeNameInEnglish = "Test Type EN",
            IsSuspended = false,
            IsCancelled = false,
            CreatedBy = 1,
            CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            LastUpdatedBy = 1,
            LastUpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            DepartmentTypeId = 1
        };

        _grpcDepartmentClientMock?
            .CreateDepartmentAsync(
                Arg.Any<HumanResource.Grpc.CreateDepartmentRequest>(),
                Arg.Any<Metadata>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

        var request = new CreateDepartmentRequest
        {
            Code = "TD001",
            Name = "Test Department",
            NameInEnglish = "Test Department EN",
            DepartmentTypeId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/departments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateDepartmentResult>();
        result.Should().NotBeNull();
        result.Department.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateDepartment_WithEmptyDepartmentTypeId_ReturnBadRequest()
    {
        // Arrange
        SetAuthHeader();

        var request = new CreateDepartmentRequest
        {
            Code = "TD001",
            Name = "Test Department",
            NameInEnglish = "Test Department EN",
            DepartmentTypeId = 0
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/departments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateDepartment_NotAuthenticated_ReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        var departmentId = 1;
        var request = new UpdateDepartmentRequest
        {
            Code = "TD001",
            Name = "Test Department",
            NameInEnglish = "Test Department EN",
            DepartmentTypeId = 1,
            IsSuspended = false,
            IsCancelled = false
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/departments/{departmentId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateDepartment_ReturnSuccess()
    {
        // Arrange
        SetAuthHeader();

        var departmentId = 1;

        var grpcResponse = new HumanResource.Grpc.DepartmentDetailModel
        {
            Code = "TD001",
            Name = "Test Department",
            NameInEnglish = "Test Department EN",
            DepartmentTypeName = "Test Type",
            DepartmentTypeNameInEnglish = "Test Type EN",
            IsSuspended = false,
            IsCancelled = false,
            CreatedBy = 1,
            CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            LastUpdatedBy = 1,
            LastUpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            DepartmentTypeId = 1
        };

        _grpcDepartmentClientMock?
            .UpdateDepartmentAsync(
                Arg.Any<HumanResource.Grpc.UpdateDepartmentRequest>(),
                Arg.Any<Metadata>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

        var request = new UpdateDepartmentRequest
        {
            Id = departmentId,
            Code = "TD001",
            Name = "Test Department",
            NameInEnglish = "Test Department EN",
            DepartmentTypeId = 1,
            IsSuspended = false,
            IsCancelled = false
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/departments/{departmentId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UpdateDepartmentResult>();
        result.Should().NotBeNull();
        result.Department.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateDepartment_WithEmptyDepartmentTypeId_ReturnBadRequest()
    {
        // Arrange
        SetAuthHeader();

        var request = new UpdateDepartmentRequest
        {
            Code = "TD001",
            Name = "Test Department",
            NameInEnglish = "Test Department EN",
            DepartmentTypeId = 0
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/departments/0", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteDepartment_NotAuthenticated_ReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        var departmentId = 1;

        // Act
        var response = await _client.DeleteAsync($"/departments/{departmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteDepartment_ReturnSuccess()
    {
        // Arrange
        SetAuthHeader();

        var departmentId = 1;

        var grpcResponse = new HumanResource.Grpc.DeleteDepartmentResponse
        {
            IsSuccess = true
        };

        _grpcDepartmentClientMock?
            .DeleteDepartmentAsync(
                Arg.Any<HumanResource.Grpc.DeleteDepartmentRequest>(),
                Arg.Any<Metadata>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

        // Act
        var response = await _client.DeleteAsync($"/departments/{departmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DeleteDepartmentResult>();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteDepartment_WithInvalidId_ReturnBadRequest()
    {
        // Arrange
        SetAuthHeader();

        var departmentId = -1;

        // Act
        var response = await _client.DeleteAsync($"/departments/{departmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetDepartmentTypes_NotAuthenticated_ReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/departments/types");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDepartmentTypes_ReturnSuccess()
    {
        // Arrange
        SetAuthHeader();

        var grpcResponse = new HumanResource.Grpc.ListDepartmentTypesResponse
        {
            Data =
            {
                new HumanResource.Grpc.DepartmentTypeModel
                {
                    Id = 1,
                    Name = "Test Type",
                    NameInEnglish = "Test Type EN",
                    Code = "AT001"
                },
                new HumanResource.Grpc.DepartmentTypeModel
                {
                    Id = 2,
                    Name = "Another Type",
                    NameInEnglish = "Another Type EN",
                    Code = "AT002"
                }
            }
        };

        _grpcDepartmentTypeClientMock?
            .ListDepartmentTypesAsync(
                Arg.Any<HumanResource.Grpc.ListDepartmentTypesRequest>(),
                Arg.Any<Metadata>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

        // Act
        var response = await _client.GetAsync("/departments/types");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetDepartmentTypesResult>();
        result.Should().NotBeNull();
        result.DepartmentTypes.Should().NotBeNull();
    }

    [Fact]
    public async Task GetEmployeesOfDepartment_NotAuthenticated_ReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/departments/1/employees");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetEmployeesOfDepartment_ReturnSuccess()
    {
        // Arrange
        SetAuthHeader();

        var grpcResponse = new HumanResource.Grpc.ListEmployeesResponse
        {
            PageIndex = 1,
            PageSize = 100,
            Count = 10,
            Data = { new HumanResource.Grpc.EmployeeSummaryModel
            {
                Id = 1,
                Code = "TD001",
                Name = "Test Department",
                IsSuspended = false,
                ProfilePictureUrl = "https://example.com/profile.jpg",
            } }
        };

        _grpcDepartmentClientMock?
            .ListEmployeesAsync(
                Arg.Any<HumanResource.Grpc.ListEmployeesRequest>(),
                Arg.Any<Metadata>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

        // Act
        var response = await _client.GetAsync("/departments/1/employees");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetEmployeesByDepartmentIdResult>();
        result.Should().NotBeNull();
        result.EmployeeList.Should().NotBeNull();
    }

    [Fact]
    public async Task GetEmployeesOfDepartment_InvalidPaginationRequest_ReturnBadRequest()
    {
        // Arrange
        SetAuthHeader();

        // Act
        var response = await _client.GetAsync("/departments/1/employees?pageIndex=-1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
