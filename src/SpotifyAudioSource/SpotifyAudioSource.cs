using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using AudioManette.Authorization;
using SpotifyAPI.Web;

namespace AudioManette.AudioSource
{
    public class SpotifyAudioSource : IAudioSource
    {
        private SpotifyClient _spotify;
        private AuthotizationConfig _authInfo;

        private SpotifyAudioSource(AuthotizationConfig authInfo)
        {
            _authInfo = authInfo;
            _spotify = new SpotifyClient("");
        }

        private async Task InitializeAsync()
        {
            if (_authInfo.Code == null)
            {
                throw new System.Exception("Code is null");
            }

            var response = await new OAuthClient().RequestToken(
                new AuthorizationCodeTokenRequest(
                    _authInfo.ClientId,
                    _authInfo.ClientSecret,
                    _authInfo.Code,
                    _authInfo.RedirectUri
                )
            );

            try
            {
                _spotify = new SpotifyClient(response.AccessToken);
            } catch (System.Exception e)
            {
                throw new System.Exception(e.Message);
            }
        }

        public static async Task<SpotifyAudioSource> CreateAsync(AuthotizationConfig authInfo)
        {
            var spotifyService = new SpotifyAudioSource(authInfo);
            await spotifyService.InitializeAsync();
            return spotifyService;
        }

        public static string GetLoginUrl(AuthotizationConfig authInfo)
        {
            var loginRequest = new LoginRequest(
              authInfo.RedirectUri,
              authInfo.ClientId,
              LoginRequest.ResponseType.Code
            )
            {
                Scope = new[]
                {
                    Scopes.PlaylistReadPrivate,
                    Scopes.PlaylistReadCollaborative,
                    Scopes.Streaming,
                    Scopes.UserReadPlaybackState
                }
            };

            return loginRequest.ToUri().AbsoluteUri;
        }

        public async Task<bool> PausePlayback()
        {
            return await _spotify.Player.PausePlayback();
        }

        public async Task<bool> PlayTrack(string uri)
        {
            PlayerResumePlaybackRequest request = new PlayerResumePlaybackRequest
            {
                Uris = new List<string> { uri }
            };

            return await _spotify.Player.ResumePlayback(request);
        }

        public Task<bool> ResumePlayback()
        {
            return _spotify.Player.ResumePlayback();
        }

        public async Task<List<Track>> SearchTrack(string name)
        {
            List<Track> tracks = new();

            SearchRequest searchRequest = new(SearchRequest.Types.Track, name);

            SearchResponse searchResponse = await _spotify.Search.Item(searchRequest);

            List<FullTrack>? fulltracks = searchResponse.Tracks.Items;

            if (fulltracks is null) return tracks;

            tracks = SpotifyAudioSourceMapper.ToTrack(fulltracks);

            return tracks;
        }

        private async Task<bool> SetRepeatMode(PlayerSetRepeatRequest.State state)
        {
            PlayerSetRepeatRequest request = new(state);
            return await _spotify.Player.SetRepeat(request);
        }

        public async Task<bool> EnableRepeatMode()
        {
            return await SetRepeatMode(PlayerSetRepeatRequest.State.Track);
        }

        public async Task<bool> DisableRepeatMode()
        {
            return await SetRepeatMode(PlayerSetRepeatRequest.State.Off);
        }

        public async Task<bool> SetVolume(int volume)
        {
            PlayerVolumeRequest request = new(volume);

            return await _spotify.Player.SetVolume(request);
        }

        public async Task<Track> GetCurrentTrack()
        {
            PlayerCurrentlyPlayingRequest req = new();

            CurrentlyPlayingContext ctx = await _spotify.Player.GetCurrentPlayback();

            return SpotifyAudioSourceMapper.ToTrack(((FullTrack)ctx.Item));
        }
    }
}
