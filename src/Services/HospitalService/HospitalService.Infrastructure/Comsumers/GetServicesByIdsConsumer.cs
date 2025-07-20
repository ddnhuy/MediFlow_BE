using BuildingBlocks.Messaging.Contracts.HospitalService;
using BuildingBlocks.Messaging.Contracts.HospitalService.GetServicesByIds;
using HospitalService.Application.Services.HospitalServices.Queries;
using HospitalService.Domain.Models;
using Mapster;
using MassTransit;
using MediatR;

namespace HospitalService.Infrastructure.Comsumers
{
    public class GetServicesByIdsConsumer : IConsumer<GetServicesByIdsRequest>
    {
        private readonly IMediator _mediator;

        public GetServicesByIdsConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<GetServicesByIdsRequest> context)
        {
            var request = context.Message;
            var query = new GetServicesByIdsQuery(request.ServiceIds);
            var result = await _mediator.Send(query);

            var response = new GetServicesByIdsResponse
            {
                Services = result.Select(s => new ServiceDTO
                {
                    Id = s.Id,
                    ServiceCode = s.ServiceCode,
                    ServiceName = s.ServiceName,
                    UnitPrice = s.UnitPrice,
                    DepartmentId = s.DepartmentId,
                    ExaminationService = s.ExaminationService,
                    ServiceTestParameters = s.ServiceTestParameters?.Select(e => new ServiceTestParameterDTO
                    {
                        ServiceId = e.ServiceId,
                        StandardValue = e.StandardValue,
                        ParameterName = e.ParameterName,
                        Unit = e.Unit
                    }).ToList(),
                }).ToList()
            };

            await context.RespondAsync(response);
        }
    }
}
