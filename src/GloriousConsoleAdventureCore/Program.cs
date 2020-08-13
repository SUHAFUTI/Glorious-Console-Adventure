using GloriousConsoleAdventure.Color;
using GloriousConsoleAdventure.Enums;
using GloriousConsoleAdventure.Mapping;
using GloriousConsoleAdventure.Menu;
using GloriousConsoleAdventure.Models.Hero;
using GloriousConsoleAdventure.Models.MapModels;
using GloriousConsoleAdventureCore.Helpers;
using System;
using System.Collections.Generic;

namespace GloriousConsoleAdventureCore
{
    class Program
    {
        public const int MapHeight = 30;
        public const int MapWidth = 40;
        private static readonly List<Block> RandomBlockConfiguration = new List<Block>
        {
            Block.Coin,
            Block.Teleport,
            Block.Teleport
        };
        private readonly MapHandler MapHandler = new MapHandler();
        static void Main(string[] args)
        {
            
            var soundDictionary = new Dictionary<string, string>();
            soundDictionary.Add("coin", AppContext.BaseDirectory + "audio\\" + Block.Coin + ".wav");
            soundDictionary.Add("teleport", AppContext.BaseDirectory + "audio\\" + Block.Teleport + ".wav");
            soundDictionary.Add("explosion", AppContext.BaseDirectory + "audio\\explosion.wav");

            var soundPlayer = new SoundPlayer(soundDictionary);
            var actionMenu = new ActionMenu(86);
            var mapHandler = new MapHandler();
            var artist = new TheArtist(actionMenu, mapHandler);

            var gameEngine = new GameEngine(artist, soundPlayer, mapHandler, actionMenu);
            gameEngine.Game();
        }

    }
}
