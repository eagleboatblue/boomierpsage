using System;
using Newtonsoft.Json;

namespace Boomi.Erp.Sage.Webapi.Models.Orders
{
    public class Payment
    {
        [JsonProperty("termsCode", Required = Required.Always)]
        public string TermsCode { get; set; }

        [JsonProperty("termsNotes", Required = Required.Default)]
        public string TermsNotes { get; set; }

        [JsonProperty("alternativeTermsCode", Required = Required.Default)]
        public string AlternativeTermsCode { get; set; }
    }
}
