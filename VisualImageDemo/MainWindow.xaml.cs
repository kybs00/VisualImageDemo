using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace VisualImageDemo
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

        private void CaptureButton_OnClick(object sender, RoutedEventArgs e)
        {
            var bitmapSource = ToImageSource(Grid1, Grid1.RenderSize);
            CaptureImage.Source = bitmapSource;
        }
        /// <summary>
        /// 截图控件
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public BitmapSource ToImageSource(Visual visual, Size size = default)
        {
            if (!(visual is FrameworkElement element))
            {
                return null;
            }
            if (!element.IsLoaded)
            {
                if (size == default)
                {
                    //计算元素的渲染尺寸
                    element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    element.Arrange(new Rect(new Point(), element.DesiredSize));
                    size = element.DesiredSize;
                }
                else
                {
                    //未加载到视觉树的，按指定大小布局
                    //按size显示，如果设计宽高大于size则按sie裁剪，如果设计宽度小于size则按size放大显示。
                    element.Measure(size);
                    element.Arrange(new Rect(size));
                }
            }
            else if (size == default)
            {
                Rect rect = VisualTreeHelper.GetDescendantBounds(visual);
                if (rect.Equals(Rect.Empty))
                {
                    return null;
                }
                size = rect.Size;
            }

            var dpi = GetAppStartDpi();
            return ToImageSource(visual, size, dpi.X, dpi.Y);
        }
        /// <summary>
        /// Visual转图片
        /// </summary>
        public static BitmapSource ToImageSource(Visual visual, Size size, double dpiX, double dpiY)
        {
            var validSize = size.Width > 0 && size.Height > 0;
            if (!validSize) throw new ArgumentException($"{nameof(size)}值无效:${size.Width},${size.Height}");
            if (Math.Abs(size.Width) > 0.0001 && Math.Abs(size.Height) > 0.0001)
            {
                RenderTargetBitmap bitmap = new RenderTargetBitmap((int)(size.Width * dpiX), (int)(size.Height * dpiY), dpiX * 96, dpiY * 96, PixelFormats.Pbgra32);
                bitmap.Render(visual);
                return bitmap;
            }
            return new BitmapImage();
        }
        /// <summary>
        /// 获取屏幕DPI
        /// </summary>
        /// <returns></returns>
        public static (double X, double Y) GetAppStartDpi()
        {
            //仅用于窗口所在屏幕DPI未变更的场景
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiXNumber = (int?)dpiXProperty?.GetValue(null, null) ?? 96;
            var dpiYNumber = (int?)dpiYProperty?.GetValue(null, null) ?? 96;
            if (dpiXNumber == 0 && dpiYNumber == 0)
            {
                dpiXNumber = 96;
                dpiYNumber = 96;
            }
            else if (dpiXNumber == 0 && dpiYNumber != 0)
            {
                dpiXNumber = dpiYNumber;
            }
            else if (dpiXNumber != 0 && dpiYNumber == 0)
            {
                dpiYNumber = dpiXNumber;
            }
            return new(dpiXNumber / 96d, dpiYNumber / 96d);
        }
    }
}