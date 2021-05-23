using System;
using System.Linq;
using System.Threading.Tasks;
using Boomi.Erp.Sage.Webapi.Models.Orders;
using Serilog;
using Boomi.Erp.Sage.Webapi.Data;
using Boomi.Erp.Sage.Webapi.Models.Common;
using System.Collections.Generic;

namespace Boomi.Erp.Sage.Webapi.Domain.Orders
{
    public class Service
    {
        private readonly ILogger logger;
        private readonly Manager dbManager;

        public Service(ILogger logger, Manager dbManager)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (dbManager == null)
            {
                throw new ArgumentNullException("dbManager");
            }

            this.logger = logger;
            this.dbManager = dbManager;
        }

        public async Task CreateOrder(Metadata metadata, Order order)
        {

            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }

            if (order == null)
            {
                throw new ArgumentNullException("order");
            }

            Boolean partMissing = false;
            Boolean warehouseMissing = false;
            Boolean quotePresent = false;
            string partNumberMissing = String.Empty;
            string warehouseNameMissing = String.Empty;
            string quotePresentInLine = String.Empty;

            for (int i = 0; i < order.Products.Count(); i++)
            {
                if (order.Products[i].PartNumber == null || order.Products[i].PartNumber == "")
                {
                    partMissing = true;
                    if (String.IsNullOrEmpty(partNumberMissing))
                    {
                        partNumberMissing = (i + 1).ToString();
                    }
                    else
                    {
                        partNumberMissing = partNumberMissing + ", " + (i + 1).ToString();
                    }
                }

                if (order.Products[i].ProductNumber.StartsWith("LSCM"))
                {
                    if (order.Products[i].WareHouseName == null || order.Products[i].WareHouseName == "")
                    {
                        warehouseMissing = true;
                        if (String.IsNullOrEmpty(warehouseNameMissing))
                        {
                            warehouseNameMissing = (i + 1).ToString();
                        }
                        else
                        {
                            warehouseNameMissing = warehouseNameMissing + ", " + (i + 1).ToString();
                        }
                    }
                }
                if (order.Products[i].ProductNumber.Contains("QUO") == true)
                {
                    quotePresent = true;
                    if (String.IsNullOrEmpty(quotePresentInLine))
                    {
                        quotePresentInLine = (i + 1).ToString();
                    }
                    else
                    {
                        quotePresentInLine = quotePresentInLine + ", " + (i + 1).ToString();
                    }
                }
            }

            if (!quotePresent)
            {
                if (partMissing)
                {
                    if (warehouseMissing)
                    {
                        throw new Exception($"Part Number is missing in Product Line '{partNumberMissing}' and Warehouse Name is missing in Product Line '{warehouseNameMissing}'");
                    }
                    throw new Exception($"Part Number is missing in Product Line '{partNumberMissing}'");
                }
                else if (warehouseMissing)
                {
                    throw new Exception($"Warehouse Name is missing in Product Line '{warehouseNameMissing}'");
                }
            }
            else
            {
                throw new Exception($"Write-in part present in Product Line '{quotePresentInLine}'");
            }

            var database = this.companyNameToDatabase(metadata.Target.Name);
            var databaseName = Enum.GetName(typeof(DatabaseRange), database);
            bool hasAccess = await this.checkUserAccess(database, metadata.User);

            if (!hasAccess)
            {

                throw new Exception($"User '{metadata.User}' does not have sufficient privileges in Sage {databaseName}. Please reach out to Sage team for support");
            }

            var ordersRepo = this.dbManager.GetOrdersRepository(database);
            var exists = await ordersRepo.ExistsAsync(order.OrderNo);

            if (exists)
            {
                throw new Exception($"Order {order.OrderNo} already exists in Sage {databaseName}.");
            }

            var missedProducts = await this.findMissedProducts(database, order.Products);

            if (missedProducts.Count > 0)
            {
                var codes = string.Join(", ", missedProducts.Select(i => i.PartNumber).ToArray());
                throw new Exception($"The following products do not exist in Sage: {codes}");
            }

            ordersRepo.Create(order);

            this.logger.Information("Successfully created a new order {orderNo}", order.OrderNo);
        }

        private DatabaseRange companyNameToDatabase(string company)
        {
            switch (company.ToLower())
            {
                case "0129 - sage-singapore":
                    return DatabaseRange.Singapore;
                case "0197 - sage-aberdeen":
                    return DatabaseRange.Kintore;
                case "0200 - sage-dubai":
                    return DatabaseRange.Dubai;
                case "0686 - sage-abu_dhabi":
                    return DatabaseRange.AbuDhabi;
                case "0815 - sage-norway":
                    return DatabaseRange.Norway;
                case "0920 - sage-saudi":
                    return DatabaseRange.Saudi;
                case "0199 - sage-perth":
                    return DatabaseRange.Perth;
                case "1107 - sage-saudi":
                    return DatabaseRange.Saudi_1107;
                default:
                    return DatabaseRange.Unknown;
            }
        }

        private async Task<List<Product>> findMissedProducts(DatabaseRange database, List<Product> products)
        {
            var orderLinesRepo = this.dbManager.GetOrderLinesRepository(database);

            var missedLines = new List<Product>();

            for (var i = 0; i < products.Count; i ++)
            {
                var product = products[i];
                var exists = await orderLinesRepo.ExistsAsync(product.PartNumber, product.IsServie());

                if (!exists)
                {
                    missedLines.Add(product);
                }
            }

            return missedLines;
        }

        private async Task<bool> checkUserAccess(DatabaseRange database, string user)
        {
            try
            {
                return await this.dbManager.HasUserAccess(database, user);
            }
            catch (Exception ex)
            {
                this.logger.Error("Failed to check user permissions {user} {@error}", user, ex);
                throw new Exception("Sage Server Issue: Cannot connect to Sage Database.", ex);
            }
        }
    }
}
