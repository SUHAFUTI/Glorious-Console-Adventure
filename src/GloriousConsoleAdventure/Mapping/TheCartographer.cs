using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Enums;
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

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray; //Reset due to menu foreground change
            Console.Clear();
            Console.Write(MapToString(map));
            ActionMenu.RenderMenu(hero);
        }

        private static string MapToString(MapHandler map, bool debug = false)
        {
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
