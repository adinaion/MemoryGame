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
        public string Category { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public User Player { get; set; }
        public List<Tile> Tiles { get; set; }
        public int TimeRemainingSeconds { get; set; }

        public Game()
        {
            Tiles = new List<Tile>();
        }
    }
}
