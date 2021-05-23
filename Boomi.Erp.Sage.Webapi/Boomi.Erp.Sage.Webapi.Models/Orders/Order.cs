using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Boomi.Erp.Sage.Webapi.Models.Common;

namespace Boomi.Erp.Sage.Webapi.Models.Orders
{
    public class Order
    {
        [JsonProperty("orderNo", Required = Required.Always)]
        public string OrderNo { get; set; }

        [JsonProperty("dateReceived", Required = Required.Always)]
        public DateTime DateReceived { get; set; }

        [JsonProperty("dateRequired", Required = Required.Always)]
        public DateTime DateRequired { get; set; }

        [JsonProperty("datePromised", Required = Required.Always)]
        public DateTime DatePromised { get; set; }

        [JsonProperty("customerOrderNo", Required = Required.Always)]
        public string CustomerOrderNo { get; set; }

        [JsonProperty("orderAlphaCode", Required = Required.Default)]
        public string OrderAlphaCode { get; set; }

        [JsonProperty("erpAccountNo", Required = Required.Default)]
        public string ErpAccountNo { get; set; }

        [JsonProperty("revisionNo", Required = Required.Always)]
        public string RevisionNo { get; set; }

        [JsonProperty("exworksLocation", Required = Required.Default)]
        public string ExworksLocation { get; set; }

        [JsonProperty("tandCNotes", Required = Required.Default)]
        public string TandCNotes { get; set; }

        [JsonProperty("termsConditions", Required = Required.Always)]
        public string TermsConditions { get; set; }

        [JsonProperty("locationName", Required = Required.Always)]
        public string LocationName { get; set; }

        [JsonProperty("ownerName", Required = Required.Always)]
        public string OwnerName { get; set; }

        [JsonProperty("quoteNo", Required = Required.Default)]
        public string QuoteNo { get; set; }

        [JsonProperty("address", Required = Required.Always)]
        public Address Address { get; set; }

        [JsonProperty("shipping", Required = Required.Always)]
        public Shipping Shipping { get; set; }

        [JsonProperty("payment", Required = Required.Default)]
        public Payment Payment { get; set; }

        [JsonProperty("contact", Required = Required.Always)]
        public Contact Contact { get; set; }

        [JsonProperty("requestContact", Required = Required.Default)]
        public Contact RequestContact { get; set; }

        [JsonProperty("totals", Required = Required.Always)]
        public Totals Totals { get; set; }

        [JsonProperty("products", Required = Required.Always)]
        public List<Product> Products { get; set; }

        [JsonProperty("comments", Required = Required.Always)]
        public List<Comment> Comments { get; set; }
    }
}
