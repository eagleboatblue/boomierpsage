using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boomi.Erp.Sage.Webapi.Server.Messaging
{
    public class RouteInfo
    {
        public Destination Deadletter { get; }

        public Destination Request { get; }

        public Destination Response { get; }

        public RouteInfo(Destination deadletter, Destination request, Destination response)
        {
            this.Deadletter = deadletter;
            this.Request = request;
            this.Response = response;
        }
    }
}
