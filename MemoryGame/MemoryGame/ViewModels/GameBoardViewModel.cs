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

namespace MemoryGame.ViewModels
{
    public class GameBoardViewModel : INotifyPropertyChanged
    {
        private Game currentGame;
        public ObservableCollection<TileViewModel> Tiles { get; set; }
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public ICommand SaveGameCommand { get; }

        private DispatcherTimer gameTimer;
        private TimeSpan timeRemaining;

        // Afișează timpul rămas în format mm:ss
        public string TimerText => timeRemaining.ToString(@"mm\:ss");

        // Variabile pentru gestionarea selecției a două tile-uri
        private TileViewModel firstSelectedTile;
        private TileViewModel secondSelectedTile;

        public GameBoardViewModel(Game game)
        {
            currentGame = game;
            Rows = game.Rows;
            Columns = game.Columns;
            Tiles = new ObservableCollection<TileViewModel>();

            // Creează view model-uri pentru fiecare tile și le ascultă evenimentul de flip
            foreach (var tile in currentGame.Tiles)
            {
                var tileVM = new TileViewModel(tile);
                tileVM.TileFlipped += OnTileFlipped;
                Tiles.Add(tileVM);
            }

            SaveGameCommand = new RelayCommand(_ => SaveGame());

            // Inițializează timer-ul cu timpul rămas din starea jocului
            timeRemaining = TimeSpan.FromSeconds(currentGame.TimeRemainingSeconds);
            StartGameTimer();
        }

        private void StartGameTimer()
        {
            gameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            gameTimer.Tick += (s, e) =>
            {
                if (timeRemaining.TotalSeconds > 0)
                {
                    timeRemaining = timeRemaining.Subtract(TimeSpan.FromSeconds(1));
                    OnPropertyChanged(nameof(TimerText));
                    // Actualizează starea jocului
                    currentGame.TimeRemainingSeconds = (int)timeRemaining.TotalSeconds;
                }
                else
                {
                    gameTimer.Stop();
                    MessageBox.Show("Timpul a expirat! Jocul este pierdut.", "Time Over", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // Opțional: dezactivează interacțiunea cu tile-urile
                }
            };
            gameTimer.Start();
        }

        // Evenimentul declanșat atunci când un tile este întors
        private async void OnTileFlipped(object sender, EventArgs e)
        {
            var tileVM = sender as TileViewModel;
            if (tileVM == null)
                return;

            // Previne selectarea aceluiași tile de două ori
            if (firstSelectedTile == tileVM || secondSelectedTile == tileVM)
                return;

            if (firstSelectedTile == null)
            {
                firstSelectedTile = tileVM;
            }
            else if (secondSelectedTile == null)
            {
                secondSelectedTile = tileVM;
                await CompareTiles();
            }
        }

        private async Task CompareTiles()
        {
            // Compară imaginile celor două tile-uri întoarse
            if (firstSelectedTile.Tile.ImagePath == secondSelectedTile.Tile.ImagePath)
            {
                firstSelectedTile.SetMatched();
                secondSelectedTile.SetMatched();
            }
            else
            {
                // Așteaptă 1 secundă pentru ca utilizatorul să vadă tile-urile
                await Task.Delay(1000);
                firstSelectedTile.ForceFlipDown();
                secondSelectedTile.ForceFlipDown();
            }
            // Resetează selecția
            firstSelectedTile.ResetFlip();
            secondSelectedTile.ResetFlip();
            firstSelectedTile = null;
            secondSelectedTile = null;
        }

        private void SaveGame()
        {
            // Salvează starea curentă a jocului
            GameService gameService = new GameService();
            gameService.SaveGame(currentGame);
            MessageBox.Show("Game saved successfully.", "Save Game", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
