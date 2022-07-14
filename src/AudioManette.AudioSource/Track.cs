namespace AudioManette.AudioSource
{
    public class Track
    {
        public Track(string uri, string title, string artist, (string Little, string Medium) imagePath, float durationMs)
        {
            Uri = uri;
            Title = title;
            Artist = artist;
            ImagePath = imagePath;
            DurationMs = durationMs;
        }

        public string Uri { get; }
        public string Title { get; }
        public string Artist { get; }
        public (string Little, string Medium) ImagePath { get; }
        public float DurationMs { get; }
    }
}
