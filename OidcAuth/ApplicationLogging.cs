using Microsoft.Extensions.Logging;
using System;
using System.Configuration;

namespace OidcAuth;

public static class ApplicationLogging
{
    private static ILoggerFactory _loggerFactory;

    public static void ConfigureLogger()
    {

        var logFileName = ConfigurationManager.AppSettings["GPTAssemblyLog"];
        if (File.Exists(logFileName))
        {
            File.WriteAllText(logFileName, string.Empty); // This will clear the file content
        }

        _loggerFactory = LoggerFactory.Create(builder =>
        {
            // Add your custom FileLoggerProvider or any other logging providers here
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddProvider(new FileLoggerProvider(logFileName));
        });
    }

    // Ensure LoggerFactory is initialized before accessing
    public static ILogger<T> CreateLogger<T>() where T : class
    {
        if (_loggerFactory == null)
        {
            throw new InvalidOperationException("LoggerFactory is not configured. Call ConfigureLogger first.");
        }
        return _loggerFactory.CreateLogger<T>();
    }
}
