using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgenticPipeline.Infrastructure.Messaging;

public static class KafkaConsumerSetup
{
    public static IServiceCollection AddPipelineMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(cfg =>
        {
            cfg.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", host =>
                {
                    host.Username(configuration["RabbitMQ:Username"] ?? "guest");
                    host.Password(configuration["RabbitMQ:Password"] ?? "guest");
                });

                rabbit.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
