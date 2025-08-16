using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.PayOSDTOs;

namespace VaccinationReception.Application.Services.PayOSServices
{
    public class PayOSService : IPayOSService
    {
        private readonly string _clientId;
        private readonly string _apiKey;
        private readonly string _checksumKey;
        private readonly string _cancelUrl;
        private readonly string _returnUrl;
        private readonly PayOS _payOS;
        private readonly ILogger<PayOSService> _logger;


        public PayOSService(IConfiguration configuration, ILogger<PayOSService> logger)
        {
            _logger = logger;
            _clientId = configuration["PayOS:ClientId"] ?? throw new InvalidOperationException("PayOS ClientId not configured");
            _apiKey = configuration["PayOS:ApiKey"] ?? throw new InvalidOperationException("PayOS ApiKey not configured");
            _checksumKey = configuration["PayOS:ChecksumKey"] ?? throw new InvalidOperationException("PayOS ChecksumKey not configured");
            _cancelUrl = configuration["PayOS:CancelUrl"];
            _returnUrl = configuration["PayOS:ReturnUrl"];
            _payOS = new PayOS(_clientId, _apiKey, _checksumKey);
        }

        public async Task<CreatePaymentResult> CreatePaymentLinkAsync(int orderCode, int amount, string description, CancellationToken cancellationToken = default)
        {
            try
            {
                var paymentData = new PaymentData(
                    orderCode,
                    amount,
                    description,
                    new List<ItemData>(),
                    _cancelUrl,
                    _returnUrl
                );
                CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

                _logger.LogInformation("Creating PayOS payment link for order: {OrderCode}, amount: {Amount}", orderCode, amount);
    
                return createPayment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PayOS payment link for order: {OrderCode}", orderCode);
                throw;
            }
        }
    }
}