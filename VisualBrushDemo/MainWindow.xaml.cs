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

namespace VisualBrushDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }
    private bool _isDown;
    private Point _relativeToBlockPosition;
    private void TestTextBlock_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _isDown = true;
        _relativeToBlockPosition = e.MouseDevice.GetPosition(TestTextBlock);
        TestTextBlock.CaptureMouse();
    }

    private void TestTextBlock_OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_isDown)
        {
            var position = e.MouseDevice.GetPosition(Grid1);
            Canvas.SetTop(TestTextBlock, position.Y - _relativeToBlockPosition.Y);
            Canvas.SetLeft(TestTextBlock, position.X - _relativeToBlockPosition.X);
        }
    }

    private void TestTextBlock_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        TestTextBlock.ReleaseMouseCapture();
        _isDown = false;
    }
    }
}