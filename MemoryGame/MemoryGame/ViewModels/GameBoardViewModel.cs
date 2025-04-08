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

        private int? firstSelectedIndex;
        private int? secondSelectedIndex;

        private bool gameEnded = false;

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

            gameLogicService = new GameLogicService();

            Tiles = new ObservableCollection<TileViewModel>(
                currentGame.Tiles.Select((tile, index) => new TileViewModel(tile, index))
            );

            FlipTileCommand = new RelayCommand(param =>
            {
                if (int.TryParse(param.ToString(), out int tileIndex))
                {
                    gameLogicService.FlipTile(currentGame, tileIndex, ref firstSelectedIndex, ref secondSelectedIndex);
                    UpdateTiles();

                    if (gameLogicService.IsGameWon(currentGame) && !gameEnded)
                    {
                        gameEnded = true;
                        var statService = new StatisticsService();
                        statService.UpdateStatistics(currentGame.Player.Name, true);

                        gameTimer.Stop();
                        MessageBox.Show("Felicitări! Ai câștigat jocul!", "Game Won", MessageBoxButton.OK, MessageBoxImage.Information);
                        CloseAction?.Invoke();
                    }
                }
            }, param => true);

            SaveGameCommand = new RelayCommand(_ => SaveGame());

            timeRemaining = TimeSpan.FromSeconds(currentGame.TimeRemainingSeconds);
            gameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            gameTimer.Tick += (s, e) =>
            {
                if (timeRemaining.TotalSeconds > 0)
                {
                    timeRemaining = timeRemaining.Subtract(TimeSpan.FromSeconds(1));
                    currentGame.TimeRemainingSeconds = (int)timeRemaining.TotalSeconds;
                    OnPropertyChanged(nameof(TimerText));
                }
                else if (!gameEnded)
                {
                    gameEnded = true;
                    var statService = new StatisticsService();
                    statService.UpdateStatistics(currentGame.Player.Name, false);

                    gameTimer.Stop();
                    MessageBox.Show("Timpul a expirat! Jocul este pierdut.", "Game Over", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CloseAction?.Invoke();

                }
            };
            gameTimer.Start();
        }

        private void SaveGame()
        {
            var gameService = new GameService();
            gameService.SaveGame(currentGame);
            MessageBox.Show("Game saved successfully.", "Save Game", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateTiles()
        {
            foreach (var tileVM in Tiles)
            {
                tileVM.Update();
            }
        }
    }
}
