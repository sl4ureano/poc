using RabbitMQ.Client;

namespace INFRASTRUCTURE.rabbitMQ
{
    public interface IRabbitMQConnection
    {
        IConnection ConConnect(string connectionName);
        (IConnection, IModel) Connect(string connectionName, string queueName, bool isDurable);
        IModel QueueDeclare(IConnection connection, string queueName, bool isDurable);
        IModel ExchangeDeclare(IConnection connection, string exchangeName, string type, bool isDurable);
        (IConnection, IModel) ConnectExchange(string connectionName, string ExchangeName, string type, bool isDurable);
    }
}
