﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static void Paint(Palettes palette, Coordinate coordinate, string symbol)
        {
            var retrievedColors = PaletteDictionary[palette];
            if(retrievedColors != null)
                SetColor(retrievedColors.Foreground, retrievedColors.Background);
            Console.SetCursorPosition(coordinate.X, coordinate.Y);
            Console.Write(symbol);
            ResetPalette();
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
