using System;

namespace AviaServerExtension.ASE.Models
{
    public class AvRequest
    {
        public Guid CardId { get; set; }
        public Guid CityRowId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class TicketParams
    {
        public string Airline { get; set; }
        public string FlightNum { get; set; }
        public decimal Price { get; set; }
    }
}
