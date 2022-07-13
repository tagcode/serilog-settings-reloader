# Serilog.Settings.Reloader
**Serilog.Settings.Reloader** provides completely reloadable settings for Serilog.

Links:
 * Github ([Serilog.Settings.Reloader](https://github.com/tagcode/serilog-settings-reloader))
 * Nuget ([Serilog.Settings.Reloader](https://www.nuget.org/packages/Serilog.Settings.Reloader))
 * License ([Apache-2.0 license](http://www.apache.org/licenses/LICENSE-2.0))

**SwitchableLogger** is assigned with new root *ILogger* when configuration is modified.
```C#
// Create switchable
SwitchableLogger switchableLogger = new SwitchableLogger();
// Assign SwitchableLogger to Serilog.Log.Logger
Serilog.Log.Logger = switchableLogger;

// Assign logger to switchableLogger
switchableLogger.Logger = new Serilog.LoggerConfiguration()
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
switchableLogger.Set(newLogger, disposePrev: true);

// Write with the previous logger instance, but with different settings
logger.Information("Hello world again");
```

<br/><br/>

**.AddSerilogConfigurationLoader()** can be used with dependency injection's *ILoggingBuilder*.
```C#
// Create switchable logger
SwitchableLogger switchableLogger = new SwitchableLogger();

// Read configuration
IConfigurationRoot configuration = new ConfigurationBuilder()
    Add(config)
    .Build();

// Service collection
IServiceCollection serviceCollection = new ServiceCollection()
    .AddLogging(loggingBuilder =>
        loggingBuilder
            .AddSerilog(switchableLogger, true)
            .AddSerilogConfigurationLoader(configuration, switchableLogger)
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

<br/><br/>

**.AddSerilogConfigurationLoader(<i>IConfiguration</i>, <i>SwitchableLogger</i>, <i>Func&lt;IConfiguration, ILogger&gt;</i>)** third argument specifies load function.
```C#
loggingBuilder
    .ClearProviders()
    .AddSerilog(switchableLogger, true)
    .AddSerilogConfigurationLoader(configuration, switchableLogger, 
        c => new Serilog.LoggerConfiguration().ReadFrom.Configuration(c).CreateLogger())
    );
```