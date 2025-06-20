using Google.Protobuf.WellKnownTypes;
using Management.API.Users.Commands;
using Management.API.Users.Queries;
using static Management.API.Controllers.UsersController;

namespace ManagementService.FunctionalTests.Tests;

public class UsersControllerTest : BaseFunctionalTest
{
    private string _testToken;

    public UsersControllerTest(FunctionalTestWebAppFactory factory) : base(factory)
    {
        _testToken = TokenHelper.GenerateTestToken();
    }

    private void SetAuthHeader()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _testToken);
    }

    [Fact]
    public async Task CallApi_NotAuthenticated_ReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUsers_ReturnSuccess()
    {
        // Arrange
        SetAuthHeader();

        var grpcResponse = new HumanResource.Grpc.ListApplicationUsersResponse
        {
            PageIndex = 1,
            PageSize = 100,
            Count = 10,
            Data = { new HumanResource.Grpc.ApplicationUserSummaryModel
            {
                Id = 1,
                UserName = "testuser",
                Email = "testemail@gmail.com",
                Code = "TD001",
                Name = "Test User",
                IsSuspended = false,
                Roles = Roles.ADMIN,
                ProfilePictureUrl = "https://example.com/profile.jpg",
            } }
        };

        _grpcUserClientMock?
            .ListApplicationUsersAsync(
                Arg.Any<HumanResource.Grpc.ListApplicationUsersRequest>(),
                Arg.Any<Metadata>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

        // Act
        var response = await _client.GetAsync("/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetUsersResult>();
        result.Should().NotBeNull();
        result.Users.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserById_ReturnSuccess()
    {
        // Arrange
        SetAuthHeader();

        var userId = 1;

        var grpcResponse = new HumanResource.Grpc.ApplicationUserDetailModel
        {
            Id = userId,
            Code = "TD001",
            Name = "Test User",
            UserName = "testuser",
            Email = "testemail@gmail.com",
            Roles = Roles.ADMIN,
            IsSuspended = false,
            IsCancelled = false,
            CreatedBy = 1,
            CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            LastUpdatedBy = 1,
            LastUpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            EmailConfirmed = true,
            PhoneNumber = "1234567890",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            Address = "123 Test St, Test City",
            Departments = { new HumanResource.Grpc.DepartmentSummaryModel
            {
                Id = 1,
                Code = "TD001",
                Name = "Test Department",
                NameInEnglish = "Test Department EN",
                DepartmentTypeName = "Test Type",
                DepartmentTypeNameInEnglish = "Test Type EN",
                IsSuspended = false,
            } },
            ProfilePictureUrl = "https://example.com/profile.jpg",
        };

        _grpcUserClientMock?
            .GetApplicationUserAsync(
                Arg.Any<HumanResource.Grpc.GetApplicationUserRequest>(),
                Arg.Any<Metadata>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

        // Act
        var response = await _client.GetAsync($"/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetUserByIdResult>();
        result.Should().NotBeNull();
        result.User.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateUser_ReturnSuccess()
    {
        // Arrange
        SetAuthHeader();

        var userId = 1;

        var grpcResponse = new HumanResource.Grpc.ApplicationUserDetailModel
        {
            Id = userId,
            Code = "TD001",
            Name = "Test User",
            UserName = "testuser",
            Email = "testemail@gmail.com",
            Roles = Roles.ADMIN,
            IsSuspended = false,
            IsCancelled = false,
            CreatedBy = 1,
            CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            LastUpdatedBy = 1,
            LastUpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            EmailConfirmed = true,
            PhoneNumber = "1234567890",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            Address = "123 Test St, Test City",
            Departments = { new HumanResource.Grpc.DepartmentSummaryModel
            {
                Id = 1,
                Code = "TD001",
                Name = "Test Department",
                NameInEnglish = "Test Department EN",
                DepartmentTypeName = "Test Type",
                DepartmentTypeNameInEnglish = "Test Type EN",
                IsSuspended = false,
            } },
            ProfilePictureUrl = "https://example.com/profile.jpg",
        };

        _grpcUserClientMock?
            .CreateApplicationUserAsync(
                Arg.Any<HumanResource.Grpc.CreateApplicationUserRequest>(),
                Arg.Any<Metadata>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

        var request = new CreateUserRequest(
            "testuser",
            "testemail@gmail.com",
            "TestPassword123!",
            "1234567890",
            "TD001",
            "Test User",
            "123 Test St, Test City",
            "https://example.com/profile.jpg",
            new List<string> { Roles.ADMIN },
            new List<int> { 1 }
        );

        // Act
        var response = await _client.PostAsJsonAsync($"/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateUserResult>();
        result.Should().NotBeNull();
        result.User.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateUser_InvalidPassword_ReturnBadRequest()
    {
        // Arrange
        SetAuthHeader();

        var request = new CreateUserRequest(
            "testuser",
            "testemail@gmail.com",
            "Test123",
            "1234567890",
            "TD001",
            "Test User",
            "123 Test St, Test City",
            "https://example.com/profile.jpg",
            new List<string> { Roles.ADMIN },
            new List<int> { 1 }
        );

        // Act
        var response = await _client.PostAsJsonAsync($"/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUser_ReturnSuccess()
    {
        // Arrange
        SetAuthHeader();

        var userId = 1;

        var grpcResponse = new HumanResource.Grpc.ApplicationUserDetailModel
        {
            Id = userId,
            Code = "TD001",
            Name = "Test User",
            UserName = "testuser",
            Email = "testemail@gmail.com",
            Roles = Roles.ADMIN,
            IsSuspended = false,
            IsCancelled = false,
            CreatedBy = 1,
            CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            LastUpdatedBy = 1,
            LastUpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            EmailConfirmed = true,
            PhoneNumber = "1234567890",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            Address = "123 Test St, Test City",
            Departments = { new HumanResource.Grpc.DepartmentSummaryModel
            {
                Id = 1,
                Code = "TD001",
                Name = "Test Department",
                NameInEnglish = "Test Department EN",
                DepartmentTypeName = "Test Type",
                DepartmentTypeNameInEnglish = "Test Type EN",
                IsSuspended = false,
            } },
            ProfilePictureUrl = "https://example.com/profile.jpg",
        };

        _grpcUserClientMock?
            .UpdateApplicationUserAsync(
                Arg.Any<HumanResource.Grpc.UpdateApplicationUserRequest>(),
                Arg.Any<Metadata>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

        var request = new UpdateUserRequest(
            "testuser",
            "testemail@gmail.com",
            "1234567890",
            "TD001",
            "Test User",
            "123 Test St, Test City",
            "https://example.com/profile.jpg",
            new List<string> { Roles.ADMIN },
            new List<int> { 1 },
            false
        );

        // Act
        var response = await _client.PutAsJsonAsync($"/users/{userId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UpdateUserResult>();
        result.Should().NotBeNull();
        result.User.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateUser_WithEmptyUserTypeId_ReturnBadRequest()
    {
        // Arrange
        SetAuthHeader();

        var request = new UpdateUserRequest(
            "testuser",
            "testemail@gmail.com",
            "1234567890",
            "TD001",
            "Test User",
            "123 Test St, Test City",
            "https://example.com/profile.jpg",
            new List<string> { Roles.ADMIN },
            new List<int> { 1 },
            false
        );

        // Act
        var response = await _client.PutAsJsonAsync($"/users/0", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteUser_ReturnSuccess()
    {
        // Arrange
        SetAuthHeader();

        var userId = 1;

        var grpcResponse = new HumanResource.Grpc.DeleteApplicationUserResponse
        {
            IsSuccess = true
        };

        _grpcUserClientMock?
            .DeleteApplicationUserAsync(
                Arg.Any<HumanResource.Grpc.DeleteApplicationUserRequest>(),
                Arg.Any<Metadata>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

        // Act
        var response = await _client.DeleteAsync($"/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DeleteUserResult>();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetUserById_ReturnBadRequest()
    {
        // Arrange
        SetAuthHeader();

        var userId = 0;

        // Act
        var response = await _client.GetAsync($"/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task CreateUserAsync_InvalidModelState_ReturnsBadRequest()
    {
        SetAuthHeader();

        var invalidRequest = new
        {
            // UserName = "testuser",
            Email = "testemail@gmail.com",
            Password = "TestPassword123!",
            PhoneNumber = "1234567890",
            Code = "TD001",
            Name = "Test User",
            Address = "123 Test St, Test City",
            ProfilePictureUrl = "https://example.com/profile.jpg",
            RoleNames = new List<string> { "ADMIN" },
            DepartmentIds = new List<int> { 1 }
        };

        var response = await _client.PostAsJsonAsync("/users", invalidRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUserAsync_InvalidModelState_ReturnsBadRequest()
    {
        SetAuthHeader();

        var userId = 1;

        var invalidRequest = new
        {
            // UserName = "testuser",
            Email = "testemail@gmail.com",
            PhoneNumber = "1234567890",
            Code = "TD001",
            Name = "Test User",
            Address = "123 Test St, Test City",
            ProfilePictureUrl = "https://example.com/profile.jpg",
            RoleNames = new List<string> { "ADMIN" },
            DepartmentIds = new List<int> { 1 },
            IsSuspended = false
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/users/{userId}", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task DeleteUserAsync_WhenDeleteFails_ReturnsBadRequest()
    {
        // Arrange
        SetAuthHeader();

        var userId = 1;

        var grpcResponse = new HumanResource.Grpc.DeleteApplicationUserResponse
        {
            IsSuccess = false
        };

        _grpcUserClientMock?
            .DeleteApplicationUserAsync(
                Arg.Any<HumanResource.Grpc.DeleteApplicationUserRequest>(),
                Arg.Any<Metadata>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

        // Act
        var response = await _client.DeleteAsync($"/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
