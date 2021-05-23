namespace Boomi.Erp.Sage.Webapi.Models.Common
{
    public class SageResponse
    {
        public string TimeStamp { get; set; }

        public string ErpStatus { get; set; }
       

        public SageRequest BoomiSageRequest { get; set; }
    }
}
