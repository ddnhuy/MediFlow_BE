using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.Vaccinations.Queries.GetListPostVaccinationMedicines
{
    public record GetListPostVaccinationMedicinesQuery(int ReceptionId) : IQuery<List<GetListPostVaccinationMedicinesResult>>;

    public record GetListPostVaccinationMedicinesResult(
         int VaccinationId,
         string MedicineName,
         int Quantity,
         DateTime VaccinationDate,
         bool ObservationConfirmed,
         DateTime? ReactionDate
    );
}
