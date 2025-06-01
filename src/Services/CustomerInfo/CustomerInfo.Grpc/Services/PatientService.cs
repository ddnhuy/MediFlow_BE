using BuildingBlocks.Pagination;
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
        private readonly ICurrentUserHelper _currentUserHelper;

        public PatientService(ApplicationDbContext context, ILogger<PatientService> logger, ICurrentUserHelper currentUserHelper)
        {
            _context = context;
            _logger = logger;
            _currentUserHelper = currentUserHelper;
        }
        private void ExtractUserIdFromMetadata(ServerCallContext context)
        {
            var userIdEntry = context.RequestHeaders.Get("user-id");
            if (userIdEntry != null && int.TryParse(userIdEntry.Value, out int userId))
            {
                _currentUserHelper.SetUserId(userId);
                _logger.LogInformation("User ID set from metadata: {UserId}", userId);
            }
            else
            {
                _logger.LogWarning("User ID not found or invalid in metadata");
            }
        }
        public override async Task<ListPatientsResponse> ListPatients(ListPatientsRequest request, ServerCallContext context)
        {
            _logger.LogInformation(PatientLogMessages.ListingPatients, request.PageIndex, request.PageSize);

            ExtractUserIdFromMetadata(context);

            var query = _context.Patients
                .Where(x => !x.IsCancelled)
                .AsQueryable();

            var count = await query.CountAsync();
            _logger.LogInformation(PatientLogMessages.FoundPatients, count);

            var patients = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(context.CancellationToken);

            var data = patients.Adapt<List<PatientSummaryModel>>();

            _logger.LogInformation(PatientLogMessages.ReturningPatients, data.Count, request.PageIndex);

            return new ListPatientsResponse
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalItem = count,
                Data = { data }
            };
        }

        public override async Task<PatientDetailModel> GetPatient(GetPatientRequest request, ServerCallContext context)
        {
            _logger.LogInformation(PatientLogMessages.GettingPatient, request.Id);

            ExtractUserIdFromMetadata(context);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsCancelled, context.CancellationToken)
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

            ExtractUserIdFromMetadata(context);

            var patient = request.Adapt<Patient>();

            ValidatePatientModel(patient);

            try
            {
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync(context.CancellationToken);

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

            ExtractUserIdFromMetadata(context);

            try
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsCancelled, context.CancellationToken);

                if (patient == null)
                {
                    _logger.LogWarning(PatientLogMessages.PatientNotFoundForUpdate, request.Id);

                    throw new RpcException(new Status(StatusCode.NotFound,
                        string.Format(Messages.PatientNotFound, request.Id)));
                }

                request.Adapt(patient);

                ValidatePatientModel(patient);

                await _context.SaveChangesAsync(context.CancellationToken);

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

            ExtractUserIdFromMetadata(context);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsCancelled, context.CancellationToken);

            if (patient == null)
            {
                _logger.LogWarning(PatientLogMessages.PatientNotFoundForDelete, request.Id);
                return new DeletePatientResponse { IsSuccess = false };
            }

            patient.IsCancelled = true;

            await _context.SaveChangesAsync(context.CancellationToken);

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