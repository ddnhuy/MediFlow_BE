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
        DbSet<RequestForm> RequestForms { get; }
        DbSet<Vaccination> Vaccinations { get; }
        DbSet<Payment> Payments { get; }
        DbSet<PaymentDetail> PaymentDetails { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DatabaseFacade Database { get; }
    }
}
