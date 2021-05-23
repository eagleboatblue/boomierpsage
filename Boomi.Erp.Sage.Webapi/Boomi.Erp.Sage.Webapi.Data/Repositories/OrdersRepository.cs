using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using FusionSDK;
using Boomi.Erp.Sage.Webapi.Data.Abstract;
using Boomi.Erp.Sage.Webapi.Data.Settings;
using Boomi.Erp.Sage.Webapi.Models.Orders;

namespace Boomi.Erp.Sage.Webapi.Data.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly DatabaseSettings settings;

        public OrdersRepository(DatabaseSettings settings)
        {
            this.settings = settings;
        }

        public async Task<bool> ExistsAsync(string orderNumber)
        {
            using (var conn = new SqlConnection(this.settings.ToString()))
            {
                await conn.OpenAsync();

                var query = "SELECT COUNT(*) FROM scheme.opheadm where order_no = @OrderNo";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@OrderNo", orderNumber));

                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public void Create(Order order)
        {
            var sageOrder = new SalesOrder();
            sageOrder.setSQL(
                this.settings.Host,
                this.settings.Database,
                this.settings.Credentials?.Username ?? "",
                this.settings.Credentials?.Password ?? ""
            );
            sageOrder.setDeveloperDebuggingMode(false);
            sageOrder.setRetryTimeout(60);
            sageOrder.setTimeout(60);
            sageOrder.setSchema("scheme");

            var binder = new Mappers.OrderMapper();
            binder.Map(sageOrder, order);

            var created = sageOrder.submitOrder(order.ErpAccountNo);

            if (created)
            {
                return;
            }

            throw new Exception(sageOrder.getLastErrorMessage());
        }
    }
}
