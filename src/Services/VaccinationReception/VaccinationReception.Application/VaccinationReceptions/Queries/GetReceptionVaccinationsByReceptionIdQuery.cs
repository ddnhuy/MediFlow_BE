using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;

namespace VaccinationReception.Application.VaccinationReceptions.Queries
{
    public record GetReceptionVaccinationsByReceptionIdQuery(int ReceptionId) : IQuery<GetReceptionVaccinationsByReceptionIdResult>;

    public record GetReceptionVaccinationsByReceptionIdResult(IEnumerable<ReceptionVaccinationDTO> ReceptionVaccinations);
}