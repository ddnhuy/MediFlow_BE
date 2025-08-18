using BuildingBlocks.CQRS;
using BuildingBlocks.Strings.Consts.HospitalServices;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.Patients.Commands.CreatePatient;
using VaccinationReception.Application.Patients.Commands.UpdatePatient;
using VaccinationReception.Application.Services.PatientServices;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class CreatePatientReceptionCommandHandler : ICommandHandler<CreatePatientReceptionCommand, CreatePatientReceptionResult>
    {
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly ILogger<CreatePatientReceptionCommand> _logger;
        private readonly IApplicationDbContext _context;
        private readonly IHospitalService _hospitalService;

        public CreatePatientReceptionCommandHandler(
            IPatientGrpcClient patientGrpcClient,
            ILogger<CreatePatientReceptionCommand> logger,
            IApplicationDbContext context,
            IHospitalService hospitalService)
        {
            _patientGrpcClient = patientGrpcClient;
            _logger = logger;
            _context = context;
            _hospitalService = hospitalService;
        }

        public async Task<CreatePatientReceptionResult> Handle(CreatePatientReceptionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                int patientId;

                // Handle patient creation/update logic
                if (request.patientId != 0)
                {
                    var patient = await _patientGrpcClient.GetPatientAsync(request.patientId, cancellationToken);

                    if (patient is not null)
                    {
                        var patientUpdate = request.createPatientCommand.Adapt<UpdatePatientCommand>() with
                        {
                            Id = request.patientId
                        };
                        await _patientGrpcClient.UpdatePatientAsync(patientUpdate, cancellationToken);
                        patientId = request.patientId;
                    }
                    else
                    {
                        var patientCreate = request.createPatientCommand.Adapt<CreatePatientCommand>();
                        var createdPatient = await _patientGrpcClient.CreatePatientAsync(patientCreate, cancellationToken);
                        patientId = createdPatient.Id;
                    }
                }
                else
                {
                    var patientCreate = request.createPatientCommand.Adapt<CreatePatientCommand>();
                    var createdPatient = await _patientGrpcClient.CreatePatientAsync(patientCreate, cancellationToken);
                    patientId = createdPatient.Id;
                }

                // Create reception
                var reception = new Reception
                {
                    PatientId = patientId,
                    ReceptionDate = request.createReceptionDTO.ReceptionDate,
                    ServiceTypeId = request.createReceptionDTO.ServiceTypeId
                };

                _context.Receptions.Add(reception);
                await _context.SaveChangesAsync(cancellationToken);

                // Handle exam fee service
                var serviceRequest = await _hospitalService.GetServicesByServiceCodeAsync(
                    new List<string> { ServiceCodeConsts.EXAM_FEE_SERVICE_CODE }, cancellationToken);

                var serviceExamFee = serviceRequest.FirstOrDefault(m => m.ServiceCode == ServiceCodeConsts.EXAM_FEE_SERVICE_CODE);

                if (serviceExamFee != null)
                {
                    var exists = await _context.ServiceRequestDetails
                        .AnyAsync(d => d.ServiceId == serviceExamFee.Id && d.ReceptionId == reception.Id, cancellationToken);

                    if (!exists)
                    {
                        var detail = new ServiceRequestDetail
                        {
                            RequestNumber = UniqueStringGenerator.GenerateUniqueString(),
                            ReceptionId = reception.Id,
                            ServiceId = serviceExamFee.Id,
                            Quantity = 1,
                            UnitPrice = serviceExamFee.UnitPrice
                        };

                        await _context.ServiceRequestDetails.AddAsync(detail, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                }

                var previousReception = await _context.Receptions
                    .Where(r => r.PatientId == patientId && r.Id < reception.Id)
                    .OrderByDescending(r => r.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (previousReception != null)
                {
                    var paidVaccinationIds = await _context.ReceptionVaccinations
                        .Where(rv => rv.ReceptionId == previousReception.Id
                            && rv.PaymentStatus == PaymentStatusForItem.Paid
                            && rv.AppointmentDate >= reception.ReceptionDate
                            && !rv.HasIssue)
                        .Select(rv => rv.Id)
                        .ToListAsync(cancellationToken);

                    if (paidVaccinationIds.Any())
                    {
                        var vaccinations = await _context.Vaccinations
                            .Where(v => paidVaccinationIds.Contains(v.ReceptionVaccinationId))
                            .ToListAsync(cancellationToken);

                        var paidVaccinations = await _context.ReceptionVaccinations
                            .Where(rv => paidVaccinationIds.Contains(rv.Id))
                            .ToListAsync(cancellationToken);

                        foreach (var paidVaccination in paidVaccinations)
                        {
                            var relatedVaccinations = vaccinations
                                .Where(v => v.ReceptionVaccinationId == paidVaccination.Id)
                                .ToList();

                            var vaccinationCountMatches = relatedVaccinations.Count != paidVaccination.Quantity;
                            var notAllConfirmed = !relatedVaccinations.Any() || relatedVaccinations.Any(v => !v.IsConfirmed);

                            if (vaccinationCountMatches && notAllConfirmed)
                            {
                                paidVaccination.SecondaryReceptionId = reception.Id;
                            }
                        }

                        if (paidVaccinations.Any(pv => pv.SecondaryReceptionId == reception.Id))
                        {
                            await _context.SaveChangesAsync(cancellationToken);
                            _logger.LogInformation("Moved {Count} outstanding vaccination schedules from reception {PreviousReceptionId} to new reception {NewReceptionId}",
                                paidVaccinations.Count(pv => pv.SecondaryReceptionId == reception.Id), previousReception.Id, reception.Id);
                        }
                    }
                }

                return new CreatePatientReceptionResult(patientId, reception.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling CreatePatientReceptionCommand");
                throw;
            }
        }
    }
}