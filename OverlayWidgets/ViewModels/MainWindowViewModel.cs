using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using OverlayWidgets.Models;
using OverlayWidgets.Services;
using OverlayWidgets.Widgets;

namespace OverlayWidgets.ViewModels;

public sealed class MainWindowViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly IWidgetRegistryService _widgetRegistryService;
    private readonly ILoggerService _logger;
    private readonly Dictionary<string, WidgetSettings> _settingsByType = new(StringComparer.OrdinalIgnoreCase);
    private bool _isEditMode;
    private bool _isHotkeyAvailable = true;

    public MainWindowViewModel(ISettingsService settingsService, IWidgetRegistryService widgetRegistryService, ILoggerService logger)
    {
        _settingsService = settingsService;
        _widgetRegistryService = widgetRegistryService;
        _logger = logger;
        ToggleEditModeCommand = new RelayCommand(_ => IsEditMode = !IsEditMode);

        LoadWidgets();
    }

    public ObservableCollection<WidgetHostViewModel> Widgets { get; } = [];
    public ObservableCollection<WidgetOptionViewModel> WidgetOptions { get; } = [];

    public ICommand ToggleEditModeCommand { get; }

    public bool IsHotkeyAvailable
    {
        get => _isHotkeyAvailable;
        set => SetProperty(ref _isHotkeyAvailable, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set
        {
            if (!SetProperty(ref _isEditMode, value))
            {
                return;
            }

            foreach (var widget in Widgets)
            {
                widget.IsEditMode = value;
            }
        }
    }

    public void Save()
    {
        KeepWidgetsInsideVisibleArea();

        foreach (var widget in Widgets)
        {
            var widgetSettings = widget.ToSettings();
            widgetSettings.IsEnabled = true;
            _settingsByType[widgetSettings.Type] = widgetSettings;
        }

        foreach (var option in WidgetOptions)
        {
            if (!_settingsByType.TryGetValue(option.Type, out var widgetSettings))
            {
                widgetSettings = CreateFallbackWidgetSettings(option.Type);
            }

            widgetSettings.IsEnabled = option.IsEnabled;
            _settingsByType[option.Type] = widgetSettings;
        }

        var settings = new AppSettings
        {
            IsEditMode = IsEditMode,
            Widgets = WidgetOptions
                .Select(option => _settingsByType[option.Type])
                .ToList()
        };

        _settingsService.Save(settings);
    }

    private void LoadWidgets()
    {
        var settings = _settingsService.Load();

        foreach (var item in settings.Widgets)
        {
            if (string.IsNullOrWhiteSpace(item.Type))
            {
                continue;
            }

            _settingsByType[item.Type] = item;
        }

        foreach (var descriptor in _widgetRegistryService.GetAvailableWidgets())
        {
            if (!_settingsByType.TryGetValue(descriptor.Type, out var widgetSettings))
            {
                widgetSettings = CreateFallbackWidgetSettings(descriptor.Type);
                _settingsByType[descriptor.Type] = widgetSettings;
            }

            var isEnabled = widgetSettings.IsEnabled ?? true;
            var option = new WidgetOptionViewModel(descriptor.Type, descriptor.DisplayName, isEnabled);
            option.PropertyChanged += OnWidgetOptionPropertyChanged;
            WidgetOptions.Add(option);

            if (isEnabled)
            {
                AddWidget(widgetSettings);
            }
        }

        IsEditMode = settings.IsEditMode;
        KeepWidgetsInsideVisibleArea();
    }

    private void OnWidgetOptionPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != nameof(WidgetOptionViewModel.IsEnabled) || sender is not WidgetOptionViewModel option)
        {
            return;
        }

        if (option.IsEnabled)
        {
            EnableWidget(option.Type);
        }
        else
        {
            DisableWidget(option.Type);
        }
    }

    private void EnableWidget(string type)
    {
        if (Widgets.Any(widget => string.Equals(widget.Type, type, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        if (!_settingsByType.TryGetValue(type, out var widgetSettings))
        {
            widgetSettings = CreateFallbackWidgetSettings(type);
        }

        widgetSettings.IsEnabled = true;
        _settingsByType[type] = widgetSettings;
        AddWidget(widgetSettings);
        KeepWidgetsInsideVisibleArea();
    }

    private void DisableWidget(string type)
    {
        var widget = Widgets.FirstOrDefault(item => string.Equals(item.Type, type, StringComparison.OrdinalIgnoreCase));
        if (widget is null)
        {
            if (_settingsByType.TryGetValue(type, out var disabledSettings))
            {
                disabledSettings.IsEnabled = false;
            }

            return;
        }

        var widgetSettings = widget.ToSettings();
        widgetSettings.IsEnabled = false;
        _settingsByType[type] = widgetSettings;
        Widgets.Remove(widget);
    }

    private void AddWidget(WidgetSettings widgetSettings)
    {
        if (!_widgetRegistryService.TryCreate(widgetSettings.Type, out var widget) || widget is null)
        {
            _logger.Warning($"Skipping unknown widget type from settings: {widgetSettings.Type}");
            return;
        }

        var host = new WidgetHostViewModel(widgetSettings, widget)
        {
            IsEditMode = IsEditMode
        };

        Widgets.Add(host);
    }

    public void KeepWidgetsInsideVisibleArea(double? width = null, double? height = null)
    {
        var maxWidth = width ?? SystemParameters.WorkArea.Width;
        var maxHeight = height ?? SystemParameters.WorkArea.Height;

        foreach (var widget in Widgets)
        {
            widget.KeepInside(maxWidth, maxHeight);
        }
    }

    private WidgetSettings CreateFallbackWidgetSettings(string type)
    {
        var index = _settingsByType.Count;
        var offset = index * 24;

        return type switch
        {
            WidgetTypes.Clock => new WidgetSettings
            {
                Type = WidgetTypes.Clock,
                IsEnabled = true,
                Left = 40,
                Top = 40,
                Width = 260,
                Height = 110
            },
            WidgetTypes.Media => new WidgetSettings
            {
                Type = WidgetTypes.Media,
                IsEnabled = true,
                Left = 40,
                Top = 170,
                Width = 320,
                Height = 120
            },
            _ => new WidgetSettings
            {
                Type = type,
                IsEnabled = true,
                Left = 40 + offset,
                Top = 40 + offset,
                Width = 260,
                Height = 110
            }
        };
    }
}
