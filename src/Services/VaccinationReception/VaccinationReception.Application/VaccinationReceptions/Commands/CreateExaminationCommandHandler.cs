using BuildingBlocks.CQRS;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class CreateExaminationCommandHandler : ICommandHandler<CreateExaminationCommand, CreateExaminationResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateExaminationCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CreateExaminationResult> Handle(CreateExaminationCommand request, CancellationToken cancellationToken)
        {
            var examination = new Examination
            {
                ReceptionId = request.ReceptionId,
                ServiceId = request.ServiceId,
                RequestNumber = request.RequestNumber,
                PatientId = request.PatientId,
                Diagnose = "",
                ReceptionTime = request.ReceptionTime,
                ExecutionTime = null,
                ReturnTime = null,
                PerformTechnicianId = null,
                PerformTechnicianName = "",
                SampleType = null,
                SampleQuality = null,
                Conclusion = "",
                Note = "",
                DoctorId = null,
                DoctorName = ""
            };

            await _context.Examinations.AddAsync(examination, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreateExaminationResult(examination.Id);
        }
    }
}
