// Bestand: Playlist.cs
using System;
using System.Collections.Generic;

namespace MuziekClient.Classes
{
    public class Playlist : SongCollection
    {
        public Playlist(string title) : base(title) { }

        public Playlist(string title, List<Song> songs) : base(title)
        {
            Songs.AddRange(songs);
        }

        public void AddSong(Song song)
        {
            if (!Songs.Contains(song))
            {
                Songs.Add(song);
                Console.WriteLine($"'{song.Title}' toegevoegd aan speellijst '{Title}'.");
            }
            else
            {
                Console.WriteLine($"'{song.Title}' staat al in speellijst '{Title}'.");
            }
        }

        public void RemoveSong(Song song)
        {
            if (Songs.Remove(song))
            {
                Console.WriteLine($"'{song.Title}' verwijderd uit speellijst '{Title}'.");
            }
            else
            {
                Console.WriteLine($"'{song.Title}' niet gevonden in speellijst '{Title}'.");
            }
        }

        public void AddAlbum(Album album)
        {
            Console.WriteLine($"Album '{album.Title}' toevoegen aan speellijst '{Title}'...");
            foreach (var song in album.Songs)
            {
                AddSong(song); // Gebruik de bestaande AddSong methode om duplicaten te voorkomen
            }
            Console.WriteLine($"Alle nummers van album '{album.Title}' zijn verwerkt.");
        }
    }
}