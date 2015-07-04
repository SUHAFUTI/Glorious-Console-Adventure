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
            {Block.Wall, "█"}, //alt + 219 = █
            {Block.Coin, "ò"},
            {Block.Teleport, "T"}
        };
    }
}
