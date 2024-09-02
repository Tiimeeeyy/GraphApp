using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Graph _graph;
    private Dictionary<string, Point> _nodePositions;

    public MainWindow()
    {
        InitializeComponent();
        _graph = new Graph();
        _nodePositions = new Dictionary<string, Point>();
    }
    
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        DrawGraph(sender, e);
    }

    private void DrawGraph(object sender, RoutedEventArgs e)
    {
        DrawingCanvas.Children.Clear();
        CalculateNodePositions();

        foreach (var node in _graph.Nodes)
        {
            var ellipse = new Ellipse
            {
                Width = 30,
                Height = 30,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
            };
            var position = _nodePositions[node];
            Canvas.SetLeft(ellipse, position.X - 15);
            Canvas.SetTop(ellipse, position.Y - 15);

            var label = new TextBlock
            {
                Text = node,
                Foreground = Brushes.Black,
                Background = Brushes.White
            };
            Canvas.SetLeft(label, position.X - 15);
            Canvas.SetTop(label, position.Y - 30);
            DrawingCanvas.Children.Add(ellipse);
            DrawingCanvas.Children.Add(label);
        }

        foreach (var edge in _graph.Edges)
        {
            var start = _nodePositions[edge.Item1];
            var end = _nodePositions[edge.Item2];

            var direction = new Vector(end.X - start.X, end.Y - start.Y);
            direction.Normalize();
            var startOffset = direction * 15;
            var endOffset = direction * -15;

            var line = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                X1 = start.X + startOffset.X,
                Y1 = start.Y + startOffset.Y,
                X2 = end.X + endOffset.X,
                Y2 = end.Y + endOffset.Y
            };
            DrawingCanvas.Children.Add(line);
            if (DirectionCheckedBox.IsChecked == true)
            {
                DrawArrowhead(line);
            }
        }
    }

    private void CalculateNodePositions()
    {
        _nodePositions.Clear();
        var nodeCount = _graph.Nodes.Count;
        var angleStep = 360.0 / nodeCount;
        var radius = Math.Min(DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight) / 2 - 50;
        var center = new Point(DrawingCanvas.ActualWidth / 2, DrawingCanvas.ActualHeight / 2);

        for (var i = 0; i < nodeCount; i++)
        {
            var angle = i * angleStep * Math.PI / 180;
            var x = center.X + radius * Math.Cos(angle);
            var y = center.Y + radius * Math.Sin(angle);
            _nodePositions[_graph.Nodes[i]] = new Point(x, y);
        }
    }

    private void AddNode(object sender, RoutedEventArgs e)
    {
        var nodeName = NodeInput.Text;
        if (!string.IsNullOrEmpty(nodeName))
        {
            if (_graph.Nodes.Contains(nodeName))
            {
                ShowErrorPopup($"Node {nodeName} already exists!");
            }
            else
            {
                _graph.AddNode(nodeName);
                DrawGraph(sender, e);
            }
        }
    }

    private void AddEdge(object sender, RoutedEventArgs e)
    {
        var edgeName = EdgeInput.Text;
        var separator = '-';
        if (!string.IsNullOrEmpty(edgeName) && edgeName.Contains(separator))
        {
            var nodes = edgeName.Split("-");
            if (nodes.Length == 2)
            {
                if (_graph.Edges.Contains((nodes[0], nodes[1])) || _graph.Edges.Contains((nodes[1], nodes[0])))
                {
                    ShowErrorPopup($"Edge '{edgeName} already exists!");
                }
                else
                {
                    _graph.AddEdge(nodes[0], nodes[1]);
                    DrawGraph(sender, e);
                }
            }
        }
    }

    private void DrawArrowhead(Line line)
    {
        var arrowHead = new Polygon
        {
            Fill = Brushes.Black,
            Points = new PointCollection
            {
                new Point(line.X2, line.Y2),
                new Point(line.X2 + 5, line.Y2 - 8),
                new Point(line.X2 + 5, line.Y2 + 8)
            }
        };
        DrawingCanvas.Children.Add(arrowHead);
    }

    private static void ShowErrorPopup(string message)
    {
        MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void DirectionCheckedBox_OnChecked(object sender, RoutedEventArgs e)
    {
        DrawGraph(sender, e);
    }
}