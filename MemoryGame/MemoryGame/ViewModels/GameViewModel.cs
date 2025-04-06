using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MemoryGame.Helpers;
using MemoryGame.Models;
using MemoryGame.Services;

namespace MemoryGame.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        // Utilizatorul curent (asociază-l după login, dacă ai un mecanism de autentificare)
        public User CurrentPlayer { get; set; }

        // Lista de categorii și categoria selectată
        public ObservableCollection<string> Categories { get; set; }
        private string selectedCategory;
        public string SelectedCategory
        {
            get => selectedCategory;
            set
            {
                if (selectedCategory != value)
                {
                    selectedCategory = value;
                    OnPropertyChanged();
                }
            }
        }

        // Proprietăți pentru modul de joc: standard vs. custom
        private bool isStandardSelected;
        public bool IsStandardSelected
        {
            get => isStandardSelected;
            set
            {
                if (isStandardSelected != value)
                {
                    isStandardSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private int customRows;
        public int CustomRows
        {
            get => customRows;
            set
            {
                if (customRows != value)
                {
                    customRows = value;
                    OnPropertyChanged();
                }
            }
        }

        private int customColumns;
        public int CustomColumns
        {
            get => customColumns;
            set
            {
                if (customColumns != value)
                {
                    customColumns = value;
                    OnPropertyChanged();
                }
            }
        }

        // Comenzile meniului
        public ICommand NewGameCommand { get; }
        public ICommand OpenGameCommand { get; }
        public ICommand StatisticsCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand ExitCommand { get; }

        private readonly GameService gameService;

        public GameViewModel()
        {
            // Inițializează lista de categorii
            Categories = new ObservableCollection<string>
            {
                "Category 1",
                "Category 2",
                "Category 3"
            };
            SelectedCategory = Categories[0];

            // Modul standard este selectat implicit
            IsStandardSelected = true;

            // Valorile custom implicite
            CustomRows = 4;
            CustomColumns = 4;

            gameService = new GameService();

            NewGameCommand = new RelayCommand(_ => NewGame());
            OpenGameCommand = new RelayCommand(_ => OpenGame());
            StatisticsCommand = new RelayCommand(_ => ShowStatistics());
            AboutCommand = new RelayCommand(_ => ShowAbout());
            ExitCommand = new RelayCommand(_ => Exit());
        }

        private void NewGame()
        {
            int rows = IsStandardSelected ? 4 : CustomRows;
            int columns = IsStandardSelected ? 4 : CustomColumns;

            // Verifică dacă numărul total de tile-uri este par
            if ((rows * columns) % 2 != 0)
            {
                MessageBox.Show("The total number of tiles must be even. Please adjust the dimensions.",
                    "Invalid Dimensions", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Inițializează un nou joc și asociază utilizatorul logat
            Game newGame = new Game
            {
                Category = SelectedCategory,
                Rows = rows,
                Columns = columns,
                Player = CurrentPlayer,
                TimeRemainingSeconds = 120 // Exemplu: 2 minute
            };

            // Generează tile-urile pentru joc
            newGame.Tiles = GenerateTiles(SelectedCategory, rows, columns);

            // Salvează jocul
            gameService.SaveGame(newGame);

            MessageBox.Show($"New Game started!\nCategory: {newGame.Category}\nDimensions: {newGame.Rows}x{newGame.Columns}",
                "New Game", MessageBoxButton.OK, MessageBoxImage.Information);

            // Deschide fereastra de joc și transmite jocul curent
            var gameBoardView = new Views.GameBoardView(newGame);
            gameBoardView.Show();
        }

        private void OpenGame()
        {
            Game loadedGame = gameService.LoadGame();
            if (loadedGame != null)
            {
                MessageBox.Show("Game loaded successfully.", "Open Game", MessageBoxButton.OK, MessageBoxImage.Information);
                var gameBoardView = new Views.GameBoardView(loadedGame);
                gameBoardView.Show();
            }
            else
            {
                MessageBox.Show("No saved game found.", "Open Game", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ShowStatistics()
        {
            MessageBox.Show("Statistics functionality not yet implemented.", "Statistics",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowAbout()
        {
            MessageBox.Show("Student: Numele Studentului\nEmail: student@example.com\nGrupa: XYZ\nSpecializare: Informatică",
                "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        // Metodă pentru generarea tile-urilor cu imagini în perechi și ordonare aleatorie
        private List<Tile> GenerateTiles(string category, int rows, int columns)
        {
            // Mapează numele categoriei la numele folderului
            // "Category 1" -> "Category1", "Category 2" -> "Category2" etc.
            string folderName = "Category1";
            if (category == "Category 1")
                folderName = "Category1";
            else if (category == "Category 2")
                folderName = "Category2";
            else if (category == "Category 3")
                folderName = "Category3";

            // Construiește calea completă către folder
            string categoryFolderPath = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources", "Data", "Images", folderName);

            // Obține toate fișierele .jpg din folder
            // (dacă ai și alte extensii, ex. .png, .jpeg, etc., ajustează filtrul)
            var imageFiles = Directory.GetFiles(categoryFolderPath, "*.jpg");

            int totalTiles = rows * columns;
            int pairsNeeded = totalTiles / 2;

            // Dacă nu ai suficiente imagini, ar trebui fie să afișezi un mesaj,
            // fie să refolosești imaginile. Aici, ca exemplu, ne asigurăm că
            // nu selectăm mai multe imagini decât avem disponibile.
            if (imageFiles.Length < pairsNeeded)
            {
                MessageBox.Show("Not enough images in the selected category folder!",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Returnează o listă goală sau aruncă excepție
                return new List<Tile>();
            }

            // Selectează aleator perechile necesare
            Random random = new Random();
            var selectedImages = imageFiles
                .OrderBy(x => random.Next())
                .Take(pairsNeeded)
                .ToList();

            List<Tile> tiles = new List<Tile>();
            foreach (var imgPath in selectedImages)
            {
                // Adaugă fiecare imagine de două ori (pentru perechi)
                tiles.Add(new Tile { ImagePath = imgPath, IsFaceUp = false, IsMatched = false });
                tiles.Add(new Tile { ImagePath = imgPath, IsFaceUp = false, IsMatched = false });
            }

            // Amestecă tile-urile înainte de a le returna
            return tiles.OrderBy(x => random.Next()).ToList();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
