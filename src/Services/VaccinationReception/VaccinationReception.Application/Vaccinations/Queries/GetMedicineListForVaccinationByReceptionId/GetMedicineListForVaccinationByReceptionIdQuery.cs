using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.Vaccinations.Queries.GetMedicineListForVaccinationByReceptionId
{
    public record GetMedicineListForVaccinationByReceptionIdQuery(int ReceptionId) : IQuery<GetMedicineListForVaccinationByReceptionIdResult>;

    public record GetMedicineListForVaccinationByReceptionIdResult(
        List<MedicineInfo> DoctorPrescribedVaccines,  // Nhóm vaccine Tiêm theo bác sĩ chỉ định
        List<MedicineInfo> CustomerWarehouseVaccines   // Nhóm vaccine gửi kho khách
    );

    public record MedicineInfo(
        int ReceptionVaccinationId,
        int MedicineId,
        string MedicineName,
        bool IsConfirmed,
        string? TestResultEntry,
        string? doctorName
    );
}
