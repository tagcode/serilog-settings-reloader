using Serilog;

namespace SampleSimple
{
    class Program
    {
        static void Main(string[] args)
        {
            // Assign SwitchableLogger.Instance to Serilog.Log.Logger
            Serilog.Log.Logger = SwitchableLogger.Instance;

            // Assign logger to SwitchableLogger.Instance
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
        }
    }
}
