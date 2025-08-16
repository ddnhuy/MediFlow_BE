using VaccinationReception.Application.Vaccinations.Queries.GetMedicineListForVaccinationByReceptionId;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public record GetMedicineListForVaccinationByReceptionIdResponse(
        List<MedicineInfoResponse> DoctorPrescribedVaccines,  // Nhóm vaccine Tiêm theo bác sĩ chỉ định
        List<MedicineInfoResponse> CustomerWarehouseVaccines   // Nhóm vaccine gửi kho khách
    );

    public record MedicineInfoResponse(
        int ReceptionVaccinationId,
        int? VaccinationId, // Nullable in case it's not yet created
        int MedicineId,
        string MedicineName,
        int MedicineBatchId,
        string MedicineBatchNumber,
        bool IsConfirmed,
        bool IsRequiredTesting,
        string? TestResultEntry,
        string? DoctorName,
        DateTime? StartTestingTime = null,
        bool IsRejected = true
    );

    public class GetMedicineListForVaccinationByReceptionIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/vaccination/reception/{receptionId}/medicines", async (int receptionId, ISender sender) =>
            {
                var query = new GetMedicineListForVaccinationByReceptionIdQuery(receptionId);
                var result = await sender.Send(query);

                // Map the result to the response structure
                var response = new GetMedicineListForVaccinationByReceptionIdResponse(
                    result.DoctorPrescribedVaccines.Select(m => new MedicineInfoResponse(
                        m.ReceptionVaccinationId,
                        m.VaccinationId,
                        m.MedicineId,
                        m.MedicineName,
                        m.MedicineBatchId,
                        m.MedicineBatchNumber,
                        m.IsConfirmed,
                        m.IsRequiredTesting,
                        m.TestResultEntry,
                        m.doctorName,
                        m.StartTestingTime,
                        m.IsRejected
                    )).ToList(),
                    result.CustomerWarehouseVaccines.Select(m => new MedicineInfoResponse(
                        m.ReceptionVaccinationId,
                        m.VaccinationId,
                        m.MedicineId,
                        m.MedicineName,
                        m.MedicineBatchId,
                        m.MedicineBatchNumber,
                        m.IsConfirmed,
                        m.IsRequiredTesting,
                        m.TestResultEntry,
                        m.doctorName,
                        m.StartTestingTime,
                        m.IsRejected
                    )).ToList()
                );

                return Results.Ok(response);
            }).RequireAuthorization()
              .WithName("GetMedicineListForVaccinationByReceptionId")
              .Produces<GetMedicineListForVaccinationByReceptionIdResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status404NotFound)
              .WithSummary("Get Medicine List For Vaccination By Reception Id")
              .WithDescription("Get Medicine List For Vaccination By Reception Id");
        }
    }
}
