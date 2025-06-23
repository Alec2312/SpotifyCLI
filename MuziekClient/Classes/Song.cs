// Bestand: Song.cs
using System;
using System.Threading; // Nodig voor Thread.Sleep en CancellationToken
using System.Threading.Tasks; // Nodig voor Task.Delay

namespace MuziekClient.Classes
{
    public class Song
    {
        public string Title { get; private set; }
        public string Artist { get; private set; }
        public int DurationInSeconds { get; private set; }
        public Genre Genre { get; private set; }

        // Deze eigenschappen zijn nu public set om directere controle vanuit de Program-klasse mogelijk te maken.
        public bool IsPlaying { get; set; } = false;
        public bool IsPaused { get; set; } = false;

        public Song(string title, string artist, int durationInSeconds, Genre genre)
        {
            Title = title;
            Artist = artist;
            DurationInSeconds = durationInSeconds;
            Genre = genre;
        }

        public async void Play(CancellationToken cancellationToken)
        {
            if (IsPlaying && !IsPaused)
            {
                Console.WriteLine($"'{Title}' speelt al af.");
                return;
            }

            if (IsPaused)
            {
                Console.WriteLine($"Hervat afspelen: {Title} - {Artist}");
                IsPaused = false;
            }
            else
            {
                Console.WriteLine($"Speelt af: {Title} - {Artist} ({TimeSpan.FromSeconds(DurationInSeconds):mm\\:ss})");
            }

            IsPlaying = true;
            int elapsedSeconds = 0;

            while (elapsedSeconds < DurationInSeconds)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Stop();
                    return;
                }
                while (IsPaused)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Stop();
                        return;
                    }
                    await Task.Delay(100);
                }

                await Task.Delay(1000);
                elapsedSeconds++;
            }
            Console.WriteLine($"\n'{Title}' is afgelopen.");
            Stop();
        }

        public void Pause()
        {
            if (IsPlaying && !IsPaused)
            {
                Console.WriteLine($"Pauzeert: {Title} - {Artist}");
                IsPaused = true;
            }
            else if (IsPlaying && IsPaused)
            {
                Console.WriteLine($"'{Title}' is al gepauzeerd.");
            }
            else
            {
                Console.WriteLine($"'{Title}' speelt niet af en kan niet gepauzeerd worden.");
            }
        }

        public void Stop()
        {
            if (IsPlaying || IsPaused)
            {
                Console.WriteLine($"Stopt met afspelen: {Title} - {Artist}");
                IsPlaying = false;
                IsPaused = false;
            }
        }

        public void DisplaySongInfo()
        {
            Console.WriteLine($"  {Title} by {Artist} ({TimeSpan.FromSeconds(DurationInSeconds):mm\\:ss}) [{Genre}]");
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Song otherSong) return false;
            return Title.Equals(otherSong.Title, StringComparison.OrdinalIgnoreCase) &&
                   Artist.Equals(otherSong.Artist, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() => HashCode.Combine(Title.ToLower(), Artist.ToLower());

        public override string ToString() => $"{Title} by {Artist} ({DurationInSeconds}s, {Genre})";
    }
}
