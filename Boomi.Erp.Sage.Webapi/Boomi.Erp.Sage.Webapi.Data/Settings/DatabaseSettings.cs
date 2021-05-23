using System;
using Boomi.Erp.Sage.Webapi.Common.Security;

namespace Boomi.Erp.Sage.Webapi.Data.Settings
{
    public class DatabaseSettings : ConnectionSettings
    {
        public string Database { get; }

        public DatabaseSettings(string host, Credentials credentials, string database) :
            base(host, credentials)
        {
            this.Database = database;
        }

        public DatabaseSettings(ConnectionSettings connection, string database) :
            base(connection.Host, connection.Credentials)
        {
            this.Database = database;
        }

        public override string ToString()
        {
            var host = this.Host.ToLower();
            var database = this.Database;
            var username = this.Credentials?.Username ?? "";
            var password = this.Credentials?.Password ?? "";

            return $"server='{host}';database={database};uid='{username}';pwd='{password}'";
        }
    }
}
