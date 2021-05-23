using Newtonsoft.Json;

namespace Boomi.Erp.Sage.Webapi.Models.Orders
{
    public class Totals
    {
        [JsonProperty("detailAmount", Required = Required.Default)]
        public double DetailAmount { get; set; }

        [JsonProperty("amount", Required = Required.Default)]
        public double Amount { get; set; }

        [JsonProperty("discount", Required = Required.Default)]
        public double Discount { get; set; }

        [JsonProperty("discountAmount", Required = Required.Default)]
        public double DiscountAmount { get; set; }

        [JsonProperty("margin", Required = Required.Default)]
        public double Margin { get; set; }

        [JsonProperty("currency", Required = Required.Default)]
        public string Currency { get; set; }
    }
}
