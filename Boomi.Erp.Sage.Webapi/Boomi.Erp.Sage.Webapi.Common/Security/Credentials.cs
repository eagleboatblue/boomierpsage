using System;
namespace Boomi.Erp.Sage.Webapi.Common.Security
{
    public class Credentials
    {
        public string Username { get; }

        public string Password { get; }

        public Credentials(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }
    }
}
