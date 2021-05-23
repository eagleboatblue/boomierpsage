using System;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Newtonsoft.Json.Serialization;
using Boomi.Erp.Sage.Webapi.Common.Security;
using Boomi.Erp.Sage.Webapi.Data;
using Boomi.Erp.Sage.Webapi.Data.Abstract;
using Boomi.Erp.Sage.Webapi.Data.Repositories;
using Owin;
using Serilog;
using Serilog.Formatting.Compact;
using Boomi.Erp.Sage.Webapi.Models.Common;
using Boomi.Erp.Sage.Webapi.Server.Services;
using Boomi.Erp.Sage.Webapi.Server.Messaging;
using Boomi.Erp.Sage.Webapi.Server.Handlers;
using System.Threading;
using RabbitMQ.Client;

namespace Boomi.Erp.Sage.Webapi.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(new CompactJsonFormatter())
                .CreateLogger();

            Log.Logger = logger;
            logger.Information("Starting Boomi ERP Sage service");
            
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var asm = Assembly.GetExecutingAssembly();
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(asm);
            builder.RegisterWebApiFilterProvider(config);
            builder.RegisterWebApiModelBinderProvider();
            builder.RegisterInstance<ILogger>(logger);

            this.useDatabase(builder);
            this.useServiceProvider(builder);
            this.useServices(builder);
            this.useMessaging(builder);

            // Build
            var container = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);

            // Messaging router
            var router = container.Resolve<Router>();
            var ordersHandler = container.Resolve<OrdersHandler>();

            // Register handlers
            router.AddHandler("request.erp.sage", ordersHandler);

            var cancellation = new CancellationTokenSource();

            try
            {
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    eventArgs.Cancel = true;
                    cancellation.Cancel();
                };

                router.Start(cancellation.Token);
            }
            catch (Exception e)
            {
                cancellation.Cancel();
                System.Console.WriteLine(e);
                Environment.Exit(1);
            }
        }

        private void useDatabase(ContainerBuilder builder)
        {
            builder.RegisterType<UsersRepository>().As<IUsersRepository>();
            builder.RegisterType<OrdersRepository>().As<IOrdersRepository>();
            builder.RegisterType<OrderLinesRepository>().As<IOrderLinesRepository>();
            builder.RegisterType<Data.Manager>().SingleInstance();
            builder.RegisterInstance(new Data.Settings.ConnectionSettings(
                 //Environment.GetEnvironmentVariable("DATABASE_SERVER"),
                 //new Credentials(
                 //    Environment.GetEnvironmentVariable("DATABASE_USER"),
                 //    Environment.GetEnvironmentVariable("DATABASE_PASSWORD")
                 "SRVKRSCPSAGESQL",
                new Credentials(
                    "scheme", "sistroc6"
                )
            ));

            ////var databases = new DatabaseCollection();
            ////databases.Add(DatabaseRange.Singapore, Environment.GetEnvironmentVariable("SAGE_SINGAPORE"));
            ////databases.Add(DatabaseRange.Kintore, Environment.GetEnvironmentVariable("SAGE_ABERDEEN"));
            ////databases.Add(DatabaseRange.Dubai, Environment.GetEnvironmentVariable("SAGE_DUBAI"));
            ////databases.Add(DatabaseRange.AbuDhabi, Environment.GetEnvironmentVariable("SAGE_ABU_DHABI"));
            ////databases.Add(DatabaseRange.Norway, Environment.GetEnvironmentVariable("SAGE_NORWAY"));
            ////databases.Add(DatabaseRange.Saudi, Environment.GetEnvironmentVariable("SAGE_SAUDI"));
            ////databases.Add(DatabaseRange.Perth, Environment.GetEnvironmentVariable("SAGE_PERTH"));
            ////databases.Add(DatabaseRange.Saudi_1107, Environment.GetEnvironmentVariable("SAGE_SAUDI_1107"));

            var databases = new DatabaseCollection();
            //databases.Add(DatabaseRange.Singapore, Environment.GetEnvironmentVariable("SAGE_SINGAPORE"));
            databases.Add(DatabaseRange.Singapore, "sys129p");
            //databases.Add(DatabaseRange.Kintore, Environment.GetEnvironmentVariable("SAGE_ABERDEEN"));
            databases.Add(DatabaseRange.Kintore, "sysp");
            databases.Add(DatabaseRange.Dubai, Environment.GetEnvironmentVariable("SAGE_DUBAI"));
            databases.Add(DatabaseRange.AbuDhabi, Environment.GetEnvironmentVariable("SAGE_ABU_DHABI"));
            databases.Add(DatabaseRange.Norway, Environment.GetEnvironmentVariable("SAGE_NORWAY"));
            databases.Add(DatabaseRange.Saudi, Environment.GetEnvironmentVariable("SAGE_SAUDI"));
            databases.Add(DatabaseRange.Perth, Environment.GetEnvironmentVariable("SAGE_PERTH"));

            //databases.Add(Database.Saudi_1107, Environment.GetEnvironmentVariable("SAGE_SAUDI_1107"));
            databases.Add(DatabaseRange.Saudi_1107, "syssap");

            builder.RegisterInstance(databases).SingleInstance();
        }

        private void useServiceProvider(ContainerBuilder builder)
        {
            builder.RegisterType<ErpSageServiceProvider>().SingleInstance();
        }

        private void useServices(ContainerBuilder builder)
        {
            builder.RegisterType<Domain.Orders.Service>().SingleInstance();
        }

        private void useMessaging(ContainerBuilder builder)
        {
            builder.Register((c) =>
            {
                var factory = new ConnectionFactory()
                {
                    // Do not set to false if you aree using async consumers (as you should do)
                    // https://gigi.nullneuron.net/gigilabs/asynchronous-rabbitmq-consumers-in-net/
                    DispatchConsumersAsync = true,
                    AutomaticRecoveryEnabled = true,
                    TopologyRecoveryEnabled = true,
                    //HostName = System.Environment.GetEnvironmentVariable("RABBITMQ_URL"),
                    //UserName = System.Environment.GetEnvironmentVariable("RABBITMQ_USER"),
                    //Password = System.Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")

                    HostName = System.Environment.GetEnvironmentVariable("localhost"),
                    UserName = System.Environment.GetEnvironmentVariable("caps"),
                    Password = System.Environment.GetEnvironmentVariable("test")
                };
                factory.Ssl.Enabled = false;

                factory.HostName = "localhost";
                factory.UserName = "caps";
                factory.Password = "test";

                return factory.CreateConnection();
            }).As<IConnection>();

            // Messaging router
            builder.RegisterType(typeof(Router)).SingleInstance();

            // Message handlers
            builder.RegisterType<OrdersHandler>();
        }
    }
}