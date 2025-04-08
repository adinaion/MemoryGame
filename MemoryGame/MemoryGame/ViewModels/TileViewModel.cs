using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MemoryGame.Models;
using MemoryGame.Helpers;

namespace MemoryGame.ViewModels
{
    public class TileViewModel : BaseViewModel
    {
        public Tile Tile { get; private set; }
        public int Index { get; private set; }

        public ICommand FlipCommand { get; set; }

        public TileViewModel(Tile tile, int index)
        {
            Tile = tile;
            Index = index;
        }

        public string ImagePath
        {
            get => (Tile.IsFaceUp || Tile.IsMatched) ? Tile.ImagePath : "Images/back.jpg";
        }

        public void Update()
        {
            OnPropertyChanged(nameof(ImagePath));
        }
    }
}
