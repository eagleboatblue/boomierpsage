using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Boomi.Erp.Sage.Webapi.Models.Common
{
    public class Metadata
    {
        public static Metadata FromJobID(string jobiD)
        {
            return new Metadata
            {
                JobID = jobiD
            };
        }

        [JsonProperty("attributes", Required = Required.Default)]
        public Attributes Attributes { get; set; }

        [JsonProperty("jobID", Required = Required.Default)]
        public string JobID { get; set; }

        [JsonProperty("target", Required = Required.Default)]
        public Target Target { get; set; }

        [JsonProperty("user", Required = Required.Default)]
        public string User { get; set; }
    }
}
