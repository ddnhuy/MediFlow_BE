using BuildingBlocks.Messaging.Contracts.HospitalService;
using BuildingBlocks.Messaging.Contracts.HospitalService.GetServicesByGroup;
using HospitalService.Application.DTOs;
using HospitalService.Application.Services.HospitalServices.Queries;
using MassTransit;
using MediatR;

namespace HospitalService.Infrastructure.Comsumers
{
    public class GetServicesByGroupConsumer : IConsumer<GetServicesByGroupRequest>
    {
        private readonly IMediator _mediator;

        public GetServicesByGroupConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<GetServicesByGroupRequest> context)
        {
            var request = context.Message;
            var query = new GetServicesByGroupQuery(request.GroupId, request.GroupType);
            var result = await _mediator.Send(query);

            var response = new BuildingBlocks.Messaging.Contracts.HospitalService.GetServicesByGroup.GetServicesByGroupResponse
            {
                Services = result.Select(s => new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                {
                    Id = s.Id,
                    ServiceCode = s.ServiceCode,
                    ServiceName = s.ServiceName,
                    UnitPrice = s.UnitPrice,
                    DepartmentId = s.DepartmentId
                }).ToList()
            };

            await context.RespondAsync(response);
        }
    }
}
