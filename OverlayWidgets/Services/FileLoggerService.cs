using System.IO;

namespace OverlayWidgets.Services;

public sealed class FileLoggerService : ILoggerService
{
    private readonly string _logDirectory;
    private readonly object _syncRoot = new();

    public FileLoggerService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _logDirectory = Path.Combine(appData, "OverlayWidgets", "logs");
    }

    public void Info(string message)
    {
        Write("INFO", message);
    }

    public void Warning(string message)
    {
        Write("WARN", message);
    }

    public void Error(string message, Exception? exception = null)
    {
        var details = exception is null ? message : $"{message}{Environment.NewLine}{exception}";
        Write("ERROR", details);
    }

    private void Write(string level, string message)
    {
        try
        {
            Directory.CreateDirectory(_logDirectory);
            var logPath = Path.Combine(_logDirectory, $"{DateTime.Now:yyyy-MM-dd}.log");
            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}{Environment.NewLine}";

            lock (_syncRoot)
            {
                File.AppendAllText(logPath, line);
            }
        }
        catch
        {
            // Logging must never interrupt the overlay.
        }
    }
}
