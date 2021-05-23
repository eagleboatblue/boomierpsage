using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boomi.Erp.Sage.Webapi.Server.Messaging
{
    public class Destination
    {
        public string Exchange { get; }

        public string Queue { get; }

        public Destination(string exchange) : this(exchange, String.Empty) { }

        public Destination(string exchange, string queue)
        {
            this.Exchange = exchange;
            this.Queue = queue;
        }
    }
}
