using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Boomi.Erp.Sage.Webapi.Data.Mappers;
using Boomi.Erp.Sage.Webapi.Domain.Orders;
using Boomi.Erp.Sage.Webapi.Models.Common;
using Boomi.Erp.Sage.Webapi.Models.Orders;
using Boomi.Erp.Sage.Webapi.Server.Databases;
using Boomi.Erp.Sage.Webapi.Server.Extensions;
using Boomi.Erp.Sage.Webapi.Server.Handlers;
using Boomi.Erp.Sage.Webapi.Server.Interfaces;
using Boomi.Erp.Sage.Webapi.Server.Messaging.Core;
using Boomi.Erp.Sage.Webapi.Server.Services;
using Serilog;
using Serilog.Formatting.Compact;

namespace Boomi.Erp.Sage.Webapi.Server.Controllers
{
    public class JobController : ApiController
    {
        private readonly ILogger logger;
        private readonly IDbManager dbManager;
        private readonly Service service;

        public JobController()
        {
            this.logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(new CompactJsonFormatter())
                .CreateLogger();
        }

        public JobController(ILogger logger, Service service)
        {
            this.logger = logger;
            this.service = service;
        }

        // POST /job
        public async Task<ErpResponse> Post([FromBody] ErpRequest<Order> erpRequest)
        {
            this.logger.Information("****************************************************************");
            this.logger.Information(DateTime.Now + ": Starting up Boomi Sage service ...");

            if (erpRequest.Metadata == null)
            {
                this.logger.Error(DateTime.Now + ": Missing Metadata. Please verify Boomi request.");
                return null;
            };

            if (erpRequest.Data == null)
            {
                this.logger.Error(DateTime.Now + ": Missing Order data. Please verify Boomi request.");
                return null;
            };

            this.logger.Information(DateTime.Now + ": JobController/Post: Request - {@erpRequest}", erpRequest);


            OrdersHandler ordersHandler = new OrdersHandler(this.logger, this.service);

            var output = await ordersHandler.Handle(erpRequest);
           

            //this.logger.Information(DateTime.Now + ": JobController/Post: Response -: {@response}! ", response);

            return new ErpResponse();
        }
    }
}