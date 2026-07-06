using OverlayWidgets.Widgets;

namespace OverlayWidgets.Services;

public sealed class WidgetRegistryService : IWidgetRegistryService
{
    private readonly Dictionary<string, WidgetDescriptor> _descriptors = new(StringComparer.OrdinalIgnoreCase);

    public WidgetRegistryService(IMediaSessionService mediaSessionService)
    {
        Register(new WidgetDescriptor(WidgetTypes.Clock, "Reloj", () => new ClockWidget()));
        Register(new WidgetDescriptor(WidgetTypes.Media, "Multimedia", () => new MediaWidget(mediaSessionService)));
    }

    public IReadOnlyCollection<WidgetDescriptor> GetAvailableWidgets()
    {
        return _descriptors.Values.ToArray();
    }

    public IWidget Create(string type)
    {
        if (!TryCreate(type, out var widget) || widget is null)
        {
            throw new InvalidOperationException($"Widget no registrado: {type}");
        }

        return widget;
    }

    public bool TryCreate(string type, out IWidget? widget)
    {
        widget = null;
        if (!_descriptors.TryGetValue(type, out var descriptor))
        {
            return false;
        }

        widget = descriptor.Factory();
        return true;
    }

    private void Register(WidgetDescriptor descriptor)
    {
        _descriptors[descriptor.Type] = descriptor;
    }
}
