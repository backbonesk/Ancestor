using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Ancestor.Extensions
{
    public static class AppExtensions
    {
        public static readonly CultureInfo CultureInfo = new CultureInfo("en-GB")
        {
            NumberFormat =
            {
                NumberDecimalSeparator = ".",
                CurrencyDecimalSeparator = "."
            }
        };

        public static void SetDefaults()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo;

            JsonConvert.DefaultSettings = () =>
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                StringEnumConverter enumConverter = new StringEnumConverter
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
                settings.Converters.Add(enumConverter);
                settings.Formatting = Formatting.Indented;
                return settings;
            };

            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location));
        }

        public static bool IsEnvironmentVariableSet(string key)
        {
            string value = GetEnvironmentVariable(key);
            return value != null;
        }

        public static string GetEnvironmentVariable(string key)
        {
            return
                Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Machine) ??
                Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
        }

        public static void SetEnvironmentVariable(string key, string newEnvironmentVariable)
        {
            Environment.SetEnvironmentVariable(key, newEnvironmentVariable, EnvironmentVariableTarget.Machine);
        }
    }
}
