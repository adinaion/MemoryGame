using System;
using System.IO;
using System.Text.Json;
using MemoryGame.Models;

namespace MemoryGame.Services
{
    public class GameService
    {
        private readonly string filePath = "savedGames.json";

        // Metodă internă pentru a încărca toate jocurile salvate din fișier
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
                // În caz de eroare, returnăm un dicționar gol
                return new Dictionary<string, Game>();
            }
        }

        // Metodă internă pentru a salva toate jocurile în fișier
        private void SaveAllGames(Dictionary<string, Game> games)
        {
            try
            {
                string json = JsonSerializer.Serialize(games, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch
            {
                // Tratați eventualele excepții după necesitate
            }
        }

        // Metoda publică pentru salvarea unui joc pentru utilizatorul curent
        public void SaveGame(Game game)
        {
            // Încarcă jocurile existente
            var games = LoadAllGames();
            // Actualizează sau adaugă jocul pentru utilizator (suprascrie dacă există)
            games[game.Player.Name] = game;
            // Salvează din nou toate jocurile
            SaveAllGames(games);
        }

        // Metoda publică pentru încărcarea jocului salvate al unui utilizator
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
