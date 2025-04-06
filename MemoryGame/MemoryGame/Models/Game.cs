using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Collections.Generic;

namespace MemoryGame.Models
{
    public class Game
    {
        // Categoria de imagini aleasă
        public string Category { get; set; }

        // Dimensiunile tablei de joc
        public int Rows { get; set; }
        public int Columns { get; set; }

        // Jucătorul care joacă
        public User Player { get; set; }

        // Lista de tile-uri (jetoane)
        public List<Tile> Tiles { get; set; }

        // Timpul rămas în secunde (de exemplu, 120 pentru 2 minute)
        public int TimeRemainingSeconds { get; set; }

        public Game()
        {
            Tiles = new List<Tile>();
        }
    }
}
