using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Enums;

namespace GloriousConsoleAdventure
{
    public static class TheCartographer
    {
        public static void DrawThisMapPlease(MapHandler map)
        {
            map.MakeCaverns();
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
