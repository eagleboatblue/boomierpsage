using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Boomi.Erp.Sage.Webapi.Data.Abstract;
using Boomi.Erp.Sage.Webapi.Data.Settings;

namespace Boomi.Erp.Sage.Webapi.Data.Repositories
{
    public class OrderLinesRepository : IOrderLinesRepository
    {
        private static readonly string productsTableName = "stockm";

        private static readonly string servicesTableName = "opservm";

        private readonly DatabaseSettings settings;

        public OrderLinesRepository(DatabaseSettings settings)
        {
            this.settings = settings;
        }

        public async Task<bool> ExistsAsync(string code, bool isService)
        {
            using (var conn = new SqlConnection(this.settings.ToString()))
            {
                await conn.OpenAsync();

                var tableName = isService ? OrderLinesRepository.servicesTableName : OrderLinesRepository.productsTableName;
                var query = $"SELECT COUNT(*) FROM scheme.{tableName} where product = @code";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@code", code.Trim()));

                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }
    }
}
