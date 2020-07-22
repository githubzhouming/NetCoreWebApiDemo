using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
            // // NLog: setup the logger first to catch all errors
            // //var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            // try
            // {
            //     //logger.Debug("init main");
            //     CreateWebHostBuilder(args).Build().Run();
            // }
            // catch (Exception ex)
            // {
            //     //NLog: catch setup errors
            //     //logger.Error(ex, "Stopped program because of exception");
            //     throw ex;
            // }
            // finally
            // {
            //     // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            //     //NLog.LogManager.Shutdown();
            // }

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseUrls()
            .UseStartup<Startup>()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                //The Logging configuration specified in appsettings.json overrides any call to SetMinimumLevel.
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                logging.AddConsole();
            })
            .UseNLog();  // NLog: setup NLog for Dependency injection
    }
}
