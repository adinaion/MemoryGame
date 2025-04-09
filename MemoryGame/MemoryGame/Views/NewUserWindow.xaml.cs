using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using MemoryGame.ViewModels;

namespace MemoryGame.Views
{
    public partial class NewUserWindow : Window
    {
        public NewUserWindow()
        {
            InitializeComponent();
            var vm = new NewUserViewModel();
            vm.CloseWindowAction = new Action(() => this.Close());
            vm.SetDialogResultAction = new Action<bool>(result => this.DialogResult = result);
            DataContext = vm;
        }
    }
}
