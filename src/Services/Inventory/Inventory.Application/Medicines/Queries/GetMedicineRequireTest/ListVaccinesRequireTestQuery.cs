namespace Inventory.Application.Medicines.Queries.GetMedicineRequireTest
{
    public record ListVaccinesRequireTestQuery(
        PaginationRequest PaginationRequest,
        string? Search
    ) : IQuery<ListVaccineResult>;

    public record ListVaccineResult(PaginatedResult<VaccinesRequireTestDTO> Vaccines);

    public class ListVaccinesRequireTestQueryHandler : IQueryHandler<ListVaccinesRequireTestQuery, ListVaccineResult>
    {
        private readonly IApplicationDbContext _context;

        public ListVaccinesRequireTestQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ListVaccineResult> Handle(ListVaccinesRequireTestQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Medicines
                .Include(m => m.VaccineType)
                .Where(m => m.IsRequiredTestingBeforeUse == true);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(m =>
                    m.MedicineName!.Contains(request.Search) ||
                    m.MedicineCode!.Contains(request.Search));
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(m => m.MedicineName)
                .Skip((request.PaginationRequest.PageIndex - 1) * request.PaginationRequest.PageSize)
                .Take(request.PaginationRequest.PageSize)
                .Select(m => new VaccinesRequireTestDTO
                {
                    Id = m.Id,
                    MedicineCode = m.MedicineCode,
                    MedicineName = m.MedicineName,
                    Unit = m.Unit,
                    ActiveIngredient = m.ActiveIngredient,
                    UsageInstructions = m.UsageInstructions,
                    Concentration = m.Concentration,
                    Indications = m.Indications,
                    MedicineClassification = m.MedicineClassification,
                    RouteOfAdministration = m.RouteOfAdministration,
                    NationalMedicineCode = m.NationalMedicineCode,
                    Description = m.Description,
                    Note = m.Note,
                    RegistrationNumber = m.RegistrationNumber,
                    VaccineTypeId = m.VaccineTypeId,
                    VaccineTypeCode = m.VaccineType != null ? m.VaccineType.VaccineTypeCode : string.Empty,
                    VaccineTypeName = m.VaccineType != null ? m.VaccineType.VaccineTypeName : string.Empty
                })
                .ToListAsync(cancellationToken);

            var paginated = new PaginatedResult<VaccinesRequireTestDTO>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                total,
                items
            );

            return new ListVaccineResult(paginated);
        }
    }
}