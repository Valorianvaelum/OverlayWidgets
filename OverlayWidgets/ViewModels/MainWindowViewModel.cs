using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using OverlayWidgets.Models;
using OverlayWidgets.Services;

namespace OverlayWidgets.ViewModels;

public sealed class MainWindowViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly IWidgetRegistryService _widgetRegistryService;
    private readonly ILoggerService _logger;
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

        var settings = new AppSettings
        {
            IsEditMode = IsEditMode,
            Widgets = Widgets.Select(widget => widget.ToSettings()).ToList()
        };

        _settingsService.Save(settings);
    }

    private void LoadWidgets()
    {
        var settings = _settingsService.Load();
        var widgetSettings = settings.Widgets.Count > 0 ? settings.Widgets : new AppSettings().Widgets;

        foreach (var item in widgetSettings)
        {
            if (!_widgetRegistryService.TryCreate(item.Type, out var widget) || widget is null)
            {
                _logger.Warning($"Skipping unknown widget type from settings: {item.Type}");
                continue;
            }

            Widgets.Add(new WidgetHostViewModel(item, widget));
        }

        IsEditMode = settings.IsEditMode;
        KeepWidgetsInsideVisibleArea();
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
}
