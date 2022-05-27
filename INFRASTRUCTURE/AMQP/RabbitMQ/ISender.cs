using System;
using System.Collections.Generic;
using System.Text;

namespace INFRASTRUCTURE.rabbitMQ
{
    public interface ISender
    {
        void SendObj(string connectionName, string queueName, object obj, bool isDurable);
        void SendLot<T>(string connectionName, string queueName, IEnumerable<T> message, bool isDurable, int size = 100);
        void SendObjToExchange(string connectionName, string ExchangeName, string type, List<string> routingKeys, object obj, bool isDurable);
    }
}
