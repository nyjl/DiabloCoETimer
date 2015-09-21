using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DiabloCoETimerWPF
{
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
