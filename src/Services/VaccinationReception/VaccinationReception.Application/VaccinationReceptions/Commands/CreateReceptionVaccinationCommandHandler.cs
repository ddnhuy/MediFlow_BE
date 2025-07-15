using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.VaccinationReceptions.EventHandlers;
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

        public CreateReceptionVaccinationCommandHandler(
            ILogger<CreateReceptionVaccinationCommandHandler> logger,
            IInventoryService inventoryService,
            IPublisher publisher,
            IApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _publisher = publisher;
            _inventoryService = inventoryService;
            _httpContextAccessor = httpContextAccessor;
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
                var medicineList = await _inventoryService.GetMedicineInformationAsync([request.VaccineId], cancellationToken);
                var medicine = medicineList.FirstOrDefault(m => m.MedicineId == request.VaccineId);

                var receptionVaccination = request.Adapt<ReceptionVaccination>();

                receptionVaccination.PaymentStatus = Domain.Enums.PaymentStatusForItem.NotPaid;
                receptionVaccination.RequestNumber = UniqueStringGenerator.GenerateUniqueString();
                receptionVaccination.UnitPrice = medicine?.UnitPrice ?? 0;
                receptionVaccination.DoctorId = int.Parse(_httpContextAccessor.HttpContext!.User!
                 .FindFirst(ClaimTypes.NameIdentifier)!.Value);

                await _context.ReceptionVaccinations.AddAsync(receptionVaccination);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Đã tạo mới ReceptionVaccination với Id: {Id} cho ReceptionId: {ReceptionId}",
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
                _logger.LogError(ex, "Lỗi khi tạo mới ReceptionVaccination cho ReceptionId: {ReceptionId}",
                    request.ReceptionId);
                throw;
            }
        }
    }
}