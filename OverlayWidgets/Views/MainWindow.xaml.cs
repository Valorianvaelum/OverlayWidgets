using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using OverlayWidgets.Services;
using OverlayWidgets.ViewModels;

namespace OverlayWidgets.Views;

public partial class MainWindow : Window
{
    private const int WmHotkey = 0x0312;
    private const int EditModeHotkeyId = 9001;

    private readonly IWindowOverlayService _windowOverlayService;
    private readonly MainWindowViewModel _viewModel;
    private readonly ILoggerService _logger;
    private HwndSource? _source;
    private Point? _dragStart;
    private WidgetHostViewModel? _draggedWidget;

    public MainWindow()
    {
        InitializeComponent();

        _logger = new FileLoggerService();
        var settingsService = new SettingsService(_logger);
        var mediaSessionService = new MediaSessionService(_logger);
        var widgetRegistryService = new WidgetRegistryService(mediaSessionService, _logger);

        _windowOverlayService = new WindowOverlayService(_logger);
        _viewModel = new MainWindowViewModel(settingsService, widgetRegistryService, _logger);
        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(MainWindowViewModel.IsEditMode))
            {
                _windowOverlayService.ApplyMode(this, _viewModel.IsEditMode);
            }
        };

        DataContext = _viewModel;
        _windowOverlayService.Configure(this);
        SourceInitialized += OnSourceInitialized;
        Closing += OnClosing;
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        try
        {
            _source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            _source?.AddHook(OnWindowMessage);
            _viewModel.IsHotkeyAvailable = _windowOverlayService.RegisterEditModeHotkey(this);
            HotkeyUnavailableText.Visibility = _viewModel.IsHotkeyAvailable ? Visibility.Collapsed : Visibility.Visible;
            _viewModel.KeepWidgetsInsideVisibleArea(Width, Height);
            _windowOverlayService.ApplyMode(this, _viewModel.IsEditMode);
            _logger.Info("OverlayWidgets started.");
        }
        catch (Exception exception)
        {
            _logger.Error("Could not complete main window initialization.", exception);
        }
    }

    private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        try
        {
            _source?.RemoveHook(OnWindowMessage);
            _windowOverlayService.UnregisterEditModeHotkey(this);
            _viewModel.Save();
            _logger.Info("OverlayWidgets closed.");
        }
        catch (Exception exception)
        {
            _logger.Error("OverlayWidgets failed during shutdown.", exception);
        }
    }

    private IntPtr OnWindowMessage(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (message == WmHotkey && wParam.ToInt32() == EditModeHotkeyId)
        {
            _viewModel.IsEditMode = !_viewModel.IsEditMode;
            handled = true;
        }

        return IntPtr.Zero;
    }

    private void Widget_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!_viewModel.IsEditMode || sender is not Border border || border.DataContext is not WidgetHostViewModel widget)
        {
            return;
        }

        _draggedWidget = widget;
        _dragStart = e.GetPosition(this);
        border.CaptureMouse();
        e.Handled = true;
    }

    private void Widget_MouseMove(object sender, MouseEventArgs e)
    {
        if (_dragStart is null || _draggedWidget is null || e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        var currentPosition = e.GetPosition(this);
        var offset = currentPosition - _dragStart.Value;

        _draggedWidget.Left += offset.X;
        _draggedWidget.Top += offset.Y;
        _draggedWidget.KeepInside(ActualWidth, ActualHeight);
        _dragStart = currentPosition;
    }

    private void Widget_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border)
        {
            border.ReleaseMouseCapture();
        }

        _dragStart = null;
        _draggedWidget = null;
        e.Handled = true;
    }

    private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
    {
        _dragStart = null;
        _draggedWidget = null;
        e.Handled = true;
    }

    private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (!_viewModel.IsEditMode || sender is not Thumb thumb || thumb.DataContext is not WidgetHostViewModel widget)
        {
            return;
        }

        widget.Width += e.HorizontalChange;
        widget.Height += e.VerticalChange;
        widget.KeepInside(ActualWidth, ActualHeight);
        e.Handled = true;
    }
}
