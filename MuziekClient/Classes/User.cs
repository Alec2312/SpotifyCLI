// Bestand: User.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading; // Nodig voor Thread.Sleep

namespace MuziekClient.Classes
{
    public class User
    {
        public string Name { get; private set; }
        public List<Playlist> Playlists { get; private set; }
        // Vriendenlijst is verwijderd

        // Nieuwe enum voor playback commando's
        private enum PlaybackCommand { None, Stop, Next, Previous }

        public User(string name)
        {
            Name = name;
            Playlists = new List<Playlist>();
        }

        public Playlist CreatePlaylist(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Titel van speellijst mag niet leeg zijn.");
                return null!;
            }
            if (Playlists.Any(p => p.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Speellijst met de titel '{title}' bestaat al.");
                return null!;
            }
            Playlist newPlaylist = new Playlist(title);
            Playlists.Add(newPlaylist);
            Console.WriteLine($"{Name} heeft speellijst '{title}' aangemaakt.");
            return newPlaylist;
        }

        // Deze methode is nu de centrale, blocking afspeelfunctie
        private PlaybackCommand PlaySongBlocking(Song song, int startSecond = 0)
        {
            bool isPaused = false;
            Console.Clear(); // Maak de console leeg voor afspelen

            // Initialisatie van de afspeelweergave
            Console.WriteLine($"Speelt af: {song.Title} - {song.Artist} ({TimeSpan.FromSeconds(song.DurationInSeconds):mm\\:ss})");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("P: Pauze/Hervat | S: Stop | N: Volgend | V: Vorig | M: Hoofdmenu");
            Console.WriteLine("------------------------------------------");
            Console.Write($"Afgespeeld: 0/{song.DurationInSeconds} seconden "); // Initialiseer de voortgangsregel

            for (int i = startSecond; i < song.DurationInSeconds; i++)
            {
                // Toon de voortgang op dezelfde lijn
                Console.SetCursorPosition(12, Console.CursorTop); // Ga naar de startpositie van de voortgang
                Console.Write($"{i + 1}/{song.DurationInSeconds} seconden ");

                // Controleer op toetsaanslagen zonder te blokkeren
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true); // Lees toets in zonder deze te tonen

                    switch (keyInfo.KeyChar.ToString().ToUpper())
                    {
                        case "P":
                            isPaused = !isPaused;
                            if (isPaused)
                            {
                                Console.WriteLine("\nAfspelen gepauzeerd. Druk 'P' om te hervatten, 'S' om te stoppen.");
                                // Wacht in deze lus zolang gepauzeerd
                                while (isPaused)
                                {
                                    if (Console.KeyAvailable)
                                    {
                                        ConsoleKeyInfo pauseKeyInfo = Console.ReadKey(intercept: true);
                                        switch (pauseKeyInfo.KeyChar.ToString().ToUpper())
                                        {
                                            case "P":
                                                isPaused = false;
                                                Console.WriteLine("\nAfspelen hervat.");
                                                // Ga terug naar de voortgangslijn
                                                Console.SetCursorPosition(0, Console.CursorTop);
                                                Console.Write($"Afgespeeld: {i + 1}/{song.DurationInSeconds} seconden ");
                                                break;
                                            case "S":
                                                Console.WriteLine("\nAfspelen gestopt vanuit pauze.");
                                                return PlaybackCommand.Stop;
                                            default:
                                                // Niets doen, blijf in pauzemodus
                                                break;
                                        }
                                    }
                                    Thread.Sleep(100); // Korte pauze tijdens gepauzeerde staat om CPU te sparen
                                }
                            }
                            break;
                        case "S":
                            Console.WriteLine("\nAfspelen gestopt.");
                            return PlaybackCommand.Stop;
                        case "N":
                            Console.WriteLine("\nNaar volgend nummer...");
                            return PlaybackCommand.Next;
                        case "V":
                            Console.WriteLine("\nNaar vorig nummer...");
                            return PlaybackCommand.Previous;
                        case "M": // Optie om terug te gaan naar hoofdmenu
                            Console.WriteLine("\nTerug naar hoofdmenu. Afspelen gestopt.");
                            return PlaybackCommand.Stop; // Behandel als stop en ga terug
                        default:
                            // Ongeldige toets, negeer en ga door
                            break;
                    }
                }

