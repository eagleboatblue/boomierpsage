using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boomi.Erp.Sage.Webapi.Models.Orders;

namespace Boomi.Erp.Sage.Webapi.Server.Messaging.Core
{
    public class ErpResponse
    {
        public string TimeStamp { get; set; }

        public string ErpStatus { get; set; }

        public Error Error { get; set; }

        public ErpRequest<Order> ErpRequest { get; set; }
    }
}
