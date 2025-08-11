using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.Application.Data;

namespace VaccinationReception.Application.Vaccinations.Commands.UpdatePreExaminationResult
{
    public class UpdatePreExaminationCommandHandler : ICommandHandler<UpdatePreExaminationCommand, UpdatePreExaminationResult>
    {
        private readonly IApplicationDbContext _context;
        public UpdatePreExaminationCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<UpdatePreExaminationResult> Handle(UpdatePreExaminationCommand request, CancellationToken cancellationToken)
        {
            var receptionVacination = await _context.ReceptionVaccinations.FirstOrDefaultAsync(x => x.Id == request.ReceptionVaccinationId, cancellationToken);

            if (receptionVacination == null)
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.NOT_FOUND_VACCINATION_RECEPTION_WITH_ID);
            }

            receptionVacination.TestResultEntry = request.TestEntryResult;
            receptionVacination.IsPreExaminationTesting = true;
            receptionVacination.VaccinationTestDate = DateTime.UtcNow;

            // Update the Reception's last updated time
            var reception = await _context.Receptions.FirstOrDefaultAsync(r => r.Id == receptionVacination.ReceptionId, cancellationToken);
            if (reception == null)
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);
            }
            else
            {
                reception.LastUpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdatePreExaminationResult(true);
        }
    }
}
