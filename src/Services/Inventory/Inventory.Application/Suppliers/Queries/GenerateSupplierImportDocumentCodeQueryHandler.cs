namespace Inventory.Application.Suppliers.Queries
{
    public class GenerateSupplierImportDocumentCodeQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GenerateSupplierImportDocumentCodeQuery, GenerateSupplierImportDocumentCodeResult>
    {
        public async Task<GenerateSupplierImportDocumentCodeResult> Handle(GenerateSupplierImportDocumentCodeQuery request, CancellationToken cancellationToken)
        {
            // DOCUMENT CODE FORMAT: PN[YYYYMMDD]-[Sequence]
            var today = DateOnly.FromDateTime(DateTime.Now);
            string dateString = $"{today.Year}{today.Month:D2}{today.Day:D2}";
            var codePrefix = $"PN{dateString}";

            // Get the highest sequence number used today
            var todayDocuments = await dbContext.SupplierImportDocuments
                .Where(d => d.DocumentCode != null && d.DocumentCode.StartsWith(codePrefix))
                .ToListAsync();

            int nextSequence = 1;
            if (todayDocuments.Any())
            {
                var sequences = todayDocuments
                    .Select(d => int.TryParse(d.DocumentCode!.Split('-').Last(), out int seq) ? seq : 0)
                    .Where(seq => seq > 0);

                if (sequences.Any())
                {
                    nextSequence = sequences.Max() + 1;
                }
            }

            // DOCUMENT NUMBER FORMAT: NK[YYYY]_[Sequence]
            var currentYear = today.Year;
            var documentNumberPrefix = $"NK{currentYear}_";

            // Get the highest document number sequence for the current year
            var yearDocuments = await dbContext.SupplierImportDocuments
                .Where(d => d.DocumentNumber != null && d.DocumentNumber.StartsWith(documentNumberPrefix))
                .ToListAsync();

            int nextDocNumSequence = 1;
            if (yearDocuments.Any())
            {
                var sequences = yearDocuments
                    .Select(d => int.TryParse(d.DocumentNumber!.Split('_').Last(), out int seq) ? seq : 0)
                    .Where(seq => seq > 0);

                if (sequences.Any())
                {
                    nextDocNumSequence = sequences.Max() + 1;
                }
            }

            var result = new GenerateSupplierImportDocumentCodeResult( DocumentCode: $"{codePrefix}-{nextSequence:D3}", DocumentNumber: $"{documentNumberPrefix}{nextDocNumSequence:D3}");

            return result;
        }
    }
}
