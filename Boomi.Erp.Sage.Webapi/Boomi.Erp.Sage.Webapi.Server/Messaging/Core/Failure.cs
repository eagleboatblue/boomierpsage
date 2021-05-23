using Newtonsoft.Json;

namespace Boomi.Erp.Sage.Webapi.Server.Messaging.Core
{
    public class Failure
    {
        [JsonProperty("flow", Required = Required.Always)]
        public string Flow { get; private set; }

        [JsonProperty("error", Required = Required.Always)]
        public Error Error { get; private set; }

        [JsonProperty("initial", Required = Required.Always)]
        public ErpRequest<dynamic> Initial { get; private set; }

        [JsonProperty("current", Required = Required.AllowNull)]
        public ErpRequest<dynamic> Current { get; private set; }

        public Failure(Error error, ErpRequest<dynamic> initial) : this(error, initial, null) { }

        public Failure(Error error, ErpRequest<dynamic> initial, ErpRequest<dynamic> current)
        {
            this.Flow = "erp.sage";
            this.Error = error;
            this.Initial = initial;
            this.Current = current;
        }
    }
}
