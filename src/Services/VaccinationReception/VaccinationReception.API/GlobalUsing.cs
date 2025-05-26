global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.IdentityModel.Tokens;
global using HealthChecks.UI.Client;

global using Carter;
global using MediatR;
global using Mapster;

global using Google.Protobuf.Collections;

global using BuildingBlocks.Authorization;
global using BuildingBlocks.Exceptions;
global using BuildingBlocks.Exceptions.Handler;
global using BuildingBlocks.Pagination;

global using VaccinationReception.Application;
global using VaccinationReception.Application.Configs;
global using VaccinationReception.Application.DTOs.PatientDTOs;
global using VaccinationReception.Application.Patients.Commands.CreatePatient;
global using VaccinationReception.Application.Patients.Commands.DeletePatient;
global using VaccinationReception.Application.Patients.Commands.UpdatePatient;
global using VaccinationReception.Application.Patients.Queries.GetPatient;
global using VaccinationReception.Application.Patients.Queries.ListPatients;

global using VaccinationReception.Infrastructure;
