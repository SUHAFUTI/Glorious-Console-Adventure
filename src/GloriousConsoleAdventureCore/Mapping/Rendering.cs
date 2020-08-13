using System;
using System.Collections.Generic;
using GloriousConsoleAdventure.Enums;

namespace GloriousConsoleAdventure.Mapping
{
    public static class Rendering
    {
        /// <summary>
        /// Dictionary containing block types
        /// </summary>
        public static Dictionary<Block, string> MapSymbols = new Dictionary<Block, string>
        {
            {Block.EmptySpace, " "},
            {Block.Wall, "▓"}, //alt + 219 = █
            {Block.Coin, "©"},
            {Block.Teleport, "т"},
            {Block.Hero, "Ϧ"},
        };
        /// <summary>
        /// Paint a background color
        /// </summary>
        /// <remarks>
        /// It is very important that you run the Clear() method after
        /// changing the background color since this causes a repaint of the background
        /// </remarks>
        static void SetBackgroundColor(ConsoleColor color)
        {
            Console.BackgroundColor = color;
            Console.Clear(); //Important!
        }
    }
}
