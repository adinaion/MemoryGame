using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using MemoryGame.Models;

namespace MemoryGame.Services
{
    public class StatisticsService
    {
        private readonly string filePath = "statistics.json";

        private Dictionary<string, PlayerStatistics> LoadAllStatistics()
        {
            if (!File.Exists(filePath))
                return new Dictionary<string, PlayerStatistics>();

            try
            {
                string json = File.ReadAllText(filePath);
                var stats = JsonSerializer.Deserialize<Dictionary<string, PlayerStatistics>>(json);
                return stats ?? new Dictionary<string, PlayerStatistics>();
            }
            catch
            {
                return new Dictionary<string, PlayerStatistics>();
            }
        }

        private void SaveAllStatistics(Dictionary<string, PlayerStatistics> stats)
        {
            try
            {
                string json = JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch
            {
                // Tratați excepțiile după necesitate
            }
        }

        // Metodă de actualizare: dacă jucătorul a câștigat, se incrementează atât GamesPlayed cât și GamesWon,
        // iar dacă a pierdut, se incrementează doar GamesPlayed.
        public void UpdateStatistics(string userName, bool won)
        {
            var stats = LoadAllStatistics();
            if (stats.ContainsKey(userName))
            {
                stats[userName].GamesPlayed++;
                if (won)
                    stats[userName].GamesWon++;
            }
            else
            {
                stats[userName] = new PlayerStatistics
                {
                    UserName = userName,
                    GamesPlayed = 1,
                    GamesWon = won ? 1 : 0
                };
            }
            SaveAllStatistics(stats);
        }

        // Returnează lista tuturor statisticilor pentru afișare.
        public List<PlayerStatistics> GetAllStatistics()
        {
            // Încarcă statisticile existente din fișier
            var stats = LoadAllStatistics();

            // Încarcă lista de utilizatori
            var userService = new UserService();
            var users = userService.LoadUsers();

            // Pentru fiecare utilizator, dacă nu există statistică, adaugă o intrare implicită
            foreach (var user in users)
            {
                if (!stats.ContainsKey(user.Name))
                {
                    stats[user.Name] = new PlayerStatistics
                    {
                        UserName = user.Name,
                        GamesPlayed = 0,
                        GamesWon = 0
                    };
                }
            }

            return stats.Values.ToList();
        }

        public void DeleteStatistics(string userName)
        {
            var stats = LoadAllStatistics();
            if (stats.ContainsKey(userName))
            {
                stats.Remove(userName);
                SaveAllStatistics(stats);
            }
        }

    }
}

