using INFRASTRUCTURE.Consumer;
using INFRASTRUCTURE.CONSUMERS;
using INFRASTRUCTURE.rabbitMQ;
using INFRASTRUCTURE.rabbitMQ.impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IOC
{
    public class DIContainer
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {


            // Infra
            services.AddScoped<IReceiver, Receiver>();
            services.AddScoped<IRabbitMQConnection, RabbitMQConnection>();
            services.AddScoped<ISender, Sender>();
            services.AddScoped<IHubConsumer, HubConsumer>();
            services.AddScoped<IHubSender, HubSender>();

        }
    }
}