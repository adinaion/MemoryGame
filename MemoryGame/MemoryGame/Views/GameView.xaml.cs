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

        // Evenimentele pentru RadioButton-uri: acestea rămân aici doar pentru a seta proprietățile din ViewModel.
        private void RbStandard_Checked(object sender, RoutedEventArgs e)
        {
            if (vm != null)
            {
                vm.IsStandardSelected = true;
            }
        }

        private void RbCustom_Checked(object sender, RoutedEventArgs e)
        {
            if (vm != null)
            {
                vm.IsStandardSelected = false;
            }
        }
    }
}
