using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Fifteen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private DispatcherTimer _dispatcherTimer;
        private string _timerValue;
        private TimeSpan _currentTimeValue;
        private DateTime? TimerStart { get; set; }

        public MainWindow()
        {
            InitializeComponent();            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MovementHelper.GenerateElements(e);
            BestScore.Content = Properties.Settings.Default.BestScore;
        }


        private void InitTimer(DateTime? timerStart)
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += new EventHandler(DispatcherTimerTick);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            TimerStart = timerStart;
        }

        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            _currentTimeValue = DateTime.Now - (DateTime)TimerStart;
            _timerValue = String.Format("{0:D2}:{1:D2}:{2:D2}", _currentTimeValue.Hours, _currentTimeValue.Minutes, _currentTimeValue.Seconds);
            Timer.Content = _timerValue;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Left) || (e.Key == Key.Right) || (e.Key == Key.Up) || (e.Key == Key.Down))
            {
                MovementHelper.PreviewKeyDown(e);
                e.Handled = true;
                UpdateStatusBar();
            }
            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
            {
                switch (e.Key)
                {
                     //handle G key
                     case Key.G:
                        Start_Click(sender, e);
                        break;
                     //handle R key
                     case Key.R:
                        Reset_Click(sender, e);
                        break;
                    }
                }
                //handle F1 key
             if (e.Key == Key.F1)
             {
                MenuItemHelp_Click(sender, e);
             }            
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            CheckState();
        }

        private void Window_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UpdateStatusBar();
            CheckState();
        }   

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetGame(e);           
        }

        private void ResetGame(RoutedEventArgs e)
        {
            MovementHelper.GenerateElements(e);

            if (_dispatcherTimer != null)
            {
                _dispatcherTimer.Stop();
            }

            Timer.Content = Properties.Settings.Default.TimerInitialValue;
            SetCurrentState(DateTime.Now, false, "Start");
            MovementHelper.NumberOfMoves = 0;
            UpdateStatusBar();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
           SetStartButtonState(e);
        }

        //Set state of the Start button
        private void SetStartButtonState(RoutedEventArgs e)
        {
            switch (Start.Content.ToString())
            {
                case "Start":
                     if ((Board.IsEnabled == false) && (Timer.Content.ToString() != Properties.Settings.Default.TimerInitialValue))
                     {
                        ResetGame(e);
                     }
                SetCurrentState(DateTime.Now, true, "Pause");                                      
                    _dispatcherTimer.Start();                   
                    break;
                case "Continue":
                    SetCurrentState(DateTime.Now - _currentTimeValue, true, "Pause");                
                    _dispatcherTimer.Start();                    
                    break;
                case "Pause":
                    SetCurrentState(null, false, "Continue");
                    _dispatcherTimer.Stop();                    
                    break;
            }
        }

        private void CheckState()
        {
            if (!Board.IsEnabled) { return; }
            if (!MovementHelper.ElementsAreOrdered(Board)) { return; }            

            _dispatcherTimer.Stop();

            MessageBox.Show(String.Format("Well done!!!\n\nYour time is {0}.\nNumber of moves: {1}", _timerValue, MovementHelper.NumberOfMoves), "Fifteen", MessageBoxButton.OK, MessageBoxImage.Information);

            SetCurrentState(null, false, "Start");

            //Compare timer value with current score
            if (Properties.Settings.Default.BestScore == Properties.Settings.Default.TimerInitialValue)
            {
                SaveNewScore(_timerValue);
            }
            else
            {
                TimeSpan currentscore, bestScore;
                TimeSpan.TryParse(_timerValue, out currentscore);
                TimeSpan.TryParse(Properties.Settings.Default.BestScore, out bestScore);

                if (TimeSpan.Compare(currentscore, bestScore) == -1)
                {
                    SaveNewScore(_timerValue);
                }
            }
            
            BestScore.Content = Properties.Settings.Default.BestScore;
        }

        private void SaveNewScore(string score)
        {
            Properties.Settings.Default.BestScore = score;
            Properties.Settings.Default.Save();
        }

        private void SetCurrentState(DateTime? dt,bool be, string sc)
        {
            if (dt != null)
            {
                InitTimer(dt);
            }
            Board.IsEnabled = be;
            Start.Content = sc;
            MenuItemStart.Header = sc;
            MenuItemStart.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("Images/" + sc + ".png", UriKind.Relative))
            };
            BestScore.Content = Properties.Settings.Default.BestScore;
        }
        private void UpdateStatusBar()
        {
            StatusBar.Text = "Moves: " + MovementHelper.NumberOfMoves;
        }
        #region Menu
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            string appRoot = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = System.IO.Path.Combine(appRoot, "Help.chm");
            if (!System.IO.File.Exists(filePath))
            {
                // The path to the Help folder.
                string directory = System.IO.Path.Combine(appRoot, "../..");
                // The path to the Help file.
                filePath = System.IO.Path.Combine(directory, "Help.chm");
            }
            // Launch the Help file.
            if (System.IO.File.Exists(filePath))
            {
                Process.Start(filePath);
            }
            else
            {
                MessageBox.Show("File not found!");
            }
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            using (AboutBox box = new AboutBox())
            {
                box.ShowDialog();
            }
        }
        #endregion

       
    }
}