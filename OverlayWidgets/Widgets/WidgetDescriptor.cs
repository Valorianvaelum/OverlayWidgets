namespace OverlayWidgets.Widgets;

public sealed record WidgetDescriptor(string Type, string DisplayName, Func<IWidget> Factory);
