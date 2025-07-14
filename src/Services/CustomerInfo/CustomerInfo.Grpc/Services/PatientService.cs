using BuildingBlocks.Pagination;
using BuildingBlocks.Strings;
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

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var name = request.Name.Trim().ToLower();
                query = query.Where(p => !string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var code = request.Code.Trim().ToLower();
                query = query.Where(p => !string.IsNullOrEmpty(p.Code) && p.Code.ToLower().Contains(code));
            }

            if (!string.IsNullOrWhiteSpace(request.IdentityCard))
            {
                var identityCard = request.IdentityCard.Trim().ToLower();
                query = query.Where(p => !string.IsNullOrEmpty(p.IdentityCard) && p.IdentityCard.ToLower().Contains(identityCard));
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var phone = request.PhoneNumber.Trim().ToLower();
                query = query.Where(p => !string.IsNullOrEmpty(p.PhoneNumber) && p.PhoneNumber.ToLower().Contains(phone));
            }

            var count = await query.CountAsync(context.CancellationToken);
            _logger.LogInformation(PatientLogMessages.FoundPatients, count);

            var patients = await query
                .OrderByDescending(p => p.Id)
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
            try
            {
                _logger.LogInformation(PatientLogMessages.GettingPatient, request.Id);

                ExtractUserIdFromMetadata(context);

                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsCancelled, context.CancellationToken)
                    ?? throw new RpcException(
                        new Status(
                            StatusCode.NotFound,
                            ExceptionKey.NOT_FOUND_PATIENT_WITH_ID.ToString()
                        )
                    );

                _logger.LogInformation(PatientLogMessages.FoundPatient, patient.Name, patient.Id);

                return patient.Adapt<PatientDetailModel>();
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetPatient with ID {Id}", request.Id);
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
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
                        ExceptionKey.PATIENT_CODE_EXISTS.ToString()));
                }

                throw new RpcException(new Status(StatusCode.Internal, ExceptionKey.FAILED_CREATE_PATIENT.ToString()));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, PatientLogMessages.UnexpectedCreateError, ex.Message);

                throw;
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
                        ExceptionKey.NOT_FOUND_PATIENT_WITH_ID.ToString()));
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
                        ExceptionKey.PATIENT_CODE_EXISTS.ToString()));
                }

                throw new RpcException(new Status(StatusCode.Internal, ExceptionKey.FAILED_UPDATE_PATIENT.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, PatientLogMessages.UnexpectedUpdateError, request.Id, ex.Message);

                throw;
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

        public override async Task<FilteredPatientsResponse> ListPatientsWithIdsAndSearch(
            FilteredPatientsRequest request,
            ServerCallContext context)
        {
            _logger.LogInformation("Fetching patients by IDs with optional search. IDs: {Count}, Search: {SearchTerm}",
                request.PatientIds.Count, request.SearchTerm);

            ExtractUserIdFromMetadata(context);

            var query = _context.Patients
                .Where(p => request.PatientIds.Contains(p.Id) && !p.IsCancelled);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var normalizedSearch = request.SearchTerm.Trim().ToLower();
                query = query.Where(p => p.Code.ToLower().Contains(normalizedSearch));
            }

            var patients = await query.ToListAsync(context.CancellationToken);

            var mapped = patients.Adapt<List<PatientSummaryModel>>();

            _logger.LogInformation("Returning {Count} filtered patients.", mapped.Count);

            return new FilteredPatientsResponse
            {
                Data = { mapped }
            };
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