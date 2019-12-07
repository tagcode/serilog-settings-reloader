// -------------------------------------------------------
// Copyright:          Toni Kalajainen
// Date:               4.12.2019
// License:            Apache 2.0
// -------------------------------------------------------
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Serilog
{
    /// <summary>
    /// Serilog ILoggingBuilder extension methods that add reload feature
    /// </summary>
    public static class SerilogLoggerBuilderReloadExtensions
    {
        /// <summary>
        /// Add reload feature.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration">configuration to monitor</param>
        /// <param name="switchableLogger">(optional) switch</param>
        /// <param name="configurationLoader">(optional) delegate that reads configuration</param>
        /// <returns></returns>
        public static ILoggingBuilder AddSerilogConfigurationLoader(this ILoggingBuilder builder, IConfiguration configuration, SwitchableLogger switchableLogger = default, Func<IConfiguration, ILogger> configurationLoader = default)
        {
            // Assert argument
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            // Instance switch instance
            if (switchableLogger == null) switchableLogger = new SwitchableLogger();
            // Create configure loader
            if (configurationLoader == null) configurationLoader = c => new Serilog.LoggerConfiguration().MinimumLevel.Verbose().ReadFrom.Configuration(c).CreateLogger();
            // Load initial configuration
            switchableLogger.Logger = configurationLoader(configuration);
            // Service collection
            IServiceCollection services = builder.Services;
            // Add switch instance as service
            ServiceCollectionServiceExtensions.AddSingleton(services, typeof(SwitchableLogger), switchableLogger);
            // Function container
            Action<object>[] handleReloadContainer = new Action<object>[1];
            // ChangeToken callback
            handleReloadContainer[0] =
                o =>
                {
                    // Create new configuration
                    ILogger newLogger = configurationLoader(configuration);
                    // Apply configuration
                    switchableLogger.Set(newLogger, disposePrev: true);
                    // Monitor next change
                    configuration.GetReloadToken().RegisterChangeCallback(handleReloadContainer[0], services);
                };
            // Monitor configuration changes
            configuration.GetReloadToken().RegisterChangeCallback(handleReloadContainer[0], services);
            // Return builder.
            return builder;
        }
    }
}
