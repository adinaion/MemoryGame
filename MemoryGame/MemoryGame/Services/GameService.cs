using System;
using System.IO;
using System.Text.Json;
using MemoryGame.Models;

namespace MemoryGame.Services
{
    public class GameService
    {
        private readonly string filePath = "game.json";

        public void SaveGame(Game game)
        {
            try
            {
                string json = JsonSerializer.Serialize(game, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch
            {
                // Tratați excepțiile conform necesităților
            }
        }

        public Game LoadGame()
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<Game>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}
