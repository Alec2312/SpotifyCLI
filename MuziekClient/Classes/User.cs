// Bestand: User.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading; // Nodig voor CancellationTokenSource, Task

namespace MuziekClient.Classes
{
    public class User
    {
        public string Name { get; private set; }
        public List<Playlist> Playlists { get; private set; }
        public List<User> Friends { get; private set; }

        // --- Afspeelbeheer velden ---
        private Song? _currentSongPlaying; // Het daadwerkelijk afspelende Song object
        private List<Song>? _currentPlaybackQueue; // De lijst van nummers in de huidige afspeelwachtrij (playlist/album)
        private int _currentPlaybackIndex = -1; // Index van het huidige nummer in de _currentPlaybackQueue
        private CancellationTokenSource? _playbackCancellationTokenSource; // Voor het annuleren van afspeeltaken
        private Task? _playbackTask; // De taak die het afspelen afhandelt

        public Song? GetCurrentlyPlayingSong() => _currentSongPlaying;
        public SongCollection? GetCurrentlyPlayingCollection()
        {
            // Dit is lastiger omdat _currentPlaybackQueue een List<Song> is, niet de originele SongCollection.
            // Voor deze simulatie volstaat het om te weten welk nummer speelt, niet per se uit welke collectie.
            // Als je dit strikt wilt bijhouden, moet je ook de originele SongCollection opslaan.
            return null; // Of voeg een veld toe zoals _currentlyPlayingSourceCollection
        }


        public User(string name)
        {
            Name = name;
            Playlists = new List<Playlist>();
            Friends = new List<User>();
        }

        public Playlist CreatePlaylist(string title)
        {
            Playlist newPlaylist = new Playlist(title);
            Playlists.Add(newPlaylist);
            Console.WriteLine($"{Name} heeft speellijst '{title}' aangemaakt.");
            return newPlaylist;
        }

        // Stopt elke actieve afspeeltaak
        private void StopCurrentPlaybackTask()
        {
            if (_playbackCancellationTokenSource != null)
            {
                _playbackCancellationTokenSource.Cancel(); // Vraag annulering aan
                _playbackTask?.Wait(100); // Wacht kort tot de taak reageert (of time-out)
                _playbackCancellationTokenSource.Dispose();
                _playbackCancellationTokenSource = null;
            }
            _currentSongPlaying?.Stop(); // Zorg dat het nummer zelf ook stopt
            _currentSongPlaying = null;
            _currentPlaybackQueue = null;
            _currentPlaybackIndex = -1;
        }

        public void PlaySong(Song song)
        {
            StopCurrentPlaybackTask(); // Stop wat er ook speelde

            _playbackCancellationTokenSource = new CancellationTokenSource();
            _currentSongPlaying = song;
            _currentPlaybackQueue = new List<Song> { song }; // Maak een queue van één liedje
            _currentPlaybackIndex = 0;

            // Start een nieuwe taak om het nummer af te spelen
            _playbackTask = Task.Run(() =>
            {
                _currentSongPlaying.Play(_playbackCancellationTokenSource.Token);
                // Als het nummer klaar is, stop dan de taak en reset de status
                if (!_currentSongPlaying.IsPlaying && !_currentSongPlaying.IsPaused)
                {
                    StopCurrentPlaybackTask(); // Nummer is afgespeeld of handmatig gestopt
                }
            }, _playbackCancellationTokenSource.Token);

            Console.WriteLine($"{Name} speelt nu af: {_currentSongPlaying.Title} by {_currentSongPlaying.Artist}");
        }

        public void PlayCollection(SongCollection collection, bool shuffle = false)
        {
            StopCurrentPlaybackTask(); // Stop wat er ook speelde

            _currentPlaybackQueue = collection.GetSongsForPlayback(shuffle);
            if (!_currentPlaybackQueue.Any())
            {
                Console.WriteLine("Kan geen nummers afspelen; de collectie is leeg.");
                return;
            }

            _playbackCancellationTokenSource = new CancellationTokenSource();
            _currentPlaybackIndex = 0;

            // Start een nieuwe taak om de nummers in de queue af te spelen
            _playbackTask = Task.Run(async () => // async nodig voor await
            {
                for (int i = _currentPlaybackIndex; i < _currentPlaybackQueue.Count; i++)
                {
                    _currentPlaybackIndex = i; // Update de index
                    _currentSongPlaying = _currentPlaybackQueue[i];

                    // Geef het token door en wacht op de afspeeltaak
                    // De Play methode van Song zal afhandelen of het gepauzeerd/gestopt wordt
                    _currentSongPlaying.Play(_playbackCancellationTokenSource.Token);

                    // Controleer of de afspeeltaak is geannuleerd (bijv. door StopCurrentPlaybackTask of Next/Previous)
                    if (_playbackCancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break; // Stop de loop
                    }

                    // Wacht niet per se tot het nummer helemaal klaar is als er een 'stop' kwam
                    // De Play-methode van Song handelt zijn eigen voltooiing af
                    // We moeten hier echter controleren of Play() is voltooid of onderbroken
                    // Dit is een simpele simulatie; in een echt systeem zou je een event-handler hebben
                    // die signaleert wanneer een nummer is afgelopen.
                    // Voor nu, na Play() is het nummer in principe 'klaar' of 'onderbroken'.
                    // We gaan alleen door naar de volgende als het nummer niet is gestopt door een externe annulering.
                    if (!_currentSongPlaying.IsPlaying && !_currentSongPlaying.IsPaused && !_playbackCancellationTokenSource.Token.IsCancellationRequested)
                    {
                        // Nummer is 'natuurlijk' afgelopen, ga door naar de volgende in de loop
                    }
                    else if (_playbackCancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break; // De afspeelreeks is onderbroken (door next/prev/stop)
                    }
                    else // Als IsPlaying of IsPaused nog true is, betekent het dat de loop is onderbroken
                    {
                        // Blijf in deze status tot de gebruiker verder gaat of stopt
                        break; // Breek de loop af, wacht op verdere commando's
                    }
                }

                // Na de loop, als alles afgespeeld is of onderbroken
                if (!_playbackCancellationTokenSource.Token.IsCancellationRequested)
                {
                    StopCurrentPlaybackTask(); // Alles is klaar
                    Console.WriteLine($"Alle nummers van '{collection.Title}' zijn afgespeeld.");
                }
            }, _playbackCancellationTokenSource.Token);
            Console.WriteLine($"{Name} start afspelen van '{collection.Title}'.");
        }


