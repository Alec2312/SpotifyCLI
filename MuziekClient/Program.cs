// Bestand: Program.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace MuziekClient.Classes
{
    class Program
    {
        // Deze lijsten fungeren nu als de centrale opslag, in plaats van MusicLibrary
        public static List<Song> AllSongs { get; private set; } = new List<Song>();
        public static List<Album> AllAlbums { get; private set; } = new List<Album>();
        public static List<User> RegisteredUsers { get; private set; } = new List<User>();

        private static User? currentUser = null;

        static void Main(string[] args)
        {
            InitializeData();
            Console.WriteLine("Welkom bij de MuziekClient!");
            while (true)
            {
                if (currentUser == null)
                {
                    ShowLoginMenu();
                }
                else
                {
                    // Delegeer de hoofdmenu-loop naar de actieve gebruiker
                    if (currentUser is SuperUser adminUser)
                    {
                        // Geeft geen musicLibrary object meer mee
                        adminUser.RunSuperUserMainMenu(ref currentUser);
                    }
                    else
                    {
                        // Geeft geen musicLibrary object meer mee
                        currentUser.RunUserMainMenu(ref currentUser);
                    }
                }
            }
        }

        static void InitializeData()
        {
            // Nummers
            var bohemianRhapsody = new Song("Bohemian Rhapsody", "Queen", 355, Genre.Rock);
            var thriller = new Song("Thriller", "Michael Jackson", 357, Genre.Pop);
            var hotelCalifornia = new Song("Hotel California", "Eagles", 390, Genre.Rock);
            var billieJean = new Song("Billie Jean", "Michael Jackson", 294, Genre.Pop);
            var fadeToBlack = new Song("Fade to Black", "Metallica", 412, Genre.Metal);
            var smoothCriminal = new Song("Smooth Criminal", "Michael Jackson", 258, Genre.Pop);
            var summerBreeze = new Song("Summer Breeze", "Seals and Crofts", 305, Genre.Country);

            // Nummers direct toevoegen aan Program.AllSongs
            AllSongs.Add(bohemianRhapsody);
            AllSongs.Add(thriller);
            AllSongs.Add(hotelCalifornia);
            AllSongs.Add(billieJean);
            AllSongs.Add(fadeToBlack);
            AllSongs.Add(smoothCriminal);
            AllSongs.Add(summerBreeze);

            // Albums
            var queenGreatestHits = new Album("Queen's Greatest Hits", "Queen");
            queenGreatestHits.AddSong(bohemianRhapsody);
            // Albums direct toevoegen aan Program.AllAlbums
            AllAlbums.Add(queenGreatestHits);

            var thrillerAlbum = new Album("Thriller", "Michael Jackson");
            thrillerAlbum.AddSong(thriller);
            thrillerAlbum.AddSong(billieJean);
            thrillerAlbum.AddSong(smoothCriminal);
            AllAlbums.Add(thrillerAlbum);

            var masterOfPuppets = new Album("Master of Puppets", "Metallica");
            masterOfPuppets.AddSong(fadeToBlack);
            AllAlbums.Add(masterOfPuppets);

            // Bestaande Gebruikers (hardcoded voor demo)
            var alice = new User("Alice");
            var bob = new User("Bob");
            var charlie = new User("Charlie");
            var david = new User("David");
            var eve = new User("Eve");
            var admin = new SuperUser("Admin");

            // Gebruikers direct registreren in Program.RegisteredUsers
            RegisteredUsers.Add(alice);
            RegisteredUsers.Add(bob);
            RegisteredUsers.Add(charlie);
            RegisteredUsers.Add(david);
            RegisteredUsers.Add(eve);
            RegisteredUsers.Add(admin);

            // Alice's speellijsten
            var aliceRockPlaylist = alice.CreatePlaylist("Alice's Rock Favorieten");
            aliceRockPlaylist.AddSong(bohemianRhapsody);
            aliceRockPlaylist.AddSong(hotelCalifornia);
            aliceRockPlaylist.AddSong(fadeToBlack);

            var alicePopPlaylist = alice.CreatePlaylist("Alice's Pop Hits");
            alicePopPlaylist.AddSong(thriller);
            alicePopPlaylist.AddSong(billieJean);
            alicePopPlaylist.AddAlbum(thrillerAlbum);

            // Bob's speellijsten (hard-coded voor demo)
            var bobPartyPlaylist = bob.CreatePlaylist("Bob's Party Mix");
            bobPartyPlaylist.AddSong(thriller);
            bobPartyPlaylist.AddSong(smoothCriminal);
            bobPartyPlaylist.AddSong(bohemianRhapsody);

            // Charlie's speellijsten (hard-coded voor demo)
            var charlieChillPlaylist = charlie.CreatePlaylist("Charlie's Chill Vibes");
            charlieChillPlaylist.AddSong(hotelCalifornia);
            charlieChillPlaylist.AddSong(summerBreeze);
        }

        static void ShowLoginMenu()
        {
            Console.WriteLine("\n--- Login ---\n1. Inloggen\n2. Registreren\n3. Afsluiten");
            Console.Write("Maak een keuze: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1": LoginUser(); break;
                case "2": RegisterNewUser(); break;
                case "3": Console.WriteLine("Bedankt voor het gebruiken van MuziekClient!"); Environment.Exit(0); break;
                default: Console.WriteLine("Ongeldige keuze. Probeer opnieuw."); break;
            }
        }

        static void LoginUser()
        {
            Console.Write("Voer gebruikersnaam in: ");
            string? username = Console.ReadLine();
            // Gebruikt nu Program.RegisteredUsers
            currentUser = RegisteredUsers.FirstOrDefault(u => u.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (currentUser == null) Console.WriteLine("Gebruiker niet gevonden. Probeer opnieuw of registreer een nieuw account.");
            else Console.WriteLine($"Welkom, {currentUser.Name}!");
        }

        static void RegisterNewUser()
        {
            Console.Write("Voer een nieuwe gebruikersnaam in: ");
            string? newUsername = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newUsername)) { Console.WriteLine("Gebruikersnaam mag niet leeg zijn."); return; }
            // Gebruikt nu Program.RegisteredUsers
            if (RegisteredUsers.Any(u => u.Name.Equals(newUsername, StringComparison.OrdinalIgnoreCase)))
            { Console.WriteLine($"Gebruikersnaam '{newUsername}' bestaat al. Kies een andere naam."); return; }

            User newUser = new User(newUsername);
            // Voegt toe aan Program.RegisteredUsers
            RegisteredUsers.Add(newUser);
            Console.WriteLine($"Gebruiker '{newUsername}' succesvol geregistreerd! U kunt nu inloggen.");
        }

        public static int GetIntegerInput(string prompt)
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
    }
}