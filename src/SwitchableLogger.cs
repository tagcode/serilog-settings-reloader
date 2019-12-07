// -------------------------------------------------------
// Copyright:          Toni Kalajainen
// Date:               4.12.2019
// License:            Apache 2.0
// -------------------------------------------------------
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;

namespace Serilog
{
    /// <summary>
    /// Switchable serilog <see cref="ILogger"/>.
    /// </summary>
    public class SwitchableLogger : ILogger, IDisposable
    {
        /// <summary>Silent logger</summary>
        static ILogger silent = new LoggerConfiguration().CreateLogger();
        /// <summary>Switchable logger singleton</summary>
        static SwitchableLogger instance = new SwitchableLogger();
        /// <summary>Switchable logger singleton</summary>
        public static SwitchableLogger Instance => instance;

        /// <summary>Current logger</summary>
        protected ILogger logger;
        /// <summary>Logger provider</summary>
        protected Func<ILogger> loggerProvider;
        /// <summary>Logger</summary>
        public ILogger Logger { get => logger ?? loggerProvider() ?? silent; set => Set(value); }

        /// <summary>Create switchable logger.</summary>
        public SwitchableLogger()
        {
        }

        /// <summary>Create logger with initial <paramref name="logger"/>.</summary>
        /// <param name="logger">(optional) initial logger</param>
        public SwitchableLogger(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>Create switchable logger.</summary>
        /// <param name="loggerProvider">(optional) logger provider</param>
        public SwitchableLogger(Func<ILogger> loggerProvider)
        {
            this.loggerProvider = loggerProvider;
        }

        /// <summary>
        /// Set <paramref name="newLogger"/> as new logger.
        /// </summary>
        /// <param name="newLogger"></param>
        /// <param name="disposePrev"></param>
        /// <returns></returns>
        public SwitchableLogger Set(ILogger newLogger, bool disposePrev = false)
        {
            if (this.logger == newLogger) return this;
            var oldLogger = this.logger;
            this.logger = newLogger;
            this.loggerProvider = null;
            if (disposePrev && oldLogger is IDisposable disp) disp.Dispose();
            return this;
        }

        /// <summary>
        /// Set <paramref name="newLoggerProvider"/> as new logger provider.
        /// </summary>
        /// <param name="newLoggerProvider"></param>
        /// <param name="disposePrev"></param>
        /// <returns></returns>
        public SwitchableLogger Set(Func<ILogger> newLoggerProvider, bool disposePrev = false)
        {
            var oldLogger = this.logger;
            this.logger = null;
            this.loggerProvider = newLoggerProvider;
            if (disposePrev && oldLogger is IDisposable disp) disp.Dispose();
            return this;
        }

        /// <summary>Function that gets current logger</summary>
        public ILogger GetCurrentLogger()
            => logger ?? loggerProvider();

        /// <inheritdoc/>
        public ILogger ForContext(ILogEventEnricher enricher)
            => enricher == null ? this : new SwitchableLogger(new LoggerDecorator(GetCurrentLogger, l => l.ForContext(enricher)).Get);

        /// <inheritdoc/>
        public ILogger ForContext(IEnumerable<ILogEventEnricher> enrichers)
            => enrichers == null ? this : new SwitchableLogger(new LoggerDecorator(GetCurrentLogger, l => l.ForContext(enrichers)).Get);

        /// <inheritdoc/>
        public ILogger ForContext(string propertyName, object value, bool destructureObjects = false)
            => new SwitchableLogger(new LoggerDecorator(GetCurrentLogger, l => l.ForContext(propertyName, value, destructureObjects)).Get);

        /// <inheritdoc/>
        public ILogger ForContext<TSource>()
            => ForContext(typeof(TSource));

        /// <inheritdoc/>
        public ILogger ForContext(Type source)
            => source == null ? this : ForContext("SourceContext", source.FullName);

        /// <inheritdoc/>
        public void Write(LogEvent logEvent)
            => Logger.Write(logEvent);

        /// <inheritdoc/>
        public void Write(LogEventLevel level, string messageTemplate)
            => Logger.Write(level, messageTemplate);

        /// <inheritdoc/>
        public void Write<T>(LogEventLevel level, string messageTemplate, T propertyValue)
            => Logger.Write<T>(level, messageTemplate, propertyValue);

        /// <inheritdoc/>
        public void Write<T0, T1>(LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
            => Logger.Write<T0, T1>(level, messageTemplate, propertyValue0, propertyValue1);

        /// <inheritdoc/>
        public void Write<T0, T1, T2>(LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
            => Logger.Write<T0, T1, T2>(level, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

        /// <inheritdoc/>
        public void Write(LogEventLevel level, string messageTemplate, params object[] propertyValues)
            => Logger.Write(level, messageTemplate, propertyValues);

        /// <inheritdoc/>
        public void Write(LogEventLevel level, Exception exception, string messageTemplate)
            => Logger.Write(level, exception, messageTemplate);

        /// <inheritdoc/>
        public void Write<T>(LogEventLevel level, Exception exception, string messageTemplate, T propertyValue)
            => Logger.Write<T>(level, exception, messageTemplate, propertyValue);

        /// <inheritdoc/>
        public void Write<T0, T1>(LogEventLevel level, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
            => Logger.Write<T0, T1>(level, exception, messageTemplate, propertyValue0, propertyValue1);

        /// <inheritdoc/>
        public void Write<T0, T1, T2>(LogEventLevel level, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
            => Logger.Write<T0, T1, T2>(level, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

        /// <inheritdoc/>
        public void Write(LogEventLevel level, Exception exception, string messageTemplate, params object[] propertyValues)
            => Logger.Write(level, exception, messageTemplate, propertyValues);

        /// <inheritdoc/>
        public bool IsEnabled(LogEventLevel level)
            => Logger.IsEnabled(level);

        /// <inheritdoc/>
        public void Verbose(string messageTemplate)
            => Logger.Verbose(messageTemplate);

        /// <inheritdoc/>
        public void Verbose<T>(string messageTemplate, T propertyValue)
            => Logger.Verbose<T>(messageTemplate, propertyValue);

        /// <inheritdoc/>
        public void Verbose<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
            => Logger.Verbose<T0, T1>(messageTemplate, propertyValue0, propertyValue1);

        /// <inheritdoc/>
        public void Verbose<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
            => Logger.Verbose<T0, T1, T2>(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

        /// <inheritdoc/>
        public void Verbose(string messageTemplate, params object[] propertyValues)
            => Logger.Verbose(messageTemplate, propertyValues);

        /// <inheritdoc/>
        public void Verbose(Exception exception, string messageTemplate)
            => Logger.Verbose(exception, messageTemplate);

        /// <inheritdoc/>
        public void Verbose<T>(Exception exception, string messageTemplate, T propertyValue)
            => Logger.Verbose<T>(exception, messageTemplate, propertyValue);

        /// <inheritdoc/>
        public void Verbose<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
            => Logger.Verbose<T0, T1>(exception, messageTemplate, propertyValue0, propertyValue1);

        /// <inheritdoc/>
        public void Verbose<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
            => Logger.Verbose<T0, T1, T2>(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

        /// <inheritdoc/>
        public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues)
            => Logger.Verbose(exception, messageTemplate, propertyValues);

        /// <inheritdoc/>
        public void Debug(string messageTemplate)
            => Logger.Debug(messageTemplate);

        /// <inheritdoc/>
        public void Debug<T>(string messageTemplate, T propertyValue)
            => Logger.Debug<T>(messageTemplate, propertyValue);

        /// <inheritdoc/>
        public void Debug<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
            => Logger.Debug<T0, T1>(messageTemplate, propertyValue0, propertyValue1);

        /// <inheritdoc/>
        public void Debug<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
            => Logger.Debug<T0, T1, T2>(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

        /// <inheritdoc/>
        public void Debug(string messageTemplate, params object[] propertyValues)
            => Logger.Debug(messageTemplate, propertyValues);

        /// <inheritdoc/>
        public void Debug(Exception exception, string messageTemplate)
            => Logger.Debug(exception, messageTemplate);

        /// <inheritdoc/>
        public void Debug<T>(Exception exception, string messageTemplate, T propertyValue)
            => Logger.Debug<T>(exception, messageTemplate, propertyValue);

        /// <inheritdoc/>
        public void Debug<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
            => Logger.Debug<T0, T1>(exception, messageTemplate, propertyValue0, propertyValue1);

        /// <inheritdoc/>
        public void Debug<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
            => Logger.Debug<T0, T1, T2>(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

        /// <inheritdoc/>
        public void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
            => Logger.Debug(exception, messageTemplate, propertyValues);

        /// <inheritdoc/>
        public void Information(string messageTemplate)
            => Logger.Information(messageTemplate);

        /// <inheritdoc/>
        public void Information<T>(string messageTemplate, T propertyValue)
            => Logger.Information<T>(messageTemplate, propertyValue);

        /// <inheritdoc/>
        public void Information<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
            => Logger.Information<T0, T1>(messageTemplate, propertyValue0, propertyValue1);

        /// <inheritdoc/>
        public void Information<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
            => Logger.Information<T0, T1, T2>(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

        /// <inheritdoc/>
        public void Information(string messageTemplate, params object[] propertyValues)
            => Logger.Information(messageTemplate, propertyValues);

        /// <inheritdoc/>
        public void Information(Exception exception, string messageTemplate)
            => Logger.Information(exception, messageTemplate);

        /// <inheritdoc/>
        public void Information<T>(Exception exception, string messageTemplate, T propertyValue)
            => Logger.Information<T>(exception, messageTemplate, propertyValue);

        /// <inheritdoc/>
        public void Information<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
            => Logger.Information<T0, T1>(exception, messageTemplate, propertyValue0, propertyValue1);

        /// <inheritdoc/>
        public void Information<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
            => Logger.Information<T0, T1, T2>(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

        /// <inheritdoc/>
        public void Information(Exception exception, string messageTemplate, params object[] propertyValues)
            => Logger.Information(exception, messageTemplate, propertyValues);

        /// <inheritdoc/>
        public void Warning(string messageTemplate)
            => Logger.Warning(messageTemplate);

        /// <inheritdoc/>
        public void Warning<T>(string messageTemplate, T propertyValue)
            => Logger.Warning<T>(messageTemplate, propertyValue);

        /// <inheritdoc/>
        public void Warning<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
            => Logger.Warning<T0, T1>(messageTemplate, propertyValue0, propertyValue1);

        /// <inheritdoc/>
        public void Warning<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
            => Logger.Warning<T0, T1, T2>(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

        /// <inheritdoc/>
        public void Warning(string messageTemplate, params object[] propertyValues)
            => Logger.Warning(messageTemplate, propertyValues);

        /// <inheritdoc/>
        public void Warning(Exception exception, string messageTemplate)
            => Logger.Warning(exception, messageTemplate);

        /// <inheritdoc/>
        public void Warning<T>(Exception exception, string messageTemplate, T propertyValue)
            => Logger.Warning<T>(exception, messageTemplate, propertyValue);

        /// <inheritdoc/>
        public void Warning<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
            => Logger.Warning<T0, T1>(exception, messageTemplate, propertyValue0, propertyValue1);

        /// <inheritdoc/>
        public void Warning<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
            => Logger.Warning<T0, T1, T2>(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

        /// <inheritdoc/>
        public void Warning(Exception exception, string messageTemplate, params object[] propertyValues)
            => Logger.Warning(exception, messageTemplate, propertyValues);

        /// <inheritdoc/>
        public void Error(string messageTemplate)
            => Logger.Error(messageTemplate);

        /// <inheritdoc/>
        public void Error<T>(string messageTemplate, T propertyValue)
            => Logger.Error<T>(messageTemplate, propertyValue);

        /// <inheritdoc/>
        public void Error<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
            => Logger.Error<T0, T1>(messageTemplate, propertyValue0, propertyValue1);

        /// <inheritdoc/>
        public void Error<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
            => Logger.Error<T0, T1, T2>(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

        /// <inheritdoc/>
        public void Error(string messageTemplate, params object[] propertyValues)
            => Logger.Error(messageTemplate, propertyValues);

        /// <inheritdoc/>
        public void Error(Exception exception, string messageTemplate)
            => Logger.Error(exception, messageTemplate);

        /// <inheritdoc/>
        public void Error<T>(Exception exception, string messageTemplate, T propertyValue)
            => Logger.Error<T>(exception, messageTemplate, propertyValue);

        /// <inheritdoc/>
        public void Error<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
            => Logger.Error<T0, T1>(exception, messageTemplate, propertyValue0, propertyValue1);

        /// <inheritdoc/>
        public void Error<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
            => Logger.Error<T0, T1, T2>(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

        /// <inheritdoc/>
        public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
            => Logger.Error(exception, messageTemplate, propertyValues);

        /// <inheritdoc/>
        public void Fatal(string messageTemplate)
            => Logger.Fatal(messageTemplate);

        /// <inheritdoc/>
        public void Fatal<T>(string messageTemplate, T propertyValue)
            => Logger.Fatal<T>(messageTemplate, propertyValue);

        /// <inheritdoc/>
        public void Fatal<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
            => Logger.Fatal<T0, T1>(messageTemplate, propertyValue0, propertyValue1);

        /// <inheritdoc/>
        public void Fatal<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
            => Logger.Fatal<T0, T1, T2>(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

        /// <inheritdoc/>
        public void Fatal(string messageTemplate, params object[] propertyValues)
            => Logger.Fatal(messageTemplate, propertyValues);

        /// <inheritdoc/>
        public void Fatal(Exception exception, string messageTemplate)
            => Logger.Fatal(exception, messageTemplate);

        /// <inheritdoc/>
        public void Fatal<T>(Exception exception, string messageTemplate, T propertyValue)
            => Logger.Fatal<T>(exception, messageTemplate, propertyValue);

        /// <inheritdoc/>
        public void Fatal<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
            => Logger.Fatal<T0, T1>(exception, messageTemplate, propertyValue0, propertyValue1);

        /// <inheritdoc/>
        public void Fatal<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
            => Logger.Fatal<T0, T1, T2>(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

        /// <inheritdoc/>
        public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
            => Logger.Fatal(exception, messageTemplate, propertyValues);

        /// <inheritdoc/>
        public bool BindMessageTemplate(string messageTemplate, object[] propertyValues, out MessageTemplate parsedTemplate, out IEnumerable<LogEventProperty> boundProperties)
            => Logger.BindMessageTemplate(messageTemplate, propertyValues, out parsedTemplate, out boundProperties);

        /// <inheritdoc/>
        public bool BindProperty(string propertyName, object value, bool destructureObjects, out LogEventProperty property)
            => Logger.BindProperty(propertyName, value, destructureObjects, out property);

        /// <summary>Dispose last attached logger</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Dispose last attached logger</summary>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing) (logger as IDisposable)?.Dispose();
        }

        /// <summary>Class that applies logger decorator</summary>
        public class LoggerDecorator
        {
            /// <summary>Logger provider</summary>
            Func<ILogger> loggerProvider;
            /// <summary>Decorator function</summary>
            Func<ILogger, ILogger> decorator;
            /// <summary>Previous logger</summary>
            ILogger prevLogger;
            /// <summary><see cref="prevLogger"/> decorated</summary>
            ILogger prevDecoratedLogger;

            /// <summary>Create logger decorator</summary>
            /// <param name="loggerProvider"></param>
            /// <param name="decorator"></param>
            public LoggerDecorator(Func<ILogger> loggerProvider, Func<ILogger, ILogger> decorator)
            {
                this.loggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
                this.decorator = decorator ?? throw new ArgumentNullException(nameof(decorator));
            }

            /// <summary>Get decorated logger.</summary>
            /// <returns>Decorated logger</returns>
            public ILogger Get()
            {
                // Get current logger
                ILogger currentLogger = loggerProvider();
                // Decorate logger
                if (currentLogger != prevLogger)
                {
                    prevDecoratedLogger = currentLogger == null ? null : decorator(currentLogger);
                    prevLogger = currentLogger;
                }
                // Return decorated logger
                return prevDecoratedLogger;
            }
        }
    }
}
