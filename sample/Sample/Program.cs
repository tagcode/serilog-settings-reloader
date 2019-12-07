using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Initial configuration
            MemoryConfiguration config = new MemoryConfiguration()
                .Set("Serilog:WriteTo:0:Name", "Console")
                .Set("Serilog:WriteTo:0:Args:OutputTemplate", "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}")
                .Set("Serilog:WriteTo:0:Args:RestrictedToMinimumLevel", "Information");

            // Read configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .Add(config)
                .Build();

            // Serilog initial configuration
            SwitchableLogger.Instance.Logger = new Serilog.LoggerConfiguration()
                .MinimumLevel.Verbose()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            // Service collection
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                    loggingBuilder
                        .AddSerilog(SwitchableLogger.Instance, true));

            // Services
            using (var services = serviceCollection.BuildServiceProvider())
            {
                // Create logger
                Microsoft.Extensions.Logging.ILogger logger = services.GetService<Microsoft.Extensions.Logging.ILogger<Program>>();

                // Write
                logger.LogInformation("Hello World");

                // Modify config
                config.Set("Serilog:WriteTo:0:Args:OutputTemplate", "[{SourceContext}] {Message:lj}{NewLine}{Exception}");
                configuration.Reload();

                // Serilog initial configuration
                Serilog.ILogger newLogger = new Serilog.LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();
                // Assign new logger
                SwitchableLogger.Instance.Set(newLogger, true);

                // Write with the previous logger instance, but with different settings
                logger.LogInformation("Hello world again");
            }
        }
    }

}
