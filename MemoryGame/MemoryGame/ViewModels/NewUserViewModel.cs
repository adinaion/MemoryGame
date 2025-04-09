using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MemoryGame.Helpers;
using MemoryGame.Models;
using MemoryGame.Services;

namespace MemoryGame.ViewModels
{
    public class NewUserViewModel : BaseViewModel
    {
        private readonly NewUserService newUserService;

        private string userName;
        public string UserName
        {
            get => userName;
            set
            {
                if (userName != value)
                {
                    userName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentImagePath
        {
            get => newUserService.GetCurrentImagePath();
        }

        public ICommand MoveLeftCommand { get; }
        public ICommand MoveRightCommand { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseWindowAction { get; set; }
        public Action<bool> SetDialogResultAction { get; set; }

        public NewUserViewModel()
        {
            newUserService = new NewUserService();

            MoveLeftCommand = new RelayCommand(_ =>
            {
                newUserService.MoveLeft();
                OnPropertyChanged(nameof(CurrentImagePath));
            });

            MoveRightCommand = new RelayCommand(_ =>
            {
                newUserService.MoveRight();
                OnPropertyChanged(nameof(CurrentImagePath));
            });

            OkCommand = new RelayCommand(_ => Ok());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        private void Ok()
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                return;
            }
            SetDialogResultAction?.Invoke(true);
            CloseWindowAction?.Invoke();
        }

        private void Cancel()
        {
            SetDialogResultAction?.Invoke(false);
            CloseWindowAction?.Invoke();
        }

        public string GetSelectedImageRelativePath()
        {
            return newUserService.GetCurrentImageRelativePath();
        }
    }
}

