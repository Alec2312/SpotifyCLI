// Bestand: MusicLibrary.cs
using System.Collections.Generic;
using System.Linq;

namespace MuziekClient.Classes
{
    public class MusicLibrary
    {
        public List<Song> AllSongs { get; private set; }
        public List<Album> AllAlbums { get; private set; }
        public List<User> RegisteredUsers { get; private set; } // Kan zowel User als SuperUser bevatten

        public MusicLibrary()
        {
            AllSongs = new List<Song>();
            AllAlbums = new List<Album>();
            RegisteredUsers = new List<User>();
        }

        public void AddSong(Song song)
        {
            if (!AllSongs.Contains(song))
            {
                AllSongs.Add(song);
                // Console.WriteLine($"Song '{song.Title}' added to the library."); // Verplaatst naar SuperUser for feedback
            }
            else
            {
                Console.WriteLine($"Song '{song.Title}' is already in the library.");
            }
        }

        public void AddAlbum(Album album)
        {
            if (!AllAlbums.Contains(album))
            {
                AllAlbums.Add(album);
                // Console.WriteLine($"Album '{album.Title}' added to the library."); // Verplaatst naar SuperUser for feedback
                foreach (var song in album.Songs)
                {
                    AddSong(song); // Zorg ervoor dat alle nummers van het album ook in AllSongs komen
                }
            }
            else
            {
                Console.WriteLine($"Album '{album.Title}' is already in the library.");
            }
        }

        public Song? SearchSong(string title, string artist)
        {
            return AllSongs.FirstOrDefault(s => s.Title.Equals(title, StringComparison.OrdinalIgnoreCase) &&
                                               s.Artist.Equals(artist, StringComparison.OrdinalIgnoreCase));
        }

        public Album? SearchAlbum(string title)
        {
            return AllAlbums.FirstOrDefault(a => a.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        }

        public List<Song> GetSongsByArtist(string artist)
        {
            return AllSongs.Where(s => s.Artist.Equals(artist, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public void RegisterUser(User user)
        {
            if (!RegisteredUsers.Any(u => u.Name.Equals(user.Name, StringComparison.OrdinalIgnoreCase)))
            {
                RegisteredUsers.Add(user);
                Console.WriteLine($"User '{user.Name}' registered.");
            }
            else
            {
                Console.WriteLine($"User '{user.Name}' is already registered.");
            }
        }
    }
}