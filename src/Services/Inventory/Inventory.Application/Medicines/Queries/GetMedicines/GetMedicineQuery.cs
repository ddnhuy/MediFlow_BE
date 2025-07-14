namespace Inventory.Application.Medicines.Queries.GetMedicines
{
    public record GetMedicinesQuery(PaginationRequest PaginationRequest, string? SearchKeyword = null) : IQuery<GetMedicinesResult>;

    public record GetMedicinesResult(PaginatedResult<MedicineDTO> Medicines);

}
