namespace Inventory.Application.Suppliers.Queries
{
    public record GenerateSupplierImportDocumentCodeQuery : IQuery<GenerateSupplierImportDocumentCodeResult>;
    public record GenerateSupplierImportDocumentCodeResult(string DocumentCode, string DocumentNumber);
}
