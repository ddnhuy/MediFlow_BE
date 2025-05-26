// GlobalUsings.FunctionalTests.cs

global using System.Net;
global using System.Net.Http.Json;
global using System.Net.Http.Headers;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Threading.Tasks;

global using Xunit;
global using FluentAssertions;

global using NSubstitute;
global using NSubstitute.ExceptionExtensions;

global using Grpc.Core;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.IdentityModel.Tokens;

global using Testcontainers.PostgreSql;

global using CustomerInfo.Grpc.Protos;
global using static CustomerInfo.Grpc.Protos.PatientProtoService;

global using VaccinationReception.API;
global using VaccinationReception.API.EndPoints.PatientEndPoints;
global using VaccinationReception.Infrastructure.Data;
global using VaccinationReception.Application.Patients.Commands.CreatePatient;

global using VaccinationReceptionService.FunctionalTests.Abstractions;
global using VaccinationReceptionService.FunctionalTests.Helpers;

global using Google.Protobuf.WellKnownTypes;
global using VaccinationReception.Application.Patients.Commands.UpdatePatient;

global using static CustomerInfo.Grpc.Protos.DeletePatientResponse;