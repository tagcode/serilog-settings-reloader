using Serilog;

namespace SampleSimple
{
    class Program
    {
        static void Main(string[] args)
        {
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
        }
    }
}
