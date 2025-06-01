namespace Inventory.Application.Countries.Queries
{
    public record GetCountriesQuery() : IQuery<GetCountriesResult>;
    public record GetCountriesResult(List<CountryDTO> Countries);
}
