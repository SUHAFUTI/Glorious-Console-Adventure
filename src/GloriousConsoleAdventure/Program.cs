using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Enums;
using GloriousConsoleAdventure.Mapping;
using GloriousConsoleAdventure.Models.Hero;

namespace GloriousConsoleAdventure
{
    class Program
    {
        const ConsoleColor HERO_COLOR = ConsoleColor.Cyan;
        const ConsoleColor BACKGROUND_COLOR = ConsoleColor.Black;
        private const int MAP_HEIGHT = 30;
        private const int MAP_WIDTH = 40;

        private static readonly List<Block> RandomBlockConfiguration = new List<Block>
        {
            Block.Coin,
            Block.Teleport,
            Block.Teleport
        };
 
        static readonly MapHandler _map = new MapHandler(MAP_WIDTH, MAP_HEIGHT, 40 , RandomBlockConfiguration);
        private static MapHandler _currentMap;
        public static Hero Hero { get; set; }
        public static Dictionary<Guid, MapHandler> MapDictionary;

        static void Main(string[] args)
        {
            //Init hero
            Hero = new Hero("Herald Grimrian");
            MapDictionary = new Dictionary<Guid, MapHandler>() { { _map.Id, _map } };
            Console.SetWindowSize(80, 30);
            //var map = new MapHandler();
            _currentMap = _map;
            _currentMap.GenerateExit(Direction.North);
            TheCartographer.DrawThisMapPlease(_currentMap, Hero);
            InitGame(_currentMap.GetValidStartLocation());
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
        /// Paint the new hero
        /// </summary>
        static void MoveHero(int x, int y)
        {
            Coordinate heroCoordinate = new Coordinate()
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
                    heroCoordinate.Y = MAP_HEIGHT - 1;
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
                    MapHandler _map2 = new MapHandler(40, 30, 40, RandomBlockConfiguration);
                    MapDictionary.Add(_map2.Id, _map2);
                    _currentMap.AdjacentMaps.Add(exitDirection, _map2.Id);
                    _map2.AdjacentMaps.Add(entryDirection, _currentMap.Id);
                    _currentMap = _map2;
                }

                TheCartographer.CloneExitsAndDrawThisMapPlease(_currentMap, previousMap.Map, exitDirection, Hero);
                //RemoveHero();
                Console.BackgroundColor = HERO_COLOR;
                Console.SetCursorPosition(heroCoordinate.X, heroCoordinate.Y);
                Console.Write(" ");
                Hero.Coordinates = heroCoordinate;

            }

            if (CanMove(heroCoordinate))
            {
                RemoveHero();
                Console.BackgroundColor = HERO_COLOR;
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
            Console.BackgroundColor = BACKGROUND_COLOR;
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
        /// Paint a background color
        /// </summary>
        /// <remarks>
        /// It is very important that you run the Clear() method after
        /// changing the background color since this causes a repaint of the background
        /// </remarks>
        static void SetBackgroundColor()
        {
            Console.BackgroundColor = BACKGROUND_COLOR;
            Console.Clear(); //Important!
        }
        /// <summary>
        /// Initiates the game by painting the background
        /// and initiating the hero
        /// </summary>
        static void InitGame(int[] startPosition = null)
        {
            //We don't need this when we used a map generator
            // SetBackgroundColor();


            if (startPosition == null)
            {
                Hero.Coordinates = new Coordinate()
                {
                    X = 0,
                    Y = 0
                };
            }
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
