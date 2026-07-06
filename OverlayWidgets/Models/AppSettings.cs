namespace OverlayWidgets.Models;

public sealed class AppSettings
{
    public bool IsEditMode { get; set; } = true;
    public List<WidgetSettings> Widgets { get; set; } = [];
}
