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
               
                Console.SetCursorPosition(12, Console.CursorTop); 
                Console.Write($"{i + 1}/{song.DurationInSeconds} seconden ");

                
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true); 

                    switch (keyInfo.KeyChar.ToString().ToUpper())
                    {
                        case "P":
                            isPaused = !isPaused;
                            if (isPaused)
                            {
                                Console.WriteLine("\nAfspelen gepauzeerd. Druk 'P' om te hervatten, 'S' om te stoppen.");
                                
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
                                                
                                                Console.SetCursorPosition(0, Console.CursorTop);
                                                Console.Write($"Afgespeeld: {i + 1}/{song.DurationInSeconds} seconden ");
                                                break;
                                            case "S":
                                                Console.WriteLine("\nAfspelen gestopt vanuit pauze.");
                                                return PlaybackCommand.Stop;
                                            default:
                                                
                                                break;
                                        }
                                    }
                                    Thread.Sleep(100); 
                                }
                            }
                            break;
                        case "S":