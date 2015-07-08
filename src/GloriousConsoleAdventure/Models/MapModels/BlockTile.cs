﻿using GloriousConsoleAdventure.Color;
using GloriousConsoleAdventure.Enums;

namespace GloriousConsoleAdventure.Models.MapModels
{
    public class BlockTile
    {
        public Coordinate Coordinate { get; set; }
        public Palettes Palette { get; set; }
        public Block Block { get; set; }
    }
}
