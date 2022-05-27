using System;
using RabbitMQ.Client;
using System.Text;
using Serilog;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace INFRASTRUCTURE.rabbitMQ.impl
{
    public class Sender: ISender
    {
        private readonly IConfiguration configuration;
        private readonly IRabbitMQConnection rabbitMQConnection;

        public Sender(IConfiguration configuration, IRabbitMQConnection rabbitMQConnection)
        {
            this.configuration = configuration;
            this.rabbitMQConnection = rabbitMQConnection;
        }

        public void SendObj(string connectionName, string queueName, object obj, bool isDurable)
        {
            (var connection, var channel) = rabbitMQConnection.Connect(connectionName, queueName, isDurable);
            try
            {
                Send(channel, queueName, obj, isDurable);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Erro no envio da mensagem. Nao foi possivel converter o objeto em JSON. QueueName: {queueName}. Objeto: {obj.ToString()}. Erro: {e.Message}");
                throw;
            }
            finally
            {
                CloseConnection(connection, channel);
            }
        }


        public void SendObjToExchange(string connectionName, string ExchangeName, string type, List<string> routingKeys, object obj, bool isDurable)
        {
            (var connection, var channel) = rabbitMQConnection.ConnectExchange(connectionName, ExchangeName, type, isDurable);
            try
            {
                SendExchange(channel, ExchangeName, type, routingKeys, obj, isDurable);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Erro no envio da mensagem. Nao foi possivel converter o objeto em JSON. ExchangeName: {ExchangeName}. Objeto: {obj.ToString()}. Erro: {e.Message}");
                throw;
            }
            finally
            {
                CloseConnection(connection, channel);
            }
        }

        public void SendLot<T>(string connectionName, string queueName, IEnumerable<T> message, bool isDurable, int size = 100)
        {
            (var connection, var channel) = rabbitMQConnection.Connect(connectionName, queueName, isDurable);
            try
            {

                var enumeration = message.Select((item, index) => new { index, item })
                    .GroupBy(obj => obj.index / size);
                foreach (var items in enumeration)
                {
                    var loteItems = items.Select(obj => obj.item);
                    try
                    {
                        Send(channel, queueName, loteItems, isDurable);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, $"Erro no envio de um lote. Message: '{JsonConvert.SerializeObject(loteItems)}'. Erro: {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"Erro no envio de mensagens em lote. Messages: {JsonConvert.SerializeObject(message)}. Erro: {e.Message}");
                throw;
            }
            finally
            {
                CloseConnection(connection, channel);
            }
        }

        public void SendLotToExchange<T>(string connectionName, string ExchangeName, string type, List<string> routingKeys, IEnumerable<T> message, bool isDurable, int size = 100)
        {
            (var connection, var channel) = rabbitMQConnection.ConnectExchange(connectionName, ExchangeName, type, isDurable);
            try
            {

                var enumeration = message.Select((item, index) => new { index, item })
                    .GroupBy(obj => obj.index / size);
                foreach (var items in enumeration)
                {
                    var loteItems = items.Select(obj => obj.item);
                    try
                    {
                        SendExchange(channel, ExchangeName,type,routingKeys,loteItems, isDurable);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, $"Erro no envio de um lote. Message: '{JsonConvert.SerializeObject(loteItems)}'. Erro: {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"Erro no envio de mensagens em lote. Messages: {JsonConvert.SerializeObject(message)}. Erro: {e.Message}");
                throw;
            }
            finally
            {
                CloseConnection(connection, channel);
            }
        }

        private void CloseConnection(IConnection connection, IModel channel)
        {
            channel.Close();
            connection.Close();
        }

        private void Send(IModel channel, string queueName, object obj, bool isDurable)
        {
            string message;
            try
            {
                message = JsonConvert.SerializeObject(obj);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Erro no envio da mensagem. Nao foi possivel converter o objeto em JSON. QueueName: {queueName}. Objeto: {obj.ToString()}. Erro: {e.Message}");
                throw;
            }
            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: channel.CreateBasicProperties(),
                                     body: body);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Erro no envio da mensagem. QueueName: {queueName}. Mensagem: {message}. Erro: {e.Message}");
                throw;
            }
        }

        private void SendExchange(IModel channel, string exchange, string type, List<string> routingKeys, object obj, bool isDurable)
        {
            string message;
            try
            {
                message = JsonConvert.SerializeObject(obj);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Erro no envio da mensagem. Nao foi possivel converter o objeto em JSON. ExchangeName: {exchange}. Objeto: {obj.ToString()}. Erro: {e.Message}");
                throw;
            }
            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                foreach (var routingKey in routingKeys)
                {
                    channel.BasicPublish(exchange: exchange,
                     routingKey: routingKey,
                     basicProperties: channel.CreateBasicProperties(),
                     body: body);
                }
            }

            catch (Exception e)
            {
                Log.Error(e, $"Erro no envio da mensagem. ExchangeName: {exchange}. Mensagem: {message}. Erro: {e.Message}");
                throw;
            }
        }

    }
}
