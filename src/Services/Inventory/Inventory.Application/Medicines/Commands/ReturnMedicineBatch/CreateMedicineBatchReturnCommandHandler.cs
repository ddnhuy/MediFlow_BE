using BuildingBlocks.Messaging.Contracts.Email;
using BuildingBlocks.Messaging.Enums.BuildingBlocks.Messaging.Enums;
using BuildingBlocks.Strings.Enums;
using Inventory.Application.Configs;
using MassTransit;
using Microsoft.Extensions.Options;
using System.Text;

namespace Inventory.Application.Medicines.Commands.ReturnMedicineBatch
{
    public class CreateMedicineBatchReturnCommandHandler(IApplicationDbContext dbContext, 
        IPublishEndpoint publishEndpoint,
        IOptions<ApprovalUrlConfig> approvalUrlConfig)
        : ICommandHandler<CreateMedicineBatchReturnCommand, CreateMedicineBatchReturnResult>
    {
        public async Task<CreateMedicineBatchReturnResult> Handle(CreateMedicineBatchReturnCommand request, CancellationToken cancellationToken)
        {
            // Handle duplicate return code
            var existingReturn = await dbContext.MedicineBatchReturns
                .FirstOrDefaultAsync(r => r.ReturnCode == request.ReturnCode, cancellationToken);
            if (existingReturn != null)
            {
                throw new BadRequestException(ExceptionKey.DUPLICATE_RETURN_CODE);
            }

            // Validate that all batches exist and are expired
            var batchIds = request.Details.Select(d => d.MedicineBatchId).ToList();
            var batches = await dbContext.MedicineBatches
                .Include(mb => mb.Supplier)
                .Where(x => !x.IsCancelled)
                .Where(mb => batchIds.Contains(mb.Id))
                .ToListAsync(cancellationToken);

            if (batches.Count != request.Details.Count)
            {
                throw new BadRequestException(ExceptionKey.MEDICINE_BATCH_NOT_FOUND);
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // Check if all batches are expired
            var nonExpiredBatches = batches.Where(b => b.ExpiryDate > today).ToList();
            if (nonExpiredBatches.Any())
            {
                throw new BadRequestException(ExceptionKey.CANNOT_RETURN_NON_EXPIRED_BATCHES);
            }

            // Check if any batch is already returned
            var alreadyReturnedBatches = batches.Where(b => b.Status == MedicineBatchStatus.IsReturned).ToList();
            if (alreadyReturnedBatches.Any())
            {
                throw new BadRequestException(ExceptionKey.BATCH_ALREADY_RETURNED);
            }

            // Check if any batch is already in a pending or approved return request
            var batchIdsInReturnRequests = await dbContext.MedicineBatchReturnDetails
                .Where(d => batchIds.Contains(d.MedicineBatchId))
                .Select(d => d.MedicineBatchReturnId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (batchIdsInReturnRequests.Any())
            {
                var returnRequests = await dbContext.MedicineBatchReturns
                    .Where(r => batchIdsInReturnRequests.Contains(r.Id) &&
                               (r.Status == MedicineBatchReturnStatus.Pending || r.Status == MedicineBatchReturnStatus.Approved))
                    .ToListAsync(cancellationToken);

                if (returnRequests.Any())
                {
                    throw new BadRequestException(ExceptionKey.BATCH_ALREADY_IN_RETURN_REQUEST);
                }
            }

            var supplierIds = batches.Select(b => b.SupplierId).Distinct().ToList();
            if (supplierIds.Count > 1)
            {
                throw new BadRequestException(ExceptionKey.CANNOT_RETURN_BATCHES_FROM_DIFFERENT_SUPPLIERS);
            }


            // Create the return record
            var approvalToken = Guid.NewGuid().ToString();
            var medicineBatchReturn = new MedicineBatchReturn
            {
                ReturnCode = request.ReturnCode,
                Reason = request.Reason,
                ApprovalToken = approvalToken,
                ReceiverName = request.ReceiverName,
                ReceiverEmail = request.ReceiverEmail,
                ReceiverPhone = request.ReceiverPhone,
            };

            await dbContext.MedicineBatchReturns.AddAsync(medicineBatchReturn);
            await dbContext.SaveChangesAsync(cancellationToken);

            // Create return details and update batch status
            foreach (var detail in request.Details)
            {
                // Create return detail
                var returnDetail = new MedicineBatchReturnDetail
                {
                    MedicineBatchReturnId = medicineBatchReturn.Id,
                    MedicineBatchId = detail.MedicineBatchId,
                    BatchNumber = detail.BatchNumber,
                    ExpirationDate = detail.ExpirationDate,
                    Quantity = detail.Quantity
                };

                await dbContext.MedicineBatchReturnDetails.AddAsync(returnDetail);
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            var approvalUrl = string.Format(
                approvalUrlConfig.Value.ApprovalPath,
                medicineBatchReturn.Id,
                medicineBatchReturn.ApprovalToken);
            var rejectionUrl = string.Format(
                approvalUrlConfig.Value.RejectionPath,
                medicineBatchReturn.Id,
                medicineBatchReturn.ApprovalToken);

            var fullApprovalUrl = $"{approvalUrlConfig.Value.BaseUrl}{approvalUrl}";
            var fullRejectionUrl = $"{approvalUrlConfig.Value.BaseUrl}{rejectionUrl}";

            // Send approval email
            await publishEndpoint.Publish(new SendEmailMessage
            {
                To = request.ReceiverEmail,
                SubjectCode = EmailSubjectCode.MedicineBatchReturnApproval,
                TemplateData = new Dictionary<string, string>
                {
                    ["ReceiverName"] = request.ReceiverName,
                    ["ReturnCode"] = request.ReturnCode,
                    ["Reason"] = request.Reason ?? "Không có",
                    ["CreatedDate"] = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm"),
                    ["BatchDetails"] = GenerateBatchDetailsTable(request.Details),
                    ["ApprovalUrl"] = fullApprovalUrl,
                    ["RejectionUrl"] = fullRejectionUrl
                }
            }, cancellationToken);

            return new CreateMedicineBatchReturnResult(medicineBatchReturn.Id);
        }

        private string GenerateBatchDetailsTable(IEnumerable<MedicineBatchReturnDetailDto> details)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<table style='width:100%;border-collapse:collapse;'>");
            sb.AppendLine("<thead><tr>");
            sb.AppendLine("<th style='border:1px solid #ddd;padding:8px;'>Lô hàng</th>");
            sb.AppendLine("<th style='border:1px solid #ddd;padding:8px;'>Ngày hết hạn</th>");
            sb.AppendLine("<th style='border:1px solid #ddd;padding:8px;'>Số lượng</th>");
            sb.AppendLine("</tr></thead><tbody>");
            foreach (var d in details)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td style='border:1px solid #ddd;padding:8px;'>{d.BatchNumber}</td>");
                sb.AppendLine($"<td style='border:1px solid #ddd;padding:8px;'>{d.ExpirationDate:dd/MM/yyyy}</td>");
                sb.AppendLine($"<td style='border:1px solid #ddd;padding:8px;'>{d.Quantity}</td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</tbody></table>");
            return sb.ToString();
        }
    }
}
