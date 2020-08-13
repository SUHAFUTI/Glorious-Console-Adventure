using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Enums;
using GloriousConsoleAdventure.Mapping;
using GloriousConsoleAdventure.Menu;
using GloriousConsoleAdventure.Models.Hero;
using GloriousConsoleAdventure.Models.MapModels;

namespace GloriousConsoleAdventure.Color
{
    public class TheArtist
    {
        ActionMenu _actionMenu;
        MapHandler _mapHandler;
        public TheArtist(ActionMenu actionMenu, MapHandler mapHandler)
        {
            _actionMenu = actionMenu;
            _mapHandler = mapHandler;
        }
        private static readonly Dictionary<Palettes, ColorPreset> PaletteDictionary = new Dictionary<Palettes, ColorPreset>()
        {
            { Palettes.Hero, new ColorPreset{ Background = ConsoleColor.Black, Foreground = ConsoleColor.Cyan }},
            { Palettes.Cave, new ColorPreset{ Background = ConsoleColor.Black, Foreground = ConsoleColor.Gray}},
            { Palettes.Coin, new ColorPreset{ Background = ConsoleColor.Black, Foreground = ConsoleColor.DarkYellow}},
            { Palettes.Teleport, new ColorPreset{ Background = ConsoleColor.Black, Foreground = ConsoleColor.Cyan}},
            { Palettes.Menu, new ColorPreset{ Background = ConsoleColor.Black, Foreground = ConsoleColor.DarkRed}},
            { Palettes.StatusBar, new ColorPreset{ Background = ConsoleColor.Green, Foreground = ConsoleColor.White}},
            { Palettes.Grass, new ColorPreset{ Background = ConsoleColor.DarkGreen, Foreground = ConsoleColor.Green}}

        };

        public static void SetPalette(Palettes palette)
        {
            var retrievedPalette = PaletteDictionary[palette];
            
            if (retrievedPalette == null)
                throw new Exception("Palette is not set"); //TODO: throw proper exception

            Console.ForegroundColor = retrievedPalette.Foreground;
            Console.BackgroundColor = retrievedPalette.Background;
        }
        public static void SetColor(ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        public void Paint(BlockTile blockTile)
        {
            var blockSymbol = Rendering.MapSymbols[blockTile.Block];
            Paint(blockTile.Palette, blockTile.Coordinate, blockSymbol);
        }


        public void Paint(Palettes palette, Coordinate coordinate, Block block)
        {
            var blockSymbol = Rendering.MapSymbols[block];
            Paint(palette, coordinate, blockSymbol);
        }

        private void Paint(Palettes palette, Coordinate coordinate, string symbol)
        {
            var retrievedColors = PaletteDictionary[palette];
            if(retrievedColors != null)
                SetColor(retrievedColors.Foreground, retrievedColors.Background);
            Console.SetCursorPosition(coordinate.X, coordinate.Y);
            Console.Write(symbol);
            ResetPalette();
        }

        public void DrawHouse(Coordinate coordinate, Map map)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var blockTile = new BlockTile
                    {
                        Block = Block.Occupied,
                        Coordinate = new Coordinate(coordinate.X + j, coordinate.Y + i),
                        Palette = Palettes.Transparent
                    };
                    
                    if (i == 3 && j > 5 && j < 8)
                    {
                        blockTile.Block = Block.Interactive;
                        blockTile.Interaction = Interaction.DynamiteVendor;
                    }
                    map.ActionBlocks.Add(blockTile);
                }
                    
            }
            
            Console.SetCursorPosition(coordinate.X, coordinate.Y);
            Console.WriteLine(@"  []___  ");
            Console.SetCursorPosition(coordinate.X, coordinate.Y+1);
            Console.WriteLine(@" /    /\ ");
            Console.SetCursorPosition(coordinate.X, coordinate.Y+2);
            Console.WriteLine(@"/____/__\");
            Console.SetCursorPosition(coordinate.X, coordinate.Y+3);
            Console.WriteLine(@"|[][]||||");

        }

        public static void Delete(Coordinate coordinate)
        {
            var retrievedColors = PaletteDictionary[Palettes.Cave];
            if (retrievedColors == null)
                throw new Exception("Palette is not set!"); //TODO: throw proper exception
            
            SetColor(retrievedColors.Foreground, retrievedColors.Background);
            Console.SetCursorPosition(coordinate.X, coordinate.Y);
            Console.Write(" ");
            Console.ResetColor();
        }

        public static void ResetPalette()
        {
            SetPalette(Palettes.Cave);
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
            _actionMenu.RenderMenu(hero, world);
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
                    Paint(tile.Palette, tile.Coordinate, Rendering.MapSymbols[tile.Block]);
            }
        }
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
                _mapHandler.CloneExit(adjacentMap.Value, adjacentMap.Key, map);
            }
            DrawGame(map, hero, world);
        }
        /// <summary>
        /// Renders the provided map to a string
        /// </summary>
        /// <param name="map">Map to render</param>
        /// <param name="debug">isDebug?</param>
        /// <returns>Map as string</returns>
        private string MapToString(Map map, bool debug = false)
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
