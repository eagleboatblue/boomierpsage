using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Boomi.Erp.Sage.Webapi.Data.Abstract;
using Boomi.Erp.Sage.Webapi.Data.Settings;
using Serilog;

namespace Boomi.Erp.Sage.Webapi.Data.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DatabaseSettings settings;
        private readonly ILogger logger;
        public UsersRepository(DatabaseSettings settings, ILogger logger)
        {
            this.settings = settings;
            this.logger = logger;
        }

        public async Task<bool> HasAccessAsync(string company, string email)
        {
            if (String.IsNullOrWhiteSpace(company))
            {
                throw new ArgumentNullException("company");
            }

            if (String.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("username");
            }

            using (var conn = new SqlConnection(this.settings.ToString()))
            {
                await conn.OpenAsync();

                // find corresponding username first
                var username = await this.findUsernameAsync(conn, email);

                // not found
                if (string.IsNullOrWhiteSpace(username))
                {
                    return false;
                }

                // check permissions by found username
                var hasPermissions = await this.checkUserPermissionsAsync(conn, company, username);

                return hasPermissions;
            }
        }


        private async Task<string> findUsernameAsync(SqlConnection conn, string email)
        {

            var query = @"
                    SELECT TOP 1
                        name
                    FROM
                        scheme.usermastm_email
                    WHERE
                        LOWER(email) = @email";

            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@email", email.ToLowerInvariant()));

                var result = await cmd.ExecuteScalarAsync();                

                if (result == null)
                {
                    return string.Empty;
                }

                return result.ToString();
            }
        }

        private async Task<bool> checkUserPermissionsAsync(SqlConnection conn, string company, string username) 
        {
            using (var cmd = new SqlCommand("scheme.spCheckPermissions", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@USER", username.ToLowerInvariant()));
                cmd.Parameters.Add(new SqlParameter("@WDNAME", company.ToLowerInvariant()));
                cmd.Parameters.Add(new SqlParameter("@OPTION", "op_order_entry"));
                cmd.Parameters.Add(new SqlParameter("@MODULE", "op"));

                var result = await cmd.ExecuteScalarAsync();              

                if (result == null)
                {
                    return false;
                }

                return result.ToString() == "Y";
            }
        }
    }
}
