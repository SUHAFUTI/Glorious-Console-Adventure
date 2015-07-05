using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Color;
using GloriousConsoleAdventure.Enums;

namespace GloriousConsoleAdventure.Mapping
{
    public class BlockTile
    {
        public Coordinate Coordinate { get; set; }
        public Palettes Palette { get; set; }
        public Block Block { get; set; }
    }
}
