using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public record RemoveServicesFromRequestFormCommand(
           int ReceptionId,
           List<int> ServiceIds
       ) : ICommand<RemoveServicesFromRequestFormResult>;

    public record RemoveServicesFromRequestFormResult(int RequestFormId);
}
