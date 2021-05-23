using System;
using Newtonsoft.Json;

namespace Boomi.Erp.Sage.Webapi.Models.Common
{
    public class Contact
    {
        [JsonProperty("email", Required = Required.Default)]
        public string Email { get; set; }

        [JsonProperty("name", Required = Required.Default)]
        public string Name { get; set; }
    }
}
