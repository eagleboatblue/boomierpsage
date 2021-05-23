using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boomi.Erp.Sage.Webapi.Domain.Orders;
using Boomi.Erp.Sage.Webapi.Models.Common;
using Boomi.Erp.Sage.Webapi.Models.Orders;
using Boomi.Erp.Sage.Webapi.Server.Interfaces;
using Boomi.Erp.Sage.Webapi.Server.Messaging.Core;
using Serilog;

namespace Boomi.Erp.Sage.Webapi.Server.Handlers
{
    public class OrdersHandler : IHandler<Order, string>
    {
        private readonly ILogger logger;
        private readonly Service service;

        public OrdersHandler(ILogger logger, Service service)
        {
            this.logger = logger;
            this.service = service;
        }

        public async Task<string> Handle(ErpRequest<Order> payload)
        {
            var output = payload.Data.OrderNo;

            await this.service.CreateOrder(payload.Metadata, payload.Data);

            return output;
        }
    }
}
