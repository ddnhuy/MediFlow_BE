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

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.IdentityModel.Tokens;

global using Testcontainers.PostgreSql;

global using HospitalService.FunctionalTests.Abstractions;
