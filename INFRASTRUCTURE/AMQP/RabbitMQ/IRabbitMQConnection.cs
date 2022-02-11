using RabbitMQ.Client;

namespace INFRASTRUCTURE.rabbitMQ
{
    public interface IRabbitMQConnection
    {
        IConnection ConConnect(string connectionName);
        (IConnection, IModel) Connect(string connectionName, string queueName, bool isDurable);
        IModel QueueDeclare(IConnection connection, string queueName, bool isDurable);
    }
}
