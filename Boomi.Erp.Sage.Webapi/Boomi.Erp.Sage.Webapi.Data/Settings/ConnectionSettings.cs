using System;
using Boomi.Erp.Sage.Webapi.Common.Security;

namespace Boomi.Erp.Sage.Webapi.Data.Settings
{
    public class ConnectionSettings
    {
        public string Host { get; }

        public Credentials Credentials { get; }

        public ConnectionSettings(string host, Credentials credentials)
        {
            this.Host = host;
            this.Credentials = credentials;
        }
    }
}
