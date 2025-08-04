using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using BuildingBlocks.Strings.Consts.HospitalServices;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.Patients.Commands.CreatePatient;
using VaccinationReception.Application.Services.PatientServices;
using VaccinationReception.Application.ValidationHelper;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.ReceptionVaccinationContracts.Handlers
{
    public record ReceptionContractPatientCommand(int ContractId, int PatientId) : ICommand<int>;

    public class ReceptionContractPatientCommandValidator : AbstractValidator<ReceptionContractPatientCommand>
    {
        public ReceptionContractPatientCommandValidator()
        {
            RuleFor(x => x.ContractId)
                .GreaterThan(0).WithMessage("ContractId must be greater than 0");

            RuleFor(x => x.PatientId)
                .GreaterThan(0).WithMessage("PatientId must be greater than 0");
        }
    }

    public class ReceptionContractPatientCommandHandler : ICommandHandler<ReceptionContractPatientCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly IInventoryService _inventoryService;
        private readonly IHospitalService _hospitalService;
        private readonly ILogger<ReceptionContractPatientCommandHandler> _logger;

        public ReceptionContractPatientCommandHandler(
            IApplicationDbContext context,
            IPatientGrpcClient patientGrpcClient,
            IInventoryService inventoryService,
            IHospitalService hospitalService,
            ILogger<ReceptionContractPatientCommandHandler> logger)
        {
            _context = context;
            _patientGrpcClient = patientGrpcClient;
            _hospitalService = hospitalService;
            _inventoryService = inventoryService;
            _logger = logger;
        }
        public async Task<int> Handle(ReceptionContractPatientCommand request, CancellationToken cancellationToken)
        {
            await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var contract = await _context.Contracts
                    .FirstOrDefaultAsync(c => c.Id == request.ContractId && !c.IsCancelled && !c.IsSuspended, cancellationToken);

                if (contract == null)
                {
                    _logger.LogWarning("Contract with ID {ContractId} not found or is invalid.", request.ContractId);
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_CONTRACT_WITH_ID);
                }

                var patientInfor = await _patientGrpcClient.GetPatientAsync(request.PatientId, cancellationToken);

                if (patientInfor == null)
                {
                    _logger.LogError("Patient with ID {PatientId} not found via gRPC service.", request.PatientId);
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_PATIENT_WITH_ID);
                }

                var existingReception = await _context.Receptions
                    .FirstOrDefaultAsync(r => r.PatientId == request.PatientId &&
                                             r.ContractId == request.ContractId &&
                                             !r.IsCancelled && !r.IsSuspended, cancellationToken);

                if (existingReception != null)
                {
                    _logger.LogWarning("Patient {PatientId} has already been received for contract {ContractId}.", request.PatientId, request.ContractId);
                    throw new BadRequestException(ExceptionKey.PATIENT_ALREADY_RECEIVED_IN_CONTRACT);
                }

                var contractPatientVaccinations = await _context.ContractPatientVaccinations
                    .Where(cpv => cpv.ContractId == request.ContractId &&
                                   cpv.PatientId == request.PatientId &&
                                   !cpv.IsCancelled && !cpv.IsSuspended)
                    .ToListAsync(cancellationToken);

                if (!contractPatientVaccinations.Any())
                {
                    _logger.LogWarning("No vaccination plan found for patient {PatientId} in contract {ContractId}.", request.PatientId, request.ContractId);
                    throw new BadRequestException(ExceptionKey.NO_VACCINATION_PLAN_FOUND_FOR_PATIENT_IN_CONTRACT);
                }

                var vaccinationServiceType = await _context.ServiceTypes
                    .FirstOrDefaultAsync(st => st.Code == "VAC003" && !st.IsCancelled && !st.IsSuspended, cancellationToken)
                    ?? await _context.ServiceTypes
                                            .AsNoTracking()
                                            .Where(st => !st.IsCancelled && !st.IsSuspended)
                                            .FirstAsync(cancellationToken);

                var reception = new Reception
                {
                    PatientId = request.PatientId,
                    ContractId = request.ContractId,
                    ReceptionDate = DateTime.UtcNow,
                    ServiceTypeId = vaccinationServiceType.Id,
                    IsVaccinationTodayConfirmed = false
                };

                _context.Receptions.Add(reception);
                await _context.SaveChangesAsync(cancellationToken);

                var vaccineIds = contractPatientVaccinations.Select(cpv => cpv.VaccineId).Distinct().ToList();
                var vaccineInformation = await _inventoryService.GetMedicineInformationAsync(vaccineIds, cancellationToken);

                var serviceCodes = vaccineInformation.Select(v => v.RouteOfAdministration).Distinct().ToList();
                serviceCodes.Add(ServiceCodeConsts.EXAM_FEE_SERVICE_CODE);

                var services = await _hospitalService.GetServicesByServiceCodeAsync(serviceCodes, cancellationToken);

                var serviceByCode = services.ToDictionary(s => s.ServiceCode);

                if (!serviceByCode.TryGetValue(ServiceCodeConsts.EXAM_FEE_SERVICE_CODE, out var examFeeService))
                {
                    _logger.LogError("Exam fee service with code {ServiceCode} not found.", ServiceCodeConsts.EXAM_FEE_SERVICE_CODE);
                    throw new BadRequestException(ExceptionKey.EXAM_FEE_CODE_NOT_FOUND);
                }

                var examFeeServiceRequestDetail = new ServiceRequestDetail
                {
                    ReceptionId = reception.Id,
                    RequestNumber = UniqueStringGenerator.GenerateUniqueString(),
                    ServiceId = examFeeService.Id,
                    Quantity = 1,
                    UnitPrice = examFeeService.UnitPrice,
                    PaymentStatus = PaymentStatusForItem.NotPaid
                };
                _context.ServiceRequestDetails.Add(examFeeServiceRequestDetail);

                foreach (var cpv in contractPatientVaccinations)
                {
                    var vaccineInfo = vaccineInformation.FirstOrDefault(v => v.MedicineId == cpv.VaccineId);
                    if (vaccineInfo == null)
                    {
                        _logger.LogWarning("Medicine information not found for vaccine ID {VaccineId}. Skipping.", cpv.VaccineId);
                        continue;
                    }

                    if (!serviceByCode.TryGetValue(vaccineInfo.RouteOfAdministration, out var matchedService))
                    {
                        _logger.LogWarning("Service with code {ServiceCode} not found for vaccine ID {VaccineId}. Skipping.", vaccineInfo.RouteOfAdministration, cpv.VaccineId);
                        continue;
                    }

                    var requestNumber = UniqueStringGenerator.GenerateUniqueString();

                    var serviceRequestDetail = new ServiceRequestDetail
                    {
                        ReceptionId = reception.Id,
                        RequestNumber = requestNumber,
                        ServiceId = matchedService.Id,
                        Quantity = cpv.Quantity ?? 1,
                        UnitPrice = matchedService.UnitPrice,
                        PaymentStatus = PaymentStatusForItem.NotPaid
                    };
                    _context.ServiceRequestDetails.Add(serviceRequestDetail);

                    var receptionVaccination = new ReceptionVaccination
                    {
                        ReceptionId = reception.Id,
                        VaccineId = cpv.VaccineId,
                        Quantity = cpv.Quantity ?? 1,
                        DoseNumber = cpv.DoseNumber,
                        IsReadyToUse = false,
                        AppointmentDate = DateTime.UtcNow,
                        PaymentStatus = PaymentStatusForItem.NotPaid,
                        RequestNumber = requestNumber,
                        UnitPrice = vaccineInfo.UnitPrice ?? 0,
                        Note = $"Contract: {contract.ContractCode} - Dose {cpv.DoseNumber}"
                    };
                    _context.ReceptionVaccinations.Add(receptionVaccination);

                    await _context.SaveChangesAsync(cancellationToken);

                    cpv.ReceptionVaccinationId = receptionVaccination.Id;
                    cpv.Status = ContractPatientVaccinationStatus.Reception;
                }

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Successfully received patient {PatientId} for contract {ContractId}. Created reception {ReceptionId}.",
                    request.PatientId, request.ContractId, reception.Id);

                return reception.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Failed to receive patient {PatientId} for contract {ContractId}. Transaction rolled back.", request.PatientId, request.ContractId);
                throw;
            }
        }
    }
}