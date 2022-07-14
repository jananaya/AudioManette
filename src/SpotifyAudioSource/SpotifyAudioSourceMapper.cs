using System;
using System.Collections.Generic;
using System.Linq;

using SpotifyAPI.Web;

namespace AudioManette.AudioSource
{
    internal class SpotifyAudioSourceMapper
    {
        public static Track ToTrack(FullTrack fulltrack)
        {
            return new Track(
                    fulltrack.Uri,
                    fulltrack.Name,
                    fulltrack.Artists.First().Name,
                    (
                        Little: FindImageUrlByWidth(fulltrack.Album.Images, 64),
                        Medium: FindImageUrlByWidth(fulltrack.Album.Images, 300)
                    ),
                    fulltrack.DurationMs
                );
        }

        public static List<Track> ToTrack(List<FullTrack> fullTracks)
        {
            List<Track> tracks = new List<Track>();

            foreach (var fulltrack in fullTracks)
            {
                tracks.Add(SpotifyAudioSourceMapper.ToTrack(fulltrack));
            }

            return tracks;
        }

        private static string FindImageUrlByWidth(List<Image> images, int width)
        {
            Image? image = images.Find(e => e.Width == width);

            return image?.Url ?? "";
        }
    }
}
