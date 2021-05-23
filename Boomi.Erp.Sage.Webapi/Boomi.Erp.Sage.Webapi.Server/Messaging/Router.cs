using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Boomi.Erp.Sage.Webapi.Server.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using Serilog;

namespace Boomi.Erp.Sage.Webapi.Server.Messaging
{
    public class Router : IDisposable
    {
        private static readonly string deadLetterExchangeName = "dead.letter.ex";
        // private static readonly string deadLetterQueueName = "dead.letter";
        private static readonly string exchangeName = "flow.ex";
        private static readonly string responseQueueName = "response";

        private bool running = false;
        private bool disposedValue = false; // To detect redundant calls
        private readonly ILogger logger;
        private readonly IConnection connection;
        private readonly JsonSerializer serializer;
        private readonly Dictionary<string, Func<IConnection, CancellationToken, Task>> routes;

        public Router(ILogger logger, IConnection connection)
        {
            this.logger = logger;
            this.connection = connection;
            this.connection = connection;
            this.serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,

                DefaultValueHandling = DefaultValueHandling.Populate,

                ContractResolver = new CamelCasePropertyNamesContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        OverrideSpecifiedNames = false,
                        ProcessDictionaryKeys = true,
                        ProcessExtensionDataNames = true
                    }
                }
            };

            this.routes = new Dictionary<string, Func<IConnection, CancellationToken, Task>>();
        }

        public void AddHandler<TIn, TOut>(string queueName, IHandler<TIn, TOut> handler)
        {
            if (this.routes.ContainsKey(queueName))
            {
                throw new InvalidOperationException($"'{queueName}' queue handler is already defined");
            }

            this.routes[queueName] = (IConnection conn, CancellationToken cancellation) =>
            {
                var route = new Route<TIn, TOut>(
                    this.logger,
                    this.serializer,
                    new RouteInfo(
                        new Destination(deadLetterExchangeName),
                        new Destination(exchangeName, queueName),
                        new Destination(exchangeName, responseQueueName)
                    ),
                    handler
                );

                return route.Use(conn, cancellation);
            };
        }

        public Task Start(CancellationToken cancellation)
        {
            lock (this)
            {
                if (this.running)
                {
                    throw new Exception("Manager is already running");
                }

                this.running = true;
            }

            return Task.Run(() => {
                this.routes.Values.Select(i => i(this.connection, cancellation)).ToArray();

                cancellation.WaitHandle.WaitOne();

                lock (this)
                {
                    if (this.running)
                    {
                        this.running = false;
                    }
                }
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.connection.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
    }
}
