using INFRASTRUCTURE.Consumer;
using INFRASTRUCTURE.rabbitMQ;

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
            this.sender.SendObj("HubOne", "ha.save.cotacao.processada", msg,true);
        }


    }
}
