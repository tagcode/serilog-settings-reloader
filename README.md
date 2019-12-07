# Serilog.Settings.Reloader
**Serilog.Settings.Reloader** provides completely reloadable settings for Serilog.

Links:
 * Github ([Serilog.Settings.Reloader])(https://github.com/tagcode/serilog-settings-reloader)
 * Nuget ([Serilog.Settings.Reloader])(https://www.nuget.org/packages/Serilog.Settings.Reloader)
 * License ([Apache-2.0 license])(http://www.apache.org/licenses/LICENSE-2.0)
 
New configurations can be loaded into **SwitchableLogger**.
```C#
            // Assign SerilogSwitch.Instance to Serilog.Log.Logger
            Serilog.Log.Logger = SwitchableLogger.Instance;

            // Assign logger to SerilogSwitch.Instance
            SwitchableLogger.Instance.Logger = new Serilog.LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            // Create logger
            ILogger logger = Serilog.Log.ForContext<Program>();

            // Write
            logger.Information("Hello World");

            // Reconfigure 
            ILogger newLogger = new Serilog.LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Console(outputTemplate: "[{SourceContext}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();
            // Assign new logger
            SwitchableLogger.Instance.Set(newLogger, true);

            // Write with the previous logger instance, but with different settings
            logger.Information("Hello world again");
```

**.AddSerilogConfigurationLoader()** can be used with dependency injection's *ILoggingBuilder*.
```C#
            // Read configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .Add(config)
                .Build();

            // Service collection
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddSingleton<IConfigurationRoot>(configuration)
                .AddSingleton<IConfiguration>(configuration)
                .AddLogging(loggingBuilder =>
                    loggingBuilder
                        .AddSerilog(SwitchableLogger.Instance, true)
                        .AddSerilogConfigurationLoader(configuration, SwitchableLogger.Instance)
                    );

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

                // Write with the previous logger instance, but with different settings
                logger.LogInformation("Hello world again");
            }
```