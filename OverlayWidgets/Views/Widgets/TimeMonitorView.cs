using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace OverlayWidgets.Views.Widgets;

public sealed class TimeMonitorView : UserControl
{
    public TimeMonitorView()
    {
        Content = BuildLayout();
    }

    private static Grid BuildLayout()
    {
        var root = new Grid();
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var header = BuildHeader();
        Grid.SetRow(header, 0);
        root.Children.Add(header);

        var body = BuildBody();
        Grid.SetRow(body, 1);
        root.Children.Add(body);

        var footer = BuildFooter();
        Grid.SetRow(footer, 2);
        root.Children.Add(footer);

        return root;
    }

    private static Grid BuildHeader()
    {
        var grid = new Grid { Margin = new Thickness(0, 0, 0, 9) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var bars = new StackPanel { Orientation = Orientation.Horizontal };
        bars.Children.Add(Bar(5, 24, "#FF2D2D"));
        bars.Children.Add(Bar(4, 24, "#FFB000", new Thickness(3, 0, 0, 0)));
        bars.Children.Add(Bar(2, 24, "#7CFF55", new Thickness(3, 0, 0, 0)));
        grid.Children.Add(bars);

        var title = new StackPanel { Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
        title.Children.Add(Text("TIME CODE", "#FFB000", 13, FontWeights.Bold, "Bahnschrift Condensed"));
        title.Children.Add(Text("LOCAL DISPLAY / CH A-01", "#B7AFA892", 9, FontWeights.Bold));
        Grid.SetColumn(title, 1);
        grid.Children.Add(title);

        var status = new Border
        {
            BorderBrush = Brush("#FF7CFF55"),
            BorderThickness = new Thickness(1),
            Background = Brush("#2211220A"),
            Padding = new Thickness(7, 2, 7, 2),
            VerticalAlignment = VerticalAlignment.Top,
            Child = Text("OK", "#7CFF55", 10, FontWeights.Bold)
        };
        Grid.SetColumn(status, 2);
        grid.Children.Add(status);

        return grid;
    }

    private static Grid BuildBody()
    {
        var grid = new Grid { VerticalAlignment = VerticalAlignment.Center };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var leftBars = new StackPanel { Width = 22, Margin = new Thickness(0, 2, 10, 2) };
        leftBars.Children.Add(Bar(double.NaN, 6, "#FF2D2D"));
        leftBars.Children.Add(Bar(double.NaN, 6, "#FFB000", new Thickness(0, 5, 0, 0)));
        leftBars.Children.Add(Bar(double.NaN, 6, "#7CFF55", new Thickness(0, 5, 0, 0)));
        leftBars.Children.Add(Bar(double.NaN, 6, "#38DDF2", new Thickness(0, 5, 0, 0)));
        grid.Children.Add(leftBars);

        var center = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        var time = Text(string.Empty, "#FFF7EFD8", 42, FontWeights.Bold, "Consolas");
        time.SetBinding(TextBlock.TextProperty, new Binding("Time"));
        center.Children.Add(time);
        center.Children.Add(BuildSignalBars());
        Grid.SetColumn(center, 1);
        grid.Children.Add(center);

        var sync = new StackPanel { Width = 54, Margin = new Thickness(10, 0, 0, 0) };
        sync.Children.Add(Text("SYNC", "#B7AFA892", 8, FontWeights.Bold, horizontalAlignment: HorizontalAlignment.Right));
        sync.Children.Add(Text("100", "#7CFF55", 18, FontWeights.Bold, horizontalAlignment: HorizontalAlignment.Right));
        sync.Children.Add(new Rectangle { Height = 24, Stroke = Brush("#7738DDF2"), StrokeThickness = 1, Margin = new Thickness(0, 5, 0, 0) });
        Grid.SetColumn(sync, 2);
        grid.Children.Add(sync);

        return grid;
    }

    private static Grid BuildSignalBars()
    {
        var grid = new Grid { Margin = new Thickness(2, 3, 10, 0) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
        grid.Children.Add(Bar(double.NaN, 3, "#7CFF55"));
        var amber = Bar(double.NaN, 3, "#FFB000", new Thickness(6, 0, 0, 0));
        Grid.SetColumn(amber, 1);
        grid.Children.Add(amber);
        var red = Bar(double.NaN, 3, "#FF2D2D", new Thickness(6, 0, 0, 0));
        Grid.SetColumn(red, 2);
        grid.Children.Add(red);
        return grid;
    }

    private static Grid BuildFooter()
    {
        var grid = new Grid { Margin = new Thickness(0, 10, 0, 0) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var label = new Border
        {
            BorderBrush = Brush("#88FFB000"),
            BorderThickness = new Thickness(1),
            Background = Brush("#221D1000"),
            Padding = new Thickness(6, 3, 6, 3),
            Child = Text("DATE", "#FFB000", 9, FontWeights.Bold)
        };
        grid.Children.Add(label);

        var date = Text(string.Empty, "#B7AFA892", 12, FontWeights.SemiBold);
        date.Margin = new Thickness(8, 2, 0, 0);
        date.SetBinding(TextBlock.TextProperty, new Binding("Date"));
        Grid.SetColumn(date, 1);
        grid.Children.Add(date);

        var status = Text("OK", "#7CFF55", 9, FontWeights.Bold);
        status.VerticalAlignment = VerticalAlignment.Center;
        Grid.SetColumn(status, 2);
        grid.Children.Add(status);

        return grid;
    }

    private static Rectangle Bar(double width, double height, string color, Thickness? margin = null)
    {
        return new Rectangle
        {
            Width = width,
            Height = height,
            Fill = Brush(color),
            Margin = margin ?? new Thickness(0)
        };
    }

    private static TextBlock Text(string value, string color, double size, FontWeight weight, string? fontFamily = null, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
    {
        return new TextBlock
        {
            Text = value,
            Foreground = Brush(color),
            FontSize = size,
            FontWeight = weight,
            FontFamily = new FontFamily(fontFamily ?? "Bahnschrift, Segoe UI"),
            HorizontalAlignment = horizontalAlignment
        };
    }

    private static Brush Brush(string color)
    {
        return (Brush)new BrushConverter().ConvertFromString(color)!;
    }
}
