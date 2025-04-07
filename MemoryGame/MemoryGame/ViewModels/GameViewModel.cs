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
    public class GameViewModel : BaseViewModel
    {
        // Utilizatorul curent (setat după login)
        public User CurrentPlayer { get; set; }

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
        private readonly GameLogicService gameLogicService;

        public GameViewModel()
        {
            // Inițializează lista de categorii
            Categories = new ObservableCollection<string>
            {
                "Category 1",
                "Category 2",
                "Category 3"
            };
            SelectedCategory = Categories.First();

            IsStandardSelected = true;
            CustomRows = 4;
            CustomColumns = 4;

            gameService = new GameService();
            gameLogicService = new GameLogicService();

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

            if ((rows * columns) % 2 != 0)
            {
                MessageBox.Show("The total number of tiles must be even. Please adjust the dimensions.",
                    "Invalid Dimensions", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Creează un nou joc folosind logica din serviciu
            Game newGame = gameLogicService.CreateNewGame(SelectedCategory, rows, columns, CurrentPlayer, 120);
            // Salvare inițială, dacă e nevoie
            gameService.SaveGame(newGame);

            MessageBox.Show($"New Game started!\nCategory: {newGame.Category}\nDimensions: {newGame.Rows}x{newGame.Columns}",
                "New Game", MessageBoxButton.OK, MessageBoxImage.Information);

            // Deschide fereastra de joc
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
            var gameView = Application.Current.Windows.OfType<Views.GameView>().FirstOrDefault();
            if (gameView != null)
            {
                gameView.Hide();
            }
            var signInView = new Views.SignInView();
            signInView.Show();
        }
    }
}
