using System;

namespace AudioManette.AudioSource
{
    public class CurrentPlayingState
    {
        public CurrentPlayingState(bool isPlaying, bool isRepeatModeEnabled,
            float volumePercentage, Track track)
        {
            IsPlaying = isPlaying;
            IsRepeatModeEnabled = isRepeatModeEnabled;
            VolumePercentage = volumePercentage;
            Track = track;
        }

        public bool IsPlaying { get; set; }
        public bool IsRepeatModeEnabled { get; set; }
        public float VolumePercentage { get; set; }
        public Track Track { get; set; }
    }
}
