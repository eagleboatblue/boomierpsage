using System;
using System.Collections.Generic;
using Boomi.Erp.Sage.Webapi.Models.Common;

namespace Boomi.Erp.Sage.Webapi.Data
{
    public class DatabaseCollection {
        private readonly Dictionary<DatabaseRange, string> items;

        public DatabaseCollection() {
            this.items = new Dictionary<DatabaseRange, string>();
        }

        public bool Has(DatabaseRange name) {
            return this.items.ContainsKey(name);
        }

        public void Add(DatabaseRange name, string value) {
            if (this.items.ContainsKey(name))
            {
                throw new Exception($"Database is already registered {name}");
            }

            this.items.Add(name, value);
        }

        public string Get(DatabaseRange name)
        {
            return this.items[name];
        }
    }
}
