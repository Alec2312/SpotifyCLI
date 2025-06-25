// Bestand: SongCollection.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace MuziekClient.Classes
{
    public abstract class SongCollection
    {
        public string Title { get; set; }
        public List<Song> Songs { get; protected set; }

        public SongCollection(string title)
        {
            Title = title;
            Songs = new List<Song>();
        }

        public int GetTotalDuration()
        {
            return Songs.Sum(song => song.DurationInSeconds);
        }

        public virtual List<Song> GetSongsForPlayback(bool shuffle = false)
        {
            if (!Songs.Any())
            {
                Console.WriteLine($"De verzameling '{Title}' bevat geen nummers om af te spelen.");
                return new List<Song>();
            }

            if (shuffle)
            {
                Console.WriteLine($"Voorbereiden van '{Title}' in willekeurige volgorde...");
                return Songs.OrderBy(s => Guid.NewGuid()).ToList();
            }
            else
            {
                Console.WriteLine($"Voorbereiden van '{Title}' in sequentiële volgorde...");
                return new List<Song>(Songs);
            }
        }

        public Playlist CompareWith(SongCollection other)
        {
            var sharedSongs = Songs.Intersect(other.Songs, new SongComparer()).ToList();
            return new Playlist($"Overeenkomstige nummers tussen '{this.Title}' en '{other.Title}'", sharedSongs);
        }

        private class SongComparer : IEqualityComparer<Song>
        {
            public bool Equals(Song? x, Song? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;
                return x.Title.Equals(y.Title, StringComparison.OrdinalIgnoreCase) &&
                       x.Artist.Equals(y.Artist, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(Song obj)
            {
                return HashCode.Combine(obj.Title.ToLower(), obj.Artist.ToLower());
            }
        }

        public abstract void DisplayInfo();

        public override bool Equals(object? obj)
        {
            if (obj is not SongCollection other) return false;
            return Title.Equals(other.Title, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Title.ToLower().GetHashCode();
        }
    }
}