using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;
using RollingBeads.Models;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI.Core;

namespace RollingBeads.Presentation;

public sealed partial class MainPage : Page
{
    private BeadCollection _beadCollection;
    private List<List<(Bead, Ellipse)>> _list = new List<List<(Bead, Ellipse)>>();
    private List<Line> lines = new List<Line>();
    private Timer _timer;
    private bool _isMoving = false;

    private int _lineCount = 10;
    private const int _xPoint = 700;
    private const int _largeY = 700;
    private const int _smallY = 300;
    private int _cycleSecond = 2;

    public MainPage()
    {
        this.InitializeComponent();

        _beadCollection = new BeadCollection(_lineCount, (double)_cycleSecond, new Point(_xPoint, _smallY), new Point(_xPoint, _largeY));
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        DrawElement();
    }

    private void DrawElement()
    {
        int count = 0;
        List<(Bead, Ellipse)> list = new List<(Bead, Ellipse)>();
        lines.Clear();
        _list.Clear();
        foreach (var bead in _beadCollection.Beads)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 10;
            ellipse.Height = 10;
            ellipse.Fill = Colors.Black;
            Canvas.SetLeft(ellipse, bead.XPoint);
            Canvas.SetTop(ellipse, bead.YPoint);
            canvas.Children.Add(ellipse);
            list.Add((bead, ellipse));
            count++;
            _list.Add(list);

            Line line = new Line();
            line.Stroke = Colors.Black;
            line.Fill = Colors.Black;
            line.X1 = list[0].Item1.MinXPoint + 5;
            line.Y1 = list[0].Item1.MinYPoint + 5;
            line.X2 = list[0].Item1.MaxXPoint + 5;
            line.Y2 = list[0].Item1.MaxYPoint + 5;
            line.StrokeThickness = 1;
            lines.Add(line);
            
            list = new List<(Bead, Ellipse)>();
        }
    }

    private void Move_Click(object sender, RoutedEventArgs e)
    {
        ChangeControlState(false);

        _timer = new Timer((o) =>
        {
            foreach (var pair in _list)
            {
                for (int i = 0; i < pair.Count; i++)
                {
                    _ = pair[i].Item1.Move();
                    double x = pair[i].Item1.XPoint;
                    double y = pair[i].Item1.YPoint;

                    Canvas.SetLeft(pair[i].Item2, x);
                    Canvas.SetTop(pair[i].Item2, y);
                }
            }
        });
        _timer.Change(0, 16);
    }

    private void beadCountInput_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        _lineCount = (int)args.NewValue;
        InitBeads();
    }

    private void drawLineCheck_Checked(object sender, RoutedEventArgs e)
    {
        lines.ForEach(line => canvas.Children.Add(line));
    }

    private void drawLineCheck_Unchecked(object sender, RoutedEventArgs e)
    {
        lines.ForEach(line => canvas.Children.Remove(line));
    }

    private async Task Reset_Click(object sender, RoutedEventArgs e)
    {
        if (_timer != null)
            await _timer.DisposeAsync();

        InitBeads();
        ChangeControlState(true);
    }

    private void InitBeads()
    {
        canvas.Children.Clear();
        _beadCollection = new BeadCollection(_lineCount, (double)_cycleSecond, new Point(_xPoint, _smallY), new Point(_xPoint, _largeY));
        DrawElement();
        if (drawLineCheck.IsChecked == true)
            drawLineCheck_Checked(null, null);
        else
            drawLineCheck_Unchecked(null, null);

    }

    private void ChangeControlState(bool isStoped)
    {
        beadCountInput.IsEnabled = isStoped;
        cycleSecondInput.IsEnabled = isStoped;
        Move.IsEnabled = isStoped;
    }

    private void cycleSecondInput_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        _cycleSecond = (int)args.NewValue;
        InitBeads();
    }
}
