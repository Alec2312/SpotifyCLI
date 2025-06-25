using System;
using System.Collections.Generic;
using System.Linq;

namespace MuziekClient.Classes
{
    public class MusicLibrary
    {
        public List<Song> AllSongs { get; private set; }
        public List<Album> AllAlbums { get; private set; }
        public List<User> RegisteredUsers { get; private set; }

        public MusicLibrary()
        {
            AllSongs = new List<Song>();
            AllAlbums = new List<Album>();
            RegisteredUsers = new List<User>();
        }

        public void AddSong(Song song)
        {
            if (song != null && !AllSongs.Any(s => s.Title.Equals(song.Title, StringComparison.OrdinalIgnoreCase) && s.Artist.Equals(song.Artist, StringComparison.OrdinalIgnoreCase)))
            {
                AllSongs.Add(song);
            }
            else if (song != null)
            {
                Console.WriteLine($"Song '{song.Title}' is already in the library.");
            }
        }

        public void AddAlbum(Album album)
        {
            if (album != null && !AllAlbums.Any(a => a.Title.Equals(album.Title, StringComparison.OrdinalIgnoreCase) && a.Artist.Equals(album.Artist, StringComparison.OrdinalIgnoreCase)))
            {
                AllAlbums.Add(album);
                foreach (var song in album.Songs)
                {
                    AddSong(song);
                }
            }
            else if (album != null)
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
            if (user != null && !RegisteredUsers.Any(u => u.Name.Equals(user.Name, StringComparison.OrdinalIgnoreCase)))
            {
                RegisteredUsers.Add(user);
                Console.WriteLine($"User '{user.Name}' registered.");
            }
            else if (user != null)
            {
                Console.WriteLine($"User '{user.Name}' is already registered.");
            }
        }
    }
}