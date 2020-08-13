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
    /// <summary>
    /// Handles map related stuff
    /// </summary>
    public class TheCartographer
    {
        TheArtist _artist = new TheArtist();
        
        /// <summary>
        /// CLones the exit from provided map and renders game 
        /// </summary>
        /// <param name="map">Map to draw</param>
        /// <param name="exitMap">Map from where we came</param>
        /// <param name="exitDirection">The direction to generate the exit</param>
        /// <param name="hero">Hero to render</param>
        /// <param name="world">World to render</param>
        
        public void CloneExitsAndDrawThisMapPlease(Map map, Dictionary<Direction, Map> adjactenMaps, Hero hero, World world)
        {
            foreach (var adjacentMap in adjactenMaps)
            {
                MapHandler.CloneExit(adjacentMap.Value, adjacentMap.Key, map);
            }
            DrawGame(map, hero, world);
        }

        /// <summary>
        /// Draws the entire game screen
        /// </summary>
        /// <param name="map">Map to draw</param>
        /// <param name="hero">Hero to draw</param>
        /// <param name="world">World to draw</param>
        public void DrawGame(Map map, Hero hero, World world)
        {
            Console.Clear();
            Console.Write(MapToString(map));
            DrawActionBlocks(map);
            ActionMenu.RenderMenu(hero, world);
        }

        /// <summary>
        /// Draw the different action blocks
        /// </summary>
        /// <param name="map">The map we want to draw the blocks on</param>
        private void DrawActionBlocks(Map map)
        {
            foreach (var tile in map.ActionBlocks)
            {
                if (tile.Block != Block.Impenetrable && tile.Block != Block.Occupied && tile.Block != Block.Interactive)
                    _artist.Paint(tile.Palette, tile.Coordinate, Rendering.MapSymbols[tile.Block]);
            }
        }

        /// <summary>
        /// Renders the provided map to a string
        /// </summary>
        /// <param name="map">Map to render</param>
        /// <param name="debug">isDebug?</param>
        /// <returns>Map as string</returns>
        private string MapToString(Map map, bool debug = false)
        {
            _artist.SetPalette(map.MapPalette);
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
