using BuildingBlocks.Behaviors;
using BuildingBlocks.Messaging.MassTransit;
using CustomerInfo.Grpc.Protos;
using FluentValidation;
using FluentValidation.AspNetCore;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;
using System.Reflection;
using VaccinationReception.Application.Jobs;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<PatientProtoService.PatientProtoServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcSettings:CustomerInfoUrl"]!);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                return handler;
            });

            services.AddGrpcClient<ApplicationUserProtoService.ApplicationUserProtoServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcSettings:HumanResourceUrl"]!);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                return handler;
            });

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddFluentValidationAutoValidation();

            services.TryAddScoped<IPatientGrpcClient, PatientGrpcClient>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddQuartz(q =>
            {
                var jobConfig = configuration.GetSection("QuartzJobs:CleanupUnpaidItemsJob");

                var jobKey = new JobKey(jobConfig.GetValue<string>("JobKey") ?? "CleanupUnpaidItemsJob");

                q.AddJob<CleanupUnpaidItemsJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity(jobConfig.GetValue<string>("Trigger") ?? "CleanupUnpaidItemsJob-trigger")
                    .WithCronSchedule(
                        jobConfig.GetValue<string>("Cron") ?? "0 30 23 * * ?",
                        x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))));
            });

            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            return services;
        }
    }
}