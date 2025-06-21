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
                .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

            if (vaccination == null)
            {
                throw new BadRequestException($"Không tìm thấy mũi tiêm với ID {request.Id}");
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

            await dbContext.SaveChangesAsync(cancellationToken);

            return new UpdatePostVaccinationResult(true);
        }
    }
}
