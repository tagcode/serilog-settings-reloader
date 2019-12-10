using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Events;
using System;

namespace Serilog.Settings.Reloader.Test
{
    [TestClass]
    public class SerilogConfigurationLoaderExtensionsTests
    {
        [TestMethod]
        public void Test1()
        {

        }

        [TestMethod]
        public void ReloadErroneousConfigurationTest()
        {
            // Initial configuration
            MemoryConfiguration config = new MemoryConfiguration()
                .Set("Serilog:WriteTo:0:Name", "Console")
                .Set("Serilog:WriteTo:0:Args:OutputTemplate", "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}")
                .Set("Serilog:MinimumLevel", "Information");

            // Read configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .Add(config)
                .Build();

            // Service collection
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                    loggingBuilder
                        .AddSerilog(SwitchableLogger.Instance, true)
                        .AddSerilogConfigurationLoader(configuration, SwitchableLogger.Instance)
                        .SetMinimumLevel(LogLevel.Trace)
                    );

            // Services
            using (var services = serviceCollection.BuildServiceProvider())
            {
                // Create logger
                Microsoft.Extensions.Logging.ILogger logger = services.GetService<Microsoft.Extensions.Logging.ILogger<SerilogSwitchTests>>();

                // Assert levels
                Assert.IsTrue(SwitchableLogger.Instance.IsEnabled(LogEventLevel.Information));
                Assert.IsFalse(SwitchableLogger.Instance.IsEnabled(LogEventLevel.Debug));

                // Write
                logger.LogInformation("Hello World");

                // Modify erroneus config
                config.Set("Serilog:MinimumLevel", "Blaa");
                try
                {
                    configuration.Reload();
                    Assert.Fail("Expected load error");
                }
                catch (Exception) { 
                    // Expected
                }
                // Assert debug is information retains
                Assert.IsTrue(SwitchableLogger.Instance.IsEnabled(LogEventLevel.Information));
                Assert.IsFalse(SwitchableLogger.Instance.IsEnabled(LogEventLevel.Debug));

                // Write with the previous logger instance, but with different settings
                logger.LogInformation("Hello world again");

                // Modify ok config
                config.Set("Serilog:MinimumLevel", "Debug");
                configuration.Reload();

                // Assert debug is applied
                Assert.IsTrue(SwitchableLogger.Instance.IsEnabled(LogEventLevel.Information));
                Assert.IsTrue(SwitchableLogger.Instance.IsEnabled(LogEventLevel.Debug));

            }
        }

    }
}
