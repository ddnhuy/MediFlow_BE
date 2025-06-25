using VaccinationReception.Application.Vaccinations.Queries.GetMedicineListForVaccinationByReceptionId;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public record GetMedicineListForVaccinationByReceptionIdResponse(
        List<MedicineInfoResponse> DoctorPrescribedVaccines,  // Nhóm vaccine Tiêm theo bác sĩ chỉ định
        List<MedicineInfoResponse> CustomerWarehouseVaccines   // Nhóm vaccine gửi kho khách
    );

    public record MedicineInfoResponse(
        int MedicineId,
        string MedicineName,
        bool IsConfirmed,
        string? TestResultEntry,
        string? DoctorName
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
                        m.MedicineId,
                        m.MedicineName,
                        m.IsConfirmed,
                        m.TestResultEntry,
                        m.doctorName
                    )).ToList(),
                    result.CustomerWarehouseVaccines.Select(m => new MedicineInfoResponse(
                        m.MedicineId,
                        m.MedicineName,
                        m.IsConfirmed,
                        m.TestResultEntry,
                        m.doctorName
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
