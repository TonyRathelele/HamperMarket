using HamperMarket.Models;

namespace HamperMarket.Services
{
    /// <summary>
    /// Simulates South Africa's PAXI (door-to-store / store-to-store parcel service, run through
    /// PEP stores) and PEP courier options. Prices, ETAs and store lists here are illustrative demo
    /// data only - this does NOT call any real PEP/PAXI API.
    /// </summary>
    public class ShippingService
    {
        public List<ShippingOption> GetOptions()
        {
            return new List<ShippingOption>
            {
                new ShippingOption
                {
                    Carrier = ShippingCarrier.Paxi,
                    ServiceName = "PAXI Standard",
                    Description = "Collect from your chosen PEP/PAXI point in store.",
                    EtaText = "5-9 business days",
                    Cost = 65.00m
                },
                new ShippingOption
                {
                    Carrier = ShippingCarrier.Paxi,
                    ServiceName = "PAXI Express",
                    Description = "Priority collection from your chosen PEP/PAXI point in store.",
                    EtaText = "2-3 business days",
                    Cost = 109.95m
                },
                new ShippingOption
                {
                    Carrier = ShippingCarrier.Pep,
                    ServiceName = "PEP Door-to-Door",
                    Description = "Courier delivery straight to your address.",
                    EtaText = "3-5 business days",
                    Cost = 149.00m
                }
            };
        }

        public ShippingOption? GetOption(ShippingCarrier carrier, string serviceName) =>
            GetOptions().FirstOrDefault(o => o.Carrier == carrier && o.ServiceName == serviceName);

        public List<ShippingPoint> GetPickupPoints()
        {
            // Demo set of PEP/PAXI store points across major South African centres.
            return new List<ShippingPoint>
            {
                new ShippingPoint { Name = "PEP Sandton City", Address = "Sandton City Shopping Centre, Rivonia Rd", City = "Johannesburg" },
                new ShippingPoint { Name = "PEP Maponya Mall", Address = "Chris Hani Rd, Klipspruit West", City = "Soweto" },
                new ShippingPoint { Name = "PEP Cape Town CBD", Address = "89 Strand St", City = "Cape Town" },
                new ShippingPoint { Name = "PEP Bellville", Address = "Voortrekker Rd, Bellville", City = "Cape Town" },
                new ShippingPoint { Name = "PEP Durban Workshop", Address = "99 Aliwal St, The Workshop", City = "Durban" },
                new ShippingPoint { Name = "PEP Gateway", Address = "1 Palm Blvd, Umhlanga", City = "Durban" },
                new ShippingPoint { Name = "PEP Menlyn Park", Address = "Atterbury Rd, Menlyn", City = "Pretoria" },
                new ShippingPoint { Name = "PEP Bloemfontein Mimosa Mall", Address = "Kenneth Kaunda Rd", City = "Bloemfontein" },
                new ShippingPoint { Name = "PEP Gqeberha Greenacres", Address = "Cape Rd, Greenacres", City = "Gqeberha" },
                new ShippingPoint { Name = "PEP Polokwane Mall", Address = "Grobler St", City = "Polokwane" }
            };
        }

        public string GenerateTrackingNumber(ShippingCarrier carrier)
        {
            var prefix = carrier == ShippingCarrier.Paxi ? "PAXI" : "PEP";
            var rand = new Random();
            return $"{prefix}-{rand.Next(100000, 999999)}ZA";
        }
    }
}
