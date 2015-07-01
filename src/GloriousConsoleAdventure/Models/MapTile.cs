using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Enums;

namespace GloriousConsoleAdventure.Models
{
    public class MapTile
    {
        public Block Block { get; set; }
        public BlockType BlockType { get; set; }
    }
}
