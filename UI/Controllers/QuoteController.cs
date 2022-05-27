using DOMAIN.Entities;
using INFRASTRUCTURE.Consumer;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuoteController
    {
        private readonly IHubSender _HubSender;
        public QuoteController(IHubSender HubSender)
        {
            this._HubSender = HubSender;
        }

        [HttpGet]
        public void Get()
        {
            var quote = new Equity() { Price = 10 };
            _HubSender.publishMSG(quote);
        }

    }
}
