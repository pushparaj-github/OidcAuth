using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace OidcAuth;

public class FileLogger : ILogger
{
    private readonly string _categoryName;
    private readonly StreamWriter _logFileWriter;
    
    public FileLogger(string categoryName, StreamWriter logFileWriter)
    {
        _categoryName = categoryName;
        _logFileWriter = logFileWriter;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var logRecord = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {_categoryName} - {formatter(state, exception)}";
        _logFileWriter.WriteLine(logRecord);
        _logFileWriter.Flush();
    }
}

public class FileLoggerProvider : ILoggerProvider
{
    private readonly StreamWriter _logFileWriter;

    public FileLoggerProvider(string logFilePath)
    {
        _logFileWriter = new StreamWriter(logFilePath, append: true);
    }

    public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName, _logFileWriter);

    public void Dispose() => _logFileWriter.Dispose();
}
