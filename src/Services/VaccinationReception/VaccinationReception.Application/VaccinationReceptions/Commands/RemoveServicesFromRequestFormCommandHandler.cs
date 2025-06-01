using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Infrastructure.Data;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class RemoveServicesFromRequestFormCommandHandler : ICommandHandler<RemoveServicesFromRequestFormCommand, RemoveServicesFromRequestFormResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveServicesFromRequestFormCommand> _logger;

        public RemoveServicesFromRequestFormCommandHandler(
            ApplicationDbContext context,
            ILogger<RemoveServicesFromRequestFormCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RemoveServicesFromRequestFormResult> Handle(RemoveServicesFromRequestFormCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var reception = await _context.Receptions
                    .FirstOrDefaultAsync(r => r.Id == request.ReceptionId, cancellationToken);

                if (reception == null)
                    throw new InvalidOperationException("Reception không tồn tại");

                var requestForm = await _context.RequestForms
                    .FirstOrDefaultAsync(rf => rf.ReceptionId == request.ReceptionId, cancellationToken);

                if (requestForm == null)
                    throw new InvalidOperationException("Không tìm thấy phiếu yêu cầu cho lần tiếp nhận này");

                var serviceRequestDetails = await _context.ServiceRequestDetails
                    .Where(srd => srd.RequestFormId == requestForm.Id && request.ServiceIds.Contains(srd.ServiceId))
                    .ToListAsync(cancellationToken);

                if (!serviceRequestDetails.Any())
                    throw new InvalidOperationException("Không tìm thấy dịch vụ cần xóa");

                foreach (var detail in serviceRequestDetails)
                {
                    detail.IsCancelled = true;
                    detail.LastUpdatedAt = DateTime.UtcNow;
                }

                var activeServices = await _context.ServiceRequestDetails
                    .AnyAsync(srd => srd.RequestFormId == requestForm.Id && !srd.IsCancelled, cancellationToken);

                if (!activeServices)
                {
                    requestForm.IsCancelled = true;
                    requestForm.LastUpdatedAt = DateTime.UtcNow;
                    _logger.LogInformation("All services cancelled, marking request form {RequestFormId} as cancelled", requestForm.Id);
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Cancelled {Count} services from request form {RequestFormId} for reception {ReceptionId}",
                    serviceRequestDetails.Count, requestForm.Id, request.ReceptionId);

                return new RemoveServicesFromRequestFormResult(requestForm.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling RemoveServicesFromRequestFormCommand");
                throw;
            }
        }
    }
}