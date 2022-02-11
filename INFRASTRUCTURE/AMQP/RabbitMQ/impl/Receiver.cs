using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace INFRASTRUCTURE.rabbitMQ.impl
{
    public class Receiver : IReceiver
    {
        private readonly IRabbitMQConnection rabbitMQConnection;
        private IConnection connection;

        public Receiver(IRabbitMQConnection rabbitMQConnection)
        {
            this.rabbitMQConnection = rabbitMQConnection;
        }

        public void Connect(string connectionName)
        {
            connection = rabbitMQConnection.ConConnect(connectionName);
        }

        public void Receive(string queueName, Action<string> func, bool isDurable)
        {
            var channel = rabbitMQConnection.QueueDeclare(connection, queueName, isDurable);
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                try
                {
                    Log.Information($"Recebendo na fila '{queueName}' a mensagem: {message}");
                    func.Invoke(message);
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Erro no recebimento da mensagem. QueueName: {queueName}. Mensagem: {ea.Body.ToString()}. Erro: {e.Message}");
                    throw;
                }
                finally
                {
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);
        }

        public void Receive(string queueName, string exchange, string type, List<string> routingKeys, Action<string> func, bool isDurable)
        {
            var channel = rabbitMQConnection.QueueDeclare(connection, queueName, isDurable);

            channel.ExchangeDeclare(exchange: exchange,
                                    type: type, durable: true);

            foreach (var key in routingKeys)
            {
                channel.QueueBind(queue: queueName, exchange: exchange, routingKey: key);
            }

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                try
                {
                    Log.Information($"Recebendo na fila '{queueName}' a mensagem: {message}");

                    func.Invoke(message);
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Erro no recebimento da mensagem. QueueName: {queueName}. Mensagem: {ea.Body.ToString()}. Erro: {e.Message}");
                    throw;
                }
                finally
                {
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);
        }
    }
}