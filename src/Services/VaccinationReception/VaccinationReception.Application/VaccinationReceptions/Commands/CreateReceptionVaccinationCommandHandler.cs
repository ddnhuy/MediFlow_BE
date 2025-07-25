using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using BuildingBlocks.Strings.Consts.HospitalServices;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.VaccinationReceptions.EventHandlers;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class CreateReceptionVaccinationCommandHandler : ICommandHandler<CreateReceptionVaccinationCommand, CreateReceptionVaccinationResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CreateReceptionVaccinationCommandHandler> _logger;
        private readonly IInventoryService _inventoryService;
        private readonly IPublisher _publisher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHospitalService _hospitalService;

        public CreateReceptionVaccinationCommandHandler(
            ILogger<CreateReceptionVaccinationCommandHandler> logger,
            IInventoryService inventoryService,
            IPublisher publisher,
            IApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IHospitalService hospitalService)
        {
            _context = context;
            _logger = logger;
            _publisher = publisher;
            _inventoryService = inventoryService;
            _httpContextAccessor = httpContextAccessor;
            _hospitalService = hospitalService;
        }

        public async Task<CreateReceptionVaccinationResult> Handle(CreateReceptionVaccinationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var reception = await _context.Receptions
                    .FirstOrDefaultAsync(r => r.Id == request.ReceptionId && !r.IsCancelled, cancellationToken);

                if (reception == null)
                {
                    _logger.LogWarning("Không tìm thấy Reception với Id: {ReceptionId}", request.ReceptionId);
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_VACCINATION_RECEPTION_WITH_ID);
                }
                var checkInteraction = await  _inventoryService.GetMedicineInteractionsResponseAsync(request.VaccineId);

                var existingVaccines = await _context.ReceptionVaccinations
                    .Where(rv => rv.ReceptionId == request.ReceptionId)
                    .Select(rv => rv.VaccineId)
                    .ToListAsync(cancellationToken);


                if (existingVaccines.Any())
                {
                    var interactionResponse = await _inventoryService.GetMedicineInteractionsResponseAsync(request.VaccineId);

                    var hasInteraction = existingVaccines
                        .Where(id => id != request.VaccineId)
                        .Any(existingVaccineId =>
                        interactionResponse.Interactions.Any(interaction =>
                            interaction.MedicineId1 == existingVaccineId ||
                            interaction.MedicineId2 == existingVaccineId));

                    if (hasInteraction)
                    {
                        var conflictingVaccines = existingVaccines
                            .Where(id => id != request.VaccineId)
                            .Where(existingVaccineId =>
                            interactionResponse.Interactions.Any(interaction =>
                                interaction.MedicineId1 == existingVaccineId ||
                                interaction.MedicineId2 == existingVaccineId)).ToList();

                        _logger.LogWarning("Vaccine {VaccineId} has interactions with existing vaccines in reception {ReceptionId}. " +
                                           "Conflicting vaccines: {ConflictingVaccines}",
                                           request.VaccineId, request.ReceptionId, string.Join(", ", conflictingVaccines));

                        throw new BadRequestException( ExceptionKey.VACCINE_INTERACTION_HAS_BEEN_ADDED);
                    }
                }

                var existingVaccineQuantities = await _context.ReceptionVaccinations
                    .Where(rv => rv.VaccineId == request.VaccineId && rv.PaymentStatus == PaymentStatusForItem.Paid)
                    .SumAsync(rv => rv.Quantity, cancellationToken);

                var quantityVaccineCheck = existingVaccineQuantities + request.Quantity;

                var checkStockVaccine = await _inventoryService.CheckMedicineStockResponseAsync(request.VaccineId, quantityVaccineCheck);

                if (!checkStockVaccine.IsEnough)
                {
                    var shortageAmount = Math.Abs(checkStockVaccine.Difference);
                    _logger.LogWarning("Insufficient stock for vaccine {VaccineId}. Required: {Required}, Available: {Available}, Shortage: {Shortage}",
                                       request.VaccineId, quantityVaccineCheck, checkStockVaccine.CurrentStock, shortageAmount);

                    throw new BadRequestException(
                          ExceptionKey.INSUFFICIENT_VACCINE_STOCK,
                          $"Not enough vaccines in stock. Shortage amount: {shortageAmount}");
                }

                var medicineList = await _inventoryService.GetMedicineInformationAsync([request.VaccineId], cancellationToken);
                var medicine = medicineList.FirstOrDefault(m => m.MedicineId == request.VaccineId);

                var serviceRequest = await _hospitalService.GetServicesByServiceCodeAsync(
                    new List<string> { medicine.RouteOfAdministration},
                    cancellationToken
                );

                var serviceMedicine = serviceRequest.FirstOrDefault(m => m.ServiceCode == medicine.RouteOfAdministration);

                if (serviceMedicine != null)
                {
                    var existingDetail = await _context.ServiceRequestDetails
                        .FirstOrDefaultAsync(d => d.ServiceId == serviceMedicine.Id && d.ReceptionId == request.ReceptionId, cancellationToken);

                    if (existingDetail != null)
                    {
                        existingDetail.Quantity += request.Quantity;
                        _context.ServiceRequestDetails.Update(existingDetail);
                    }
                    else
                    {
                        var detail = new ServiceRequestDetail
                        {
                            RequestNumber = UniqueStringGenerator.GenerateUniqueString(),
                            ReceptionId = reception.Id,
                            ServiceId = serviceMedicine.Id,
                            Quantity = request.Quantity,
                            UnitPrice = serviceMedicine.UnitPrice
                        };

                        await _context.ServiceRequestDetails.AddAsync(detail, cancellationToken);
                    }
                }

                var receptionVaccination = request.Adapt<ReceptionVaccination>();

                receptionVaccination.PaymentStatus = PaymentStatusForItem.NotPaid;
                receptionVaccination.RequestNumber = UniqueStringGenerator.GenerateUniqueString();
                receptionVaccination.UnitPrice = medicine?.UnitPrice ?? 0;
                receptionVaccination.DoctorId = int.Parse(_httpContextAccessor.HttpContext!.User!
                 .FindFirst(ClaimTypes.NameIdentifier)!.Value);

                await _context.ReceptionVaccinations.AddAsync(receptionVaccination);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created new ReceptionVaccination with Id: {Id} for ReceptionId: {ReceptionId}",
                                       receptionVaccination.Id, request.ReceptionId);

                // Publish ReceptionVaccinationCreatedEvent
                var createdEvent = new ReceptionVaccinationCreatedEvent
                {
                    PatientId = reception.PatientId,
                    VaccineId = receptionVaccination.VaccineId,
                    AppointmentDate = receptionVaccination.AppointmentDate,
                    Note = receptionVaccination.Note,
                    VaccineName = medicine!.MedicineName,
                    Dose = "N/A",   
                    DoctorId = receptionVaccination.DoctorId.Value
                };
                await _publisher.Publish(createdEvent, cancellationToken);

                return new CreateReceptionVaccinationResult(receptionVaccination.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating new ReceptionVaccination for ReceptionId: {ReceptionId}",
                                 request.ReceptionId);
                throw;
            }
        }
    }
}