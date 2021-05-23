using System;
using Newtonsoft.Json;

namespace Boomi.Erp.Sage.Webapi.Models.Orders
{
    public class Shipping
    {
        [JsonProperty("method", Required = Required.Default)]
        public string Method { get; set; }

        [JsonProperty("terms", Required = Required.Always)]
        public string Terms { get; set; }

        [JsonProperty("termsName", Required = Required.Default)]
        public string TermsName { get; set; }

        [JsonProperty("partial", Required = Required.Always)]
        public string Partial { get; set; }
    }
}
