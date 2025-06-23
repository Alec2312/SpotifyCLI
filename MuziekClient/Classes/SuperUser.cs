// Bestand: SuperUser.cs
using System;

namespace MuziekClient.Classes
{
    public class SuperUser : User
    {
        public SuperUser(string name) : base(name)
        {
            Console.WriteLine($"SuperUser '{Name}' is aangemaakt.");
        }

        // Nieuwe functionaliteiten die alleen een SuperUser kan doen
        public void AddSongToLibrary(MusicLibrary library, Song song)
        {
            Console.WriteLine($"SuperUser {Name} voegt nummer toe aan bibliotheek: {song.Title} - {song.Artist}");
            library.AddSong(song);
        }

        public void AddAlbumToLibrary(MusicLibrary library, Album album)
        {
            Console.WriteLine($"SuperUser {Name} voegt album toe aan bibliotheek: {album.Title} by {album.Artist}");
            library.AddAlbum(album);
        }

        // Je kunt hier meer methoden toevoegen voor admin-specifieke taken
        // zoals:
        // public void RemoveUser(MusicLibrary library, User userToRemove) { /* ... */ }
        // public void ViewAllUsers(MusicLibrary library) { /* ... */ }
    }
}