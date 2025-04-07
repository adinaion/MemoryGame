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
        // Indexul tile-ului în colecția de tile-uri (folosit pentru comanda de flip)
        public int Index { get; private set; }

        // Comanda de flip este setată de părinte (GameBoardViewModel) prin binding la FlipTileCommand
        public ICommand FlipCommand { get; set; }

        public TileViewModel(Tile tile, int index)
        {
            Tile = tile;
            Index = index;
        }

        // Proprietatea ImagePath afișează imaginea corespunzătoare în funcție de starea tile-ului
        public string ImagePath
        {
            get => (Tile.IsFaceUp || Tile.IsMatched) ? Tile.ImagePath : "Images/back.jpg";
        }

        // Metodă simplă pentru actualizare – notifică schimbarea proprietății
        public void Update()
        {
            OnPropertyChanged(nameof(ImagePath));
        }
    }
}
