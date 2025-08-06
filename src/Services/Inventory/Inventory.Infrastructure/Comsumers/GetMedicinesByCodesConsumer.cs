using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using Inventory.Application.Helpers;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Comsumers
{
    public class GetMedicinesByCodesConsumer : IConsumer<GetMedicineByCodeRequest>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetMedicinesByCodesConsumer> _logger;

        public GetMedicinesByCodesConsumer(IApplicationDbContext context, ILogger<GetMedicinesByCodesConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetMedicineByCodeRequest> context)
        {
            var request = context.Message;

            _logger.LogInformation("Received medicines by codes request for MedicineCodes: {MedicineCodes}, RequestId: {RequestId}",
                string.Join(", ", request.MedicineCodes), request.RequestId);

            try
            {
                var medicineCodes = request.MedicineCodes.Select(code => code.Trim()).ToList();

                var medicines = await _context.Medicines
                    .Where(m => medicineCodes.Contains(m.MedicineCode) && !m.IsSuspended && !m.IsCancelled)
                    .Include(m => m.MedicineType)
                    .Include(m => m.VaccineType)
                    .ToListAsync(context.CancellationToken);

                var medicineIds = medicines.Select(m => m.Id).ToList();

                var prices = await _context.MedicinePrices
                    .Where(mp => medicineIds.Contains(mp.MedicineId) && !mp.IsSuspended && !mp.IsCancelled)
                    .GroupBy(mp => mp.MedicineId)
                    .Select(g => g.OrderByDescending(mp => mp.CreatedAt).First())
                    .ToListAsync(context.CancellationToken);

                var responses = medicines.Select(medicine =>
                {
                    var price = prices.FirstOrDefault(p => p.MedicineId == medicine.Id);

                    return new GetMedicineInformationResponse
                    {
                        MedicineId = medicine.Id,
                        MedicineCode = medicine.MedicineCode,
                        MedicineName = medicine.MedicineName,
                        VaccineTypeName = medicine.VaccineType?.VaccineTypeName,
                        MedicineTypeName = medicine.MedicineType?.MedicineTypeName,
                        Unit = medicine.Unit,
                        UnitPrice = price?.UnitPrice ?? 0,
                        ActiveIngredient = medicine.ActiveIngredient,
                        UsageInstructions = medicine.UsageInstructions,
                        Concentration = medicine.Concentration,
                        Indications = medicine.Indications,
                        MedicineClassification = medicine.MedicineClassification,
                        RouteOfAdministration = EnumHelper.ToEnumString(medicine.RouteOfAdministration),
                        NationalMedicineCode = medicine.NationalMedicineCode,
                        Description = medicine.Description,
                        Note = medicine.Note,
                        RegistrationNumber = medicine.RegistrationNumber,
                        IsRequiredTestingBeforeUse = medicine.IsRequiredTestingBeforeUse,
                        MedicineTypeId = medicine.MedicineTypeId,
                        VaccineTypeId = medicine.VaccineTypeId,
                        IsSuspended = medicine.IsSuspended,
                        IsCancelled = medicine.IsCancelled,
                        CreatedAt = medicine.CreatedAt,
                        LastUpdatedAt = medicine.LastUpdatedAt,
                        RequestId = request.RequestId,
                        IsSuccess = true
                    };
                }).ToList();

                var foundCodes = medicines.Select(m => m.MedicineCode).ToList();
                var missingCodes = medicineCodes.Except(foundCodes, StringComparer.OrdinalIgnoreCase).ToList();

                if (missingCodes.Any())
                {
                    _logger.LogWarning("Some medicine codes were not found: {MissingCodes}, RequestId: {RequestId}",
                        string.Join(", ", missingCodes), request.RequestId);

                    foreach (var missingCode in missingCodes)
                    {
                        responses.Add(new GetMedicineInformationResponse
                        {
                            MedicineCode = missingCode,
                            RequestId = request.RequestId,
                            IsSuccess = false,
                            ErrorMessage = $"Medicine with code '{missingCode}' not found or is cancelled"
                        });
                    }
                }

                var response = new GetMedicinesInformationResponse
                {
                    Medicines = responses
                };

                await context.RespondAsync(response);

                _logger.LogInformation("Successfully responded with medicines information for RequestId: {RequestId}, Found: {FoundCount}, Missing: {MissingCount}",
                    request.RequestId, foundCodes.Count, missingCodes.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing medicines by codes request for RequestId: {RequestId}",
                    request.RequestId);

                var errorResponse = new GetMedicinesInformationResponse
                {
                    Medicines = request.MedicineCodes.Select(code => new GetMedicineInformationResponse
                    {
                        MedicineCode = code,
                        RequestId = request.RequestId,
                        IsSuccess = false,
                        ErrorMessage = "An error occurred while retrieving medicine information"
                    }).ToList()
                };

                await context.RespondAsync(errorResponse);
            }
        }
    }
}