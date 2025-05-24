using CustomerInfo.Grpc.Consts;
using CustomerInfo.Grpc.Database;
using CustomerInfo.Grpc.Helpers;
using CustomerInfo.Grpc.Models;
using CustomerInfo.Grpc.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static CustomerInfo.Grpc.Consts.PatientMessages;

namespace CustomerInfo.Grpc.Services
{
    public class PatientService : PatientProtoService.PatientProtoServiceBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PatientService> _logger;

        public PatientService(ApplicationDbContext context, ILogger<PatientService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task<ListPatientsResponse> ListPatients(ListPatientsRequest request, ServerCallContext context)
        {
            _logger.LogInformation(PatientLogMessages.ListingPatients, request.Keyword, request.PageIndex, request.PageSize);

            var query = _context.Patients
                .Where(x => !x.IsCancelled)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                query = query.Where(p => p.Name.Contains(request.Keyword) || p.Code.Contains(request.Keyword));
            }

            var count = await query.CountAsync();
            _logger.LogInformation(PatientLogMessages.FoundPatients, count);

            var patients = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var data = patients.Adapt<List<PatientSummaryModel>>();

            _logger.LogInformation(PatientLogMessages.ReturningPatients, data.Count, request.PageIndex);

            return new ListPatientsResponse
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Count = count,
                Data = { data }
            };
        }

        public override async Task<PatientDetailModel> GetPatient(GetPatientRequest request, ServerCallContext context)
        {
            _logger.LogInformation(PatientLogMessages.GettingPatient, request.Id);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsCancelled)
                ?? throw new RpcException(
                    new Status(
                        StatusCode.NotFound,
                        string.Format(Messages.PatientNotFound, request.Id)
                    )
                );

            _logger.LogInformation(PatientLogMessages.FoundPatient, patient.Name, patient.Id);

            return patient.Adapt<PatientDetailModel>();
        }

        public override async Task<PatientDetailModel> CreatePatient(CreatePatientRequest request, ServerCallContext context)
        {
            _logger.LogInformation(PatientLogMessages.CreatingPatient, request.Code);

            var patient = request.Adapt<Patient>();

            ValidatePatientModel(patient);

            try
            {
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                _logger.LogInformation(PatientLogMessages.CreatedPatient, patient.Name, patient.Id);

                return patient.Adapt<PatientDetailModel>();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, PatientLogMessages.DbCreateError, dbEx.Message);

                if (DbExceptionHelper.IsDuplicateKeyException(dbEx))
                {
                    throw new RpcException(new Status(StatusCode.AlreadyExists,
                        string.Format(Messages.PatientCodeExists, request.Code)));
                }

                throw new RpcException(new Status(StatusCode.Internal, Messages.CreateError));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, PatientLogMessages.UnexpectedCreateError, ex.Message);

                throw new RpcException(new Status(StatusCode.Internal, Messages.UnexpectedError));
            }
        }

        public override async Task<PatientDetailModel> UpdatePatient(UpdatePatientRequest request, ServerCallContext context)
        {
            _logger.LogInformation(PatientLogMessages.UpdatingPatient, request.Id);

            try
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsCancelled);

                if (patient == null)
                {
                    _logger.LogWarning(PatientLogMessages.PatientNotFoundForUpdate, request.Id);

                    throw new RpcException(new Status(StatusCode.NotFound,
                        string.Format(Messages.PatientNotFound, request.Id)));
                }

                request.Adapt(patient);

                ValidatePatientModel(patient);

                await _context.SaveChangesAsync();

                _logger.LogInformation(PatientLogMessages.UpdatedPatient, patient.Name, patient.Id);

                return patient.Adapt<PatientDetailModel>();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, PatientLogMessages.DbUpdateError, request.Id, dbEx.Message);

                if (DbExceptionHelper.IsDuplicateKeyException(dbEx))
                {
                    throw new RpcException(new Status(StatusCode.AlreadyExists,
                        string.Format(Messages.PatientCodeExists, request.Code)));
                }

                throw new RpcException(new Status(StatusCode.Internal, Messages.UpdateError));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, PatientLogMessages.UnexpectedUpdateError, request.Id, ex.Message);

                throw new RpcException(new Status(StatusCode.Internal, Messages.UnexpectedError));
            }
        }

        public override async Task<DeletePatientResponse> DeletePatient(DeletePatientRequest request, ServerCallContext context)
        {
            _logger.LogInformation(PatientLogMessages.DeletingPatient, request.Id);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsCancelled);

            if (patient == null)
            {
                _logger.LogWarning(PatientLogMessages.PatientNotFoundForDelete, request.Id);
                return new DeletePatientResponse { IsSuccess = false };
            }

            patient.IsCancelled = true;

            await _context.SaveChangesAsync();

            _logger.LogInformation(PatientLogMessages.DeletedPatient, patient.Name, patient.Id);

            return new DeletePatientResponse { IsSuccess = true };
        }

        private void ValidatePatientModel(Patient patient)
        {
            var validationContext = new ValidationContext(patient);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(patient, validationContext, validationResults, validateAllProperties: true);

            if (!isValid && validationResults.Count > 0)
            {
                var firstError = validationResults[0].ErrorMessage ?? "Validation failed";
                throw new RpcException(new Status(StatusCode.InvalidArgument, firstError));
            }
        }
    }
}