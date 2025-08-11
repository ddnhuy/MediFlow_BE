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
                .Include(v => v.ReceptionVaccination)
                .ThenInclude(rv => rv.Reception)
                .Include(v => v.ReceptionVaccination)
                .ThenInclude(rv => rv.SecondaryReception)
                .FirstOrDefaultAsync(v => v.Id == request.VaccinationId, cancellationToken);

            if (vaccination == null)
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.NOT_FOUND_VACCINATION_WITH_ID);
            }

            vaccination.IsConfirmed = request.Status;

            // Find the Reception and update its last updated time
            var currentReception = vaccination.ReceptionVaccination.SecondaryReception ?? vaccination.ReceptionVaccination.Reception;

            if (currentReception == null)
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);
            }
            else
            {
                currentReception.LastUpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateVaccinationStatusCommandResult(true);
        }
    }
}
