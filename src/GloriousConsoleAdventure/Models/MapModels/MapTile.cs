using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Enums;

namespace GloriousConsoleAdventure.Models.MapModels
{
    public class MapTile
    {
        public Block Block { get; set; }
        public char CustomSymbol { get; set; }
        public bool Indestructable { get; set; }
    }
}
