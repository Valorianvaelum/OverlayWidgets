using OverlayWidgets.Models;
using Windows.Media.Control;

namespace OverlayWidgets.Services;

public sealed class MediaSessionService : IMediaSessionService
{
    private readonly ILoggerService _logger;

    public MediaSessionService(ILoggerService logger)
    {
        _logger = logger;
    }

    public async Task<MediaInfo?> GetCurrentMediaAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            cancellationToken.ThrowIfCancellationRequested();

            var session = manager.GetCurrentSession();
            if (session is null)
            {
                return null;
            }

            GlobalSystemMediaTransportControlsSessionMediaProperties? mediaProperties = null;
            try
            {
                mediaProperties = await session.TryGetMediaPropertiesAsync();
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (Exception exception)
            {
                _logger.Warning($"Could not read media metadata for session {session.SourceAppUserModelId}: {exception.Message}");
            }

            var playbackInfo = session.GetPlaybackInfo();
            var isPlaying = playbackInfo?.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;
            var appName = session.SourceAppUserModelId ?? string.Empty;
            var title = mediaProperties?.Title;

            return new MediaInfo
            {
                Title = string.IsNullOrWhiteSpace(title) ? "Reproduccion multimedia" : title,
                Artist = mediaProperties?.Artist ?? string.Empty,
                AppName = appName,
                IsPlaying = isPlaying
            };
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (Exception exception)
        {
            _logger.Warning($"Could not read current media session: {exception.Message}");
            return null;
        }
    }
}
