using System.Windows;
using OverlayWidgets.Models;
using OverlayWidgets.Widgets;

namespace OverlayWidgets.ViewModels;

public sealed class WidgetHostViewModel : ObservableObject
{
    private double _left;
    private double _top;
    private double _width;
    private double _height;
    private bool _isEditMode;

    public WidgetHostViewModel(WidgetSettings settings, IWidget widget)
    {
        Id = settings.Id;
        Type = settings.Type;
        DisplayName = widget.DisplayName;
        Content = widget.CreateView();
        _left = settings.Left;
        _top = settings.Top;
        _width = settings.Width;
        _height = settings.Height;
    }

    public string Id { get; }
    public string Type { get; }
    public string DisplayName { get; }
    public FrameworkElement Content { get; }

    public double Left
    {
        get => _left;
        set => SetProperty(ref _left, value);
    }

    public double Top
    {
        get => _top;
        set => SetProperty(ref _top, value);
    }

    public double Width
    {
        get => _width;
        set => SetProperty(ref _width, value);
    }

    public double Height
    {
        get => _height;
        set => SetProperty(ref _height, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public WidgetSettings ToSettings()
    {
        return new WidgetSettings
        {
            Id = Id,
            Type = Type,
            IsEnabled = true,
            Left = Left,
            Top = Top,
            Width = Width,
            Height = Height
        };
    }

    public void KeepInside(double maxWidth, double maxHeight)
    {
        Width = Math.Clamp(Width, 120, Math.Max(120, maxWidth));
        Height = Math.Clamp(Height, 72, Math.Max(72, maxHeight));
        Left = Math.Clamp(Left, 0, Math.Max(0, maxWidth - Width));
        Top = Math.Clamp(Top, 0, Math.Max(0, maxHeight - Height));
    }
}
