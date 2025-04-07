using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MemoryGame.Models;
using MemoryGame.ViewModels;

namespace MemoryGame.Views
{
    public partial class GameView : Window
    {
        private GameViewModel vm;

        public GameView()
        {
            InitializeComponent();
            vm = new GameViewModel();
            DataContext = vm;
        }

        // Nou constructor care primește utilizatorul curent
        public GameView(User currentUser)
        {
            InitializeComponent();
            vm = new GameViewModel();
            vm.CurrentPlayer = currentUser;
            DataContext = vm;
        }
    }
}
