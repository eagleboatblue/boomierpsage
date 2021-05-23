using System;
using Newtonsoft.Json;

namespace Boomi.Erp.Sage.Webapi.Models.Common
{
    public class Address
    {
        [JsonProperty("line1", Required = Required.Default)]
        public string Line1 { get; set; }

        [JsonProperty("line2", Required = Required.Default)]
        public string Line2 { get; set; }

        [JsonProperty("line3", Required = Required.Default)]
        public string Line3 { get; set; }

        [JsonProperty("city", Required = Required.Default)]
        public string City { get; set; }

        [JsonProperty("state", Required = Required.Default)]
        public string State { get; set; }

        [JsonProperty("country", Required = Required.Default)]
        public string Country { get; set; }
    }
}
