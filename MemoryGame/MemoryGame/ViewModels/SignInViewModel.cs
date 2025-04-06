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

namespace MemoryGame.ViewModels
{
    public class SignInViewModel : INotifyPropertyChanged
    {
        private User selectedUser;
        private UserService userService;

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

        // Comenzile
        public ICommand NewUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand NextImageCommand { get; }
        public ICommand PreviousImageCommand { get; }

        public SignInViewModel()
        {
            userService = new UserService();
            var users = userService.LoadUsers();
            Users = new ObservableCollection<User>(users);

            NewUserCommand = new RelayCommand(NewUser);
            DeleteUserCommand = new RelayCommand(DeleteUser, CanModifyUser);
            PlayCommand = new RelayCommand(Play, CanModifyUser);

            // Inițializarea noilor comenzi pentru schimbarea imaginii
            NextImageCommand = new RelayCommand(NextImage, CanChangeImage);
            PreviousImageCommand = new RelayCommand(PreviousImage, CanChangeImage);
        }

        private void NewUser(object parameter)
        {
            var newUserWindow = new MemoryGame.Views.NewUserWindow();
            if (newUserWindow.ShowDialog() == true)
            {
                // Preluăm datele introduse
                string userName = newUserWindow.UserName;
                string absoluteImagePath = newUserWindow.ImagePath;

                // Convertim calea absolută într-o cale relativă
                // Pentru .NET Framework, folosește o metodă helper bazată pe Uri (vedea exemplul anterior)
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string relativeImagePath = PathHelper.GetRelativePath(baseDir, absoluteImagePath);

                // Creăm noul utilizator folosind clasa User din Models
                var newUser = new User
                {
                    Name = userName,
                    ImagePath = relativeImagePath
                };

                // Adăugăm utilizatorul în colecția Users și salvăm lista actualizată în fișierul JSON
                Users.Add(newUser);
                userService.SaveUsers(new List<User>(Users));
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
                Users.Remove(SelectedUser);
                // Salvează lista actualizată în fișier
                userService.SaveUsers(new List<User>(Users));
                SelectedUser = null;
            }
        }

        private void Play(object parameter)
        {
            // Logica de lansare a jocului cu utilizatorul selectat
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

            // Convertim calea relativă a imaginii curente în cale absolută
            string currentAbsolute = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SelectedUser.ImagePath);
            int currentIndex = files.FindIndex(f => f.Equals(currentAbsolute, StringComparison.OrdinalIgnoreCase));

            if (currentIndex < 0)
                currentIndex = 0;

            int nextIndex = (currentIndex + 1) % files.Count;
            string nextAbsolute = files[nextIndex];

            string relativeNext = PathHelper.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, nextAbsolute);
            SelectedUser.ImagePath = relativeNext;

            // Salvează modificările în fișierul JSON
            userService.SaveUsers(new List<User>(Users));
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

            // Salvează modificările în fișierul JSON
            userService.SaveUsers(new List<User>(Users));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
