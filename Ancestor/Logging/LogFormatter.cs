using System;
using System.Collections.Generic;
using Ancestor.Extensions;
using Sentry.Protocol;

namespace Ancestor.Logging
{
    public static class LogFormatter
    {
        public static string Format(Exception exception, SentryLevel logLevel = SentryLevel.Error)
        {
            var data = new Dictionary<object, object>();

            if (exception != null)
            {
                var i = 0;
                
                exception.WithAllInnerExceptions(x =>
                {
                    foreach (var key in x.Data.Keys)
                    { 
                        data.Add($"exception[{i}].{key}", x.Data[key]);
                    }
                
                    i++;
                });
            
                foreach (var key in exception.Data.Keys)
                { 
                    data.Add($"exception[{i}].{key}", exception.Data[key]);
                }
            }

            return new
            {
                CreatedAt = DateTime.Now,
                ExceptionMessage = exception?.GetFullMessage(),
                exception?.StackTrace,
                data,
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
