using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.VaccinationReceptions.EventHandlers;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Data;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class CreateReceptionVaccinationCommandHandler : ICommandHandler<CreateReceptionVaccinationCommand, CreateReceptionVaccinationResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateReceptionVaccinationCommandHandler> _logger;
        private readonly IPublisher _publisher;

        public CreateReceptionVaccinationCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateReceptionVaccinationCommandHandler> logger,
            IPublisher publisher)
        {
            _context = context;
            _logger = logger;
            _publisher = publisher;
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

                var receptionVaccination = request.Adapt<ReceptionVaccination>();

                _context.ReceptionVaccinations.Add(receptionVaccination);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Đã tạo mới ReceptionVaccination với Id: {Id} cho ReceptionId: {ReceptionId}",
                    receptionVaccination.Id, request.ReceptionId);

                // Publish ReceptionVaccinationCreatedEvent
                var createdEvent = new ReceptionVaccinationCreatedEvent
                {
                    PatientId = reception.PatientId,
                    VaccineId = receptionVaccination.VaccineId,
                    AppointmentDate = receptionVaccination.AppointmentDate,
                    Note = receptionVaccination.Note
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