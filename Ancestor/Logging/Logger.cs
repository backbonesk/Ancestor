using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using Ancestor.Extensions;
using Sentry;
using Sentry.Protocol;

namespace Ancestor.Logging
{
    public static class Logger
    {
        public static string SentryEnvironment = null;
        public static string Identifier = Guid.NewGuid().ToString();
        private const string LogsPath = "./Logs";
        private const string ZipFileExtension = ".zip";

        private static readonly ReaderWriterLock Locker = new ReaderWriterLock();

        public static void LogToErrorOutput(Exception exception, SentryLevel logLevel = SentryLevel.Error)
        {
            var log = LogFormatter.Format(exception, logLevel);
            WriteToFile(log);
            Console.Error.WriteLine(log);
        }

        public static void LogToErrorOutput(object value, SentryLevel logLevel = SentryLevel.Error, bool format = true)
        {
            var log = format ? LogFormatter.Format(value.ToJson(), logLevel) : (value?.ToString() ?? "null");
            WriteToFile(log);
            Console.Error.WriteLine(log);
        }

        public static void LogToStandardOutput(object value, SentryLevel logLevel = SentryLevel.Info, bool format = true)
        {
            var log = format ? LogFormatter.Format(value.ToJson(), logLevel) : (value?.ToString() ?? "null");
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

                if (!Directory.Exists(LogsPath))
                {
                    Directory.CreateDirectory(LogsPath);
                }

                CompressLogs();

                File.AppendAllText($"{LogsPath}/log{DateTime.Now:yyyy-MM-dd}_{Identifier}.txt", log + "\n");
            }
            finally
            {
                Locker.ReleaseWriterLock();
            }
        }

        private static void CompressLogs()
        {
            string[] files = Directory.GetFiles(LogsPath);

            List<FileInfo> filesToZip = new List<FileInfo>();

            foreach (var fileName in files)
            {
                FileInfo file = new FileInfo(fileName);

                if (file.CreationTime.Month != DateTime.Now.Month && !file.Name.EndsWith(ZipFileExtension))
                {
                    filesToZip.Add(file);
                }
            }

            if (filesToZip.Any())
            {
                ZipLogs(filesToZip, $"logs_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}{ZipFileExtension}");
            }
        }

        private static void ZipLogs(IEnumerable<FileInfo> files, string archiveName)
        {
            using var stream = File.OpenWrite(Path.Combine(LogsPath, archiveName));

            using ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create);
            
            foreach (var item in files)
            {
                archive.CreateEntryFromFile(item.FullName, item.Name, CompressionLevel.Optimal);
                item.Delete();
            }
        }
    }
}