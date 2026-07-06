using System.Windows;
using OverlayWidgets.Services;
using OverlayWidgets.ViewModels;
using OverlayWidgets.Views.Widgets;

namespace OverlayWidgets.Widgets;

public sealed class MediaWidget : IWidget
{
    private readonly IMediaSessionService _mediaSessionService;

    public MediaWidget(IMediaSessionService mediaSessionService)
    {
        _mediaSessionService = mediaSessionService;
    }

    public string Type => WidgetTypes.Media;
    public string DisplayName => "Multimedia";

    public FrameworkElement CreateView()
    {
        return new MediaWidgetView
        {
            DataContext = new MediaWidgetViewModel(_mediaSessionService)
        };
    }
}
