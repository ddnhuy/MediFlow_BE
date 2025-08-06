using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using BuildingBlocks.Strings.Consts.HospitalServices;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore; // Added for transaction
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.ReceptionVaccinationContractDTOs;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.Patients.Commands.CreatePatient;
using VaccinationReception.Application.Patients.Commands.UpdatePatient;
using VaccinationReception.Application.Services.ExcelServices;
using VaccinationReception.Application.Services.PatientServices;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.ReceptionVaccinationContracts.Handlers
{
    public record RegisterContractCommand(
        ContractDTO Contract,
        string VaccinationEnrollmentDownloadUrl
    ) : ICommand<ContractResponse>;

    public class RegisterContractCommandHandler : ICommandHandler<RegisterContractCommand, ContractResponse>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IExcelDataReaderService _excelDataReaderService;
        private readonly ILogger<RegisterContractCommandHandler> _logger;
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly IInventoryService _inventoryService;
        private readonly IHospitalService _hospitalService;

        public RegisterContractCommandHandler(
            IApplicationDbContext dbContext,
            IExcelDataReaderService excelDataReaderService,
            ILogger<RegisterContractCommandHandler> logger,
            IPatientGrpcClient patientGrpcClient,
            IInventoryService inventoryService,
            IHospitalService hospitalService)
        {
            _dbContext = dbContext;
            _excelDataReaderService = excelDataReaderService;
            _logger = logger;
            _patientGrpcClient = patientGrpcClient;
            _inventoryService = inventoryService;
            _hospitalService = hospitalService;
        }

        public async Task<ContractResponse> Handle(RegisterContractCommand request, CancellationToken cancellationToken)
        {
            // Start a new database transaction
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var contractVaccinationExcel = await _excelDataReaderService.ReadContractVaccinationExcelFromUrl(request.VaccinationEnrollmentDownloadUrl);

                var totalQuantityPerVaccine = contractVaccinationExcel
                    .SelectMany(x => x.Vaccines)
                    .GroupBy(v => v.VaccineCode)
                    .ToDictionary(g => g.Key, g => g.Sum(v => v.Quantity));

                var contract = request.Contract.Adapt<Contract>();
                contract.Status = ContractStatus.Active;
                contract.ExpectedPatientCount = contractVaccinationExcel.Count();
                contract.ExpectedVaccineCount = totalQuantityPerVaccine.Sum(x => x.Value);

                await _dbContext.Contracts.AddAsync(contract, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                var existingPatients = await _patientGrpcClient.GetAllPatientAsync(cancellationToken);
                var patientDict = existingPatients
                    .Where(p => !string.IsNullOrWhiteSpace(p.IdentityCard))
                    .ToDictionary(p => p.IdentityCard!.Trim().ToLower(), p => p);

                var allVaccineCodes = contractVaccinationExcel
                    .SelectMany(x => x.Vaccines)
                    .Select(v => v.VaccineCode)
                    .Distinct()
                    .ToList();

                var vaccineInformationList = await _inventoryService.GetMedicinesByCodeAsync(allVaccineCodes);

                if (vaccineInformationList.Count != allVaccineCodes.Count)
                {
                    throw new BadRequestException(ExceptionKey.INVALID_VACCINE_CODE);
                }

                var vaccineInfoDict = vaccineInformationList.ToDictionary(v => v.MedicineCode, v => v);



                var totalQuantityPerRoute = contractVaccinationExcel
                    .SelectMany(x => x.Vaccines)
                    .Where(x => vaccineInfoDict.ContainsKey(x.VaccineCode) && !string.IsNullOrEmpty(vaccineInfoDict[x.VaccineCode].RouteOfAdministration))
                    .GroupBy(x => vaccineInfoDict[x.VaccineCode].RouteOfAdministration!)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

                var routeOfAdministrations = totalQuantityPerRoute.Keys
                    .Append(ServiceCodeConsts.EXAM_FEE_SERVICE_CODE)
                    .Distinct()
                    .ToList();

                var serviceRequests = await _hospitalService.GetServicesByServiceCodeAsync(routeOfAdministrations, cancellationToken);

                var serviceRequestDict = serviceRequests.ToDictionary(s => s.ServiceCode);

                foreach (var (vaccineCode, totalQuantity) in totalQuantityPerVaccine)
                {
                    if (!vaccineInfoDict.TryGetValue(vaccineCode, out var medicine))
                    {
                        throw new BadRequestException(ExceptionKey.INVALID_VACCINE_CODE);
                    }

                    var stockCheck = await _inventoryService.CheckMedicineStockResponseAsync(medicine.MedicineId, totalQuantity);
                    if (!stockCheck.IsEnough)
                    {
                        throw new BadRequestException(ExceptionKey.INSUFFICIENT_VACCINE_STOCK);
                    }
                }

                var contractServiceDetails = new List<ContractServiceDetail>();

                foreach (var (vaccineCode, totalQuantity) in totalQuantityPerVaccine)
                {
                    var medicine = vaccineInfoDict[vaccineCode];

                    var contractServiceDetail = new ContractServiceDetail
                    {
                        ContractId = contract.Id,
                        VaccineId = medicine.MedicineId,
                        ServiceId = null,
                        Quantity = totalQuantity,
                        UnitPrice = medicine.UnitPrice,
                        TotalAmount = totalQuantity * medicine.UnitPrice
                    };
                    contractServiceDetails.Add(contractServiceDetail);
                }

                foreach (var serviceCode in routeOfAdministrations)
                {
                    if (!serviceRequestDict.TryGetValue(serviceCode, out var service))
                    {
                        _logger.LogWarning("Service with code {ServiceCode} not found.", serviceCode);
                        continue;
                    }

                    if (service.ServiceCode == ServiceCodeConsts.EXAM_FEE_SERVICE_CODE)
                    {
                        var examServiceDetail = new ContractServiceDetail
                        {
                            ContractId = contract.Id,
                            VaccineId = null,
                            ServiceId = service.Id,
                            Quantity = contractVaccinationExcel.Count(),
                            UnitPrice = service.UnitPrice,
                            TotalAmount = contractVaccinationExcel.Count() * service.UnitPrice
                        };
                        contractServiceDetails.Add(examServiceDetail);
                    }
                    else if (totalQuantityPerRoute.TryGetValue(service.ServiceCode, out var quantity))
                    {
                        var routeServiceDetail = new ContractServiceDetail
                        {
                            ContractId = contract.Id,
                            VaccineId = null,
                            ServiceId = service.Id,
                            Quantity = quantity,
                            UnitPrice = service.UnitPrice,
                            TotalAmount = quantity * service.UnitPrice
                        };
                        contractServiceDetails.Add(routeServiceDetail);
                    }
                }

                if (contractServiceDetails.Any())
                {
                    await _dbContext.ContractServiceDetails.AddRangeAsync(contractServiceDetails, cancellationToken);
                }

                contract.ContractValue = contractServiceDetails.Sum(detail => detail.TotalAmount ?? 0);

                _dbContext.Contracts.Update(contract);
                
                var patientIdentityToIdMap = new Dictionary<string, int>();

                foreach (var item in contractVaccinationExcel)
                {
                    var identityKey = item.IdentityCard?.Trim().ToLower();
                    int currentPatientId;

                    if (!string.IsNullOrWhiteSpace(identityKey))
                    {
                        if (patientDict.TryGetValue(identityKey, out var existingPatient))
                        {
                            var updateCommand = new UpdatePatientCommand(
                                Id: existingPatient.Id,
                                Code: existingPatient.Code,
                                Name: item.PatientName,
                                Gender: item.Gender,
                                Dob: item.DOB,
                                PhoneNumber: item.PhoneNumber ?? string.Empty,
                                Email: item.Email ?? string.Empty,
                                IdentityCard: item.IdentityCard ?? string.Empty,
                                AddressDetail: item.AddressDetail ?? string.Empty,
                                Province: item.Province ?? string.Empty,
                                District: item.District ?? string.Empty,
                                Ward: item.Ward ?? string.Empty,
                                IsPregnant: item.IsPregnant,
                                IsForeigner: item.IsForeigner,
                                IsSuspended: false,
                                IsCancelled: false
                            );
                            await _patientGrpcClient.UpdatePatientAsync(updateCommand, cancellationToken);
                            currentPatientId = existingPatient.Id;
                        }
                        else
                        {
                            var createCommand = new CreatePatientCommand(
                                Code: await UniqueStringGenerator.GeneratePatientIdentifierAsync(),
                                Name: item.PatientName,
                                Gender: item.Gender,
                                Dob: item.DOB,
                                PhoneNumber: item.PhoneNumber ?? string.Empty,
                                Email: item.Email ?? string.Empty,
                                IdentityCard: item.IdentityCard ?? string.Empty,
                                AddressDetail: item.AddressDetail ?? string.Empty,
                                Province: item.Province ?? string.Empty,
                                District: item.District ?? string.Empty,
                                Ward: item.Ward ?? string.Empty,
                                IsPregnant: item.IsPregnant,
                                IsForeigner: item.IsForeigner,
                                IsSuspended: false,
                                IsCancelled: false
                            );
                            var createdPatientResponse = await _patientGrpcClient.CreatePatientAsync(createCommand, cancellationToken);
                            currentPatientId = createdPatientResponse.Id;
                        }
                        patientIdentityToIdMap[identityKey] = currentPatientId;
                    }
                    else
                    {
                        throw new BadRequestException(ExceptionKey.INVALID_PATIENT_ID);
                    }
                }

                var contractPatientVaccineDetails = new List<ContractPatientVaccination>();

                var vaccineCodeToMedicineIdMap = vaccineInformationList.ToDictionary(v => v.MedicineCode, v => v.MedicineId);
                var patientContractVaccinesAdded = new Dictionary<int, List<int>>();

                foreach (var item in contractVaccinationExcel)
                {
                    var identityKey = item.IdentityCard?.Trim().ToLower();

                    if (string.IsNullOrWhiteSpace(identityKey))
                    {
                        continue;
                    }

                    if (patientIdentityToIdMap.TryGetValue(identityKey, out var patientId))
                    {
                        if (!patientContractVaccinesAdded.ContainsKey(patientId))
                        {
                            patientContractVaccinesAdded[patientId] = new List<int>();
                        }

                        foreach (var vaccineDto in item.Vaccines)
                        {
                            if (vaccineCodeToMedicineIdMap.TryGetValue(vaccineDto.VaccineCode, out var vaccineMedicineId))
                            {
                                var existingVaccinesForPatient = patientContractVaccinesAdded[patientId];

                                if (existingVaccinesForPatient.Any())
                                {
                                    var interactionResponse = await _inventoryService.GetMedicineInteractionsResponseAsync(vaccineMedicineId);

                                    var hasInteraction = existingVaccinesForPatient
                                        .Where(id => id != vaccineMedicineId)
                                        .Any(existingContractVaccineId =>
                                            interactionResponse.Interactions.Any(interaction =>
                                                (interaction.MedicineId1 == existingContractVaccineId && interaction.MedicineId2 == vaccineMedicineId) ||
                                                (interaction.MedicineId2 == existingContractVaccineId && interaction.MedicineId1 == vaccineMedicineId)
                                            )
                                        );

                                    if (hasInteraction)
                                    {
                                        var conflictingVaccines = existingVaccinesForPatient
                                            .Where(id => id != vaccineMedicineId)
                                            .Where(existingContractVaccineId =>
                                                interactionResponse.Interactions.Any(interaction =>
                                                    (interaction.MedicineId1 == existingContractVaccineId && interaction.MedicineId2 == vaccineMedicineId) ||
                                                    (interaction.MedicineId2 == existingContractVaccineId && interaction.MedicineId1 == vaccineMedicineId)
                                                )
                                            ).ToList();

                                        _logger.LogWarning("Vaccine {VaccineId} (Code: {VaccineCode}) has interactions with existing contract vaccines for patient {PatientId}. Conflicting vaccines: {ConflictingVaccines}",
                                            vaccineMedicineId, vaccineDto.VaccineCode, patientId, string.Join(", ", conflictingVaccines));

                                        throw new BadRequestException(ExceptionKey.VACCINE_INTERACTION_HAS_BEEN_ADDED);
                                    }
                                }

                                var contractPatientVaccineDetail = new ContractPatientVaccination
                                {
                                    ContractId = contract.Id,
                                    PatientId = patientId,
                                    VaccineId = vaccineMedicineId,
                                    Quantity = vaccineDto.Quantity,
                                    DoseNumber = vaccineDto.DoseNumber,
                                    Status = ContractPatientVaccinationStatus.Planned
                                };
                                contractPatientVaccineDetails.Add(contractPatientVaccineDetail);

                                patientContractVaccinesAdded[patientId].Add(vaccineMedicineId);
                            }
                            else
                            {
                                throw new BadRequestException(ExceptionKey.INVALID_VACCINE_CODE);
                            }
                        }
                    }
                    else
                    {
                        throw new BadRequestException(ExceptionKey.INVALID_PATIENT_ID);
                    }
                }

                if (contractPatientVaccineDetails.Any())
                {
                    await _dbContext.ContractPatientVaccinations.AddRangeAsync(contractPatientVaccineDetails, cancellationToken);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                var contractResponse = contract.Adapt<ContractResponse>();

                return contractResponse; 
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error registering contract. Transaction rolled back.");
                throw;
            }
        }
    }
}