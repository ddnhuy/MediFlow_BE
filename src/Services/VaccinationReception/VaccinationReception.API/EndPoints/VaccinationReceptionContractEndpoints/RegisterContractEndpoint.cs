using BuildingBlocks.Strings;
using VaccinationReception.Application.DTOs.ReceptionVaccinationContractDTOs;
using VaccinationReception.Application.ReceptionVaccinationContracts.Handlers;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionContractEndpoints
{
    public record RegisterContractRequest(
           ContractDTO Contract,
           string VaccinationEnrollmentDownloadUrl
       );

    public record RegisterContractResponse(
        int Id,
        string ContractCode,
        int ContractNumber,
        string ContractName,
        string CompanyName,
        string UnitName,
        DateTime ContractDate,
        DateTime? ExpectedDate,
        decimal ContractValue,
        decimal? AdvanceAmount,
        string? Description,
        Guid? FileContractId,
        string? FileContractName,
        Guid? FileVaccinationEnrollmentId,
        string? FileVaccinationEnrollmentName
    );

    public class RegisterContractEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/contracts/register", async (RegisterContractRequest request, ISender sender) =>
            {

                var command = new RegisterContractCommand(
                    Contract: request.Contract,
                    VaccinationEnrollmentDownloadUrl: request.VaccinationEnrollmentDownloadUrl
                );

                var result = await sender.Send(command);

                var response = new RegisterContractResponse(
                    Id: result.Id,
                    ContractCode: result.ContractCode,
                    ContractNumber: result.ContractNumber,
                    ContractName: result.ContractName,
                    CompanyName: result.CompanyName,
                    UnitName: result.UnitName,
                    ContractDate: result.ContractDate,
                    ExpectedDate: result.ExpectedDate,
                    ContractValue: result.ContractValue,
                    AdvanceAmount: result.AdvanceAmount,
                    Description: result.Description,
                    FileContractId: result.FileContractId,
                    FileContractName: result.FileContractName,
                    FileVaccinationEnrollmentId: result.FileVaccinationEnrollmentId,
                    FileVaccinationEnrollmentName: result.FileVaccinationEnrollmentName
                );

                return Results.Created($"/contracts/{response.Id}", response);
            })
            .RequireAuthorization()
            .WithName("RegisterContract")
            .WithTags("Contract")
            .Produces<RegisterContractResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Register a new vaccination contract")
            .WithDescription("Registers a new vaccination contract with patient data from Excel file. The system will validate vaccine codes, check inventory, create/update patients, and set up vaccination plans.");
        }
    }
}
