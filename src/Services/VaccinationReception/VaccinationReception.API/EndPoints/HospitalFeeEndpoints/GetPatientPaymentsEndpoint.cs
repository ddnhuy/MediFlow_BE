using BuildingBlocks.Strings;
using VaccinationReception.Application.HospitalFees.Queries;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.API.EndPoints.HospitalFeeEndpoints
{
    public record GetPatientPaymentsResponse(
            int PatientId,
            List<PaymentResponse> Payments
        );
    public record PaymentResponse(
        int Id,
        int ReceptionId,
        decimal TotalAmount,
        PaymentMethod Method,
        string? Note,
        string? ATMTransactionCode,
        string PaymentType,
        string? InvoiceNumber,
        PaymentStatus? Status,
        int? OriginalPaymentId,
        DateTime CreatedAt,
        DateTime LastUpdatedAt
    );
    public class GetPatientPaymentsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/patients/{patientId:int}/payments", async (int patientId, ISender sender) =>
            {
                if (patientId <= 0)
                {
                    throw new BadRequestException(ExceptionKey.INVALID_PATIENT_ID);
                }
                var query = new GetPatientPaymentsQuery(patientId);
                var result = await sender.Send(query);

                var response = new GetPatientPaymentsResponse(
                    result.PatientId,
                    result.Payments.Select(p => new PaymentResponse(
                        p.Id,
                        p.ReceptionId,
                        p.TotalAmount,
                        p.Method,
                        p.Note,
                        p.ATMTransactionCode,
                        p.PaymentType.ToString(),
                        p.InvoiceNumber,
                        p.Status,
                        p.OriginalPaymentId,
                        p.CreatedAt,
                        p.LastUpdatedAt
                    )).ToList()
                );

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetPatientPayments")
            .Produces<GetPatientPaymentsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get all payments for a patient")
            .WithDescription("Retrieves all payments associated with a specific patient.");
        }
    }
}
