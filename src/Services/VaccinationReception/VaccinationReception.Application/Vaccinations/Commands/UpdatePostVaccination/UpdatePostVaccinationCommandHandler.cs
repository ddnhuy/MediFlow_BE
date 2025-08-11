using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.Application.Data;

namespace VaccinationReception.Application.Vaccinations.Commands.UpdatePostVaccination
{
    public class UpdatePostVaccinationCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<UpdatePostVaccinationCommand, UpdatePostVaccinationResult>
    {
        public async Task<UpdatePostVaccinationResult> Handle(UpdatePostVaccinationCommand request, CancellationToken cancellationToken)
        {
            var vaccination = await dbContext.Vaccinations
                .Include(x => x.ReceptionVaccination)
                .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

            if (vaccination == null)
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.NOT_FOUND_VACCINATION_WITH_ID);
            }

            // Update post-vaccination properties
            vaccination.ObservationConfirmed = request.ObservationConfirmed;
            vaccination.HasReaction = request.HasReaction;
            vaccination.ReactionDate = request.ReactionDate;
            vaccination.PostVaccinationResult = request.PostVaccinationResult;
            vaccination.PostVaccinationDate = request.PostVaccinationDate;
            vaccination.HasFeverAbove39 = request.HasFeverAbove39;
            vaccination.HasInjectionSiteReaction = request.HasInjectionSiteReaction;
            vaccination.HasOtherReaction = request.HasOtherReaction;
            vaccination.OtherReactionDescription = request.OtherReactionDescription;

            // Find the Reception and update its last updated time
            var reception = await dbContext.Receptions
                .FirstOrDefaultAsync(r => r.Id == vaccination.ReceptionVaccination!.ReceptionId, cancellationToken);
            if (reception == null)
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);
            }
            else
            {
                reception.LastUpdatedAt = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return new UpdatePostVaccinationResult(true);
        }
    }
}
