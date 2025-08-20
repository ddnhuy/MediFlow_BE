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
            var receptionVacination = await _context.ReceptionVaccinations
                .Include(rv => rv.Reception)
                .Include(rv => rv.SecondaryReception)
                .FirstOrDefaultAsync(x => x.Id == request.ReceptionVaccinationId, cancellationToken);

            if (receptionVacination == null)
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.NOT_FOUND_VACCINATION_RECEPTION_WITH_ID);
            }

            // Check if vaccination has been confirmed (any confirmed vaccination exists for this ReceptionVaccination)
            var hasConfirmedVaccination = await _context.Vaccinations
                .AnyAsync(v => v.ReceptionVaccinationId == request.ReceptionVaccinationId && v.IsConfirmed, cancellationToken);

            if (hasConfirmedVaccination)
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.CANNOT_UPDATE_PRE_EXAMINATION_RESULT_AFTER_VACCINATION_CONFIRMED);
            }

            receptionVacination.TestResultEntry = request.TestEntryResult;
            receptionVacination.IsPreExaminationTesting = true;
            receptionVacination.VaccinationTestDate = DateTime.UtcNow;


            // Update the Reception's last updated time
            var currentReception = receptionVacination.SecondaryReception ?? receptionVacination.Reception;
            if (currentReception == null)
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);
            }
            else
            {
                currentReception.LastUpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdatePreExaminationResult(true);
        }
    }
}
