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
                tiles.Add(new Tile { ImagePath = relativePath, IsFaceUp = false, IsMatched = false });
                tiles.Add(new Tile { ImagePath = relativePath, IsFaceUp = false, IsMatched = false });
            }

            return tiles.OrderBy(x => random.Next()).ToList();
        }

        public void FlipTile(Game game, int tileIndex, ref int? firstSelectedIndex, ref int? secondSelectedIndex)
        {
            if (tileIndex < 0 || tileIndex >= game.Tiles.Count)
                return;

            Tile selectedTile = game.Tiles[tileIndex];

            if (selectedTile.IsFaceUp || selectedTile.IsMatched)
                return;

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

            selectedTile.IsFaceUp = true;

            if (!firstSelectedIndex.HasValue)
                firstSelectedIndex = tileIndex;
            else if (!secondSelectedIndex.HasValue)
                secondSelectedIndex = tileIndex;

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
            }
        }

        public bool IsGameWon(Game game)
        {
            return game.Tiles.All(tile => tile.IsMatched);
        }

        public bool IsGameLost(Game game)
        {
            return game.TimeRemainingSeconds <= 0;
        }
    }
}

