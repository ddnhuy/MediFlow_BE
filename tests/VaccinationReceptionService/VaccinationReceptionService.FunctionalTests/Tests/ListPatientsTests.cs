using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BuildingBlocks.Pagination;
using CustomerInfo.Grpc.Protos;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using VaccinationReception.API.EndPoints.PatientEndPoints;
using VaccinationReception.Application.DTOs.PatientDTOs;
using ListPatientsResponse = CustomerInfo.Grpc.Protos.ListPatientsResponse;

namespace VaccinationReception.FunctionalTests.Tests;

public class ListPatientsTests : BaseFunctionalTest
{
    private readonly string _testToken;

    public ListPatientsTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        _testToken = TokenHelper.GenerateTestToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
    }

    [Fact]
    public async Task ListPatients_WithInvalidPagination_ReturnsBadRequest()
    {
        // Arrange
        var pageIndex = 0;
        var pageSize = 0;

        // Act
        var response = await _client.GetAsync($"/patients?pageIndex={pageIndex}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        error.Should().NotBeNull();
    }

    [Fact]
    public async Task ListPatients_WhenGrpcThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;

        _grpcClientMock?
            .ListPatientsAsync(Arg.Any<ListPatientsRequest>(), Arg.Any<Metadata>())
            .Throws(new Exception("GRPC call failed"));

        // Act
        var response = await _client.GetAsync($"/patients?pageIndex={pageIndex}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        error.Should().NotBeNull();
        error!.Detail.Should().Contain("GRPC call failed");
    }

    [Fact]
    public async Task ListPatients_WithoutAuthorization_ReturnsUnauthorized()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync($"/patients?pageIndex={pageIndex}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}