// Bestand: Program.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuziekClient.Classes
{
    class Program
    {
        private static MusicLibrary musicLibrary = new MusicLibrary();
        private static User? currentUser = null;

        static void Main(string[] args)
        {
            InitializeData();
            Console.WriteLine("Welkom bij de MuziekClient!");
            while (true)
            {
                if (currentUser == null) ShowLoginMenu();
                else if (currentUser is SuperUser) ShowSuperUserMainMenu();
                else ShowNormalUserMainMenu();
            }
        }

        static void InitializeData()
        {
            var bohemianRhapsody = new Song("Bohemian Rhapsody", "Queen", 355, Genre.Rock);
            var thriller = new Song("Thriller", "Michael Jackson", 357, Genre.Pop);
            var hotelCalifornia = new Song("Hotel California", "Eagles", 390, Genre.Rock);
            var billieJean = new Song("Billie Jean", "Michael Jackson", 294, Genre.Pop);
            var fadeToBlack = new Song("Fade to Black", "Metallica", 412, Genre.Metal);
            var smoothCriminal = new Song("Smooth Criminal", "Michael Jackson", 258, Genre.Pop);
            var summerBreeze = new Song("Summer Breeze", "Seals and Crofts", 305, Genre.Country);

            var queenGreatestHits = new Album("Queen's Greatest Hits", "Queen");
            queenGreatestHits.AddSong(bohemianRhapsody);
            musicLibrary.AddAlbum(queenGreatestHits);

            var thrillerAlbum = new Album("Thriller", "Michael Jackson");
            thrillerAlbum.AddSong(thriller);
            thrillerAlbum.AddSong(billieJean);
            thrillerAlbum.AddSong(smoothCriminal);
            musicLibrary.AddAlbum(thrillerAlbum);

            var masterOfPuppets = new Album("Master of Puppets", "Metallica");
            masterOfPuppets.AddSong(fadeToBlack);
            musicLibrary.AddAlbum(masterOfPuppets);

            var alice = new User("Alice");
            var bob = new User("Bob");
            var charlie = new User("Charlie");
            var admin = new SuperUser("Admin");

            musicLibrary.RegisterUser(alice);
            musicLibrary.RegisterUser(bob);
            musicLibrary.RegisterUser(charlie);
            musicLibrary.RegisterUser(admin);

            var aliceRockPlaylist = alice.CreatePlaylist("Alice's Rock Favorieten");
            aliceRockPlaylist.AddSong(bohemianRhapsody);
            aliceRockPlaylist.AddSong(hotelCalifornia);
            aliceRockPlaylist.AddSong(fadeToBlack);

            var alicePopPlaylist = alice.CreatePlaylist("Alice's Pop Hits");
            alicePopPlaylist.AddSong(thriller);
            alicePopPlaylist.AddSong(billieJean);
            alicePopPlaylist.AddAlbum(thrillerAlbum);

            var bobPartyPlaylist = bob.CreatePlaylist("Bob's Party Mix");
            bobPartyPlaylist.AddSong(thriller);
            bobPartyPlaylist.AddSong(smoothCriminal);
            bobPartyPlaylist.AddSong(bohemianRhapsody);

            var charlieChillPlaylist = charlie.CreatePlaylist("Charlie's Chill Vibes");
            charlieChillPlaylist.AddSong(hotelCalifornia);
            charlieChillPlaylist.AddSong(summerBreeze);

            alice.AddFriend(bob);
            alice.AddFriend(charlie);
            bob.AddFriend(alice);
        }

        static void ShowLoginMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Login ---\n1. Inloggen\n2. Registreren\n3. Afsluiten");
                Console.Write("Maak een keuze: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": LoginUser(); return;
                    case "2": RegisterNewUser(); break;
                    case "3": Console.WriteLine("Bedankt voor het gebruiken van MuziekClient!"); Environment.Exit(0); break;
                    default: Console.WriteLine("Ongeldige keuze. Probeer opnieuw."); PressAnyKeyToContinue(); break;
                }
            }
        }

        static void LoginUser()
        {
            Console.Write("Voer gebruikersnaam in: ");
            string? username = Console.ReadLine();
            currentUser = musicLibrary.RegisteredUsers.FirstOrDefault(u => u.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (currentUser == null) Console.WriteLine("Gebruiker niet gevonden. Probeer opnieuw of registreer een nieuw account.");
            else Console.WriteLine($"Welkom, {currentUser.Name}!");
            PressAnyKeyToContinue();
        }

        static void RegisterNewUser()
        {
            Console.Write("Voer een nieuwe gebruikersnaam in: ");
            string? newUsername = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newUsername)) { Console.WriteLine("Gebruikersnaam mag niet leeg zijn."); PressAnyKeyToContinue(); return; }
            if (musicLibrary.RegisteredUsers.Any(u => u.Name.Equals(newUsername, StringComparison.OrdinalIgnoreCase)))
            { Console.WriteLine($"Gebruikersnaam '{newUsername}' bestaat al. Kies een andere naam."); PressAnyKeyToContinue(); return; }

            User newUser = new User(newUsername);
            musicLibrary.RegisterUser(newUser);
            Console.WriteLine($"Gebruiker '{newUsername}' succesvol geregistreerd! U kunt nu inloggen.");
            PressAnyKeyToContinue();
        }

        static void ShowNormalUserMainMenu()
        {
            while (true)
            {
                Console.WriteLine($"\n--- Hoofdmenu ({currentUser?.Name}) ---");
                Console.WriteLine("1. Mijn speellijsten bekijken\n2. Nummers afspelen uit bibliotheek\n3. Albums afspelen uit bibliotheek\n4. Vrienden beheren/bekijken\n5. Afspeelcontrole (Pauze, Stop, Volgende, Vorige, Herhaal)\n6. Afmelden\n7. Afsluiten");
                Console.Write("Maak een keuze: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ViewMyPlaylists(); break;
                    case "2": PlaySongsMenu(); break;
                    case "3": PlayAlbumsMenu(); break;
                    case "4": FriendsMenu(); break;
                    case "5": PlaybackControlMenu(); break;
                    case "6": currentUser?.StopPlayback(); currentUser = null; Console.WriteLine("U bent afgemeld."); PressAnyKeyToContinue(); return;
                    case "7": currentUser?.StopPlayback(); Console.WriteLine("Bedankt voor het gebruiken van MuziekClient!"); Environment.Exit(0); break;
                    default: Console.WriteLine("Ongeldige keuze. Probeer opnieuw."); PressAnyKeyToContinue(); break;
                }
            }
        }

        static void ShowSuperUserMainMenu()
        {
            while (true)
            {
                Console.WriteLine($"\n--- SUPERUSER MENU ({currentUser?.Name}) ---\n1. Normaal gebruikersmenu\n2. Muziek toevoegen aan bibliotheek\n3. Gebruikers beheren (niet geïmplementeerd)\n4. Afmelden\n5. Afsluiten");
                Console.Write("Maak een keuze: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ShowNormalUserMainMenu(); break;
                    case "2": ManageLibraryContent(); break;
                    case "3": Console.WriteLine("Deze functionaliteit is nog niet geïmplementeerd."); PressAnyKeyToContinue(); break;
                    case "4": currentUser?.StopPlayback(); currentUser = null; Console.WriteLine("U bent afgemeld."); PressAnyKeyToContinue(); return;
                    case "5": currentUser?.StopPlayback(); Console.WriteLine("Bedankt voor het gebruiken van MuziekClient!"); Environment.Exit(0); break;
                    default: Console.WriteLine("Ongeldige keuze. Probeer opnieuw."); PressAnyKeyToContinue(); break;
                }
            }
        }

        static void ManageLibraryContent()
        {
            if (currentUser is not SuperUser adminUser) { Console.WriteLine("Alleen SuperUsers kunnen content beheren."); PressAnyKeyToContinue(); return; }
            while (true)
            {
                Console.WriteLine("\n--- Beheer Bibliotheek Content ---\n1. Nummer toevoegen aan bibliotheek\n2. Album toevoegen aan bibliotheek\n3. Terug naar SuperUser menu");
                Console.Write("Maak een keuze: ");
                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddSongToLibraryAsAdmin(adminUser); break;
                    case "2": AddAlbumToLibraryAsAdmin(adminUser); break;
                    case "3": return;
                    default: Console.WriteLine("Ongeldige keuze."); PressAnyKeyToContinue(); break;
                }
            }
        }

        static void AddSongToLibraryAsAdmin(SuperUser adminUser)
        {
            Console.Write("Titel nummer: "); string? songTitle = Console.ReadLine();
            Console.Write("Artiest nummer: "); string? songArtist = Console.ReadLine();
            Console.Write("Duur in seconden: "); int songDuration = GetIntegerInput("0");
            Console.Write("Genre (Pop, Rock, Metal, RnB, HipHop, Electronic, Classical, Jazz, Blues, Country): ");
            if (Enum.TryParse(Console.ReadLine(), true, out Genre songGenre)) adminUser.AddSongToLibrary(musicLibrary, new Song(songTitle!, songArtist!, songDuration, songGenre));
            else Console.WriteLine("Ongeldig genre.");
            PressAnyKeyToContinue();
        }

        static void AddAlbumToLibraryAsAdmin(SuperUser adminUser)
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
                Console.Write("Nummer duur (sec): "); int newAlbumSongDuration = GetIntegerInput("0");
                Console.Write("Nummer genre: ");
                if (Enum.TryParse(Console.ReadLine(), true, out Genre newAlbumSongGenre)) newAlbum.AddSong(new Song(newAlbumSongTitle!, newAlbumSongArtist!, newAlbumSongDuration, newAlbumSongGenre));
                else Console.WriteLine("Ongeldig genre.");
            }
            adminUser.AddAlbumToLibrary(musicLibrary, newAlbum);
            PressAnyKeyToContinue();
        }

        static void ViewMyPlaylists()
        {
            if (currentUser == null) return;
            while (true)
            {
                Console.WriteLine($"\n--- {currentUser.Name}'s Speellijsten ---");
                if (!currentUser.Playlists.Any()) Console.WriteLine("Je hebt nog geen speellijsten.");
                for (int i = 0; i < currentUser.Playlists.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {currentUser.Playlists[i].Title} ({currentUser.Playlists[i].Songs.Count} nummers, {TimeSpan.FromSeconds(currentUser.Playlists[i].GetTotalDuration()):mm\\:ss})");
                }
                Console.WriteLine("\nOpties:\nA. Speellijst aanmaken\nB. Terug naar hoofdmenu");
                Console.Write("Maak een keuze of voer het nummer van een speellijst in om deze te beheren: ");
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int playlistIndex) && playlistIndex > 0 && playlistIndex <= currentUser.Playlists.Count) ManagePlaylist(currentUser.Playlists[playlistIndex - 1]);
                else if (input?.ToUpper() == "A")
                {
                    Console.Write("Voer de titel van de nieuwe speellijst in: ");
                    string? newPlaylistTitle = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newPlaylistTitle))
                    {
                        Playlist newPlaylist = currentUser.CreatePlaylist(newPlaylistTitle);
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
                PressAnyKeyToContinue();
            }
        }

        static void AddSongsOrAlbumsToPlaylistLoop(Playlist playlist)
        {
            while (true)
            {
                Console.WriteLine($"\n--- Nummers/albums toevoegen aan {playlist.Title} ---\n1. Voeg nummer toe\n2. Voeg album toe\n3. Klaar met toevoegen");
                Console.Write("Maak een keuze: ");
                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddSongToPlaylist(playlist); break;
                    case "2": AddAlbumToPlaylist(playlist); break;
                    case "3": return;
                    default: Console.WriteLine("Ongeldige keuze."); break;
                }
                PressAnyKeyToContinue();
            }
        }

        static void ManagePlaylist(Playlist playlist)
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
                    case "1": AddSongToPlaylist(playlist); break;
                    case "2": AddAlbumToPlaylist(playlist); break;
                    case "3": RemoveSongFromPlaylist(playlist); break;
                    case "4": currentUser?.PlayCollection(playlist, shuffle: false); break;
                    case "5": currentUser?.PlayCollection(playlist, shuffle: true); break;
                    case "6": return;
                    default: Console.WriteLine("Ongeldige keuze."); break;
                }
                PressAnyKeyToContinue();
            }
        }

        static void AddSongToPlaylist(Playlist playlist)
        {
            Console.WriteLine("\n--- Nummer toevoegen aan speellijst ---");
            if (!musicLibrary.AllSongs.Any()) { Console.WriteLine("De muziekbibliotheek bevat nog geen nummers."); return; }
            Console.WriteLine("Beschikbare nummers in de bibliotheek:");
            for (int i = 0; i < musicLibrary.AllSongs.Count; i++) Console.WriteLine($"{i + 1}. {musicLibrary.AllSongs[i].Title} - {musicLibrary.AllSongs[i].Artist} ({TimeSpan.FromSeconds(musicLibrary.AllSongs[i].DurationInSeconds):mm\\:ss})");
            int songIndex = GetIntegerInput("Voer het nummer in van het nummer dat je wilt toevoegen (of '0' voor terug): ");

            if (songIndex > 0 && songIndex <= musicLibrary.AllSongs.Count) playlist.AddSong(musicLibrary.AllSongs[songIndex - 1]);
            else if (songIndex == 0) { /* Terug */ }
            else Console.WriteLine("Ongeldige keuze.");
        }

        static void AddAlbumToPlaylist(Playlist playlist)
        {
            Console.WriteLine("\n--- Album toevoegen aan speellijst ---");
            if (!musicLibrary.AllAlbums.Any()) { Console.WriteLine("De muziekbibliotheek bevat nog geen albums."); return; }
            Console.WriteLine("Beschikbare albums in de bibliotheek:");
            for (int i = 0; i < musicLibrary.AllAlbums.Count; i++) Console.WriteLine($"{i + 1}. {musicLibrary.AllAlbums[i].Title} by {musicLibrary.AllAlbums[i].Artist}");
            int albumIndex = GetIntegerInput("Voer het nummer in van het album dat je wilt toevoegen (of '0' voor terug): ");

            if (albumIndex > 0 && albumIndex <= musicLibrary.AllAlbums.Count) playlist.AddAlbum(musicLibrary.AllAlbums[albumIndex - 1]);
            else if (albumIndex == 0) { /* Terug */ }
            else Console.WriteLine("Ongeldige keuze.");
        }

        static void RemoveSongFromPlaylist(Playlist playlist)
        {
            if (!playlist.Songs.Any()) { Console.WriteLine("De speellijst is al leeg."); return; }
            Console.WriteLine("Welk nummer wil je verwijderen?");
            for (int i = 0; i < playlist.Songs.Count; i++) Console.WriteLine($"{i + 1}. {playlist.Songs[i].Title} - {playlist.Songs[i].Artist}");
            int songIndex = GetIntegerInput("Voer het nummer in: ");

            if (songIndex > 0 && songIndex <= playlist.Songs.Count) playlist.RemoveSong(playlist.Songs[songIndex - 1]);
            else Console.WriteLine("Ongeldige keuze.");
        }

        static void PlaySongsMenu()
        {
            Console.WriteLine("\n--- Nummers afspelen ---");
            if (!musicLibrary.AllSongs.Any()) { Console.WriteLine("Geen nummers beschikbaar in de bibliotheek."); PressAnyKeyToContinue(); return; }
            Console.WriteLine("Beschikbare nummers:");
            for (int i = 0; i < musicLibrary.AllSongs.Count; i++) Console.WriteLine($"{i + 1}. {musicLibrary.AllSongs[i].Title} - {musicLibrary.AllSongs[i].Artist} ({musicLibrary.AllSongs[i].Genre})");
            int songIndex = GetIntegerInput("Voer het nummer in van de song die je wilt afspelen (of '0' voor terug): ");

            if (songIndex > 0 && songIndex <= musicLibrary.AllSongs.Count) currentUser?.PlaySong(musicLibrary.AllSongs[songIndex - 1]);
            else if (songIndex == 0) { /* Terug */ }
            else Console.WriteLine("Ongeldige keuze.");
            PressAnyKeyToContinue();
        }

        static void PlayAlbumsMenu()
        {
            Console.WriteLine("\n--- Albums afspelen ---");
            if (!musicLibrary.AllAlbums.Any()) { Console.WriteLine("Geen albums beschikbaar."); PressAnyKeyToContinue(); return; }
            Console.WriteLine("Beschikbare albums:");
            for (int i = 0; i < musicLibrary.AllAlbums.Count; i++) Console.WriteLine($"{i + 1}. {musicLibrary.AllAlbums[i].Title} by {musicLibrary.AllAlbums[i].Artist}");
            int albumIndex = GetIntegerInput("Voer het nummer van het album in om af te spelen (of '0' voor terug): ");

            if (albumIndex > 0 && albumIndex <= musicLibrary.AllAlbums.Count)
            {
                Album selectedAlbum = musicLibrary.AllAlbums[albumIndex - 1];
                Console.Write("Wilt u dit album in willekeurige volgorde afspelen? (ja/nee): ");
                bool shuffle = Console.ReadLine()?.ToLower() == "ja";
                currentUser?.PlayCollection(selectedAlbum, shuffle);
            }
            else if (albumIndex == 0) { /* Terug naar hoofdmenu */ }
            else Console.WriteLine("Ongeldige keuze.");
            PressAnyKeyToContinue();
        }

        static void FriendsMenu()
        {
            if (currentUser == null) return;
            while (true)
            {
                Console.WriteLine("\n--- Vriendenbeheer ---\n1. Vriendenlijst bekijken\n2. Speellijsten van een vriend bekijken\n3. Speellijst vergelijken met die van een vriend\n4. Terug naar hoofdmenu");
                Console.Write("Maak een keuze: ");
                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": ViewFriendsList(); break;
                    case "2": ViewFriendPlaylistsOption(); break;
                    case "3": CompareFriendPlaylistsOption(); break;
                    case "4": return;
                    default: Console.WriteLine("Ongeldige keuze."); PressAnyKeyToContinue(); break;
                }
            }
        }

        static void ViewFriendsList()
        {
            if (currentUser == null) return;
            Console.WriteLine($"\n--- {currentUser.Name}'s Vrienden ---");
            if (!currentUser.Friends.Any()) Console.WriteLine("Je hebt nog geen vrienden toegevoegd.");
            else for (int i = 0; i < currentUser.Friends.Count; i++) Console.WriteLine($"{i + 1}. {currentUser.Friends[i].Name}");
            PressAnyKeyToContinue();
        }

        static void ViewFriendPlaylistsOption()
        {
            if (currentUser == null || !currentUser.Friends.Any()) { Console.WriteLine("Je hebt nog geen vrienden om hun speellijsten te bekijken."); PressAnyKeyToContinue(); return; }
            Console.WriteLine("\nWelke vriend's speellijsten wil je bekijken?");
            for (int i = 0; i < currentUser.Friends.Count; i++) Console.WriteLine($"{i + 1}. {currentUser.Friends[i].Name}");
            int friendIndex = GetIntegerInput("Voer het nummer in: ");

            if (friendIndex > 0 && friendIndex <= currentUser.Friends.Count) currentUser.ViewFriendPlaylists(currentUser.Friends[friendIndex - 1]);
            else Console.WriteLine("Ongeldige keuze.");
            PressAnyKeyToContinue();
        }

        static void CompareFriendPlaylistsOption()
        {
            if (currentUser == null || !currentUser.Playlists.Any() || !currentUser.Friends.Any()) { Console.WriteLine("Je hebt geen speellijsten of vrienden om te vergelijken."); PressAnyKeyToContinue(); return; }
            Console.WriteLine("\n--- Speellijsten Vergelijken ---");

            Console.WriteLine("Kies jouw speellijst:");
            for (int i = 0; i < currentUser.Playlists.Count; i++) Console.WriteLine($"{i + 1}. {currentUser.Playlists[i].Title}");
            int myPlaylistIndex = GetIntegerInput("Voer het nummer in: ");
            if (myPlaylistIndex <= 0 || myPlaylistIndex > currentUser.Playlists.Count) { Console.WriteLine("Ongeldige keuze voor jouw speellijst."); PressAnyKeyToContinue(); return; }
            Playlist mySelectedPlaylist = currentUser.Playlists[myPlaylistIndex - 1];

            Console.WriteLine("\nKies een vriend wiens speellijst je wilt vergelijken:");
            for (int i = 0; i < currentUser.Friends.Count; i++) Console.WriteLine($"{i + 1}. {currentUser.Friends[i].Name}");
            int friendIndex = GetIntegerInput("Voer het nummer in: ");
            if (friendIndex <= 0 || friendIndex > currentUser.Friends.Count) { Console.WriteLine("Ongeldige keuze voor vriend."); PressAnyKeyToContinue(); return; }
            User selectedFriend = currentUser.Friends[friendIndex - 1];

            if (!selectedFriend.Playlists.Any()) { Console.WriteLine($"{selectedFriend.Name} heeft geen speellijsten om te vergelijken."); PressAnyKeyToContinue(); return; }
            Console.WriteLine($"\nKies een speellijst van {selectedFriend.Name}:");
            for (int i = 0; i < selectedFriend.Playlists.Count; i++) Console.WriteLine($"{i + 1}. {selectedFriend.Playlists[i].Title}");
            int friendPlaylistIndex = GetIntegerInput("Voer het nummer in: ");
            if (friendPlaylistIndex <= 0 || friendPlaylistIndex > selectedFriend.Playlists.Count) { Console.WriteLine("Ongeldige keuze voor vriend's speellijst."); PressAnyKeyToContinue(); return; }
            Playlist friendSelectedPlaylist = selectedFriend.Playlists[friendPlaylistIndex - 1];

            currentUser.ComparePlaylistWithFriendPlaylist(mySelectedPlaylist, selectedFriend, friendSelectedPlaylist);
            PressAnyKeyToContinue();
        }

        static void PlaybackControlMenu()
        {
            if (currentUser == null) return;
            while (true)
            {
                Console.WriteLine("\n--- Afspeelcontrole ---");
                Song? currentlyPlaying = currentUser.GetCurrentlyPlayingSong();
                if (currentlyPlaying == null) { Console.WriteLine("Er wordt momenteel niets afgespeeld."); PressAnyKeyToContinue(); return; }

                Console.WriteLine($"Speelt nu af: {currentlyPlaying.Title} - {currentlyPlaying.Artist}");
                Console.WriteLine("1. Pauze/Hervat\n2. Stop afspelen\n3. Volgend nummer\n4. Vorig nummer\n5. Herhaal huidig nummer\n6. Terug naar hoofdmenu");
                Console.Write("Maak een keuze: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        if (currentlyPlaying.IsPlaying && !currentlyPlaying.IsPaused) currentUser.PauseCurrentPlayback();
                        // De aanpassing is hier, nu IsPaused een public setter heeft, kan Program.cs dit direct instellen
                        else if (currentlyPlaying.IsPaused) { currentlyPlaying.IsPaused = false; Console.WriteLine($"Afspelen van '{currentlyPlaying.Title}' hervat."); }
                        break;
                    case "2": currentUser.StopPlayback(); break;
                    case "3": currentUser.NextSong(); break;
                    case "4": currentUser.PreviousSong(); break;
                    case "5": currentUser.RepeatCurrentSong(); break;
                    case "6": return;
                    default: Console.WriteLine("Ongeldige keuze."); break;
                }
                PressAnyKeyToContinue();
            }
        }

        private static int GetIntegerInput(string prompt)
        {
            int result;
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (int.TryParse(input, out result)) return result;
                else Console.WriteLine("Ongeldige invoer. Voer alstublieft een geheel getal in.");
            }
        }

        private static void PressAnyKeyToContinue()
        {
            Console.WriteLine("Druk op een toets om verder te gaan...");
            Console.ReadKey();
        }
    }
}
