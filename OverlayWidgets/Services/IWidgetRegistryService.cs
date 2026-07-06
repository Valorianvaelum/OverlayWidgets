using OverlayWidgets.Widgets;

namespace OverlayWidgets.Services;

public interface IWidgetRegistryService
{
    IReadOnlyCollection<WidgetDescriptor> GetAvailableWidgets();
    IWidget Create(string type);
    bool TryCreate(string type, out IWidget? widget);
}
