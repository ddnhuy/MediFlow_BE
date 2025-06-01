namespace Inventory.Application.Countries.Queries
{
    public class GetCountriesQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetCountriesQuery, GetCountriesResult>
    {
        public async Task<GetCountriesResult> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
        {
            var countries = await dbContext.Countries
                .Where(c => !c.IsCancelled)
                .OrderBy(c => c.CountryName)
                .ToListAsync();

            var countriesDTO = countries.Adapt<List<CountryDTO>>();

            return new GetCountriesResult(countriesDTO);
        }
    }
}
