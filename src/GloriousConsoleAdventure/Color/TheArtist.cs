using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Enums;
using GloriousConsoleAdventure.Mapping;
using GloriousConsoleAdventure.Models.MapModels;

namespace GloriousConsoleAdventure.Color
{
    public static class TheArtist
    {
        private static readonly Dictionary<Palettes, ColorPreset> PaletteDictionary = new Dictionary<Palettes, ColorPreset>()
        {
            { Palettes.Hero, new ColorPreset{ Background = ConsoleColor.Cyan, Foreground = ConsoleColor.Cyan }},
            { Palettes.Cave, new ColorPreset{ Background = ConsoleColor.Black, Foreground = ConsoleColor.Gray}},
            { Palettes.Coin, new ColorPreset{ Background = ConsoleColor.Black, Foreground = ConsoleColor.DarkYellow}},
            { Palettes.Teleport, new ColorPreset{ Background = ConsoleColor.Black, Foreground = ConsoleColor.Blue}},
            { Palettes.Menu, new ColorPreset{ Background = ConsoleColor.Black, Foreground = ConsoleColor.DarkRed}},
            { Palettes.StatusBar, new ColorPreset{ Background = ConsoleColor.Green, Foreground = ConsoleColor.White}},
            { Palettes.Grass, new ColorPreset{ Background = ConsoleColor.DarkGreen, Foreground = ConsoleColor.Green}}

        };
        public static void SetPalette(Palettes palette)
        {
            var retrievedPalette = PaletteDictionary[palette];
            
            if (retrievedPalette == null)
                throw new ConfigurationErrorsException("Palette is not set");

            Console.ForegroundColor = retrievedPalette.Foreground;
            Console.BackgroundColor = retrievedPalette.Background;
        }
        public static void SetColor(ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        public static void Paint(BlockTile blockTile)
        {
            var blockSymbol = Rendering.MapSymbols[blockTile.Block];
            Paint(blockTile.Palette, blockTile.Coordinate, blockSymbol);
        }


        public static void Paint(Palettes palette, Coordinate coordinate, Block block)
        {
            var blockSymbol = Rendering.MapSymbols[block];
            Paint(palette, coordinate, blockSymbol);
        }

        public static void Paint(Palettes palette, Coordinate coordinate, string symbol)
        {
            var retrievedColors = PaletteDictionary[palette];
            if(retrievedColors != null)
                SetColor(retrievedColors.Foreground, retrievedColors.Background);
            Console.SetCursorPosition(coordinate.X, coordinate.Y);
            Console.Write(symbol);
            ResetPalette();
        }

        public static void DrawHouse(Coordinate coordinate, Map map)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    map.ActionBlocks.Add(new BlockTile
                    {
                        Block = Block.Impenetrable,
                        Coordinate = new Coordinate(coordinate.X + j, coordinate.Y + i),
                        Palette = Palettes.Transparent
                    });    
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
                throw new ConfigurationErrorsException("Palette is not set!");
            
            SetColor(retrievedColors.Foreground, retrievedColors.Background);
            Console.SetCursorPosition(coordinate.X, coordinate.Y);
            Console.Write(" ");
            Console.ResetColor();
        }

        public static void ResetPalette()
        {
            SetPalette(Palettes.Cave);
        }
    }
}
