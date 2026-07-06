namespace OverlayWidgets.ViewModels;

public sealed class WidgetOptionViewModel : ObservableObject
{
    private bool _isEnabled;

    public WidgetOptionViewModel(string type, string displayName, bool isEnabled)
    {
        Type = type;
        DisplayName = displayName;
        _isEnabled = isEnabled;
    }

    public string Type { get; }
    public string DisplayName { get; }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }
}
