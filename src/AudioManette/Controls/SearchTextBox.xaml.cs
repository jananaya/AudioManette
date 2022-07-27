using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

using AudioManette.AudioSource;

namespace AudioManette.Controls
{
    /// <summary>
    /// Interaction logic for SearchTextBox.xaml
    /// </summary>
    public partial class SearchTextBox : UserControl
    {
        private IAudioSource _audioSource;
        
        public delegate void SearchTrackEventHandler(object sender);
        public event SearchTrackEventHandler? searchTrackEvent;

        public SearchTextBox(IAudioSource audioSource)
        {
            _audioSource = audioSource;
            
            InitializeComponent();
        }
        
        
        private async void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (searchTrackEvent == null)
            {
                return;
            }

            var textbox = (TextBox)sender;

            if (textbox.Text == "")
            {
                return;
            }

            List<Track> tracks = await _audioSource.SearchTrack(textbox.Text);

            searchTrackEvent(tracks);
        }

        private void ClearSearchInputBtn_Click(object sender, RoutedEventArgs e)
        {
            searchTextbox.Text = "";
        }
    }
}