                Thread.Sleep(1000); // Wacht 1 seconde
            }

            Console.WriteLine($"\n'{song.Title}' is afgelopen.");
            return PlaybackCommand.None; // Nummer is natuurlijk geëindigd
        }

        public void PlayCollection(SongCollection collection, bool shuffle = false)
        {
            List<Song> songsToPlay = collection.GetSongsForPlayback(shuffle);
            if (!songsToPlay.Any())
            {
                Console.WriteLine("De collectie bevat geen nummers om af te spelen.");
                return;
            }

            int currentIndex = 0;
            while (currentIndex >= 0 && currentIndex < songsToPlay.Count)
            {
                Song currentSong = songsToPlay[currentIndex];
                PlaybackCommand command = PlaySongBlocking(currentSong);

                switch (command)
                {
                    case PlaybackCommand.Next:
                        currentIndex++;
                        break;
                    case PlaybackCommand.Previous:
                        currentIndex--;
                        break;
                    case PlaybackCommand.Stop:
                        return; // Stop hele afspeellijst
                    case PlaybackCommand.None: // Nummer is afgelopen
                        currentIndex++;
                        break;
                }
            }
            Console.WriteLine("Einde van de afspeellijst bereikt.");
        }

        // StopPlayback wordt intern aangeroepen door de menu's (voor uitloggen/afsluiten)
        public void StopPlayback() { /* Status wordt beheerd door PlaySongBlocking */ }

        // Hoofdmenu voor gebruikers - MusicLibrary parameter verwijderd
        public void RunUserMainMenu(ref User? currentUser)
        {
            while (true)
            {
                Console.WriteLine($"\n--- Hoofdmenu ({Name}) ---\n1. Mijn speellijsten bekijken\n2. Nummers afspelen uit bibliotheek\n3. Albums afspelen uit bibliotheek\n4. Afmelden\n5. Afsluiten");
                Console.Write("Maak een keuze: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    // Geeft Program.AllSongs/AllAlbums door aan de interactieve methoden
                    case "1": ManagePlaylistsInteractively(); break;
                    case "2": PlayMusicInteractively(PlayableType.Song); break;
                    case "3": PlayMusicInteractively(PlayableType.Album); break;
                    case "4": StopPlayback(); currentUser = null; Console.WriteLine("U bent afgemeld."); return;
                    case "5": StopPlayback(); Console.WriteLine("Bedankt voor het gebruiken van MuziekClient!"); Environment.Exit(0); break;
                    default: Console.WriteLine("Ongeldige keuze. Probeer opnieuw."); break;
                }
            }
        }

        // MusicLibrary parameter verwijderd
        private void ManagePlaylistsInteractively()
        {
            while (true)
            {
                Console.WriteLine($"\n--- {Name}'s Speellijsten ---");
                if (!Playlists.Any()) Console.WriteLine("Je hebt nog geen speellijsten.");
                for (int i = 0; i < Playlists.Count; i++) Console.WriteLine($"{i + 1}. {Playlists[i].Title} ({Playlists[i].Songs.Count} nummers, {TimeSpan.FromSeconds(Playlists[i].GetTotalDuration()):mm\\:ss})");
                Console.WriteLine("\nOpties:\nA. Speellijst aanmaken\nB. Terug naar hoofdmenu");
                Console.Write("Maak een keuze of voer het nummer van een speellijst in om deze te beheren: ");
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int playlistIndex) && playlistIndex > 0 && playlistIndex <= Playlists.Count) ManageSpecificPlaylistInteractively(Playlists[playlistIndex - 1]);
                else if (input?.ToUpper() == "A")
                {
                    Console.Write("Voer de titel van de nieuwe speellijst in: ");
                    string? newPlaylistTitle = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newPlaylistTitle))
                    {
                        Playlist newPlaylist = CreatePlaylist(newPlaylistTitle);
                        if (newPlaylist != null)
                        {
                            Console.Write("Wil je nu nummers toevoegen aan deze nieuwe speellijst? (ja/nee): ");
                            if (Console.ReadLine()?.ToLower() == "ja") AddSongsOrAlbumsToPlaylistLoop(newPlaylist);
                        }
                    }
                    else Console.WriteLine("Ongeldige titel.");
                }
                else if (input?.ToUpper() == "B") return;
                else Console.WriteLine("Ongeldige keuze.");
            }
        }

        // MusicLibrary parameter verwijderd
        private void AddSongsOrAlbumsToPlaylistLoop(Playlist playlist)
        {
            while (true)
            {
                Console.WriteLine($"\n--- Nummers/albums toevoegen aan {playlist.Title} ---\n1. Voeg nummer toe\n2. Voeg album toe\n3. Klaar met toevoegen");
                Console.Write("Maak een keuze: ");
                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddSongToPlaylistInteractively(playlist); break;
                    case "2": AddAlbumToPlaylistInteractively(playlist); break;
                    case "3": return;
                    default: Console.WriteLine("Ongeldige keuze."); break;
                }
            }
        }

        // MusicLibrary parameter verwijderd
        private void ManageSpecificPlaylistInteractively(Playlist playlist)
        {
            while (true)
            {
                Console.WriteLine($"\n--- Beheer Speellijst: {playlist.Title} ({playlist.Songs.Count} nummers, {TimeSpan.FromSeconds(playlist.GetTotalDuration()):mm\\:ss}) ---");
                if (!playlist.Songs.Any()) Console.WriteLine("Deze speellijst is leeg.");
                else for (int i = 0; i < playlist.Songs.Count; i++) Console.WriteLine($"{i + 1}. {playlist.Songs[i].Title} - {playlist.Songs[i].Artist} ({TimeSpan.FromSeconds(playlist.Songs[i].DurationInSeconds):mm\\:ss})");
                Console.WriteLine("\nOpties:\n1. Nummer toevoegen aan speellijst\n2. Album toevoegen aan speellijst\n3. Nummer verwijderen uit speellijst\n4. Speellijst afspelen (sequentiëel)\n5. Speellijst afspelen (willekeurig)\n6. Terug naar mijn speellijsten");
                Console.Write("Maak een keuze: ");
                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddSongToPlaylistInteractively(playlist); break;
                    case "2": AddAlbumToPlaylistInteractively(playlist); break;
                    case "3": RemoveSongFromPlaylistInteractively(playlist); break;
                    case "4": PlayCollection(playlist, shuffle: false); break;
                    case "5": PlayCollection(playlist, shuffle: true); break;
                    case "6": return;
                    default: Console.WriteLine("Ongeldige keuze."); break;
                }
            }
        }

        // MusicLibrary parameter verwijderd, gebruikt nu Program.AllSongs
        private void AddSongToPlaylistInteractively(Playlist playlist)
        {
            Console.WriteLine("\n--- Nummer toevoegen aan speellijst ---");
            if (!Program.AllSongs.Any()) { Console.WriteLine("De muziekbibliotheek bevat nog geen nummers."); return; }
            Console.WriteLine("Beschikbare nummers in de bibliotheek:");
            for (int i = 0; i < Program.AllSongs.Count; i++) Console.WriteLine($"{i + 1}. {Program.AllSongs[i].Title} - {Program.AllSongs[i].Artist} ({TimeSpan.FromSeconds(Program.AllSongs[i].DurationInSeconds):mm\\:ss})");
            int songIndex = Program.GetIntegerInput("Voer het nummer in van het nummer dat je wilt toevoegen (of '0' voor terug): ");
            if (songIndex > 0 && songIndex <= Program.AllSongs.Count) playlist.AddSong(Program.AllSongs[songIndex - 1]);
            else if (songIndex == 0) { /* Terug */ }
            else Console.WriteLine("Ongeldige keuze.");
        }

        // MusicLibrary parameter verwijderd, gebruikt nu Program.AllAlbums
        private void AddAlbumToPlaylistInteractively(Playlist playlist)
        {
            Console.WriteLine("\n--- Album toevoegen aan speellijst ---");
            if (!Program.AllAlbums.Any()) { Console.WriteLine("De muziekbibliotheek bevat nog geen albums."); return; }
            Console.WriteLine("Beschikbare albums in de bibliotheek:");
            for (int i = 0; i < Program.AllAlbums.Count; i++) Console.WriteLine($"{i + 1}. {Program.AllAlbums[i].Title} by {Program.AllAlbums[i].Artist}");
            int albumIndex = Program.GetIntegerInput("Voer het nummer in van het album dat je wilt toevoegen (of '0' voor terug): ");
            if (albumIndex > 0 && albumIndex <= Program.AllAlbums.Count) playlist.AddAlbum(Program.AllAlbums[albumIndex - 1]);
            else if (albumIndex == 0) { /* Terug */ }
            else Console.WriteLine("Ongeldige keuze.");
        }

        private void RemoveSongFromPlaylistInteractively(Playlist playlist)
        {
            if (!playlist.Songs.Any()) { Console.WriteLine("De speellijst is al leeg."); return; }
            Console.WriteLine("Welk nummer wil je verwijderen?");
            for (int i = 0; i < playlist.Songs.Count; i++) Console.WriteLine($"{i + 1}. {playlist.Songs[i].Title} - {playlist.Songs[i].Artist}");
            int songIndex = Program.GetIntegerInput("Voer het nummer in: ");
            if (songIndex > 0 && songIndex <= playlist.Songs.Count) playlist.RemoveSong(playlist.Songs[songIndex - 1]);
            else Console.WriteLine("Ongeldige keuze.");
        }

        private enum PlayableType { Song, Album }
        // MusicLibrary parameter verwijderd, gebruikt nu Program.AllSongs/AllAlbums
        private void PlayMusicInteractively(PlayableType type)
        {
            if (type == PlayableType.Song)
            {
                Console.WriteLine("\n--- Nummers afspelen ---");
                if (!Program.AllSongs.Any()) { Console.WriteLine("Geen nummers beschikbaar in de bibliotheek."); return; }
                Console.WriteLine("Beschikbare nummers:");
                for (int i = 0; i < Program.AllSongs.Count; i++) Console.WriteLine($"{i + 1}. {Program.AllSongs[i].Title} - {Program.AllSongs[i].Artist} ({Program.AllSongs[i].Genre})");
                int songIndex = Program.GetIntegerInput("Voer het nummer in van de song die je wilt afspelen (of '0' voor terug): ");
                if (songIndex > 0 && songIndex <= Program.AllSongs.Count) PlaySongBlocking(Program.AllSongs[songIndex - 1]);
                else if (songIndex == 0) { /* Terug */ }
                else Console.WriteLine("Ongeldige keuze.");
            }
            else // PlayableType.Album
            {
                Console.WriteLine("\n--- Albums afspelen ---");
                if (!Program.AllAlbums.Any()) { Console.WriteLine("Geen albums beschikbaar."); return; }
                Console.WriteLine("Beschikbare albums:");
                for (int i = 0; i < Program.AllAlbums.Count; i++) Console.WriteLine($"{i + 1}. {Program.AllAlbums[i].Title} by {Program.AllAlbums[i].Artist}");
                int albumIndex = Program.GetIntegerInput("Voer het nummer van het album in om af te spelen (of '0' voor terug): ");
                if (albumIndex > 0 && albumIndex <= Program.AllAlbums.Count)
                {
                    Album selectedAlbum = Program.AllAlbums[albumIndex - 1];
                    Console.Write("Wilt u dit album in willekeurige volgorde afspelen? (ja/nee): ");
                    bool shuffle = Console.ReadLine()?.ToLower() == "ja";
                    PlayCollection(selectedAlbum, shuffle);
                }
                else if (albumIndex == 0) { /* Terug naar hoofdmenu */ }
                else Console.WriteLine("Ongeldige keuze.");
            }
        }
    }
}