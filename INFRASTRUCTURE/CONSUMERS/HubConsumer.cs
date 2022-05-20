using DOMAIN.Entities;
using INFRASTRUCTURE.Consumer;
using INFRASTRUCTURE.rabbitMQ;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace INFRASTRUCTURE.CONSUMERS
{
    public class HubConsumer : IHubConsumer
    {
        readonly IHubSender hubsender;
        public HubConsumer(IReceiver receiver, IHubSender hubsender)
        {
            this.hubsender = hubsender;
            receiver.Connect("HubOne");

            //exemplo direct - recebimento mais comum
            receiver.Receive("ha.process.cotacao", ReceiveCotacao, isDurable: true);

            //exemplo topic - recebe só o que quer
            receiver.Receive("ha.process.cotacao1", "process.cotacao.t", "topic", new List<string>() { "" }, ReceiveCotacao, isDurable: true);
            receiver.Receive("ha.process.cotacao1.dolar", "process.cotacao.t", "topic", new List<string>() { "*.dolar" }, ReceiveCotacaoDolar, isDurable: true);
            receiver.Receive("ha.process.cotacao1.euro","process.cotacao.t", "topic", new List<string>() { "*.euro" }, ReceiveCotacaoEuro, isDurable: true);


            //exemplo fanout - quando publica todos recebem
            receiver.Receive("ha.process.f.cotacao2", "process.cotacao.f", "fanout", new List<string>() { "" }, ReceiveCotacao, isDurable: true);
            receiver.Receive("ha.process.f.cotacao3", "process.cotacao.f", "fanout", new List<string>() { "" }, ReceiveCotacao, isDurable: true);
            receiver.Receive("ha.process.f.cotacao4", "process.cotacao.f", "fanout", new List<string>() { "*" }, ReceiveCotacao, isDurable: true);
            receiver.Receive("ha.process.f.cotacao5", "process.cotacao.f", "fanout", new List<string>() { "*.dolar" }, ReceiveCotacao, isDurable: true);
            receiver.Receive("ha.process.f.cotacao6", "process.cotacao.f", "fanout", new List<string>() { "*.dolar" }, ReceiveCotacao, isDurable: true);
            receiver.Receive("ha.process.f.cotacao7", "process.cotacao.f", "fanout", new List<string>() { "*.euro" }, ReceiveCotacao, isDurable: true);
            receiver.Receive("ha.process.f.cotacao8", "process.cotacao.f", "fanout", new List<string>() { "*.euro" }, ReceiveCotacao, isDurable: true);
        }

        public void ReceiveCotacaoEuro(string message)
        {

            //{
            //    "equityId": 1111,
            //  "symbol": "PETR4",
            //  "dateTime": "2022-01-01T00:00:00",
            //  "price": 12.0,
            //  "volume": 0.0
            //}
            var equity = JsonConvert.DeserializeObject<Equity>(message);
            equity.Price = equity.Price / 2;

            hubsender.publishMSG(equity);
        }

        public void ReceiveCotacaoDolar(string message)
        {

            //{
            //    "equityId": 1111,
            //  "symbol": "PETR4",
            //  "dateTime": "2022-01-01T00:00:00",
            //  "price": 12.0,
            //  "volume": 0.0
            //}
            var equity = JsonConvert.DeserializeObject<Equity>(message);
            equity.Price = equity.Price / 2;

            hubsender.publishMSG(equity);
        }

        public void ReceiveCotacao(string message)
        {

            //{
            //    "equityId": 1111,
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
