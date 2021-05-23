using System;
using Newtonsoft.Json;

namespace Boomi.Erp.Sage.Webapi.Models.Common
{
    public class Target
    {
        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; set; }

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("operation", Required = Required.Always)]
        public string Operation { get; set; }
    }
}
