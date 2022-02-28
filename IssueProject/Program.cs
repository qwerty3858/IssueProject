using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Debugging;

namespace IssueProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args)
                    .Build()
                    .Run();
            }
            catch (Exception vEx)
            {
                Log.Fatal(vEx, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            IConfiguration vBuiltConfig = new ConfigurationBuilder()
                .AddJsonFile("HostSettings.json", false)
                .Build();

            SelfLog.Enable(msg => File.AppendAllText("C:\\Log\\Serilog.log", msg));

            var vLoggingLevelSwitch = new LoggingLevelSwitch();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(vLoggingLevelSwitch)
                .ReadFrom.Configuration(vBuiltConfig)
                .CreateLogger();

            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton(vLoggingLevelSwitch);
                })
                .ConfigureHostConfiguration(builder =>
                {
                    builder.AddConfiguration(vBuiltConfig);
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {

                })
                .ConfigureLogging((context, builder) =>
                {
                    builder.ClearProviders();
                    builder.AddSerilog();
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .UseKestrel((context, options) =>
                        {
                            options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
                        })
                        .UseIISIntegration()
                        .UseStartup<Startup>()
                        .UseSerilog();
                });
        }
    }
}
