using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Reception> Receptions { get; }
        DbSet<ScreeningEvaluationReport> ScreeningEvaluationReports { get; }
        DbSet<ServiceType> ServiceTypes { get; }
        DbSet<ReceptionVaccination> ReceptionVaccinations { get; }
        DbSet<ServiceRequestDetail> ServiceRequestDetails { get; }
        DbSet<Vaccination> Vaccinations { get; }
        DbSet<Payment> Payments { get; }
        DbSet<PaymentDetail> PaymentDetails { get; }
        DbSet<Examination> Examinations { get; }
        DbSet<ExaminationTestResult> ExaminationTestResults { get; }
        DbSet<Contract> Contracts { get; }
        DbSet<ContractPatientVaccination> ContractPatientVaccinations { get; }
        DbSet<ContractServiceDetail> ContractServiceDetails { get; }
        DbSet<PaymentContract> PaymentContracts { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DatabaseFacade Database { get; }
    }
}
