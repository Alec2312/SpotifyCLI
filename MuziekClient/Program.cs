// Bestand: Program.cs
using System;
using System.Collections.Generic;
using System.Linq;

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
                if (currentUser == null)
                {
                    ShowLoginMenu();
                }
                else
                {
                    // Delegeer de hoofdmenu-loop naar de actieve gebruiker
                    if (currentUser is SuperUser adminUser)
                    {
                        adminUser.RunSuperUserMainMenu(musicLibrary, ref currentUser); // Geef MusicLibrary mee
                    }
                    else
                    {
                        currentUser.RunUserMainMenu(musicLibrary, ref currentUser); // Geef MusicLibrary mee
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

            musicLibrary.AddSong(bohemianRhapsody);
            musicLibrary.AddSong(thriller);
            musicLibrary.AddSong(hotelCalifornia);
            musicLibrary.AddSong(billieJean);
            musicLibrary.AddSong(fadeToBlack);
            musicLibrary.AddSong(smoothCriminal);
            musicLibrary.AddSong(summerBreeze);

            // Albums
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

            // Bestaande Gebruikers (hardcoded voor demo)
            var alice = new User("Alice");
            var bob = new User("Bob");
            var charlie = new User("Charlie");
            var david = new User("David");
            var eve = new User("Eve");
            var admin = new SuperUser("Admin"); // SuperUser account

            musicLibrary.RegisterUser(alice);
            musicLibrary.RegisterUser(bob);
            musicLibrary.RegisterUser(charlie);
            musicLibrary.RegisterUser(david);
            musicLibrary.RegisterUser(eve);
            musicLibrary.RegisterUser(admin); // Registreer de SuperUser

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

            // Vrienden toevoegen functionaliteit is verwijderd.
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
            currentUser = musicLibrary.RegisteredUsers.FirstOrDefault(u => u.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (currentUser == null) Console.WriteLine("Gebruiker niet gevonden. Probeer opnieuw of registreer een nieuw account.");
            else Console.WriteLine($"Welkom, {currentUser.Name}!");
        }

        static void RegisterNewUser()
        {
            Console.Write("Voer een nieuwe gebruikersnaam in: ");
            string? newUsername = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newUsername)) { Console.WriteLine("Gebruikersnaam mag niet leeg zijn."); return; }
            if (musicLibrary.RegisteredUsers.Any(u => u.Name.Equals(newUsername, StringComparison.OrdinalIgnoreCase)))
            { Console.WriteLine($"Gebruikersnaam '{newUsername}' bestaat al. Kies een andere naam."); return; }

            User newUser = new User(newUsername);
            musicLibrary.RegisterUser(newUser);
            Console.WriteLine($"Gebruiker '{newUsername}' succesvol geregistreerd! U kunt nu inloggen.");
        }

        // Hulpmethoden voor algemene console-invoer
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