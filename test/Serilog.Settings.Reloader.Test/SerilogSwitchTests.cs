using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace Serilog.Settings.Reloader.Test
{
    [TestClass]
    public class SerilogSwitchTests
    {
        [TestMethod]
        public void ConcurrencyTest()
        {
            Parallel.For(0, 8, t =>
            {
                Random r = new Random();
                for (int i=0; i<5000000; i++)
                {
                    ILogger l = new LoggerConfiguration().MinimumLevel.Is((LogEventLevel)r.Next(0, 6)).CreateLogger();
                    if (r.Next(0, 2) == 0) SwitchableLogger.Instance.Set(l); else SwitchableLogger.Instance.Set(()=>l);
                    SwitchableLogger.Instance.IsEnabled((LogEventLevel)r.Next(0, 6));
                    switch(r.Next(0, 6))
                    {
                        case 0:SwitchableLogger.Instance.Verbose("x"); break;
                        case 1:SwitchableLogger.Instance.Debug("x"); break;
                        case 2:SwitchableLogger.Instance.Information("x"); break;
                        case 3:SwitchableLogger.Instance.Warning("x"); break; 
                        case 4:SwitchableLogger.Instance.Error("x"); break;
                        case 5:SwitchableLogger.Instance.Fatal("x"); break;
                    }                    
                }
            });
        }


    }
}
