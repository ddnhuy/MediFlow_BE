using BuildingBlocks.Exceptions;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.DTOs;
using VaccinationReception.Domain.IServiceClients;

namespace VaccinationReception.Infrastructure.ServiceClients
{
    public class InventoryServiceClient : IInventoryServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<InventoryServiceClient> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public InventoryServiceClient(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor,
            ILogger<InventoryServiceClient> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.DefaultRequestHeaders.Add("Authorization", _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString());
        }

        public record GetMedicinePricesByMedicineIdResponse(MedicinePriceDTO MedicinePrices);

        public async Task<MedicinePriceDTO?> GetMedicineByIdAsync(int medicineId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/medicine-prices/{medicineId}", cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<GetMedicinePricesByMedicineIdResponse>(cancellationToken: cancellationToken);
                    return result?.MedicinePrices?.Adapt<MedicinePriceDTO>();
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Medicine with ID {MedicineId} not found", medicineId);
                }
                else
                {
                    _logger.LogWarning("Unexpected status code {StatusCode} when getting medicine ID {MedicineId}", (int)response.StatusCode, medicineId);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling InventoryService API to get medicine by ID {MedicineId}", medicineId);
                // throw new InternalServerException("An unexpected error occurred while calling InventoryService", ex.Message);
                return null;
            }
        }
    }
}
