using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.HospitalFees.Queries
{
    public record GetPaymentStatusQuery(int? PaymentId, int? PaymentContractId) : IQuery<PaymentStatus?>;

    public class GetPaymentStatusQueryHandler : IQueryHandler<GetPaymentStatusQuery, PaymentStatus?>
    {
        private readonly IApplicationDbContext _context;

        public GetPaymentStatusQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentStatus?> Handle(GetPaymentStatusQuery request, CancellationToken cancellationToken)
        {
            if (!request.PaymentId.HasValue && !request.PaymentContractId.HasValue)
            {
                throw new BadRequestException(ExceptionKey.INVALID_REQUEST);
            }

            if (request.PaymentContractId.HasValue)
            {
                var paymentContractStatus = await _context.PaymentContracts
                    .Where(pc => pc.Id == request.PaymentContractId.Value)
                    .Select(pc => pc.Status)
                    .FirstOrDefaultAsync(cancellationToken);

                if (paymentContractStatus == null)
                {
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_PAYMENT_CONTRACT_WITH_ID);
                }

                return paymentContractStatus;
            }
            else if (request.PaymentId.HasValue)
            {
                var paymentStatus = await _context.Payments
                    .Where(p => p.Id == request.PaymentId.Value)
                    .Select(p => p.Status)
                    .FirstOrDefaultAsync(cancellationToken);

                if (paymentStatus == null)
                {
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_PAYMENT_WITH_ID);
                }

                return paymentStatus;
            }
            throw new BadRequestException(ExceptionKey.INVALID_REQUEST);
        }
    }
}
