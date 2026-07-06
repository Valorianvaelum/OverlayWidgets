using System.Windows;

namespace OverlayWidgets.Widgets;

public interface IWidget
{
    string Type { get; }
    string DisplayName { get; }
    FrameworkElement CreateView();
}
