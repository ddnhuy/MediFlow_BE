using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Medicines.Queries.GetVaccineTypes
{
    namespace Inventory.Application.VaccineTypes.Queries
    {
        public record GetVaccineTypesQuery() : IQuery<GetVaccineTypesResult>;
        public record GetVaccineTypesResult(List<VaccineTypeDTO> VaccineTypes);

        public record VaccineTypeDTO(
            int VaccineTypeId,
            string VaccinatTypeName
        );
    }
}
