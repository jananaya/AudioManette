using System;
using System.Windows;
using System.Windows.Controls;
using AudioManette.AudioSource;

namespace AudioManette.Controls
{
    /// <summary>
    /// Interaction logic for TrackCard.xaml
    /// </summary>
    public partial class TrackCard : UserControl
    {
        private IAudioSource _audioSource;
        private Track _track;
        
        public event TrackChangedEventHandler? trackChangedEvent;
        public delegate void TrackChangedEventHandler(object sender);
        
        public TrackCard(IAudioSource audioSource, Track track)
        {
            _audioSource = audioSource;
            _track = track;

            var duration = new TimeSpan(0, 0, 0, 0, (int)track.DurationMs);
            var durationString = duration.ToString(@"mm\:ss");

            this.DataContext = new
            {
                ImagePath = track.ImagePath.Medium,
                Title = track.Title,
                Artist = track.Artist,
                Duration = durationString
            };

            InitializeComponent();
        }

        private async void TrackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (trackChangedEvent != null)
            {
                await _audioSource.PlayTrack(_track.Uri);
                trackChangedEvent(_track);
            }
        }
    }
}
