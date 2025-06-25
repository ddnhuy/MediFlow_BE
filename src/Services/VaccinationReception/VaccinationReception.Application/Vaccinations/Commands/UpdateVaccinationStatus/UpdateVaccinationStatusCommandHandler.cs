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
            var receptionVacciation = await  _dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(v => v.Id == request.ReceptionVaccinationId);

            if (receptionVacciation == null)
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.NOT_FOUND_VACCINATION_RECEPTION_WITH_ID);
            }

            receptionVacciation.IsConfirmed = request.Status;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateVaccinationStatusCommandResult(true);
        }
    }
}
