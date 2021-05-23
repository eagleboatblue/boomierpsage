using Newtonsoft.Json;

namespace Boomi.Erp.Sage.Webapi.Models.Common
{
    public class SageRequest
    {
        [JsonProperty("data", Required = Required.Always)]
        public Data Data { get; set; }

        [JsonProperty("metadata", Required = Required.Always)]
        public Metadata Metadata { get; set; }
    }
}
