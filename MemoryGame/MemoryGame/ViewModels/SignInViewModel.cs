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
                    // Notificăm modificarea pentru ca comenzile dependente de SelectedUser să fie re-evaluate
                    (DeleteUserCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (PlayCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        // Comenzile
        public ICommand NewUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand CancelCommand { get; }

        public SignInViewModel()
        {
            userService = new UserService();
            var users = userService.LoadUsers();
            Users = new ObservableCollection<User>(users);

            NewUserCommand = new RelayCommand(NewUser);
            DeleteUserCommand = new RelayCommand(DeleteUser, CanModifyUser);
            PlayCommand = new RelayCommand(Play, CanModifyUser);
            CancelCommand = new RelayCommand(Cancel);
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

        private void Cancel(object parameter)
        {
            // Logica pentru închiderea ferestrei de sign in
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
