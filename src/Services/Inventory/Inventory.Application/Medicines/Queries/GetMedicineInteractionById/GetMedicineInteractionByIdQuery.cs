using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Medicines.Queries.GetMedicineInteractionById
{
    public record GetMedicineInteractionByIdQuery(int Id) : IQuery<GetMedicineInteractionByIdResponse>;

    public record GetMedicineInteractionByIdResponse(MedicineInteractionDTO MedicineInteraction);
}
