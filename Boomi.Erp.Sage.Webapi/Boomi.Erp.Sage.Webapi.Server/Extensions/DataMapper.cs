using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boomi.Erp.Sage.Webapi.Models.Orders;

namespace Boomi.Erp.Sage.Webapi.Server.Extensions
{
    public static class DataMapper
    {
        public static Order ToOrderMap(Boomi.Erp.Sage.Webapi.Models.Common.Data data)
        {
            var crmOrder = new Order
            {
                OrderNo = data.OrderNo,
                Address = data.Address,
                Comments = data.Comments,
                Contact = data.Contact,
                RequestContact = data.RequestContact,
                CustomerOrderNo = data.CustomerOrderNo,
                DatePromised = Convert.ToDateTime(data.DatePromised),
                DateReceived = Convert.ToDateTime(data.DateReceived),
                DateRequired = Convert.ToDateTime(data.DateRequired),
                ErpAccountNo = data.ErpAccountNo,
                ExworksLocation = data.ExworksLocation,
                OrderAlphaCode = data.OrderAlphaCode,
                RevisionNo = data.RevisionNo,
                TandCNotes = data.TandCNotes,
                TermsConditions = data.TermsConditions,
                LocationName = data.LocationName,
                OwnerName = data.OwnerName,
                QuoteNo = data.QuoteNo,
                Shipping = data.Shipping,
                Payment = data.Payment,
                Totals = data.Totals,
                Products = data.Products
            };

            return crmOrder;
        }
    }
}
