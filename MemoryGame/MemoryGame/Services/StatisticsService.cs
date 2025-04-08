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
            }
        }

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

        public List<PlayerStatistics> GetAllStatistics()
        {
            var stats = LoadAllStatistics();

            var userService = new UserService();
            var users = userService.LoadUsers();

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

