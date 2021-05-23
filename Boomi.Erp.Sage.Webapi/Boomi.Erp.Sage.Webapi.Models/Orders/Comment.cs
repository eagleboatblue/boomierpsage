using System;
using Newtonsoft.Json;

namespace Boomi.Erp.Sage.Webapi.Models.Orders
{
    public class Comment
    {
        [JsonProperty("groupDescription", Required = Required.Default)]
        public string GroupDescription { get; set; }

        [JsonProperty("description", Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; set; }
    }
}
