using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MemoryGame.Models;
using MemoryGame.Helpers;

namespace MemoryGame.ViewModels
{
    public class TileViewModel : INotifyPropertyChanged
    {
        public Tile Tile { get; private set; }
        private bool canFlip = true; // Previi flip-ul repetat
        public ICommand FlipCommand { get; }

        public TileViewModel(Tile tile)
        {
            Tile = tile;
            FlipCommand = new RelayCommand(_ => Flip(), _ => canFlip && !Tile.IsFaceUp && !Tile.IsMatched);
        }

        public string ImagePath
        {
            get
            {
                // Dacă tile-ul este întors sau a fost găsit un match, afișează imaginea de față
                return Tile.IsFaceUp || Tile.IsMatched ? Tile.ImagePath : "Images/back.jpg";
            }
        }

        // Eveniment pentru a notifica flip-ul către GameBoardViewModel
        public event EventHandler TileFlipped;

        private void Flip()
        {
            if (!canFlip || Tile.IsFaceUp || Tile.IsMatched)
                return;

            Tile.IsFaceUp = true;
            OnPropertyChanged(nameof(ImagePath));

            // Dezactivează flip-ul imediat după ce a fost întors
            canFlip = false;
            (FlipCommand as RelayCommand)?.RaiseCanExecuteChanged();

            // Notifică ViewModel-ul central
            TileFlipped?.Invoke(this, EventArgs.Empty);
        }

        // Metodă pentru a reabilita posibilitatea de a flip-a
        public void ResetFlip()
        {
            canFlip = true;
            (FlipCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        // Metodă pentru a întoarce tile-ul la starea facedown
        public void ForceFlipDown()
        {
            Tile.IsFaceUp = false;
            OnPropertyChanged(nameof(ImagePath));
            ResetFlip();
        }

        // Marchează tile-ul ca fiind găsit (match)
        public void SetMatched()
        {
            Tile.IsMatched = true;
            OnPropertyChanged(nameof(ImagePath));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
