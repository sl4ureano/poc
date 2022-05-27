using INFRASTRUCTURE.Consumer;
using INFRASTRUCTURE.rabbitMQ;
using System.Collections.Generic;

namespace INFRASTRUCTURE.CONSUMERS
{
    public class HubSender : IHubSender
    {
        readonly ISender sender;
        public HubSender(ISender sender)
        {
            this.sender = sender;
        }

        public void publishMSG(object msg)
        {
            // publica na fila
            // this.sender.SendObj("HubOne", "ha.save.cotacao.processada", msg,true);

            // publica na Exchange
            this.sender.SendObjToExchange("HubOne", "process.cotacao.t", "topic", new List<string>() { "*.dolar", "*.euro" }, msg, true);
        }


    }
}
