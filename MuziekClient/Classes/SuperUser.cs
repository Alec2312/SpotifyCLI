// Bestand: SuperUser.cs
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

        // AddSongToLibrary past nu Program.AllSongs aan
        public void AddSongToLibrary(Song song)
        {
            Console.WriteLine($"SuperUser {Name} voegt nummer toe aan bibliotheek: {song.Title} - {song.Artist}");
            if (!Program.AllSongs.Any(s => s.Title.Equals(song.Title, StringComparison.OrdinalIgnoreCase) && s.Artist.Equals(song.Artist, StringComparison.OrdinalIgnoreCase)))
            {
                Program.AllSongs.Add(song);
            }
            else
            {
                Console.WriteLine($"Song '{song.Title}' is already in the library.");
            }
        }

        // AddAlbumToLibrary past nu Program.AllAlbums aan
        public void AddAlbumToLibrary(Album album)
        {
            Console.WriteLine($"SuperUser {Name} voegt album toe aan bibliotheek: {album.Title} by {album.Artist}");
            if (!Program.AllAlbums.Any(a => a.Title.Equals(album.Title, StringComparison.OrdinalIgnoreCase) && a.Artist.Equals(album.Artist, StringComparison.OrdinalIgnoreCase)))
            {
                Program.AllAlbums.Add(album);
                // Voeg ook de nummers van het album toe aan de globale lijst
                foreach (var song in album.Songs)
                {
                    AddSongToLibrary(song); // Gebruik de SuperUser's methode om het nummer toe te voegen
                }
            }
            else
            {
                Console.WriteLine($"Album '{album.Title}' is already in the library.");
            }
        }

        // MusicLibrary parameter verwijderd
        public void RunSuperUserMainMenu(ref User? currentUser)
        {
            while (true)
            {
                // De optie 'Gebruikers beheren' is verwijderd, en de nummering is aangepast.
                Console.WriteLine($"\n--- SUPERUSER MENU ({Name}) ---\n1. Normaal gebruikersmenu\n2. Muziek toevoegen aan bibliotheek\n3. Afmelden\n4. Afsluiten");
                Console.Write("Maak een keuze: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": base.RunUserMainMenu(ref currentUser); break; // Ga naar het normale gebruikersmenu
                    case "2": ManageLibraryContentInteractively(); break;
                    case "3": StopPlayback(); currentUser = null; Console.WriteLine("U bent afgemeld."); return;
                    case "4": StopPlayback(); Console.WriteLine("Bedankt voor het gebruiken van MuziekClient!"); Environment.Exit(0); break;
                    default: Console.WriteLine("Ongeldige keuze. Probeer opnieuw."); break;
                }
            }
        }

        // MusicLibrary parameter verwijderd
        private void ManageLibraryContentInteractively()
        {
            while (true)
            {
                Console.WriteLine("\n--- Beheer Bibliotheek Content ---\n1. Nummer toevoegen aan bibliotheek\n2. Album toevoegen aan bibliotheek\n3. Terug naar SuperUser menu");
                Console.Write("Maak een keuze: ");
                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddSongToLibraryInteractively(); break;
                    case "2": AddAlbumToLibraryInteractively(); break;
                    case "3": return;
                    default: Console.WriteLine("Ongeldige keuze."); break;
                }
            }
        }

        // AddSongToLibraryInteractively roept nu de AddSongToLibrary van de SuperUser aan
        private void AddSongToLibraryInteractively()
        {
            Console.Write("Titel nummer: "); string? songTitle = Console.ReadLine();
            Console.Write("Artiest nummer: "); string? songArtist = Console.ReadLine();
            Console.Write("Duur in seconden: "); int songDuration = Program.GetIntegerInput("0");
            Console.Write("Genre (Pop, Rock, Metal, RnB, HipHop, Electronic, Classical, Jazz, Blues, Country): ");
            if (Enum.TryParse(Console.ReadLine(), true, out Genre songGenre)) AddSongToLibrary(new Song(songTitle!, songArtist!, songDuration, songGenre));
            else Console.WriteLine("Ongeldig genre.");
        }

        // AddAlbumToLibraryInteractively roept nu de AddAlbumToLibrary van de SuperUser aan
        private void AddAlbumToLibraryInteractively()
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
            AddAlbumToLibrary(newAlbum);
        }
    }
}