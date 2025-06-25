using System;
using System.Linq;

namespace MuziekClient.Classes
{
    public class SuperUser : User
    {
        public SuperUser(string name) : base(name)
        {
            Console.WriteLine($"SuperUser '{Name}' is aangemaakt.");
        }

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

        public void RunSuperUserMainMenu(MusicLibrary musicLibrary, ref User? currentUser)
        {
            while (true)
            {
                // De optie 'Gebruikers beheren' is verwijderd, en de nummering is aangepast.
                Console.WriteLine($"\n--- SUPERUSER MENU ({Name}) ---\n1. Normaal gebruikersmenu\n2. Muziek toevoegen aan bibliotheek\n3. Afmelden\n4. Afsluiten");
                Console.Write("Maak een keuze: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": base.RunUserMainMenu(musicLibrary, ref currentUser); break; // Ga naar het normale gebruikersmenu
                    case "2": ManageLibraryContentInteractively(musicLibrary); break;
                    case "3": StopPlayback(); currentUser = null; Console.WriteLine("U bent afgemeld."); return;
                    case "4": StopPlayback(); Console.WriteLine("Bedankt voor het gebruiken van MuziekClient!"); Environment.Exit(0); break;
                    default: Console.WriteLine("Ongeldige keuze. Probeer opnieuw."); break;
                }
            }
        }

        private void ManageLibraryContentInteractively(MusicLibrary musicLibrary)
        {
            while (true)
            {
                Console.WriteLine("\n--- Beheer Bibliotheek Content ---\n1. Nummer toevoegen aan bibliotheek\n2. Album toevoegen aan bibliotheek\n3. Terug naar SuperUser menu");
                Console.Write("Maak een keuze: ");
                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddSongToLibraryInteractively(musicLibrary); break;
                    case "2": AddAlbumToLibraryInteractively(musicLibrary); break;
                    case "3": return;
                    default: Console.WriteLine("Ongeldige keuze."); break;
                }
            }
        }

        private void AddSongToLibraryInteractively(MusicLibrary musicLibrary)
        {
            Console.Write("Titel nummer: "); string? songTitle = Console.ReadLine();
            Console.Write("Artiest nummer: "); string? songArtist = Console.ReadLine();
            Console.Write("Duur in seconden: "); int songDuration = Program.GetIntegerInput("0");
            Console.Write("Genre (Pop, Rock, Metal, RnB, HipHop, Electronic, Classical, Jazz, Blues, Country): ");
            if (Enum.TryParse(Console.ReadLine(), true, out Genre songGenre)) AddSongToLibrary(musicLibrary, new Song(songTitle!, songArtist!, songDuration, songGenre));
            else Console.WriteLine("Ongeldig genre.");
        }

        private void AddAlbumToLibraryInteractively(MusicLibrary musicLibrary)
        {
            Console.Write("Titel album: "); string? albumTitle = Console.ReadLine();
            Console.Write("Artiest album: "); string? albumArtist = Console.ReadLine();
            Album newAlbum = new Album(albumTitle!, albumArtist!);
            Console.WriteLine("Je kunt nu nummers toevoegen aan dit album (type 'klaar' om te stoppen):");
            while (true)
            {
                Console.Write("Nummer titel (of 'klaar'): "); string? newAlbumSongTitle = Console.ReadLine();
                if (newAlbumSongTitle?.ToLower() == "klaar") break;
                Console.Write("Nummer artiest: "); string? newAlbumSongArtist = Console.ReadLine();
                Console.Write("Nummer duur (sec): "); int newAlbumSongDuration = Program.GetIntegerInput("0");
                Console.Write("Nummer genre: ");
                if (Enum.TryParse(Console.ReadLine(), true, out Genre newAlbumSongGenre)) newAlbum.AddSong(new Song(newAlbumSongTitle!, newAlbumSongArtist!, newAlbumSongDuration, newAlbumSongGenre));
                else Console.WriteLine("Ongeldig genre.");
            }
            AddAlbumToLibrary(musicLibrary, newAlbum);
        }
    }
}