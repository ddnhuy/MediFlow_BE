using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class DeleteReceptionVaccinationsCommandHandler : ICommandHandler<DeleteReceptionVaccinationsCommand, DeleteReceptionVaccinationsResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DeleteReceptionVaccinationsCommandHandler> _logger;

        public DeleteReceptionVaccinationsCommandHandler(
            IApplicationDbContext context,
            ILogger<DeleteReceptionVaccinationsCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DeleteReceptionVaccinationsResult> Handle(DeleteReceptionVaccinationsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var receptionVaccinations = await _context.ReceptionVaccinations
                    .Where(rv => request.ReceptionVaccinationIds.Contains(rv.Id) && !rv.IsCancelled)
                    .ToListAsync(cancellationToken);

                if (!receptionVaccinations.Any())
                {
                    _logger.LogWarning("Không tìm thấy ReceptionVaccination nào để xóa với Ids: {Ids}",
                        string.Join(", ", request.ReceptionVaccinationIds));
                    return new DeleteReceptionVaccinationsResult(false, 0);
                }

                foreach (var vaccination in receptionVaccinations)
                {
                    vaccination.IsCancelled = true;
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Đã xóa thành công {Count} ReceptionVaccination với Ids: {Ids}",
                    receptionVaccinations.Count,
                    string.Join(", ", receptionVaccinations.Select(rv => rv.Id)));

                return new DeleteReceptionVaccinationsResult(true, receptionVaccinations.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa ReceptionVaccination với Ids: {Ids}",
                    string.Join(", ", request.ReceptionVaccinationIds));
                throw;
            }
        }
    }
}