using System.Windows;
using OverlayWidgets.Services;
using OverlayWidgets.ViewModels;
using OverlayWidgets.Views.Widgets;

namespace OverlayWidgets.Widgets;

public sealed class MediaWidget : IWidget
{
    private readonly IMediaSessionService _mediaSessionService;
    private readonly ILoggerService _logger;

    public MediaWidget(IMediaSessionService mediaSessionService, ILoggerService logger)
    {
        _mediaSessionService = mediaSessionService;
        _logger = logger;
    }

    public string Type => WidgetTypes.Media;
    public string DisplayName => "Multimedia";

    public FrameworkElement CreateView()
    {
        return new MediaWidgetView
        {
            DataContext = new MediaWidgetViewModel(_mediaSessionService, _logger)
        };
    }
}
