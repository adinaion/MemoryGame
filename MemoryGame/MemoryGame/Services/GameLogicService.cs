using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MemoryGame.Models;
using MemoryGame.Helpers;

namespace MemoryGame.Services
{
    public class GameLogicService
    {
        // Metoda de creare a unui nou joc
        public Game CreateNewGame(string category, int rows, int columns, User player, int totalTimeSeconds)
        {
            Game newGame = new Game
            {
                Category = category,
                Rows = rows,
                Columns = columns,
                Player = player,
                TimeRemainingSeconds = totalTimeSeconds,
                Tiles = GenerateTiles(category, rows, columns)
            };

            return newGame;
        }

        // Generează tile-uri aleatorii pentru joc
        private List<Tile> GenerateTiles(string category, int rows, int columns)
        {
            string folderName = category.Replace(" ", "");
            string categoryFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", folderName);
            var imageFiles = Directory.GetFiles(categoryFolderPath, "*.jpg");

            int totalTiles = rows * columns;
            int pairsNeeded = totalTiles / 2;

            if (imageFiles.Length < pairsNeeded)
            {
                throw new Exception("Not enough images in the selected category folder!");
            }

            Random random = new Random();
            var selectedImages = imageFiles.OrderBy(x => random.Next()).Take(pairsNeeded).ToList();

            List<Tile> tiles = new List<Tile>();
            foreach (var imgPath in selectedImages)
            {
                string relativePath = PathHelper.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, imgPath);
                // Fiecare imagine este adăugată de două ori pentru a forma perechi
                tiles.Add(new Tile { ImagePath = relativePath, IsFaceUp = false, IsMatched = false });
                tiles.Add(new Tile { ImagePath = relativePath, IsFaceUp = false, IsMatched = false });
            }

            return tiles.OrderBy(x => random.Next()).ToList();
        }

        // Logica de flip a unui tile
        // Se folosesc indicii pentru a identifica tile-urile selectate
        public void FlipTile(Game game, int tileIndex, ref int? firstSelectedIndex, ref int? secondSelectedIndex)
        {
            if (tileIndex < 0 || tileIndex >= game.Tiles.Count)
                return;

            Tile selectedTile = game.Tiles[tileIndex];

            // Nu permite flip-ul unui tile deja întors sau care este deja potrivit
            if (selectedTile.IsFaceUp || selectedTile.IsMatched)
                return;

            // Dacă există deja două tile-uri întoarse care nu sunt un match,
            // întoarce-le la spate la apăsarea unui al treilea tile.
            if (firstSelectedIndex.HasValue && secondSelectedIndex.HasValue)
            {
                var firstTile = game.Tiles[firstSelectedIndex.Value];
                var secondTile = game.Tiles[secondSelectedIndex.Value];

                if (firstTile.ImagePath != secondTile.ImagePath)
                {
                    firstTile.IsFaceUp = false;
                    secondTile.IsFaceUp = false;
                }
                firstSelectedIndex = null;
                secondSelectedIndex = null;
            }

            // Flip-ul tile-ului curent
            selectedTile.IsFaceUp = true;

            // Actualizează selecția
            if (!firstSelectedIndex.HasValue)
                firstSelectedIndex = tileIndex;
            else if (!secondSelectedIndex.HasValue)
                secondSelectedIndex = tileIndex;

            // Dacă avem două tile-uri întoarse, verificăm dacă se potrivesc
            if (firstSelectedIndex.HasValue && secondSelectedIndex.HasValue)
            {
                var firstTile = game.Tiles[firstSelectedIndex.Value];
                var secondTile = game.Tiles[secondSelectedIndex.Value];

                if (firstTile.ImagePath == secondTile.ImagePath)
                {
                    firstTile.IsMatched = true;
                    secondTile.IsMatched = true;
                    firstSelectedIndex = null;
                    secondSelectedIndex = null;
                }
                // Dacă nu se potrivesc, se vor întoarce la apăsarea unui alt tile (conform logicii de mai sus)
            }
        }

        // Metodă pentru verificarea unui joc câștigat
        public bool IsGameWon(Game game)
        {
            // Jocul este câștigat dacă toate tile-urile sunt marcate ca fiind potrivite.
            return game.Tiles.All(tile => tile.IsMatched);
        }

        // Metodă pentru verificarea unui joc pierdut
        // Aceasta poate fi apelată, de exemplu, atunci când timpul s-a scurs.
        public bool IsGameLost(Game game)
        {
            // În mod implicit, dacă timpul a expirat, jocul este pierdut.
            return game.TimeRemainingSeconds <= 0;
        }
    }
}

