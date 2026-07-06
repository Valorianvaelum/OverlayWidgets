using System.Windows.Threading;
using OverlayWidgets.Services;

namespace OverlayWidgets.ViewModels;

public sealed class MediaWidgetViewModel : ObservableObject
{
    private readonly IMediaSessionService _mediaSessionService;
    private readonly DispatcherTimer _timer;
    private bool _isRefreshing;
    private string _title = "Sin reproduccion";
    private string _subtitle = "No hay una sesion multimedia activa";
    private string _status = "Inactivo";

    public MediaWidgetViewModel(IMediaSessionService mediaSessionService)
    {
        _mediaSessionService = mediaSessionService;
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        _timer.Tick += async (_, _) => await RefreshAsync();
        _timer.Start();
        _ = RefreshAsync();
    }

    public string Title
    {
        get => _title;
        private set => SetProperty(ref _title, value);
    }

    public string Subtitle
    {
        get => _subtitle;
        private set => SetProperty(ref _subtitle, value);
    }

    public string Status
    {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    private async Task RefreshAsync()
    {
        if (_isRefreshing)
        {
            return;
        }

        try
        {
            _isRefreshing = true;
            var media = await _mediaSessionService.GetCurrentMediaAsync();
            if (media is null)
            {
                Title = "Sin reproduccion";
                Subtitle = "No hay una sesion multimedia activa";
                Status = "Inactivo";
                return;
            }

            Title = media.Title;
            Subtitle = BuildSubtitle(media.Artist, media.AppName);
            Status = media.IsPlaying ? "Reproduciendo" : "Pausado";
        }
        finally
        {
            _isRefreshing = false;
        }
    }

    private static string BuildSubtitle(string artist, string appName)
    {
        if (!string.IsNullOrWhiteSpace(artist))
        {
            return artist;
        }

        if (!string.IsNullOrWhiteSpace(appName))
        {
            return appName;
        }

        return "Metadatos no disponibles";
    }
}
