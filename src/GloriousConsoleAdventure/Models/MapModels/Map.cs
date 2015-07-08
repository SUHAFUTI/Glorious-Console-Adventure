using System;
using System.Collections.Generic;
using GloriousConsoleAdventure.Color;
using GloriousConsoleAdventure.Enums;

namespace GloriousConsoleAdventure.Models.MapModels
{
    public class Map
    {
        public Block[,] MapBlocks { get; set; }
        public Guid Id { get; set; }
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public Palettes MapPalette { get; set; }
        public List<BlockTile> ActionBlocks { get; set; }
        public  int WallPercentage { get; set; }
    }
}
