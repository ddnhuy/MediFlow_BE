using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace HospitalFee.FunctionalTests.DataTest
{
    public static class TestDataFactory
    {
        public static async Task<Payment> SeedPaidPaymentScenarioAsync(IApplicationDbContext dbContext)
        {
            var reception = new Reception
            {
                Id = 1,
                PatientId = 1,
                ReceptionDate = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Unspecified),
                ServiceTypeId = 1,
            };

            var serviceRequestDetail = new ServiceRequestDetail
            {
                Id = 1,
                ServiceId = 101,
                ReceptionId = 1,
                RequestNumber = "REQ-001"
            };

            var paymentDetail = new PaymentDetail
            {
                Id = 1,
                PaymentId = 1,
                ServiceRequestDetailId = 1,
                ServiceRequestDetail = serviceRequestDetail
            };

            var payment = new Payment
            {
                Id = 1,
                ReceptionId = 1,
                Method = PaymentMethod.Cash,
                PaymentDetails = new List<PaymentDetail> { paymentDetail }
            };

            // By adding the parent entities, EF Core's change tracker
            // will automatically add the related child entities.
            dbContext.Receptions.Add(reception);
            dbContext.Payments.Add(payment);

            await dbContext.SaveChangesAsync(CancellationToken.None);

            return payment;
        }
        public static async Task<(Reception reception, Payment payment, ServiceRequestDetail unpaidService)> SeedScenarioForCancellingUnpaidItemAsync(IApplicationDbContext dbContext)
        {
            var reception = new Reception { Id = 2, PatientId = 2, ServiceTypeId = 1 };

            //var requestForm = new RequestForm { Id = 2, Reception = reception, RequestNumber = "REQ-002" };

            var originalPayment = new Payment { Id = 2, Reception = reception, Method = PaymentMethod.Cash, Status = PaymentStatus.Completed, TotalAmount = 0 };

            var unpaidService = new ServiceRequestDetail { Id = 12, Reception = reception, ServiceId = 102, PaymentStatus = PaymentStatusForItem.NotPaid, UnitPrice = 50, Quantity = 1, RequestNumber = "REQ-002"};

            // Add aggregate roots. EF Core will track the rest.
            dbContext.Receptions.Add(reception);
            dbContext.Payments.Add(originalPayment);
            dbContext.ServiceRequestDetails.Add(unpaidService);

            await dbContext.SaveChangesAsync(CancellationToken.None);

            return (reception, originalPayment, unpaidService);
        }
    }
}
