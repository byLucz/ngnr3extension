using AviaServerExtension.ASE.Models;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.Platform.WebClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AviaServerExtension.ASE
{
    public sealed class ASEServiceImpl : IASEService
    {
        private const string Token = "b165d8c4be5500d4da61df5067fd34ad";
        private const string AvUrl = "https://api.travelpayouts.com/aviasales/v3/prices_for_dates";
        private const string OfficeCode = "LED";

        private readonly HttpClient httpClient;

        public ASEServiceImpl(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IList<TicketParams>> GetTicketsAsync(SessionContext sessionContext, AvRequest model)
        {
            if (sessionContext == null) throw new ArgumentNullException(nameof(sessionContext));

            var oc = sessionContext.ObjectContext;

            if (model.CityRowId == Guid.Empty || !model.DateFrom.HasValue || !model.DateTo.HasValue)
                return Array.Empty<TicketParams>();

            var cityItem = oc.GetObject<BaseUniversalItem>(model.CityRowId);
            if (cityItem == null || cityItem.ItemCard == null)
                return Array.Empty<TicketParams>();

            var cityCard = cityItem.ItemCard;  
            var cityMain = cityCard.MainInfo;
            var iataCodeObj = cityMain["iataCode"];
            var airportCode = iataCodeObj as string;

            if (string.IsNullOrWhiteSpace(airportCode))
                return Array.Empty<TicketParams>();

            var departure = model.DateFrom.Value.ToString("yyyy-MM-dd");
            var ret = model.DateTo.Value.ToString("yyyy-MM-dd");

            var url = $"{AvUrl}?origin={OfficeCode}" +
                      $"&destination={airportCode}" +
                      $"&departure_at={departure}" +
                      $"&return_at={ret}" +
                      $"&unique=false&sorting=price&direct=true" +
                      $"&currency=rub&limit=10&page=1&one_way=false" +
                      $"&token={Token}";

            using var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var result = new List<TicketParams>();

            if (root.TryGetProperty("data", out var dt) && dt.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in dt.EnumerateArray())
                {
                    if (!item.TryGetProperty("airline", out var airlineProp) || !item.TryGetProperty("price", out var priceProp))
                        continue;

                    var airlineName = airlineProp.GetString();
                    if (string.IsNullOrWhiteSpace(airlineName))
                        continue;

                    var price = priceProp.GetDecimal();
                    if (price <= 0) 
                        continue;

                    string flightNumber = null;
                    if (item.TryGetProperty("flight_number", out var flightNumProp))
                    {
                        flightNumber = flightNumProp.GetString();
                    }

                    result.Add(new TicketParams
                    {
                        Airline = airlineName,
                        FlightNum = flightNumber,
                        Price = price
                    });
                }
            }

            return result.OrderBy(x => x.Price).ToList();
        }
    }
}
