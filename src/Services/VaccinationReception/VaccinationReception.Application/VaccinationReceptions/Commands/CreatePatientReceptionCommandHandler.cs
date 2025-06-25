using BuildingBlocks.CQRS;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
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

        public CreatePatientReceptionCommandHandler(
            IPatientGrpcClient patientGrpcClient,
            ILogger<CreatePatientReceptionCommand> logger,
            IApplicationDbContext context)
        {
            _patientGrpcClient = patientGrpcClient;
            _logger = logger;
            _context = context;
        }
        public async Task<CreatePatientReceptionResult> Handle(CreatePatientReceptionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                int patientId;

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

                var reception = new Reception
                {
                    PatientId = patientId,
                    ReceptionDate = request.createReceptionDTO.ReceptionDate,
                    ServiceTypeId = request.createReceptionDTO.ServiceTypeId
                };

                _context.Receptions.Add(reception);
                await _context.SaveChangesAsync(cancellationToken);

                var previousReception = await _context.Receptions
                    .Where(r => r.PatientId == patientId && r.Id < reception.Id)
                    .OrderByDescending(r => r.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (previousReception != null)
                {
                    var unpaidVaccinations = await _context.ReceptionVaccinations
                        .Where(rv => rv.ReceptionId == previousReception.Id
                            && rv.PaymentStatus == PaymentStatusForItem.NotPaid
                            && rv.AppointmentDate >= reception.ReceptionDate)
                        .ToListAsync(cancellationToken);

                    if (unpaidVaccinations.Any())
                    {
                        foreach (var vaccination in unpaidVaccinations)
                        {
                            vaccination.ReceptionId = reception.Id;
                        }

                        await _context.SaveChangesAsync(cancellationToken);
                        _logger.LogInformation("Moved {Count} outstanding vaccination schedules from reception {PreviousReceptionId} to new reception {NewReceptionId}",
                            unpaidVaccinations.Count, previousReception.Id, reception.Id);
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