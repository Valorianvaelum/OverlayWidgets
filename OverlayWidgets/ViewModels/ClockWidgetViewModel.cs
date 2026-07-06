using System.Windows.Threading;

namespace OverlayWidgets.ViewModels;

public sealed class ClockWidgetViewModel : ObservableObject
{
    private readonly DispatcherTimer _timer;
    private string _time = string.Empty;
    private string _date = string.Empty;

    public ClockWidgetViewModel()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += (_, _) => Refresh();
        _timer.Start();
        Refresh();
    }

    public string Time
    {
        get => _time;
        private set => SetProperty(ref _time, value);
    }

    public string Date
    {
        get => _date;
        private set => SetProperty(ref _date, value);
    }

    private void Refresh()
    {
        var now = DateTime.Now;
        Time = now.ToString("HH:mm:ss");
        Date = now.ToString("dddd, dd MMMM");
    }
}
