using System.Windows;
using OverlayWidgets.ViewModels;
using OverlayWidgets.Views.Widgets;

namespace OverlayWidgets.Widgets;

public sealed class ClockWidget : IWidget
{
    public string Type => WidgetTypes.Clock;
    public string DisplayName => "Reloj";

    public FrameworkElement CreateView()
    {
        return new ClockWidgetView
        {
            DataContext = new ClockWidgetViewModel()
        };
    }
}
