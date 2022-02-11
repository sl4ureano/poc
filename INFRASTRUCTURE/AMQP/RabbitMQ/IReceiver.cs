using System;
using System.Collections.Generic;
using System.Text;

namespace INFRASTRUCTURE.rabbitMQ
{
    public interface IReceiver
    {
        void Connect(string connectionName);
        void Receive(string queueName, Action<string> func, bool isDurable);
        void Receive(string queueName, string exchange, string type, List<string> routingKeys, Action<string> func, bool isDurable);
    
    }
}
