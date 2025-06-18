using Microsoft.EntityFrameworkCore;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Reception> Receptions { get; }
        DbSet<ScreeningEvaluationReport> ScreeningEvaluationReports { get; }
        DbSet<ServiceType> ServiceTypes { get; }
        DbSet<ReceptionVaccination> ReceptionVaccinations { get; }
        DbSet<DiseaseGroup> DiseaseGroups { get; }
        DbSet<DiseaseGroupService> DiseaseGroupServices { get; }
        DbSet<ServiceGroup> ServiceGroups { get; }
        DbSet<ServiceGroupService> ServiceGroupServices { get; }
        DbSet<Service> Services { get; }
        DbSet<ServiceRequestDetail> ServiceRequestDetails { get; }
        DbSet<RequestForm> RequestForms { get; }
        DbSet<Vaccination> Vaccinations { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
