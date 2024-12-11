using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace _2024_WpfApp5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 窗口淡入動畫
            this.Opacity = 0;
            var fadeIn = new DoubleAnimation(0, 1, new Duration(System.TimeSpan.FromSeconds(0.5)));
            this.BeginAnimation(Window.OpacityProperty, fadeIn);
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            MyDocumentViewer myDocumentViewer = new MyDocumentViewer();
            myDocumentViewer.Owner = this; // 設定擁有者
            myDocumentViewer.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            myDocumentViewer.ShowDialog();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 允許拖動窗口
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}
