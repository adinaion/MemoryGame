using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Models
{
    public class Tile
    {
        public string ImagePath { get; set; }
        public bool IsFaceUp { get; set; }
        public bool IsMatched { get; set; }
    }
}
