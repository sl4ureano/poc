using INFRASTRUCTURE.Consumer;
using INFRASTRUCTURE.rabbitMQ;
using System;
using System.Collections.Generic;
using System.Text;

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
            this.sender.SendObj("HubTwo", "ha.save.cotacao", msg,true);
        }


    }
}
