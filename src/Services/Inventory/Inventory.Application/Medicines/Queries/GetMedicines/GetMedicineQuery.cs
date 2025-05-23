namespace Inventory.Application.Medicines.Queries.GetMedicines
{
    public record GetMedicinesQuery(PaginationRequest PaginationRequest) : IQuery<GetMedicinesResult>;

    public record GetMedicinesResult(PaginatedResult<MedicineDTO> Medicines);

}
