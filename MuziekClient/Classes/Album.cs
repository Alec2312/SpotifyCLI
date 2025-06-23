// Bestand: Album.cs
using System;
using System.Collections.Generic;

namespace MuziekClient.Classes
{
    public class Album : SongCollection
    {
        public string Artist { get; private set; } // Een album heeft meestal één hoofdartiest

        public Album(string title, string artist) : base(title)
        {
            Artist = artist;
        }

        public void AddSong(Song song)
        {
            // Optionele waarschuwing als de artiest van het nummer niet overeenkomt met de album artiest
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
    }
}