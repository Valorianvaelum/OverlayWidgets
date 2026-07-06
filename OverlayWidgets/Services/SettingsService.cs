using System.IO;
using System.Text.Json;
using OverlayWidgets.Models;
using OverlayWidgets.Widgets;

namespace OverlayWidgets.Services;

public sealed class SettingsService : ISettingsService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly string _settingsPath;
    private readonly ILoggerService _logger;

    public SettingsService(ILoggerService logger)
    {
        _logger = logger;
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _settingsPath = Path.Combine(appData, "OverlayWidgets", "settings.json");
    }

    public AppSettings Load()
    {
        if (!File.Exists(_settingsPath))
        {
            _logger.Info("settings.json not found. Loading default settings.");
            return NormalizeSettings(CreateDefaultSettings());
        }

        try
        {
            var json = File.ReadAllText(_settingsPath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, SerializerOptions);
            return NormalizeSettings(settings ?? CreateDefaultSettings());
        }
        catch (Exception exception)
        {
            _logger.Error("settings.json is invalid. Creating backup and loading default settings.", exception);
            BackupCorruptSettings();
            return NormalizeSettings(CreateDefaultSettings());
        }
    }

    public void Save(AppSettings settings)
    {
        try
        {
            var directory = Path.GetDirectoryName(_settingsPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(NormalizeSettings(settings), SerializerOptions);
            File.WriteAllText(_settingsPath, json);
            _logger.Info("Settings saved.");
        }
        catch (Exception exception)
        {
            _logger.Error("Could not save settings.", exception);
        }
    }

    private void BackupCorruptSettings()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                return;
            }

            var directory = Path.GetDirectoryName(_settingsPath);
            if (string.IsNullOrWhiteSpace(directory))
            {
                return;
            }

            Directory.CreateDirectory(directory);
            var backupPath = Path.Combine(directory, $"settings.corrupt.{DateTime.Now:yyyyMMdd-HHmmss}.json");
            File.Copy(_settingsPath, backupPath, overwrite: false);
            _logger.Warning($"Corrupt settings backup created: {backupPath}");
        }
        catch (Exception exception)
        {
            _logger.Error("Could not create corrupt settings backup.", exception);
        }
    }

    private static AppSettings NormalizeSettings(AppSettings settings)
    {
        var defaultSettings = CreateDefaultSettings();
        var normalizedWidgets = new List<WidgetSettings>();
        var inputWidgets = settings.Widgets ?? [];

        foreach (var defaultWidget in defaultSettings.Widgets)
        {
            var configuredWidget = inputWidgets.FirstOrDefault(widget => string.Equals(widget.Type, defaultWidget.Type, StringComparison.OrdinalIgnoreCase));
            normalizedWidgets.Add(NormalizeWidget(configuredWidget ?? defaultWidget, defaultWidget));
        }

        foreach (var configuredWidget in inputWidgets)
        {
            if (string.IsNullOrWhiteSpace(configuredWidget.Type) ||
                normalizedWidgets.Any(widget => string.Equals(widget.Type, configuredWidget.Type, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            normalizedWidgets.Add(NormalizeWidget(configuredWidget, configuredWidget));
        }

        settings.Widgets = normalizedWidgets;
        return settings;
    }

    private static WidgetSettings NormalizeWidget(WidgetSettings widget, WidgetSettings fallback)
    {
        return new WidgetSettings
        {
            Id = string.IsNullOrWhiteSpace(widget.Id) ? Guid.NewGuid().ToString("N") : widget.Id,
            Type = string.IsNullOrWhiteSpace(widget.Type) ? fallback.Type : widget.Type,
            IsEnabled = widget.IsEnabled,
            Left = SanitizeNumber(widget.Left, fallback.Left),
            Top = SanitizeNumber(widget.Top, fallback.Top),
            Width = Math.Clamp(SanitizeNumber(widget.Width, fallback.Width), 120, 900),
            Height = Math.Clamp(SanitizeNumber(widget.Height, fallback.Height), 72, 600)
        };
    }

    private static double SanitizeNumber(double value, double fallback)
    {
        return double.IsFinite(value) ? value : fallback;
    }

    private static AppSettings CreateDefaultSettings()
    {
        return new AppSettings
        {
            IsEditMode = true,
            Widgets =
            [
                new WidgetSettings
                {
                    Type = WidgetTypes.Clock,
                    IsEnabled = true,
                    Left = 40,
                    Top = 40,
                    Width = 260,
                    Height = 110
                },
                new WidgetSettings
                {
                    Type = WidgetTypes.Media,
                    IsEnabled = true,
                    Left = 40,
                    Top = 170,
                    Width = 320,
                    Height = 120
                }
            ]
        };
    }
}
