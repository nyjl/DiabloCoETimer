using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace DiabloCoETimerWPF
{
    public enum MagicSchool
    {
        Arcane,
        Cold,
        Fire,
        Holy,
        Lightning,
        Physical,
        Poison
    };

    public enum Class
    {
        Barb,
        Monk,
        DH,
        WD,
        Crusader,
        Sorc
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private const int COE_DURATION = 4;
        private const int TIMER_TICK = 1;

        private SoundPlayer _player;
        private string _soundPath;
        private DispatcherTimer _timer;
        private TimeSpan _time;
        private MagicSchool _currentSchool;
        private List<MagicSchool> _favouriteSchools = new List<MagicSchool> { MagicSchool.Physical };

        private bool _isLabelOnTop;
        private TimerLabel _labelWindow;
        private MainWindow _mainWindow;

        public Class Class
        {
            get
            {
                if (IsBarb) return Class.Barb;
                if (IsMonk) return Class.Monk;
                if (IsDH) return Class.DH;
                if (IsWD) return Class.WD;
                if (IsCrusader) return Class.Crusader;
                if (IsSorc) return Class.Sorc;

                return Class.Barb;
            }
        }

        public MagicSchool[] AvailableSchools
        {
            get
            {
                switch (Class)
                {
                    case Class.Barb:
                        return new MagicSchool[] { MagicSchool.Fire, MagicSchool.Lightning, MagicSchool.Physical, MagicSchool.Cold };
                    case Class.Monk:
                        return new MagicSchool[] { MagicSchool.Fire, MagicSchool.Holy, MagicSchool.Lightning, MagicSchool.Physical, MagicSchool.Cold };
                    case Class.DH:
                        return new MagicSchool[] { MagicSchool.Fire, MagicSchool.Lightning, MagicSchool.Physical, MagicSchool.Cold };
                    case Class.WD:
                        return new MagicSchool[] { MagicSchool.Fire, MagicSchool.Physical, MagicSchool.Poison, MagicSchool.Cold };
                    case Class.Crusader:
                        return new MagicSchool[] { MagicSchool.Fire, MagicSchool.Holy, MagicSchool.Lightning, MagicSchool.Physical };
                    case Class.Sorc:
                        return new MagicSchool[] { MagicSchool.Fire, MagicSchool.Lightning, MagicSchool.Arcane, MagicSchool.Cold };
                    default:
                        return null;
                }
            }
        }

        public bool IsBarb { get; set; }
        public bool IsMonk { get; set; }
        public bool IsDH { get; set; }
        public bool IsWD { get; set; }
        public bool IsCrusader { get; set; }
        public bool IsSorc { get; set; }

        public List<string> FavouriteSchoolOptions
        {
            get
            {
                return Enum.GetNames(typeof(MagicSchool)).ToList();
            }
        }

        public MagicSchool FavouriteSchool //todo: multiple fav
        {
            get
            {
                return _favouriteSchools[0];
            }
            set
            {
                _favouriteSchools = new List<MagicSchool> {value};
            }
        }

        public string MainTimer
        {
            get
            {
                return _time.ToString("ss");
            }
        }

        public Brush MainTimerColor
        {
            get
            {
                switch (_currentSchool)
                {
                    case MagicSchool.Arcane:
                        return new SolidColorBrush(Colors.Pink);
                    case MagicSchool.Cold:
                        return new SolidColorBrush(Colors.AliceBlue);
                    case MagicSchool.Fire:
                        return new SolidColorBrush(Colors.Red);
                    case MagicSchool.Holy:
                        return new SolidColorBrush(Colors.Yellow);
                    case MagicSchool.Lightning:
                        return new SolidColorBrush(Colors.DarkBlue);
                    case MagicSchool.Physical:
                        return new SolidColorBrush(Colors.Brown);
                    case MagicSchool.Poison:
                        return new SolidColorBrush(Colors.Green);
                    default:
                        return new SolidColorBrush(Colors.Black);
                }
            }
        }

        public bool IsLabelOnTop
        {
            get
            {
                return _isLabelOnTop;
            }
            set
            {
                _isLabelOnTop = value;
                if (_isLabelOnTop)
                {
                    _labelWindow = new TimerLabel(this);
                    _labelWindow.Left = _mainWindow.Left;
                    _labelWindow.Top = _mainWindow.Top;
                    _labelWindow.Topmost = true;
                    _labelWindow.Deactivated += (o, e) => { _labelWindow.Topmost = true; };
                    _labelWindow.Show();
                }
                else
                {
                    _labelWindow.Close();
                }
            }
        }

        public string SoundPath
        {
            get
            {
                return _soundPath;
            }
            set
            {
                _soundPath = value;
                _player = new SoundPlayer(_soundPath);
            }
        }

        public bool IsSoundOn
        {
            get;
            set;
        }

        public MainWindowViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(TIMER_TICK);
            _timer.Tick += timer_Tick;
            _timer.Start();
        }

        public void btnReset_Click()
        {
            _time = TimeSpan.FromSeconds(COE_DURATION);
            _currentSchool = MagicSchool.Fire;
            _timer.Stop();
            _timer.Start();
            RaisePropertyChanged("MainTimer");
            RaisePropertyChanged("MainTimerColor");
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            _time -= TimeSpan.FromSeconds(TIMER_TICK);
            if (_time <= TimeSpan.Zero)
            {
                _time = TimeSpan.FromSeconds(COE_DURATION);
                MoveToNextSchool();
            }
            RaisePropertyChanged("MainTimer");
        }

        public void Path_Click()
        {
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = ".wav";

            if (dialog.ShowDialog() != true)
                return;

            SoundPath = dialog.FileName;
            RaisePropertyChanged("SoundPath");
        }

        private void MoveToNextSchool()
        {
            var index = Array.IndexOf(AvailableSchools, _currentSchool) + 1;
            if (index >= AvailableSchools.Length)
                index = 0;

            _currentSchool = AvailableSchools[index];
            if (_favouriteSchools.Contains(_currentSchool) && _player != null && IsSoundOn)
                _player.Play();

            RaisePropertyChanged("MainTimerColor");
        }

        #region PropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
