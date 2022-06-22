using System;
using System.Collections.Generic;
using System.Text;

namespace Ancestor.Extensions
{
    public static class ExceptionExtensions
    {
        public static void WithAllExceptions(this Exception exception, Action<Exception> execute)
        {
            if (exception is AggregateException aggregateException)
            {
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    WithAllExceptions(innerException, execute);
                }
            }
            else
            {
                execute(exception);

                var innerExceptionTemp = exception.InnerException;

                while (innerExceptionTemp != null)
                {
                    if (innerExceptionTemp is AggregateException aggregateInnerException)
                    {
                        WithAllExceptions(aggregateInnerException, execute);
                        innerExceptionTemp = null;
                    }
                    else
                    {
                        execute(innerExceptionTemp);
                        innerExceptionTemp = innerExceptionTemp.InnerException;
                    }
                }
            }
        }

        public static string GetFullMessage(this Exception exception)
        {
            var sb = new StringBuilder("Error occured: ");

            var errors = new List<string> { exception.Message };

            exception.WithAllExceptions(innerException =>
            {
                if (innerException.Message != null)
                {
                    errors.Add(innerException.Message);
                }
            });

            sb.Append(string.Join(" ", errors));

            return sb.ToString();
        }
    }
}
