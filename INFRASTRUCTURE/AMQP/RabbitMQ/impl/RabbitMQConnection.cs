using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Serilog;
using System;

namespace INFRASTRUCTURE.rabbitMQ.impl
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConfiguration configuration;

        public RabbitMQConnection(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IConnection ConConnect(string connectionName)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(configuration[$"RabbitMQ:{connectionName}:Uri"])
                };

                var connection = factory.CreateConnection();
                Log.Information($"Conexao criada. connectionName: '{connectionName}'");
                return connection;
            }
            catch (Exception e)
            {
                Log.Error(e, $"Nao foi possivel criar a conexao com rabbitMQ. connectionName: '{connectionName}'. Erro: {e.Message}");
                throw;
            }
        }

        public (IConnection, IModel) Connect(string connectionName, string queueName, bool isDurable)
        {
            var connection = ConConnect(connectionName);
            var channel = QueueDeclare(connection, queueName, isDurable);
            return (connection, channel);
        }

        public IModel QueueDeclare(IConnection connection, string queueName, bool isDurable)
        {
            IModel channel;
            try
            {
                channel = connection.CreateModel();

                channel.QueueDeclare(
                    queue: queueName,
                    durable: isDurable,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                channel.BasicQos(0, 1, false);

                Log.Information($"Fila declarada. queueName: '{queueName}'");
            }
            catch (Exception e)
            {
                Log.Error(e, $"Nao foi possivel declarar queue. queueName: {queueName}. Erro: {e.Message}");
                throw;
            }
            return channel;
        }
    }
}
