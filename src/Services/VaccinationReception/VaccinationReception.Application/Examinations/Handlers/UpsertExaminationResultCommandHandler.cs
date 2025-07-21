using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.Examinations.Handlers
{
    public class UpsertExaminationResultCommandHandler : ICommandHandler<UpsertExaminationResultCommand, UpsertExaminationResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationUserProtoService.ApplicationUserProtoServiceClient _applicationUserProto;

        public UpsertExaminationResultCommandHandler(IApplicationDbContext context, ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _applicationUserProto = applicationUserProto;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UpsertExaminationResult> Handle(UpsertExaminationResultCommand request, CancellationToken cancellationToken)
        {
            foreach (var dto in request.Results)
            {
                // Update Examination
                var examination = await _context.Examinations
                    .FirstOrDefaultAsync(e => e.Id == dto.ExaminationId, cancellationToken);

                if (examination == null)
                {
                    throw new BadRequestException(ExceptionKey.NOT_FOUND_EXAMINATION_WITH_ID);
                }

                examination.PatientId = dto.PatientId;
                examination.Diagnose = dto.Diagnose;
                examination.ReturnTime = dto.ReturnTime;
                examination.PerformTechnicianId = dto.PerformTechnicianId;
                examination.PerformTechnicianName = await GetUserName(dto.PerformTechnicianId);
                examination.SampleType = dto.SampleType;
                examination.SampleQuality = dto.SampleQuality;
                examination.DoctorId = dto.DoctorId;
                examination.DoctorName = await GetUserName(dto.DoctorId);
                examination.Conclusion = dto.Conclusion;
                examination.Note = dto.Note;

                // Upsert ExaminationTestResults
                foreach (var resultItem in dto.ExaminationResults)
                {
                    var testResult = await _context.ExaminationTestResults
                        .FirstOrDefaultAsync(r =>
                            r.ExaminationId == dto.ExaminationId &&
                            r.StandardValue == resultItem.StandardValue,
                            cancellationToken);

                    if (testResult == null)
                    {
                        testResult = new ExaminationTestResult
                        {
                            ExaminationId = dto.ExaminationId,
                            StandardValue = resultItem.StandardValue,
                            ParameterName = resultItem.ParameterName,
                            ResultValue = resultItem.ResultValue,
                            Unit = resultItem.Unit
                        };
                        _context.ExaminationTestResults.Add(testResult);
                    }
                    else
                    {
                        testResult.ResultValue = resultItem.ResultValue;
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return new UpsertExaminationResult(true);
        }

        private async Task<string> GetUserName(int userId)
        {
            var rolesClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int id = int.TryParse(userIdClaim, out var parsedId) ? parsedId : 1;

            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(roles: rolesClaim);

            var user = await _applicationUserProto.GetApplicationUserAsync(new GetApplicationUserRequest
            {
                Id = userId
            }, metadata);

            return user.Name;
        }
    }
}
