using System.IO;
using System.Windows;
using System.Windows.Threading;
using OverlayWidgets.Views;

namespace OverlayWidgets;

public partial class App : Application
{
    private const string AppName = "OverlayWidgets";

    private void OnStartup(object sender, StartupEventArgs e)
    {
        try
        {
            ShutdownMode = ShutdownMode.OnMainWindowClose;

            var mainWindow = new MainWindow();
            MainWindow = mainWindow;
            mainWindow.Show();
            mainWindow.Activate();
        }
        catch (Exception exception)
        {
            ReportFatalError("OverlayWidgets no pudo iniciar.", exception);
            Shutdown(-1);
        }
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        ReportFatalError("OverlayWidgets encontro un error inesperado.", e.Exception);
        e.Handled = true;
        Shutdown(-1);
    }

    private static void ReportFatalError(string message, Exception exception)
    {
        try
        {
            var directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                AppName,
                "logs");

            Directory.CreateDirectory(directory);
            var logPath = Path.Combine(directory, $"startup-{DateTime.Now:yyyy-MM-dd-HHmmss}.log");
            File.WriteAllText(logPath, $"{message}{Environment.NewLine}{exception}");

            MessageBox.Show(
                $"{message}{Environment.NewLine}{Environment.NewLine}{exception.Message}{Environment.NewLine}{Environment.NewLine}Log: {logPath}",
                AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        catch
        {
            MessageBox.Show(
                $"{message}{Environment.NewLine}{Environment.NewLine}{exception.Message}",
                AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
