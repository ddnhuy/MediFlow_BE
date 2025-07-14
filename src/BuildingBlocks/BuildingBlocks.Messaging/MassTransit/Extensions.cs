using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Messaging.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMessageBroker(
            this IServiceCollection services,
            IConfiguration configuration,
            Assembly? assembly = null,
            bool useCompetingConsumers = false)
        {
            services.AddMassTransit(config =>
            {
                config.SetKebabCaseEndpointNameFormatter();

                if (assembly != null)
                    config.AddConsumers(assembly);

                config.UsingRabbitMq((context, configurator) =>
                {
                    var host = configuration["MessageBroker:Host"]
                        ?? throw new InvalidOperationException("MessageBroker:Host is not configured.");
                    var username = configuration["MessageBroker:UserName"]
                        ?? throw new InvalidOperationException("MessageBroker:UserName is not configured.");
                    var password = configuration["MessageBroker:Password"]
                        ?? throw new InvalidOperationException("MessageBroker:Password is not configured.");

                    configurator.Host(new Uri(host), host =>
                    {
                        host.Username(username);
                        host.Password(password);
                    });

                    if (useCompetingConsumers && assembly != null)
                    {
                        var consumerTypes = assembly.GetTypes()
                            .Where(t => typeof(IConsumer).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass);

                        foreach (var consumerType in consumerTypes)
                        {
                            var queueName = $"{consumerType.Name.ToLowerInvariant()}-queue";
                            configurator.ReceiveEndpoint(queueName, endpoint =>
                            {
                                endpoint.ConfigureConsumer(context, consumerType);
                            });
                        }
                    }
                    else
                    {
                        configurator.ConfigureEndpoints(context);
                    }
                });
            });

            return services;
        }
    }
}