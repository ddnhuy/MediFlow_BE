namespace Inventory.Application.UniqueCode.Queries
{
    public record GenerateUniqueCodeQuery : IQuery<GenerateUniqueCodeResult>;
    public record GenerateUniqueCodeResult(string UniqueCode);
}
