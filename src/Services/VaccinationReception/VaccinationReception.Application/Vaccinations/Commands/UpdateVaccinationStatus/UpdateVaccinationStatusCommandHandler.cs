using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.Application.Data;

namespace VaccinationReception.Application.Vaccinations.Commands.UpdateVaccinationStatus
{
    public class UpdateVaccinationStatusCommandHandler : ICommandHandler<UpdateVaccinationStatusCommand, UpdateVaccinationStatusCommandResult>
    {
        private readonly IApplicationDbContext _dbContext;

        public UpdateVaccinationStatusCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UpdateVaccinationStatusCommandResult> Handle(UpdateVaccinationStatusCommand request, CancellationToken cancellationToken)
        {
            var vaccination = await _dbContext.Vaccinations
                .FirstOrDefaultAsync(v => v.Id == request.VaccinationId, cancellationToken);

            if (vaccination == null)
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.NOT_FOUND_VACCINATION_WITH_ID);
            }

            vaccination.IsConfirmed = request.Status;

            // Find the Reception and update its last updated time
            var reception = await _dbContext.Receptions
                .FirstOrDefaultAsync(r => r.Id == vaccination.ReceptionVaccinationId, cancellationToken);

            if (reception == null)
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);
            }
            else
            {
                reception.LastUpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateVaccinationStatusCommandResult(true);
        }
    }
}
