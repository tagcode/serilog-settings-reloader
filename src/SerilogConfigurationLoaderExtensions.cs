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
    public static class SerilogConfigurationLoaderExtensions
    {
        /// <summary>Default configuration loader function</summary>
        static Func<IConfiguration, ILogger> defaultConfigurationLoader = c => new Serilog.LoggerConfiguration().MinimumLevel.Verbose().ReadFrom.Configuration(c).CreateLogger();
        /// <summary>Default configuration loader function</summary>
        public static Func<IConfiguration, ILogger> DefaultConfigurationLoader => defaultConfigurationLoader;

        /// <summary>
        /// Add configuration load feature to <paramref name="builder"/>. 
        /// 
        /// If <paramref name="loadInitialConfiguration"/> is true, then load initial configuration <paramref name="configurationLoader"/> upon call. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration">configuration to monitor</param>
        /// <param name="switchableLogger">(optional) switch instance to load new configuration to.</param>
        /// <param name="configurationLoader">(optional) delegate that reads configuration. If not provided then uses <see cref="SerilogConfigurationLoaderExtensions.DefaultConfigurationLoader"/></param>
        /// <param name="loadInitialConfiguration">(optional) policy whether <paramref name="configurationLoader"/> is ran at this method call</param>
        /// <returns></returns>
        public static ILoggingBuilder AddSerilogConfigurationLoader(this ILoggingBuilder builder, IConfiguration configuration, SwitchableLogger switchableLogger = default, Func<IConfiguration, ILogger> configurationLoader = default, bool loadInitialConfiguration = true)
        {
            // Assert argument
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            // Instance switch instance
            if (switchableLogger == null) switchableLogger = new SwitchableLogger();
            // Create configure loader
            if (configurationLoader == null) configurationLoader = DefaultConfigurationLoader;
            // Load initial configuration
            if (loadInitialConfiguration) switchableLogger.Logger = configurationLoader(configuration);
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
