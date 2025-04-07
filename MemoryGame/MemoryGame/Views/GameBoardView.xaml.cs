using MemoryGame.Models;
using MemoryGame.ViewModels;
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

namespace MemoryGame.Views
{
    public partial class GameBoardView : Window
    {
        public GameBoardView(Game game)
        {
            InitializeComponent();
            // DataContext-ul este setat în ViewModel, care apelează serviciile corespunzătoare.
            DataContext = new GameBoardViewModel(game);
        }
    }
}
