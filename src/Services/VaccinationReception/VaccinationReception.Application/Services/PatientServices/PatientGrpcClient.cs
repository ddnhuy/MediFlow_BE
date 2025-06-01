using BuildingBlocks.Exceptions;
using BuildingBlocks.Pagination;
using CustomerInfo.Grpc.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Application.Patients.Commands;
using VaccinationReception.Application.Patients.Commands.CreatePatient;
using VaccinationReception.Application.Patients.Commands.DeletePatient;
using VaccinationReception.Application.Patients.Commands.UpdatePatient;
using VaccinationReception.Infrastructure.Helpers;
using static VaccinationReception.Application.Const.LogMessages;

namespace VaccinationReception.Application.Services.PatientServices
{
    public class PatientGrpcClient : IPatientGrpcClient
    {
        private readonly PatientProtoService.PatientProtoServiceClient _client;
        private readonly ICurrentUserHelper _currentUserHelper;
        private readonly ILogger<PatientGrpcClient> _logger;
        private readonly Metadata _metadata;

        public PatientGrpcClient(
            PatientProtoService.PatientProtoServiceClient client,
            ILogger<PatientGrpcClient> logger,
            ICurrentUserHelper currentUserHelper)
        {
            _client = client;
            _logger = logger;
            _currentUserHelper = currentUserHelper;
            _metadata = new Metadata
            {
                { "user-id", _currentUserHelper.UserId.ToString() }
            };
        }

        public async Task<PaginatedResult<PatientSummaryDTO>> ListPatientsAsync(PaginationRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(PatientLogMessages.ListPatients_SendingRequest, request.PageIndex, request.PageSize);
                
                var grpcRequest = new ListPatientsRequest
                {
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize
                };

                var response = await _client.ListPatientsAsync(grpcRequest, _metadata, cancellationToken: cancellationToken);

                _logger.LogInformation(PatientLogMessages.ListPatients_Received, response.Data.Count);

                return new PaginatedResult<PatientSummaryDTO>(
                    response.PageIndex,
                    response.PageSize,
                    response.TotalItem,
                    response.Data.Adapt<List<PatientSummaryDTO>>()
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, PatientLogMessages.ListPatients_Error);
                throw;
            }
        }

        public async Task<PatientDetailDTO> GetPatientAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid patient ID", nameof(id));
            }

            try
            {
                _logger.LogInformation(PatientLogMessages.GetPatient_SendingRequest, id);

                var request = new GetPatientRequest { Id = id };
                var response = await _client.GetPatientAsync(request, _metadata, cancellationToken: cancellationToken);

                _logger.LogInformation(PatientLogMessages.GetPatient_Success, id);

                return response.Adapt<PatientDetailDTO>();
            }
            catch (RpcException rpcEx) when (rpcEx.StatusCode == StatusCode.NotFound)
            {
                throw new NotFoundException("Patient", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, PatientLogMessages.GetPatient_Error, id);
                throw;
            }
        }


        public async Task<CreatePatientResult> CreatePatientAsync(CreatePatientCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var request = command.Adapt<CreatePatientRequest>();
                var response = await _client.CreatePatientAsync(request, _metadata, cancellationToken: cancellationToken);

                if(response is null)
                {
                    _logger.LogError(PatientLogMessages.CreatePatient_Error);
                    throw new InternalServerException("Tạo bệnh nhân thất bại");
                }
                _logger.LogInformation(PatientLogMessages.CreatePatient_Success, response.Id);

                return new CreatePatientResult(response.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, PatientLogMessages.CreatePatient_Error);
                throw;
            }
        }

        public async Task<UpdatePatientResult> UpdatePatientAsync(UpdatePatientCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(PatientLogMessages.UpdatePatient_SendingRequest, command.Id);

                var request = command.Adapt<UpdatePatientRequest>();
                var response = await _client.UpdatePatientAsync(request, _metadata, cancellationToken: cancellationToken);

                _logger.LogInformation(PatientLogMessages.UpdatePatient_Success, command.Id);

                return new UpdatePatientResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, PatientLogMessages.UpdatePatient_Error, command.Id);
                throw;
            }
        }

        public async Task<DeletePatientResult> DeletePatientAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(PatientLogMessages.DeletePatient_SendingRequest, id);

                var request = new DeletePatientRequest { Id = id };
                var response = await _client.DeletePatientAsync(request, _metadata, cancellationToken: cancellationToken);

                _logger.LogInformation(PatientLogMessages.DeletePatient_Success, id);

                return new DeletePatientResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, PatientLogMessages.DeletePatient_Error, id);
                throw;
            }
        }
    }
}