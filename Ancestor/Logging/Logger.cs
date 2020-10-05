using System;
using System.IO;
using System.Threading;
using Sentry;
using Sentry.Protocol;

namespace Ancestor.Logging
{
    public static class Logger
    {
        public static string SentryEnvironment = null;

        private static string _logsPath = "./Logs";

        private static readonly ReaderWriterLock Locker = new ReaderWriterLock();

        public static void LogToErrorOutput(Exception exception, SentryLevel logLevel = SentryLevel.Error)
        {
            var log = LogFormatter.Format(exception, logLevel);
            WriteToFile(log);
            Console.Error.WriteLine(log);
        }

        public static void LogToErrorOutput(object value, SentryLevel logLevel = SentryLevel.Error)
        {
            var log = LogFormatter.Format(value.ToJson(), logLevel);
            WriteToFile(log);
            Console.Error.WriteLine(log);
        }

        public static void LogToStandardOutput(object value, SentryLevel logLevel = SentryLevel.Info)
        {
            var log = LogFormatter.Format(value.ToJson(), logLevel);
            WriteToFile(log);
            Console.WriteLine(log);
        }

        public static void LogToStandardOutput(Exception exception, SentryLevel logLevel = SentryLevel.Info)
        {
            var log = LogFormatter.Format(exception, logLevel);
            WriteToFile(log);
            Console.WriteLine(log);
        }

        public static void LogToSentry(Exception exception, SentryLevel logLevel = SentryLevel.Error)
        {
            LogToErrorOutput(exception, logLevel);
            WriteToSentry(exception, logLevel);
        }

        private static void WriteToSentry(Exception exception, SentryLevel logLevel = SentryLevel.Error)
        {
            if (SentrySdk.IsEnabled)
            {
                SentrySdk.WithScope(scope =>
                {
                    scope.Environment = SentryEnvironment;
                    scope.Level = logLevel;
                    SentrySdk.CaptureException(exception);
                });
            }
        }

        private static void WriteToFile(string log)
        {
            try
            {
                Locker.AcquireWriterLock(int.MaxValue);

                if (!Directory.Exists(_logsPath))
                {
                    Directory.CreateDirectory(_logsPath);
                }

                RemoveOldLogs();

                File.AppendAllText($"{_logsPath}/log{DateTime.Now:yyyy-MM-dd}.txt", log + "\n");
            }
            finally
            {
                Locker.ReleaseWriterLock();
            }
        }

        private static void RemoveOldLogs()
        {
            string[] files = Directory.GetFiles(_logsPath);

            foreach (var fileName in files)
            {
                FileInfo file = new FileInfo(fileName);
                if (file.LastAccessTime < DateTime.Now.AddDays(-30))
                    file.Delete();
            }
        }
    }
}