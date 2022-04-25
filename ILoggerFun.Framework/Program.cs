using ILoggerFun.Framework;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;

namespace ILoggerFun.Framework
{
    internal class Program
    {
        //static async Task Main(string[] args)
        //{
        //}
        //public class Program
        //{
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, configuration) =>
            {
                configuration.Sources.Clear();

                IHostEnvironment env = context.HostingEnvironment;

                configuration
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddDebug();
            })
            .ConfigureServices((hostBuilderContext, services) =>
                services.Configure<AlertNotificationOptions>(
                    hostBuilderContext.Configuration.GetSection(
                        nameof(AlertNotificationOptions))))
            .ConfigureServices((hostBuilderContext, services) =>
                services.AddTransient<AlertNotification>())
            .Build();

            var service = host.Services
                .GetService<AlertNotification>();


            host.RunAsync();
            service.Start();
            WaitForAnyKeyPress(service);
        }

        private static void WaitForAnyKeyPress(AlertNotification notification)
        {
            ConsoleKey key;

            do
            {
                while (!Console.KeyAvailable)
                {
                    //
                }

                // Key is available - read it
                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Spacebar)
                    notification.Flip();
                else if (key == ConsoleKey.MediaNext)
                    notification.Faster();
                else if (key == ConsoleKey.MediaPrevious)
                    notification.Slower();

            } while (key != ConsoleKey.Escape);
        }

    }
}
namespace Microsoft.Extensions.Hosting
{
public static class IHostingExtensions
    {
        public static IHostBuilder ConfigureIOptions(this IHostBuilder hostContext) =>
    hostContext.ConfigureServices((hostBuilderContext, services) => services
        .Configure<AlertNotificationOptions>(
            hostBuilderContext.Configuration.GetSection(
                nameof(AlertNotificationOptions))));


        public static IHostBuilder ConfigureServices(this IHostBuilder hostContext) =>
            hostContext.ConfigureServices((hostBuilderContext, services) => services
                .AddTransient<AlertNotification>());
    }
}