using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiabloCoETimerWPF
{
    /// <summary>
    /// Interaction logic for TimerLabel.xaml
    /// </summary>
    public partial class TimerLabel : Window
    {
        private TimerLabelViewModel _viewModel;
        public TimerLabel(MainWindowViewModel _mainViewModel)
        {
            InitializeComponent();
            _viewModel = new TimerLabelViewModel(_mainViewModel);
            DataContext = _viewModel;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
