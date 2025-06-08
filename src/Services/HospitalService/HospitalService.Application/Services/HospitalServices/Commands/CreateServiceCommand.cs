using BuildingBlocks.CQRS;
using HospitalService.Domain.Models;
using HospitalService.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public record CreateServiceCommand(
        string ServiceCode,
        string ServiceName,
        decimal UnitPrice,
        int DepartmentId
    ) : ICommand<CreateServiceResult>;
    public record CreateServiceResult(int ServiceId);

    public class CreateServiceCommandHandler : ICommandHandler<CreateServiceCommand, CreateServiceResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateServiceCommand> _logger;

        public CreateServiceCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateServiceCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CreateServiceResult> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var service = new Service
                {
                    ServiceCode = request.ServiceCode,
                    ServiceName = request.ServiceName,
                    UnitPrice = request.UnitPrice,
                    DepartmentId = request.DepartmentId,
                };

                _context.Services.Add(service);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created new service with ID {ServiceId}", service.Id);
                return new CreateServiceResult(service.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating service");
                throw;
            }
        }
    }
}