using System;
using Ancestor.Extensions;
using Sentry.Protocol;

namespace Ancestor.Logging
{
    public static class LogFormatter
    {
        public static string Format(Exception exception, SentryLevel logLevel = SentryLevel.Error)
        {
            return new
            {
                CreatedAt = DateTime.Now,
                ExceptionMessage = exception?.GetFullMessage(),
                exception?.StackTrace,
                exception?.Data,
                LogLevel = logLevel
            }.ToJson();
        }
        public static string Format(object log, SentryLevel logLevel = SentryLevel.Error)
        {
            return new
            {
                CreatedAt = DateTime.Now,
                Log = log,
                LogLevel = logLevel
            }.ToJson();
        }
    }
}
