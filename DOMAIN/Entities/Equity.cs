using System;

namespace DOMAIN.Entities
{
    public class Equity
    {
        public int EquityId { get; set; }
        public string Symbol { get; set; }
        public DateTime DateTime { get; set; }
        public double Price { get; set; }
        public double Volume { get; set; }

    }
}
