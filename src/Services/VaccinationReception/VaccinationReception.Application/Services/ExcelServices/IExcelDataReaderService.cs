using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.ExcelDTOs;
using VaccinationReception.Application.DTOs.PatientDTOs;

namespace VaccinationReception.Application.Services.ExcelServices
{
    public interface IExcelDataReaderService
    {
        Task<List<ContractVaccinationExcelDto>> ReadContractVaccinationExcelFromUrl(string downloadUrl);
    }
}
