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
    public class GameService
    {
        private readonly string filePath = "game.json";

        // Salvează starea jocului în fișierul game.json
        public void SaveGame(Game game)
        {
            try
            {
                string json = JsonSerializer.Serialize(game, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch
            {
                // Tratează eventualele excepții după necesitate
            }
        }

        // Încarcă starea jocului din fișierul game.json
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
