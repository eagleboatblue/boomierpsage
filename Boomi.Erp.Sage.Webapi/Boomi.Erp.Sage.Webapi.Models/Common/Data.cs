using System.Collections.Generic;
using Boomi.Erp.Sage.Webapi.Models.Orders;

namespace Boomi.Erp.Sage.Webapi.Models.Common
{
    public class Data
    {
        public string OrderNo { get; set; }

        public string DateReceived { get; set; }

        public string DateRequired { get; set; }

        public string DatePromised { get; set; }

        public string CustomerOrderNo { get; set; }

        public string OrderAlphaCode { get; set; }

        public string ErpAccountNo { get; set; }

        public string RevisionNo { get; set; }

        public string ExworksLocation { get; set; }

        public string TandCNotes { get; set; }

        public string TermsConditions { get; set; }

        public string LocationName { get; set; }

        public string OwnerName { get; set; }

        public string QuoteNo { get; set; }

        public Address Address { get; set; }

        public Shipping Shipping { get; set; }

        public Payment Payment { get; set; }

        public Contact Contact { get; set; }

        public Contact RequestContact { get; set; }

        public Totals Totals { get; set; }

        public List<Product> Products { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
