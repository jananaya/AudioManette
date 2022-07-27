using System.Threading.Tasks;
using System.Collections.Generic;

namespace AudioManette.AudioSource
{
    public interface IAudioSource
    {
        Task<List<Track>> SearchTrack(string name);
        Task<bool> PlayTrack(string uri);
        Task<bool> PausePlayback();
        Task<bool> ResumePlayback();
        Task<bool> SetVolume(int volumePercent); 
        Task<bool> EnableRepeatMode();
        Task<bool> DisableRepeatMode();
        Task<CurrentPlayingState> GetCurrentPlayingState();
    }
}
