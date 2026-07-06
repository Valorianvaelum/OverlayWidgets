namespace OverlayWidgets.Models;

public sealed class MediaInfo
{
    public string Title { get; init; } = "Sin reproduccion";
    public string Artist { get; init; } = string.Empty;
    public string AppName { get; init; } = string.Empty;
    public bool IsPlaying { get; init; }
}
