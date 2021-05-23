using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Boomi.Erp.Sage.Webapi.Models.Orders
{
    public class Product
    {
        [JsonProperty("productNumber", Required = Required.Always)]
        public string ProductNumber { get; set; }

        [JsonProperty("type", Required = Required.Default)]
        public string Type { get; set; }

        [JsonProperty("comment", Required = Required.Default)]
        public string Comment { get; set; }

        [JsonProperty("quantity", Required = Required.Always)]
        public double Quantity { get; set; }

        [JsonProperty("pricePerUnit", Required = Required.Always)]
        public double PricePerUnit { get; set; }

        [JsonProperty("manualDiscountAmt", Required = Required.Default)]
        public double ManualDiscountAmt { get; set; }

        [JsonProperty("cost", Required = Required.Default)]
        public double Cost { get; set; }

        [JsonProperty("partNumber", Required = Required.Default)]
        public string PartNumber { get; set; }

        [JsonProperty("lineNumber", Required = Required.Default)]
        public double LineNumber { get; set; }

        [JsonProperty("warehouseName", Required = Required.Default)]
        public string WareHouseName { get; set; }

        // ######## ####### TECHNICAL DEBT ALERT ######## #######
        // [5/20/20, 2:49:19 PM] Wiggin, Alyssa L:
        // I would put a big old comment in there with ######## ####### IMPORTANT LOGIC
        // bc otherwise if the data changes then it's going to break
        // and someone is going to have to go to code and fix it
        //
        // In the perfect world, property "Type" would be enough to determine
        // whether the given order line is a service or a product.
        // But, we are not living in the perfect world, therefore we have to suffer.
        // And write wonky logic.
        // In the beginning, this method tries its luck and check if the product type equals to "service".
        // It's logical, after all!
        // But probably, it wont. So, it moves forward and check if the product type equals to "other".
        // If so, it extracts first 4 characters of the product part number and checks whether they are all numbers.
        // 4 numbers as a prefix mean we are dealing with the desired service. TADAM! Profit.
        // Otherwise, it's just a meaningless hardware entity, who nobody cares about.
        public bool IsServie()
        {
            var productType = this.Type.ToLowerInvariant();

            if (productType == "service")
            {
                return true;
            }

            // wanky logic
            if (productType == "other")
            {
                var prefix = this.ProductNumber.Substring(0, 4);

                // If the prefix consists of numbers, it's a service
                return Regex.IsMatch(prefix, "^[0-9]*$");
            }

            return false;
        }
    }
}
