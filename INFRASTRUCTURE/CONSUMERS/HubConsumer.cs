using DOMAIN.Entities;
using INFRASTRUCTURE.Consumer;
using INFRASTRUCTURE.rabbitMQ;
using Newtonsoft.Json;


namespace INFRASTRUCTURE.CONSUMERS
{
    public class HubConsumer : IHubConsumer
    {
        readonly IHubSender hubsender;
        public HubConsumer(IReceiver receiver, IHubSender hubsender)
        {
            this.hubsender = hubsender;
            receiver.Connect("HubOne");
            receiver.Receive("ha.process.cotacao", ReceiveCotacao, isDurable: true);
        }

        public void ReceiveCotacao(string message)
        {

            //{
            //  "equityId": 1111,
            //  "symbol": "PETR4",
            //  "dateTime": "2022-01-01T00:00:00",
            //  "price": 12.0,
            //  "volume": 0.0
            //}
            var equity = JsonConvert.DeserializeObject<Equity>(message);
            equity.Price = equity.Price / 2;

            hubsender.publishMSG(equity);
        }

    }
}
