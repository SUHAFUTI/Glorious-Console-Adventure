using System;
using System.Collections.Generic;
using System.Linq;
using GloriousConsoleAdventure.Color;
using GloriousConsoleAdventure.Enums;

namespace GloriousConsoleAdventure.Models.MapModels
{
    /// <summary>
    /// Basic map entity. Holds all the information for a map.
    /// </summary>
    public class Map
    {
        public Block[,] MapBlocks { get; set; }
        public Guid Id { get; set; }
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public Palettes MapPalette { get; set; }
        public List<BlockTile> ActionBlocks { get; set; }
        public Dictionary<Direction, Coordinate> Exits { get; set; }
        public  int WallPercentage { get; set; }
        public List<Map> MapStructures { get; set; }
        public Coordinate ParentMap { get; set; }

        public Map()
        {
            Exits = new Dictionary<Direction, Coordinate>();
            MapStructures = new List<Map>();
        }

        public BlockTile GetActionBlock(Coordinate coordinate)
        {
            return ActionBlocks.FirstOrDefault(x => x.Coordinate.Equals(coordinate));
        }

    }
}
