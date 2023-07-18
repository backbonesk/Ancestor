using Microsoft.Extensions.Logging;
using Sentry;

namespace Ancestor.Logging
{
    public static class Extensions
    {
        public static SentryLevel ToSentryLevel(this LogLevel logLevel)
        {
            SentryLevel sentryLogLevel;
            
            switch (logLevel)
            {
                case LogLevel.None:
                    sentryLogLevel = SentryLevel.Info;
                    break;
                case LogLevel.Information:
                    sentryLogLevel = SentryLevel.Info;
                    break;
                case LogLevel.Trace:
                    sentryLogLevel = SentryLevel.Info;
                    break;
                case LogLevel.Debug:
                    sentryLogLevel = SentryLevel.Debug;
                    break;
                case LogLevel.Warning:
                    sentryLogLevel = SentryLevel.Warning;
                    break;
                case LogLevel.Error:
                    sentryLogLevel = SentryLevel.Error;
                    break;
                case LogLevel.Critical:
                    sentryLogLevel = SentryLevel.Fatal;
                    break;
                default:
                    sentryLogLevel = SentryLevel.Info;
                    break;
            }

            return sentryLogLevel;
        }
    }
}