using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DiabloCoETimerWPF
{
    public static class WindowsServices
    {
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
    }

    public class TimerLabelViewModel:INotifyPropertyChanged
    {
        public string MainTimer
        {
            get
            {
                return _mainViewModel.MainTimer;
            }
        }
        
        public Brush MainTimerColor
        {
            get
            {
                return _mainViewModel.MainTimerColor;
            }
        }

        public TimerLabelViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _mainViewModel.PropertyChanged += RaisePropertyChanged;
        }

        private MainWindowViewModel _mainViewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(e.PropertyName));
        }

    }
}
