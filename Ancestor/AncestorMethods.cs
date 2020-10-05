using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Ancestor
{
    public static class AncestorMethods
    { 
        public static string ToJson(this object input)
        {
            return JsonConvert.SerializeObject(input, Formatting.Indented);
        }

        public static T FromJson<T>(this string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }

        public static void WithAllInnerExceptions(this Exception exception, Action<Exception> execute)
        {
            var innerExceptionTemp = exception.InnerException;

            while (innerExceptionTemp != null)
            {
                execute(innerExceptionTemp);
                innerExceptionTemp = innerExceptionTemp.InnerException;
            }
        }

        public static string GetFullMessage(this Exception exception)
        {
            var sb = new StringBuilder("Error occured: ");

            var errors = new List<string> {exception.Message};

            exception.WithAllInnerExceptions(innerException =>
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