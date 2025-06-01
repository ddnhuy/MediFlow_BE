using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public record DeleteReceptionVaccinationsCommand(List<int> ReceptionVaccinationIds) : ICommand<DeleteReceptionVaccinationsResult>;

    public record DeleteReceptionVaccinationsResult(bool IsSuccess, int DeletedCount);
}