using System;
using System.Collections.Generic;
using System.IO;

namespace RednaConsole
{
    public class LoggerFactory
    {

        public static List<ILogger> RegisteredLoggers = new List<ILogger>();
        public static void RegisterLogger(ILogger logger)
        {
            RegisteredLoggers.Add(logger);
        }

        internal static void RegisterLogger(object instance)
        {
            throw new NotImplementedException();
        }

        public static ILogger<T> GetLogger<T>()
        {
            var listenLogger = new ListenLogger();
            return new Logger<T>(listenLogger);
        }
    }


    public class Logger<T> : ILogger<T>
    {
        private ILogger logger;
        public Logger(ILogger innerLogger)
        {
            this.logger = innerLogger;
        }

        public void Log(string s) => logger.Log(s);
        public void Log(string format, params object[] args) => logger.Log(format, args);
        public void LogDebug(string s) => logger.LogDebug(s);
        public void LogDebug(string format, params object[] args) => logger.LogDebug(format, args);
        public void LogError(string s) => logger.LogError(s);
        public void LogError(string format, params object[] args) => logger.LogError(format, args);
        public void LogInfo(string s) => logger.LogInfo(s);
        public void LogInfo(string format, params object[] args) => logger.LogInfo(format, args);
        public void LogWarn(string s) => logger.LogWarn(s);
        public void LogWarn(string format, params object[] args) => logger.LogWarn(format, args);
    }


    public class ConsoleLogger : StreamLogger
    {
        private ConsoleLogger() : base(Console.Out) { }

        static ILogger instance = new ConsoleLogger();
        public static ILogger Instance => instance;
    }
    public class StreamLogger : ILogger
    {

        private TextWriter writer;
        public StreamLogger(Stream baseStream)
        {
            this.writer = new StreamWriter(baseStream);
        }
        public StreamLogger(TextWriter writer)
        {
            this.writer = writer;
        }
        public void Log(string s) => writer.WriteLine($"{DateTime.Now}: {s}");

        public void Log(string format, params object[] args) => Log(string.Format(format, args));
        public void LogDebug(string s) => Log($"[DEBUG] {s}");
        public void LogDebug(string format, params object[] args) => LogDebug(string.Format(format, args));
        public void LogError(string s) => Log($"[ERROR] {s}");
        public void LogError(string format, params object[] args) => LogError(string.Format(format, args));
        public void LogInfo(string s) => Log($"[INFO] {s}");
        public void LogInfo(string format, params object[] args) => LogInfo(string.Format(format, args));
        public void LogWarn(string s) => Log($"[WARN] {s}");
        public void LogWarn(string format, params object[] args) => LogWarn(string.Format(format, args));
    }



    public interface ILogger<T> : ILogger
    {

    }
    public interface ILogger
    {
        void Log(string s);
        void Log(string format, params object[] args);
        void LogDebug(string s);
        void LogDebug(string format, params object[] args);
        void LogInfo(string s);
        void LogInfo(string format, params object[] args);
        void LogError(string s);
        void LogError(string format, params object[] args);
        void LogWarn(string s);
        void LogWarn(string format, params object[] args);
    }
    public class ListenLogger : ILogger
    {

        private List<ILogger> loggers => LoggerFactory.RegisteredLoggers;
        public void Log(string s) => loggers.ForEach(logger => logger.Log(s));
        public void Log(string format, params object[] args) => loggers.ForEach(logger => logger.Log(format, args));
        public void LogDebug(string s) => loggers.ForEach(logger => logger.LogDebug(s));
        public void LogDebug(string format, params object[] args) => loggers.ForEach(logger => logger.LogDebug(format, args));
        public void LogError(string s) => loggers.ForEach(logger => logger.LogError(s));
        public void LogError(string format, params object[] args) => loggers.ForEach(logger => logger.LogError(format, args));
        public void LogInfo(string s) => loggers.ForEach(logger => logger.LogInfo(s));
        public void LogInfo(string format, params object[] args) => loggers.ForEach(logger => logger.LogInfo(format, args));
        public void LogWarn(string s) => loggers.ForEach(logger => logger.LogWarn(s));
        public void LogWarn(string format, params object[] args) => loggers.ForEach(logger => logger.LogWarn(format, args));
    }
}