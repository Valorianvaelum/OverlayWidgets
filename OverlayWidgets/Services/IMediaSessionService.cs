using OverlayWidgets.Models;

namespace OverlayWidgets.Services;

public interface IMediaSessionService
{
    Task<MediaInfo?> GetCurrentMediaAsync(CancellationToken cancellationToken = default);
}
