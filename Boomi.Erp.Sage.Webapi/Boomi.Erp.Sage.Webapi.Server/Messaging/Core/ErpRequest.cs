using Boomi.Erp.Sage.Webapi.Models.Common;
using Newtonsoft.Json;

namespace Boomi.Erp.Sage.Webapi.Server.Messaging.Core
{
    public class ErpRequest<T>
    {
        [JsonProperty("metadata", Required = Required.Always)]
        public Metadata Metadata { get; private set; }

        [JsonProperty("data", Required = Required.Always)]
        public T Data { get; private set; }

        public ErpRequest(Metadata metadata, T data)
        {
            this.Metadata = metadata;
            this.Data = data;
        }
    }
}
