using System.Threading;
using System.Windows;
using AudioManette.AudioSource;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;

using AudioManette.Controls;
using AudioManette.Utils;

namespace AudioManette.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        private readonly Geometry PLAYING_ICON = Geometry.Parse("M14,19H18V5H14M6,19H10V5H6V19Z");
        private readonly Geometry PAUSE_ICON = Geometry.Parse("M8,5.14V19.14L19,12.14L8,5.14Z");
        private readonly Color SECOUNDARY_COLOR = (Color)ColorConverter.ConvertFromString("#949f94");

        private IAudioSource _audioSource;
        private List<Track> _tracks = new List<Track>();

        private bool _isEnlarge = false;
        private bool _isPlaying;
        private bool _isRepeatModeEnabled;

        private SearchTextBox _searchTextBox;

        #endregion

        public MainWindow(IAudioSource audioSource)
        {
            _audioSource = audioSource;
            _searchTextBox = new SearchTextBox(_audioSource);
            _searchTextBox.searchTrackEvent += SearchTrackEvent;

            CurrentPlayingState playingState = _audioSource.GetCurrentPlayingState().GetAwaiter().GetResult();

            _isRepeatModeEnabled = playingState.IsRepeatModeEnabled;
            _isPlaying = playingState.IsPlaying;

            InitializeComponent();

            var repeatIconColor = _isRepeatModeEnabled ? SECOUNDARY_COLOR : Colors.White;
            repeatModeIcon.Brush = new SolidColorBrush(repeatIconColor);
            playIcon.Geometry = _isPlaying ? PLAYING_ICON : PAUSE_ICON;

            Track currentTrack = playingState.Track;
            this.SetDataContext(currentTrack);

            new Thread(() =>
            {
                while (true)
                {
                    titleTextBlock.Dispatcher.Invoke(() =>
                    {
                        titleTextBlock.Text = ViewsUtil.Displace(titleTextBlock.Text);
                    });

                    Thread.Sleep(100);
                }
            }).Start();
        }

        private string GetHeaderTextFromTrack(Track track)
        {
            return $" {track.Title} - {track.Artist}. ";
        }
        
        private void SetDataContext(Track track)
        {
            this.DataContext = new{ Title = this.GetHeaderTextFromTrack(track), ImagePath = track.ImagePath.Little };
        }

        #region EventHandlers

        private void TrackChangedEventHandler(object sender)
        {
            var track = (Track)sender;

            this.SetDataContext(track);

            titleTextBlock.Text = this.GetHeaderTextFromTrack(track);
            playIcon.Geometry = PLAYING_ICON;
        }
        
        private void SearchTrackEvent(object sender)
        {
            _tracks = ((List<Track>)sender).Take(3).ToList();

            stackTracks.Children.Clear();

            foreach (var track in _tracks)
            {
                var card = new TrackCard(_audioSource, track);

                card.trackChangedEvent += this.TrackChangedEventHandler;

                stackTracks.Children.Add(card);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private async void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                _isPlaying = !await _audioSource.PausePlayback();
                playIcon.Geometry = PAUSE_ICON;
            }
            else
            {
                _isPlaying = await _audioSource.ResumePlayback();
                playIcon.Geometry = PLAYING_ICON;
            }
        }
        
        private void EnlargeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isEnlarge)
            {
                gridSearchTrack.Children.Remove(_searchTextBox);
                stackTracks.Children.Clear();
            }
            else
            {
                gridSearchTrack.Children.Add(_searchTextBox);
            }

            _isEnlarge = !_isEnlarge;
        }
        
        private async void RepeatModeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isRepeatModeEnabled)
            {
                _isRepeatModeEnabled = !await _audioSource.DisableRepeatMode();
                repeatModeIcon.Brush = new SolidColorBrush(Colors.White);
            }
            else
            {
                _isRepeatModeEnabled = await _audioSource.EnableRepeatMode();
                repeatModeIcon.Brush = new SolidColorBrush(SECOUNDARY_COLOR);
            }
        }
        
        #endregion
    }
}
