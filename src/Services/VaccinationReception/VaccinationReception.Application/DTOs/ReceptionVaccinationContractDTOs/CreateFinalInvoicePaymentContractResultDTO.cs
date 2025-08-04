using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.DTOs.ReceptionVaccinationContractDTOs
{
    public class CreateFinalInvoicePaymentContractResultDTO
    {
        public int ContractId { get; set; }
        public PaymentContractDTO PaymentContract { get; set; }
        public List<ContractServiceDetailDTO> Details { get; set; }
    }
}
