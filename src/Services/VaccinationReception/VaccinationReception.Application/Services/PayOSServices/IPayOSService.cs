using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.PayOSDTOs;

namespace VaccinationReception.Application.Services.PayOSServices
{
    public interface IPayOSService
    {
        Task<CreatePaymentResult> CreatePaymentLinkAsync(int orderCode, int amount, string description, CancellationToken cancellationToken = default);
    }
}
