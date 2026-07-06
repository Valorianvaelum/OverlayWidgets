namespace OverlayWidgets.Models;

public sealed class WidgetSettings
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Type { get; set; } = string.Empty;
    public double Left { get; set; }
    public double Top { get; set; }
    public double Width { get; set; } = 240;
    public double Height { get; set; } = 96;
}
