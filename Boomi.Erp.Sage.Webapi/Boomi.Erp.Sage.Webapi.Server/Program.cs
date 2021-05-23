using System;
using System.Threading;
using Microsoft.Owin.Hosting;

namespace Boomi.Erp.Sage.Webapi.Server
{
    class Program
    {
        private static readonly ManualResetEvent exitEvent = new ManualResetEvent(false);

        public static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            string port = "8080";
            string baseAddress = $"http://+:{port}";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress)) {
                Console.WriteLine("Server listening at 8080");

                exitEvent.WaitOne();
            }
        }
    }
}
