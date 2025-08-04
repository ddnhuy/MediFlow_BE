using System;
using System.Collections.Generic;
using OfficeOpenXml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.ExcelDTOs;
using VaccinationReception.Application.DTOs.VaccinationDTOs;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;

namespace VaccinationReception.Application.Services.ExcelServices
{
    public class ExcelDataReaderService : IExcelDataReaderService
    {
        private readonly ILogger<ExcelDataReaderService> _logger;
        private readonly HttpClient _httpClient;

        public ExcelDataReaderService(ILogger<ExcelDataReaderService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }
        public async Task<List<ContractVaccinationExcelDto>> ReadContractVaccinationExcelFromUrl(string downloadUrl)
        {
            var dataList = new List<ContractVaccinationExcelDto>();
            var patientDictionary = new Dictionary<string, ContractVaccinationExcelDto>(); // Key: IdentityCard

            if (string.IsNullOrWhiteSpace(downloadUrl))
            {
                _logger.LogError("Download URL is null or empty");
                throw new BadRequestException(ExceptionKey.DOWNLOAD_URL_CANNOT_BE_NULL_OR_EMPTY);
            }

            try
            {
                if (_httpClient.Timeout == TimeSpan.FromSeconds(100))
                {
                    _httpClient.Timeout = TimeSpan.FromMinutes(5);
                }

                _logger.LogInformation("Downloading Excel file from URL: {Url}", downloadUrl);

                var fileBytes = await _httpClient.GetByteArrayAsync(downloadUrl);

                _logger.LogInformation("Successfully downloaded file. Size: {Size} bytes", fileBytes.Length);

                using (var stream = new MemoryStream(fileBytes))
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        _logger.LogError("The Excel file does not contain any worksheet.");
                        throw new BadRequestException(ExceptionKey.THE_EXCEL_FILE_DOES_NOT_CONTAIN_ANY_WORKSHEET);
                    }

                    int startRow = worksheet.Dimension.Start.Row;
                    int endRow = worksheet.Dimension.End.Row;
                    int dataStartRow = startRow + 1;

                    if (dataStartRow > endRow)
                    {
                        _logger.LogWarning("The Excel file does not contain any data after the header row.");
                        return dataList;
                    }

                    for (int rowNum = dataStartRow; rowNum <= endRow; rowNum++)
                    {
                        try
                        {
                            var patientName = worksheet.Cells[rowNum, 1].GetValue<string>()?.Trim() ?? string.Empty;

                            var genderStr = worksheet.Cells[rowNum, 2].GetValue<string>()?.Trim().ToLower();
                            var gender = genderStr switch
                            {
                                "nam" => 1,
                                "nữ" or "nu" => 0,
                                _ => 0
                            };

                            var dobRaw = worksheet.Cells[rowNum, 3].GetValue<string>();
                            DateTime.TryParse(dobRaw, out DateTime dob);

                            var phoneNumber = worksheet.Cells[rowNum, 4].GetValue<string>()?.Trim();
                            var email = worksheet.Cells[rowNum, 5].GetValue<string>()?.Trim();
                            var identityCard = worksheet.Cells[rowNum, 6].GetValue<string>()?.Trim();
                            var addressDetail = worksheet.Cells[rowNum, 7].GetValue<string>()?.Trim();
                            var province = worksheet.Cells[rowNum, 8].GetValue<string>()?.Trim();
                            var district = worksheet.Cells[rowNum, 9].GetValue<string>()?.Trim();
                            var ward = worksheet.Cells[rowNum, 10].GetValue<string>()?.Trim();

                            bool.TryParse(worksheet.Cells[rowNum, 11].Text, out bool isPregnant);
                            bool.TryParse(worksheet.Cells[rowNum, 12].Text, out bool isForeigner);

                            var vaccineCode = worksheet.Cells[rowNum, 13].GetValue<string>()?.Trim() ?? string.Empty;
                            var vaccineName = worksheet.Cells[rowNum, 14].GetValue<string>()?.Trim() ?? string.Empty;

                            int.TryParse(worksheet.Cells[rowNum, 15].Text, out int quantity);
                            int.TryParse(worksheet.Cells[rowNum, 16].Text, out int doseNumber);

                            if (string.IsNullOrWhiteSpace(patientName) ||
                                string.IsNullOrWhiteSpace(vaccineCode) ||
                                string.IsNullOrWhiteSpace(identityCard) ||
                                quantity <= 0)
                            {
                                _logger.LogWarning("Row {RowNum} contains missing or invalid data. Skipping this row.", rowNum);
                                continue;
                            }

                            var vaccine = new VaccineDTO
                            {
                                VaccineCode = vaccineCode,
                                VaccineName = vaccineName,
                                Quantity = quantity,
                                DoseNumber = doseNumber
                            };

                            if (patientDictionary.ContainsKey(identityCard))
                            {
                                var existingPatient = patientDictionary[identityCard];
                                var existingVaccine = existingPatient.Vaccines.FirstOrDefault(v =>
                                    v.VaccineCode == vaccine.VaccineCode && v.DoseNumber == vaccine.DoseNumber);

                                if (existingVaccine != null)
                                {
                                    existingVaccine.Quantity += vaccine.Quantity;
                                    _logger.LogInformation("Accumulated vaccine {VaccineCode} for patient {IdentityCard} - New quantity: {Quantity}", vaccine.VaccineCode, identityCard, existingVaccine.Quantity);
                                }
                                else
                                {
                                    existingPatient.Vaccines.Add(vaccine);
                                    _logger.LogInformation("Added new vaccine {VaccineCode} for patient {IdentityCard}", vaccine.VaccineCode, identityCard);
                                }
                            }
                            else
                            {
                                var dto = new ContractVaccinationExcelDto
                                {
                                    PatientName = patientName,
                                    Gender = gender,
                                    DOB = dob,
                                    PhoneNumber = phoneNumber,
                                    Email = email,
                                    IdentityCard = identityCard,
                                    AddressDetail = addressDetail,
                                    Province = province,
                                    District = district,
                                    Ward = ward,
                                    IsPregnant = isPregnant,
                                    IsForeigner = isForeigner,
                                    Vaccines = new List<VaccineDTO> { vaccine }
                                };

                                patientDictionary[identityCard] = dto;
                                _logger.LogInformation("Created new patient {IdentityCard} with vaccine {VaccineCode}", identityCard, vaccine.VaccineCode);
                            }
                        }
                        catch (FormatException fex)
                        {
                            _logger.LogError(fex, "Data format error at row {RowNum}. Please check date, number, and boolean formats.", rowNum);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Unknown error occurred while reading data at row {RowNum}", rowNum);
                        }
                    }

                    dataList = patientDictionary.Values.ToList();
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Failed to download the Excel file from URL: {Url}", downloadUrl);
                throw new BadRequestException(ExceptionKey.FAILED_TO_DOWNLOAD_THE_EXCEL_FILE_FROM_URL);
            }
            catch (TaskCanceledException tcEx) when (tcEx.InnerException is TimeoutException)
            {
                _logger.LogError(tcEx, "Download timeout when accessing URL: {Url}", downloadUrl);
                throw new BadRequestException(ExceptionKey.DOWNLOAD_TIMEOUT_WHEN_ACCESSING_URL);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read the Excel file from URL. Make sure the URL is valid and accessible.");
                throw;
            }

            _logger.LogInformation("Successfully read {PatientCount} patients with a total of {VaccineCount} vaccines.",
                dataList.Count, dataList.Sum(p => p.Vaccines.Count));

            return dataList;
        }
    }
}