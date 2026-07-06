namespace OverlayWidgets.Services;

public interface ILoggerService
{
    void Info(string message);
    void Warning(string message);
    void Error(string message, Exception? exception = null);
}
