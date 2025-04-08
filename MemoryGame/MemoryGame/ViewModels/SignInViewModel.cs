using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MemoryGame.Models;
using MemoryGame.Services;
using MemoryGame.Helpers;
using System.Windows;

namespace MemoryGame.ViewModels
{
    public class SignInViewModel : BaseViewModel
    {
        private User selectedUser;
        private readonly UserService userService;

        public ObservableCollection<User> Users { get; set; }
        public User SelectedUser
        {
            get => selectedUser;
            set
            {
                if (selectedUser != value)
                {
                    selectedUser = value;
                    OnPropertyChanged();
                    (DeleteUserCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (PlayCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (NextImageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (PreviousImageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand NewUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand NextImageCommand { get; }
        public ICommand PreviousImageCommand { get; }

        public Action CloseAction { get; set; }

        public SignInViewModel()
        {
            userService = new UserService();
            var users = userService.LoadUsers();
            Users = new ObservableCollection<User>(users);

            NewUserCommand = new RelayCommand(NewUser);
            DeleteUserCommand = new RelayCommand(DeleteUser, CanModifyUser);
            PlayCommand = new RelayCommand(Play, CanModifyUser);
            NextImageCommand = new RelayCommand(NextImage, CanChangeImage);
            PreviousImageCommand = new RelayCommand(PreviousImage, CanChangeImage);
        }

        private void NewUser(object parameter)
        {
            var newUserWindow = new Views.NewUserWindow();
            if (newUserWindow.ShowDialog() == true)
            {
                string userName = newUserWindow.UserName;
                string absoluteImagePath = newUserWindow.ImagePath;
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string relativeImagePath = PathHelper.GetRelativePath(baseDir, absoluteImagePath);

                var newUser = new User
                {
                    Name = userName,
                    ImagePath = relativeImagePath
                };

                Users.Add(newUser);
                userService.SaveUsers(Users.ToList());
            }
        }

        private bool CanModifyUser(object parameter)
        {
            return SelectedUser != null;
        }

        private void DeleteUser(object parameter)
        {
            if (SelectedUser != null)
            {
                string userName = SelectedUser.Name;

                Users.Remove(SelectedUser);
                userService.SaveUsers(Users.ToList());

                var gameService = new GameService();
                gameService.DeleteGame(userName);

                var statisticsService = new StatisticsService();
                statisticsService.DeleteStatistics(userName);

                SelectedUser = null;
            }
        }

        private void Play(object parameter)
        {
            if (SelectedUser == null)
                return;

            var gameView = new Views.GameView(SelectedUser);
            gameView.Show();
            CloseAction?.Invoke();
        }

        private bool CanChangeImage(object parameter)
        {
            return SelectedUser != null;
        }

        private void NextImage(object parameter)
        {
            if (SelectedUser == null)
                return;

            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Data");
            if (!Directory.Exists(imagesFolder))
                return;

            var files = Directory.GetFiles(imagesFolder, "*.*")
                        .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                    f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                    f.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                        .OrderBy(f => f)
                        .ToList();

            if (files.Count == 0)
                return;

            string currentAbsolute = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SelectedUser.ImagePath);
            int currentIndex = files.FindIndex(f => f.Equals(currentAbsolute, StringComparison.OrdinalIgnoreCase));
            if (currentIndex < 0)
                currentIndex = 0;

            int nextIndex = (currentIndex + 1) % files.Count;
            string nextAbsolute = files[nextIndex];
            string relativeNext = PathHelper.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, nextAbsolute);
            SelectedUser.ImagePath = relativeNext;

            userService.SaveUsers(Users.ToList());
        }

        private void PreviousImage(object parameter)
        {
            if (SelectedUser == null)
                return;

            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Data");
            if (!Directory.Exists(imagesFolder))
                return;

            var files = Directory.GetFiles(imagesFolder, "*.*")
                        .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                    f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                    f.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                        .OrderBy(f => f)
                        .ToList();

            if (files.Count == 0)
                return;

            string currentAbsolute = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SelectedUser.ImagePath);
            int currentIndex = files.FindIndex(f => f.Equals(currentAbsolute, StringComparison.OrdinalIgnoreCase));
            if (currentIndex < 0)
                currentIndex = 0;

            int previousIndex = (currentIndex - 1 + files.Count) % files.Count;
            string previousAbsolute = files[previousIndex];
            string relativePrevious = PathHelper.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, previousAbsolute);
            SelectedUser.ImagePath = relativePrevious;

            userService.SaveUsers(Users.ToList());
        }
    }
}

