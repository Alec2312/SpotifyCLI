// Bestand: Album.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace MuziekClient.Classes
{
    public class Album : SongCollection
    {
        public string Artist { get; private set; }

        public Album(string title, string artist) : base(title)
        {
            Artist = artist;
        }

        public void AddSong(Song song)
        {
            if (song.Artist != Artist)
            {
                Console.WriteLine($"Waarschuwing: Nummer '{song.Title}' van {song.Artist} past niet bij album artiest '{Artist}'.");
            }
            if (!Songs.Contains(song))
            {
                Songs.Add(song);
                Console.WriteLine($"'{song.Title}' toegevoegd aan album '{Title}'.");
            }
            else
            {
                Console.WriteLine($"'{song.Title}' staat al in album '{Title}'.");
            }
        }

        public string GetArtist()
        {
            return Artist;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Album: {Title} by {Artist} ({Songs.Count} nummers, {TimeSpan.FromSeconds(GetTotalDuration()):mm\\:ss})");
            foreach (var song in Songs)
            {
                song.DisplaySongInfo();
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Album otherAlbum) return false;
            return base.Equals(otherAlbum) && Artist.Equals(otherAlbum.Artist, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Artist.ToLower());
        }
    }
}