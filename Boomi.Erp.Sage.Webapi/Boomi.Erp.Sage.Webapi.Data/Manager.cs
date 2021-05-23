using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Boomi.Erp.Sage.Webapi.Data.Abstract;
using Boomi.Erp.Sage.Webapi.Data.Settings;
using Boomi.Erp.Sage.Webapi.Models.Common;

namespace Boomi.Erp.Sage.Webapi.Data
{
    public class Manager
    {
        private readonly IComponentContext container;
        private readonly ConnectionSettings settings;
        private readonly DatabaseCollection databases;

        public Manager(
            IComponentContext container,
            ConnectionSettings settings,
            DatabaseCollection databases
        )
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (databases == null)
            {
                throw new ArgumentNullException("databases");
            }

            this.container = container;
            this.settings = settings;
            this.databases = databases;
        }

        public Task<bool> HasUserAccess(DatabaseRange database, string username)
        {
            var users = this.GetUsersRepository();

            return users.HasAccessAsync(this.resolveDatabase(database), username);
        }

        public IUsersRepository GetUsersRepository()
        {
            return this.container.Resolve<IUsersRepository>(new Parameter[] {
                new TypedParameter(typeof(DatabaseSettings), new DatabaseSettings(this.settings, "csmaster"))
            });
        }

        public IOrdersRepository GetOrdersRepository(DatabaseRange database)
        {
            return this.container.Resolve<IOrdersRepository>(new Parameter[] {
                new TypedParameter(
                    typeof(DatabaseSettings),
                    new DatabaseSettings(this.settings, this.resolveDatabase(database))
                )
            });
        }

        public IOrderLinesRepository GetOrderLinesRepository(DatabaseRange database)
        {
            return this.container.Resolve<IOrderLinesRepository>(new Parameter[] {
                new TypedParameter(
                    typeof(DatabaseSettings),
                    new DatabaseSettings(this.settings, this.resolveDatabase(database))
                )
            });
        }

        private string resolveDatabase(DatabaseRange database)
        {
            if (!this.databases.Has(database))
            {
                throw new ArgumentOutOfRangeException("Unrecognized Sage Company Name");
            }

            return this.databases.Get(database);
        }
    }
}
