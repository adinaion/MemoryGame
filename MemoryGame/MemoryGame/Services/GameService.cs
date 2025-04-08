using System;
using System.IO;
using System.Text.Json;
using MemoryGame.Models;

namespace MemoryGame.Services
{
    public class GameService
    {
        private readonly string filePath = "savedGames.json";

        private Dictionary<string, Game> LoadAllGames()
        {
            if (!File.Exists(filePath))
                return new Dictionary<string, Game>();

            try
            {
                string json = File.ReadAllText(filePath);
                var games = JsonSerializer.Deserialize<Dictionary<string, Game>>(json);
                return games ?? new Dictionary<string, Game>();
            }
            catch
            {
                return new Dictionary<string, Game>();
            }
        }

        private void SaveAllGames(Dictionary<string, Game> games)
        {
            try
            {
                string json = JsonSerializer.Serialize(games, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch
            {
            }
        }

        public void SaveGame(Game game)
        {
            var games = LoadAllGames();
            games[game.Player.Name] = game;
            SaveAllGames(games);
        }

        public Game LoadGame(string userName)
        {
            var games = LoadAllGames();
            if (games.ContainsKey(userName))
                return games[userName];
            else
                return null;
        }

        public void DeleteGame(string userName)
        {
            var games = LoadAllGames();
            if (games.ContainsKey(userName))
            {
                games.Remove(userName);
                SaveAllGames(games);
            }
        }

    }
}
