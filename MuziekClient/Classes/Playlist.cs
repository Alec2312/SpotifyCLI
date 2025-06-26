// Bestand: Playlist.cs
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (song != null && !Songs.Contains(song))
            {
                Songs.Add(song);
                Console.WriteLine($"'{song.Title}' toegevoegd aan speellijst '{Title}'.");
            }
            else if (song == null)
            {
                Console.WriteLine("Ongeldig nummer. Kan niet toevoegen aan speellijst.");
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
            if (album == null || !album.Songs.Any())
            {
                Console.WriteLine("Album is leeg of ongeldig. Geen nummers toegevoegd.");
                return;
            }

            Console.WriteLine($"Album '{album.Title}' toevoegen aan speellijst '{Title}'...");
            foreach (var song in album.Songs)
            {
                AddSong(song);
            }
            Console.WriteLine($"Alle nummers van album '{album.Title}' zijn verwerkt.");
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Speellijst: {Title} ({Songs.Count} nummers, {TimeSpan.FromSeconds(GetTotalDuration()):mm\\:ss})");
            if (!Songs.Any())
            {
                Console.WriteLine("  Deze speellijst is leeg.");
                return;
            }
            foreach (var song in Songs)
            {
                song.DisplaySongInfo();
            }
        }
    }
}