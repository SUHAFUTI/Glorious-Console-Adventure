using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Enums;

namespace GloriousConsoleAdventure.Mapping
{
    public static class TheCartographer
    {
        public static void DrawThisMapPlease(MapHandler map)
        {
            PopulateMapWithBlocks(map);
            map.PrintMap();
        }

        public static void DrawMapWithExitsPlease(MapHandler map, Block[,] exitMap, Direction exitDirection)
        {
            map.PlaceExit(exitMap, exitDirection);
            PopulateMapWithBlocks(map);
           
            map.PrintMap();            
        }
        static void PopulateMapWithBlocks(MapHandler map)
        {
            map.PlaceRandomBlock(Block.Coin);
            map.PlaceRandomBlock(Block.Teleport);
            map.PlaceRandomBlock(Block.Teleport);
        }
    }
}
