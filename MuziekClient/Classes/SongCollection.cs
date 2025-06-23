// Bestand: SongCollection.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading; // Nodig voor CancellationToken (via Song)

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

        // De PlayAll en ShuffleAndPlay methoden zullen niet meer zelf de loop beheren.
        // Ze bereiden alleen de nummers voor in de juiste volgorde.
        // De daadwerkelijke afspeellogica (met pauzeren/stoppen/volgende) komt in User.cs
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
                return new List<Song>(Songs); // Retourneer een kopie
            }
        }

        public Playlist CompareWith(SongCollection other)
        {
            var sharedSongs = Songs.Intersect(other.Songs).ToList();
            return new Playlist($"Overeenkomstige nummers tussen '{this.Title}' en '{other.Title}'", sharedSongs);
        }
    }
}