using System.Threading;
using System.Windows;
using AudioManette.AudioSource;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;

using AudioManette.Controls;
using AudioManette.Utils;

namespace AudioManette
{
    /// <summary>
    /// Interaction logic for PlayerWindow.xaml
    /// </summary>
    public partial class PlayerWindow : Window
    {
        #region Properties

        private readonly Geometry PLAYING_ICON = Geometry.Parse("M14,19H18V5H14M6,19H10V5H6V19Z");
        private readonly Geometry PAUSE_ICON = Geometry.Parse("M8,5.14V19.14L19,12.14L8,5.14Z");
        private readonly Geometry VOLUME_ICON = Geometry.Parse("M14,3.23V5.29C16.89,6.15 19,8.83 19,12C19,15.17 16.89,17.84 14,18.7V20.77C18,19.86 21,16.28 21,12C21,7.72 18,4.14 14,3.23M16.5,12C16.5,10.23 15.5,8.71 14,7.97V16C15.5,15.29 16.5,13.76 16.5,12M3,9V15H7L12,20V4L7,9H3Z");
        private readonly Geometry VOLUME_OFF_ICON = Geometry.Parse("M12,4L9.91,6.09L12,8.18M4.27,3L3,4.27L7.73,9H3V15H7L12,20V13.27L16.25,17.53C15.58,18.04 14.83,18.46 14,18.7V20.77C15.38,20.45 16.63,19.82 17.68,18.96L19.73,21L21,19.73L12,10.73M19,12C19,12.94 18.8,13.82 18.46,14.64L19.97,16.15C20.62,14.91 21,13.5 21,12C21,7.72 18,4.14 14,3.23V5.29C16.89,6.15 19,8.83 19,12M16.5,12C16.5,10.23 15.5,8.71 14,7.97V10.18L16.45,12.63C16.5,12.43 16.5,12.21 16.5,12Z");
        private readonly Color SECOUNDARY_COLOR = (Color)ColorConverter.ConvertFromString("#949f94");

        private IAudioSource _audioSource;
        private List<Track> _tracks = new List<Track>();

        private bool _isEnlarge = false;
        private bool _isPlaying;
        private bool _isRepeatModeEnabled;

        private SearchTextBox _searchTextBox;

        #endregion

        public PlayerWindow(IAudioSource audioSource)
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
            volumeIcon.Geometry = playingState.VolumePercentage > 0 ? VOLUME_ICON : VOLUME_OFF_ICON;

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

        private async void VolumeBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentPlayingState playingState = await _audioSource.GetCurrentPlayingState();


            if (playingState.VolumePercentage > 0)
            {
                await _audioSource.SetVolume(0);
                volumeIcon.Geometry = VOLUME_OFF_ICON;
            }
            else
            {
                await _audioSource.SetVolume(100);
                volumeIcon.Geometry = VOLUME_ICON;
            }
        }
        
        private void Window_Deactivated(object sender, System.EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        #endregion

    }
}
