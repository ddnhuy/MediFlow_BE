using BuildingBlocks.Messaging.Contracts.HospitalService;
using BuildingBlocks.Messaging.Contracts.HospitalService.GetServiceByServiceCodes;
using BuildingBlocks.Messaging.Contracts.HospitalService.GetServicesByGroup;
using HospitalService.Application.Services.HospitalServices.Queries;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Infrastructure.Comsumers
{
    public class GetServicesByServiceCodeConsumer : IConsumer<GetServiceByServiceCode>
    {
        private readonly IMediator _mediator;

        public GetServicesByServiceCodeConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<GetServiceByServiceCode> context)
        {
            var request = context.Message;
            var query = new GetServicesByServiceCodesQuery(request.ServiceCodes);
            var result = await _mediator.Send(query);

            var response = result.Select(s => new ServiceDTO
            {
                Id = s.Id,
                ServiceCode = s.ServiceCode,
                ServiceName = s.ServiceName,
                UnitPrice = s.UnitPrice,
                DepartmentId = s.DepartmentId
            }).ToList();

            await context.RespondAsync(new GetServicesByServiceCodeResponse()
            {
                Services = response
            });
        }
    }
}