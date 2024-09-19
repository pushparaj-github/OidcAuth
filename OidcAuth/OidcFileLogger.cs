using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace OidcAuth
{
    public class OidcFileLogger
    {
        private readonly string _logFilePath;
        private static readonly object _lock = new object();

        public OidcFileLogger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public void Log(string message)
        {
            var logRecord = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            lock (_lock)
            {
                try
                {
                    using (var writer = new StreamWriter(_logFilePath, append: true))
                    {
                        writer.WriteLine(logRecord);
                    }
                }
                catch (IOException ex)
                {
                    // Handle file I/O exceptions
                    Console.Error.WriteLine($"Failed to write log: {ex.Message}");
                }
            }
        }
    }
}

//var logger = new SimpleFileLogger("log.txt");
//logger.Log("This is a log message.");