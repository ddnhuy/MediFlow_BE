using InventoryService.FunctionalTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace InventoryService.FunctionalTests.Abstractions
{
    public class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
    {
        public BaseFunctionalTest(FunctionalTestWebAppFactory factory)
        {
            _client = factory.CreateClient();
            // Add authentication header for protected endpoints
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TokenHelper.GenerateTestToken());
        }

        protected HttpClient _client = new();
    }
}
