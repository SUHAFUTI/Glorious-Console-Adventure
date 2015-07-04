using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Enums;
using GloriousConsoleAdventure.Mapping;
using GloriousConsoleAdventure.Menu;
using GloriousConsoleAdventure.Models.Hero;

namespace GloriousConsoleAdventure
{
    class Program
    {
        const ConsoleColor HeroColor = ConsoleColor.Cyan;
        const ConsoleColor BackgroundColor = ConsoleColor.Black;
        private const int MapHeight = 30;
        private const int MapWidth = 40;

        private static readonly List<Block> RandomBlockConfiguration = new List<Block>
        {
            Block.Coin,
            Block.Teleport,
            Block.Teleport
        };

        static readonly MapHandler Map = new MapHandler(MapWidth, MapHeight, 40, RandomBlockConfiguration);
        private static MapHandler _currentMap;
        public static Hero Hero { get; set; }
        public static Dictionary<Guid, MapHandler> MapDictionary;

        static void Main(string[] args)
        {
            //Remove cursor
            Console.CursorVisible = false;
            //Init hero
            Hero = new Hero("Herald Grimrian");
            MapDictionary = new Dictionary<Guid, MapHandler>() { { Map.Id, Map } };
            Console.SetWindowSize(80, 30);
            //var map = new MapHandler();
            _currentMap = Map;
            _currentMap.GenerateExit(Direction.North);
           // _currentMap.GenerateRandomExits(4);
            TheCartographer.DrawThisMapPlease(_currentMap, Hero);
            InitGame(_currentMap.GetValidStartLocation(15, 15));
            ConsoleKeyInfo keyInfo;
            while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Escape)
            {
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        MoveHero(0, -1);
                        break;

                    case ConsoleKey.RightArrow:
                        MoveHero(1, 0);
                        break;

                    case ConsoleKey.DownArrow:
                        MoveHero(0, 1);
                        break;

                    case ConsoleKey.LeftArrow:
                        MoveHero(-1, 0);
                        break;
                }
            }
        }
        /// <summary>
        /// Paint the hero
        /// </summary>
        /// <param name="x">Move x</param>
        /// <param name="y">Move y</param>
        static void MoveHero(int x, int y)
        {
            var heroCoordinate = new Coordinate()
            {
                X = Hero.Coordinates.X + x,
                Y = Hero.Coordinates.Y + y
            };

            if (_currentMap.IsMapExit(heroCoordinate.X, heroCoordinate.Y))
            {
                Direction exitDirection = Direction.North;
                Direction entryDirection = Direction.South;
                if (heroCoordinate.X == 0)
                {
                    exitDirection = Direction.East;
                    entryDirection = Direction.West;
                }
                if (heroCoordinate.Y == 0)
                {
                    exitDirection = Direction.North;
                    entryDirection = Direction.South;
                    heroCoordinate.Y = MapHeight - 1;
                }
                if (heroCoordinate.X == _currentMap.MapWidth)
                {
                    exitDirection = Direction.West;
                    entryDirection = Direction.East;
                }
                if (heroCoordinate.Y == _currentMap.MapHeight)
                {
                    exitDirection = Direction.South;
                    entryDirection = Direction.North;
                    heroCoordinate.Y = 1;
                }
                var previousMap = _currentMap;

                if (_currentMap.AdjacentMaps.ContainsKey(exitDirection) && MapDictionary.ContainsKey(_currentMap.AdjacentMaps[exitDirection]))
                {
                    _currentMap = MapDictionary[_currentMap.AdjacentMaps[exitDirection]];
                }
                else
                {
                    MapHandler nextMap = new MapHandler(40, 30, 40, RandomBlockConfiguration);
                    MapDictionary.Add(nextMap.Id, nextMap);
                    _currentMap.AdjacentMaps.Add(exitDirection, nextMap.Id);
                    nextMap.AdjacentMaps.Add(entryDirection, _currentMap.Id);
                    _currentMap = nextMap;
                }

                TheCartographer.CloneExitsAndDrawThisMapPlease(_currentMap, previousMap.Map, exitDirection, Hero);
                //RemoveHero();
                Console.BackgroundColor = HeroColor;
                Console.SetCursorPosition(heroCoordinate.X, heroCoordinate.Y);
                Console.Write(" ");
                Hero.Coordinates = heroCoordinate;

            }

            if (CanMove(heroCoordinate))
            {
                RemoveHero();
                Console.BackgroundColor = HeroColor;
                Console.SetCursorPosition(heroCoordinate.X, heroCoordinate.Y);
                Console.Write(" ");
                Hero.Coordinates = heroCoordinate;
                BlockAction(heroCoordinate);
            }
            Hero.Steps++;
            ActionMenu.RenderMenu(Hero);
        }
        /// <summary>
        /// This is a method that checks if a block is hit and what action to run
        /// </summary>
        /// <param name="coordinate"></param>
        private static void BlockAction(Coordinate coordinate)
        {
            var block = _currentMap.GetCurrentBlock(coordinate.X, coordinate.Y);

            switch (block)
            {
                case Block.Coin:
                    var coinPath = AppDomain.CurrentDomain.BaseDirectory + "audio\\" + Block.Coin + ".wav";
                    var coinPlayer = new System.Media.SoundPlayer(coinPath);
                    coinPlayer.Play();
                    _currentMap.ClearBlock(coordinate.X, coordinate.Y);
                    Hero.Coins++;
                    break;
                case Block.Teleport:
                    var teleportPath = AppDomain.CurrentDomain.BaseDirectory + "audio\\" + Block.Teleport + ".wav";
                    var teleportPlayer = new System.Media.SoundPlayer(teleportPath);
                    teleportPlayer.Play();
                    _currentMap.ClearBlock(coordinate.X, coordinate.Y);
                    //TODO teleport
                    break;
            }

        }
        /// <summary>
        /// Overpaint the old hero
        /// </summary>
        static void RemoveHero()
        {
            Console.BackgroundColor = BackgroundColor;
            Console.SetCursorPosition(Hero.Coordinates.X, Hero.Coordinates.Y);
            Console.Write(" ");
        }
        /// <summary>
        /// Make sure that the new coordinate is not placed outside the
        /// console window (since that will cause a runtime crash
        /// </summary>
        static bool CanMove(Coordinate c)
        {
            if (_currentMap.IsWall(c.X, c.Y)) return false;
            if (c.X < 0 || c.X >= Console.WindowWidth)
                return false;

            if (c.Y < 0 || c.Y >= Console.WindowHeight)
                return false;

            return true;
        }
        /// <summary>
        /// Initiates the game by painting the background
        /// and initiating the hero
        /// </summary>
        static void InitGame(int[] startPosition = null)
        {
            //If we don't have a start position default to 0,0
            if (startPosition == null)
            {
                Hero.Coordinates = new Coordinate()
                {
                    X = 0,
                    Y = 0
                };
            }
            //otherwise give Hero default coordinates
            else
            {
                Hero.Coordinates = new Coordinate
                {
                    X = startPosition[0],
                    Y = startPosition[1]
                };
            }

            MoveHero(0, 0);
            ActionMenu.RenderMenu(Hero);

        }
    }
}
