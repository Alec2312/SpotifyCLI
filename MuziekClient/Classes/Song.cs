// Bestand: Song.cs
using System;
using System.Threading;

namespace MuziekClient.Classes
{
    // Genre enum verplaatst van Genre.cs naar hier
    public enum Genre
    {
        Pop,
        Rock,
        Metal,
        RnB,
        HipHop,
        Electronic,
        Classical,
        Jazz,
        Blues,
        Country
    }

    public class Song
    {
        public string Title { get; private set; }
        public string Artist { get; private set; }
        public int DurationInSeconds { get; private set; }
        public Genre Genre { get; private set; }

        public Song(string title, string artist, int durationInSeconds, Genre genre)
        {
            Title = title;
            Artist = artist;
            DurationInSeconds = durationInSeconds;
            Genre = genre;
        }

        // De Play, Pause, Stop methoden hier zijn nu alleen voor simulatiedoeleinden
        // De echte afspeellogica en interactie zit in PlaySongBlocking van User.cs
        public void PlayMessage()
        {
            Console.WriteLine($"Afspelen gesimuleerd voor: {Title} - {Artist}");
        }

        public void PauseMessage()
        {
            Console.WriteLine($"'{Title}' gepauzeerd.");
        }

        public void StopMessage()
        {
            Console.WriteLine($"'{Title}' gestopt.");
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