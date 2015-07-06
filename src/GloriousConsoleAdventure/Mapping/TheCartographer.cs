using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Color;
using GloriousConsoleAdventure.Enums;
using GloriousConsoleAdventure.Menu;
using GloriousConsoleAdventure.Models.Hero;
using GloriousConsoleAdventure.Models.MapModels;

namespace GloriousConsoleAdventure.Mapping
{
    public static class TheCartographer
    {

        public static Map CloneExitsAndDrawThisMapPlease(Map map, Block[,] exitMap, Direction exitDirection, Hero hero)
        {

            map = MapHandler.CloneExit(exitMap, exitDirection, map);
            DrawThisMapPlease(map, hero);
            return map;
        }

        public static void DrawThisMapPlease(Map map, Hero hero)
        {

            Console.Clear();
            Console.Write(MapToString(map));
            DrawActionBlocks(map);
            ActionMenu.RenderMenu(hero);
        }

        private static void DrawActionBlocks(Map map)
        {
            foreach (var tile in map.ActionBlocks)
            {
                TheArtist.Paint(tile.Palette, tile.Coordinate, Rendering.MapSymbols[tile.Block]);
            }
        }

        private static string MapToString(Map map, bool debug = false)
        {
            TheArtist.SetPalette(map.MapPalette);
            var returnString = new StringBuilder();
            if (debug)
                returnString.Append(string.Join(" ", // Seperator between each element
                    "Width:",
                    map.MapWidth.ToString(),
                    "\tHeight:",
                    map.MapHeight.ToString(),
                    "\t% Walls:",
                    map.WallPercentage.ToString(),
                    Environment.NewLine
                    ));

            for (int column = 0, row = 0; row < map.MapHeight; row++)
            {
                for (column = 0; column < map.MapWidth; column++)
                {
                    returnString.Append(Rendering.MapSymbols[map.MapBlocks[column, row]]);
                }
                returnString.Append(Environment.NewLine);
            }
            return returnString.ToString();
        }
    }
}
