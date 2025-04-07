using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using MemoryGame.Helpers;
using MemoryGame.Models;
using MemoryGame.Services;
using System.Linq;

namespace MemoryGame.ViewModels
{
    public class GameBoardViewModel : BaseViewModel
    {
        private readonly GameLogicService gameLogicService;
        private readonly Game currentGame;
        private readonly DispatcherTimer gameTimer;
        private TimeSpan timeRemaining;

        // Pentru identificarea tile-urilor selectate (indicele în colecție)
        private int? firstSelectedIndex;
        private int? secondSelectedIndex;

        public ObservableCollection<TileViewModel> Tiles { get; set; }
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public ICommand SaveGameCommand { get; }
        public ICommand FlipTileCommand { get; }

        public string TimerText => timeRemaining.ToString(@"mm\:ss");
        
        public Action CloseAction { get; set; }

        public GameBoardViewModel(Game game)
        {
            currentGame = game;
            Rows = game.Rows;
            Columns = game.Columns;

            // Inițializăm serviciul de logică
            gameLogicService = new GameLogicService();

            // Construim colecția de TileViewModel – acestea sunt „simple” și nu conțin logică proprie.
            Tiles = new ObservableCollection<TileViewModel>(
                currentGame.Tiles.Select((tile, index) => new TileViewModel(tile, index))
            );

            // Comanda de flip va primi ca parametru indicele tile-ului
            FlipTileCommand = new RelayCommand(param =>
            {
                if (int.TryParse(param.ToString(), out int tileIndex))
                {
                    gameLogicService.FlipTile(currentGame, tileIndex, ref firstSelectedIndex, ref secondSelectedIndex);
                    // După ce s-a efectuat flip-ul, actualizăm toate tile-urile
                    UpdateTiles();

                    // Verifică dacă jocul este câștigat
                    if (gameLogicService.IsGameWon(currentGame))
                    {
                        gameTimer.Stop();
                        MessageBox.Show("Felicitări! Ai câștigat jocul!", "Game Won", MessageBoxButton.OK, MessageBoxImage.Information);
                        CloseAction?.Invoke();
                    }
                }
            }, param => true);

            SaveGameCommand = new RelayCommand(_ => SaveGame());

            // Setăm timerul. Pentru UI, păstrăm logica de actualizare a timer-ului aici.
            timeRemaining = TimeSpan.FromSeconds(currentGame.TimeRemainingSeconds);
            gameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            gameTimer.Tick += (s, e) =>
            {
                if (timeRemaining.TotalSeconds > 0)
                {
                    // Opțional, putem delega scăderea timpului unui TimerService
                    timeRemaining = timeRemaining.Subtract(TimeSpan.FromSeconds(1));
                    currentGame.TimeRemainingSeconds = (int)timeRemaining.TotalSeconds;
                    OnPropertyChanged(nameof(TimerText));
                }
                else
                {
                    gameTimer.Stop();
                    // Dacă jocul nu este câștigat, atunci timpul expirat înseamnă că jocul este pierdut
                    if (!gameLogicService.IsGameWon(currentGame))
                    {
                        MessageBox.Show("Timpul a expirat! Jocul este pierdut.", "Game Over", MessageBoxButton.OK, MessageBoxImage.Warning);
                        CloseAction?.Invoke();
                    }
                }
            };
            gameTimer.Start();
        }

        private void SaveGame()
        {
            // Apelăm serviciul de salvare
            var gameService = new GameService();
            gameService.SaveGame(currentGame);
            MessageBox.Show("Game saved successfully.", "Save Game", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Actualizează starea fiecărui TileViewModel pe baza modelului
        /// </summary>
        private void UpdateTiles()
        {
            // Se actualizează fiecare TileViewModel; de ex., se notifică schimbările pentru ImagePath
            foreach (var tileVM in Tiles)
            {
                tileVM.Update();
            }
        }
    }
}
