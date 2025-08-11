using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.VaccinationDTOs;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.Services.PatientServices;
using VaccinationReception.Application.Vaccinations.Queries.GetVaccinationHistoryByPatientId;

namespace VaccinationReception.Application.Vaccinations.Queries.GetVaccinationDetailById
{
    public record GetVaccinationDetailByIdQuery(int Id) : IQuery<VaccinationDetailDTO>;

    public class GetVaccinationDetailByIdQueryHandler : IQueryHandler<GetVaccinationDetailByIdQuery, VaccinationDetailDTO>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly IInventoryService _inventoryService;
        private readonly ApplicationUserProtoService.ApplicationUserProtoServiceClient _applicationUserProto;

        public GetVaccinationDetailByIdQueryHandler(IApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IPatientGrpcClient patientGrpcClient, IInventoryService inventoryService, ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _patientGrpcClient = patientGrpcClient;
            _inventoryService = inventoryService;
            _applicationUserProto = applicationUserProto;
        }

        public async Task<VaccinationDetailDTO> Handle(GetVaccinationDetailByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
            {
                throw new BadRequestException(ExceptionKey.INVALID_VACCINATION_ID);
            }

            var vaccination = await _dbContext.Vaccinations
                .Include(v => v.ReceptionVaccination)
                .ThenInclude(rv => rv.Reception)
                .Include(v => v.ReceptionVaccination)
                .ThenInclude(rv => rv.SecondaryReception)
                .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

            if (vaccination == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_VACCINATION_WITH_ID);
            }

            var patientInfo = await _patientGrpcClient.GetPatientAsync(vaccination.PatientId, cancellationToken);

            var medicineInfoList = await _inventoryService.GetMedicineInformationAsync(new List<int> { vaccination.MedicineId }, cancellationToken);
            var medicineInfo = medicineInfoList.FirstOrDefault();

            var rolesClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(roles: rolesClaim);
            var doctor = await _applicationUserProto.GetApplicationUserAsync(new GetApplicationUserRequest { Id = vaccination.DoctorId }, metadata);
            var doctorName = doctor.Name ?? string.Empty;

            var currentReceptionId = vaccination.ReceptionVaccination.SecondaryReceptionId ?? vaccination.ReceptionVaccination.ReceptionId;

            return new VaccinationDetailDTO(
                Id: vaccination.Id,
                ReceptionId: currentReceptionId,
                VaccinationDate: vaccination.VaccinationDate ?? DateTime.MinValue,
                VaccinationTestDate: vaccination.ReceptionVaccination?.VaccinationTestDate,
                DoseNumber: $"Mũi thứ {vaccination.DoseNumber}",
                VaccinationConfirmation: vaccination.IsConfirmed,
                MedicineTypeName: medicineInfo?.VaccineTypeName ?? "",
                MedicineName: vaccination.MedicineName ?? "",
                DoctorName: $"B.S {doctorName}",

                ObservationConfirmed: vaccination.ObservationConfirmed,
                HasReaction: vaccination.HasReaction,
                ReactionDate: vaccination.ReactionDate,
                PostVaccinationResult: vaccination.PostVaccinationResult,
                PostVaccinationDate: vaccination.PostVaccinationDate,
                HasFeverAbove39: vaccination.HasFeverAbove39,
                HasInjectionSiteReaction: vaccination.HasInjectionSiteReaction,
                HasOtherReaction: vaccination.HasOtherReaction,
                OtherReactionDescription: vaccination.OtherReactionDescription,

                PatientName: patientInfo.Name,
                PatientCode: patientInfo.Code,
                Gender: patientInfo.Gender == 0 ? "Nữ" : "Nam",
                PhoneNumber: patientInfo.PhoneNumber ?? "",
                Ward: patientInfo.Ward ?? "",
                District: patientInfo.District ?? "",
                Province: patientInfo.Province ?? "",
                AddressDetail: patientInfo.AddressDetail ?? ""
            );
        }
    }
}
