using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Color;
using GloriousConsoleAdventure.Enums;
using GloriousConsoleAdventure.Menu;
using GloriousConsoleAdventure.Models.Hero;

namespace GloriousConsoleAdventure.Mapping
{
    public static class TheCartographer
    {

        public static void CloneExitsAndDrawThisMapPlease(MapHandler map, Block[,] exitMap, Direction exitDirection, Hero hero)
        {
            map.CloneExit(exitMap, exitDirection);

            DrawThisMapPlease(map, hero);
        }

        public static void DrawThisMapPlease(MapHandler map, Hero hero)
        {

            Console.Clear();
            Console.Write(MapToString(map));
            DrawActionBlocks(map);
            ActionMenu.RenderMenu(hero);
        }

        private static void DrawActionBlocks(MapHandler map)
        {

            foreach (var tile in map.ActionBlocks)
            {

                TheArtist.Paint(tile.Palette, tile.Coordinate, Rendering.MapSymbols[tile.Block]);
            }
        }

        private static string MapToString(MapHandler map, bool debug = false)
        {
            TheArtist.SetPalette(map.MapPalette);
            string returnString = "";
            if (debug) returnString = string.Join(" ", // Seperator between each element
                                            "Width:",
                                            map.MapWidth.ToString(),
                                            "\tHeight:",
                                            map.MapHeight.ToString(),
                                            "\t% Walls:",
                                            map.PercentAreWalls.ToString(),
                                            Environment.NewLine
                                           );

            for (int column = 0, row = 0; row < map.MapHeight; row++)
            {
                for (column = 0; column < map.MapWidth; column++)
                {
                    returnString += Rendering.MapSymbols[map.Map[column, row]];
                }
                returnString += Environment.NewLine;
            }
            return returnString;
        }
    }
}
