using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Domain.DTOs;
using VaccinationReception.Domain.IServiceClients;
using VaccinationReception.Infrastructure.Data;

namespace HospitalFee.FunctionalTests.Abstractions
{
    public abstract class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
    {
        protected readonly HttpClient _client;
        protected readonly ApplicationDbContext _dbContext;
        protected readonly IHospitalServiceClient _hospitalServiceClientMock;

        protected BaseFunctionalTest(FunctionalTestWebAppFactory factory)
        {
            _client = factory.CreateClient();
            _dbContext = factory.DbContext!;
            _hospitalServiceClientMock = factory.HospitalServiceClientMock!;
        }

        protected void MockGetServicesByIds(List<int> serviceIds, List<ServiceResponse> serviceResponses)
        {
            _hospitalServiceClientMock
                .GetServicesByIdsAsync(
                    Arg.Is<List<int>>(ids => ids.SequenceEqual(serviceIds)),
                    Arg.Any<CancellationToken>()
                )
                .Returns(Task.FromResult(serviceResponses));
        }

        protected async Task SeedEntityAsync<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Set<TEntity>().Add(entity);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }

        protected async Task SeedEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _dbContext.Set<TEntity>().AddRange(entities);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }
    }
}
