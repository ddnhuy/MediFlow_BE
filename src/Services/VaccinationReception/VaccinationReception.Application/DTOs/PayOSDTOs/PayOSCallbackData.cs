using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VaccinationReception.Application.DTOs.PayOSDTOs
{
    public class PayOSCallbackData
    {
        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [JsonPropertyName("transactionDateTime")]
        public string TransactionDateTime { get; set; }

        [JsonPropertyName("virtualAccountNumber")]
        public string VirtualAccountNumber { get; set; }

        [JsonPropertyName("counterAccountBankId")]
        public string CounterAccountBankId { get; set; }

        [JsonPropertyName("counterAccountBankName")]
        public string CounterAccountBankName { get; set; }

        [JsonPropertyName("counterAccountName")]
        public string CounterAccountName { get; set; }

        [JsonPropertyName("counterAccountNumber")]
        public string CounterAccountNumber { get; set; }

        [JsonPropertyName("virtualAccountName")]
        public string VirtualAccountName { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("orderCode")]
        public int OrderCode { get; set; }

        [JsonPropertyName("paymentLinkId")]
        public string PaymentLinkId { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("desc")]
        public string Desc { get; set; }
    }
}