        public void PauseCurrentPlayback()
        {
            _currentSongPlaying?.Pause();
        }

        public void StopPlayback()
        {
            StopCurrentPlaybackTask();
            Console.WriteLine($"{Name} heeft het afspelen gestopt.");
        }

        public void RepeatCurrentSong()
        {
            if (_currentSongPlaying != null)
            {
                Console.WriteLine($"{Name} herhaalt: {_currentSongPlaying.Title}");
                // Stop de huidige weergave, en start dezelfde song opnieuw
                StopCurrentPlaybackTask(); // Stop de taak die het afspeelt
                PlaySong(_currentSongPlaying); // Start het nummer opnieuw in een nieuwe taak
            }
            else
            {
                Console.WriteLine($"{Name}: Er wordt geen nummer afgespeeld om te herhalen.");
            }
        }

        public void NextSong()
        {
            if (_currentPlaybackQueue != null && _currentPlaybackQueue.Any())
            {
                StopCurrentPlaybackTask(); // Stop de huidige song en annuleer de taak

                _currentPlaybackIndex++;
                if (_currentPlaybackIndex < _currentPlaybackQueue.Count)
                {
                    Console.WriteLine("Speelt volgend nummer...");
                    PlaySong(_currentPlaybackQueue[_currentPlaybackIndex]);
                }
                else
                {
                    Console.WriteLine("Einde van de afspeellijst. Geen volgend nummer.");
                    StopCurrentPlaybackTask(); // Alles is afgespeeld
                }
            }
            else
            {
                Console.WriteLine("Geen afspeellijst actief om een volgend nummer van af te spelen.");
            }
        }

        public void PreviousSong()
        {
            if (_currentPlaybackQueue != null && _currentPlaybackQueue.Any())
            {
                StopCurrentPlaybackTask(); // Stop de huidige song en annuleer de taak

                _currentPlaybackIndex--;
                if (_currentPlaybackIndex >= 0)
                {
                    Console.WriteLine("Speelt vorig nummer...");
                    PlaySong(_currentPlaybackQueue[_currentPlaybackIndex]);
                }
                else
                {
                    Console.WriteLine("Begin van de afspeellijst. Geen vorig nummer.");
                    // Start eventueel het eerste nummer opnieuw of stop afspelen
                    StopCurrentPlaybackTask();
                }
            }
            else
            {
                Console.WriteLine("Geen afspeellijst actief om een vorig nummer van af te spelen.");
            }
        }


        // Vrienden en vergelijk methoden blijven hetzelfde
        public void AddFriend(User friend)
        {
            if (friend == this)
            {
                Console.WriteLine("Je kunt jezelf niet als vriend toevoegen.");
                return;
            }
            if (!Friends.Contains(friend))
            {
                Friends.Add(friend);
                Console.WriteLine($"{Name} is nu vrienden met {friend.Name}.");
            }
            else
            {
                Console.WriteLine($"{friend.Name} is al een vriend van {Name}.");
            }
        }

        public void ViewFriendPlaylists(User friend)
        {
            if (Friends.Contains(friend))
            {
                Console.WriteLine($"\nSpeellijsten van {friend.Name}:");
                if (friend.Playlists.Any())
                {
                    foreach (var playlist in friend.Playlists)
                    {
                        Console.WriteLine($"- {playlist.Title} ({playlist.Songs.Count} nummers, {playlist.GetTotalDuration()}s)");
                    }
                }
                else
                {
                    Console.WriteLine($"{friend.Name} heeft geen openbare speellijsten.");
                }
            }
            else
            {
                Console.WriteLine($"{friend.Name} is geen vriend van {Name}. Je kunt hun speellijsten niet zien.");
            }
        }

        public void ComparePlaylistWithFriendPlaylist(Playlist myPlaylist, User friend, Playlist friendPlaylist)
        {
            if (!Playlists.Contains(myPlaylist))
            {
                Console.WriteLine($"'{myPlaylist.Title}' is geen speellijst van {Name}.");
                return;
            }
            if (!Friends.Contains(friend) || !friend.Playlists.Contains(friendPlaylist))
            {
                Console.WriteLine($"Kan '{friendPlaylist.Title}' van {friend.Name} niet vergelijken, omdat {friend.Name} geen vriend is of de speellijst niet bestaat.");
                return;
            }

            Console.WriteLine($"\n{Name} vergelijkt '{myPlaylist.Title}' met '{friendPlaylist.Title}' van {friend.Name}:");
            Playlist commonSongs = myPlaylist.CompareWith(friendPlaylist);
            if (commonSongs.Songs.Any())
            {
                Console.WriteLine("Overeenkomende nummers:");
                foreach (var song in commonSongs.Songs)
                {
                    Console.WriteLine($"- {song.Title} by {song.Artist}");
                }
            }
            else
            {
                Console.WriteLine("Geen overeenkomende nummers gevonden.");
            }
        }
    }
}