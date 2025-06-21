using BuildingBlocks.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.DTOs;
using VaccinationReception.Domain.IServiceClients;

namespace VaccinationReception.Infrastructure.ServiceClients
{
    public class HospitalServiceClient : IHospitalServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HospitalServiceClient> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public HospitalServiceClient(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor,
            ILogger<HospitalServiceClient> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.DefaultRequestHeaders.Add("Authorization", _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString());
        }

        public async Task<List<ServiceResponse>> GetServicesByGroupAsync(
            int groupId,
            string groupType,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<ServiceResponse>>(
                    $"/services/group?groupId={groupId}&groupType={groupType}",
                    cancellationToken);

                if (response == null)
                {
                    _logger.LogWarning("No services found for group {GroupId} of type {GroupType}",
                        groupId, groupType);
                    return new List<ServiceResponse>();
                }

                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling HospitalService API");
                // throw new InternalServerException("HospitalService is currently unavailable", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while calling HospitalService API");
                // throw new InternalServerException("An unexpected error occurred while calling HospitalService", ex.Message);
                return null;
            }
        }
        public async Task<List<ServiceResponse>> GetServicesByIdsAsync(
        List<int> serviceIds,
        CancellationToken cancellationToken)
        {
            try
            {
                if (serviceIds == null || !serviceIds.Any())
                {
                    return new List<ServiceResponse>();
                }

                var response = await _httpClient.PostAsJsonAsync(
                    "/services/by-ids",
                    serviceIds,
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<ServiceResponse>>(
                    cancellationToken: cancellationToken);

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling HospitalService API to get services by IDs");
                //  throw new InternalServerException("HospitalService is currently unavailable", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while calling HospitalService API to get services by IDs");
                //  throw new InternalServerException("An unexpected error occurred while calling HospitalService", ex.Message);
                return null;
            }
        }
    }
}