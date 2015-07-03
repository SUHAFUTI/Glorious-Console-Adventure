using System;
using System.Collections.Generic;
using System.Linq;
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
        static readonly MapHandler _map = new MapHandler(MAP_WIDTH, MAP_HEIGHT);
        private static MapHandler _currentMap;
        public static Hero Hero { get; set; }

        static void Main(string[] args)
        {
            //Init hero
            Hero = new Hero("Herald Grimrian");
            Console.SetWindowSize(80, 30);
            //var map = new MapHandler();
            _currentMap = _map;
            TheCartographer.DrawThisMapPlease(_currentMap);
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
                if (heroCoordinate.X == 0)
                {
                    exitDirection = Direction.East;
                }
                if (heroCoordinate.Y == 0)
                {
                    exitDirection = Direction.North;
                    heroCoordinate.Y = MAP_HEIGHT - 1;
                }
                if (heroCoordinate.X == _currentMap.MapWidth + 1)
                {
                    exitDirection = Direction.West;
                }
                if (heroCoordinate.Y == _currentMap.MapHeight + 1)
                {
                    exitDirection = Direction.South;
                }
                var previousMap = _currentMap;
                MapHandler _map2 = new MapHandler(40, 30);
                _currentMap = _map2;
                TheCartographer.DrawMapWithExitsPlease(_currentMap, previousMap.Map, exitDirection);
                RemoveHero();
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
        }
        /// <summary>
        /// This is a method that checks if a block is hit and what action to run
        /// </summary>
        /// <param name="coordinate"></param>
        private static void BlockAction(Coordinate coordinate)
        {
            var block = _map.GetCurrentBlock(coordinate.X, coordinate.Y);

            switch (block)
            {
                case Block.Coin:
                    var coinPath = AppDomain.CurrentDomain.BaseDirectory + "audio\\" + Block.Coin + ".wav";
                    var coinPlayer = new System.Media.SoundPlayer(coinPath);
                    coinPlayer.Play();
                    break;
                case Block.Teleport:
                    var teleportPath = AppDomain.CurrentDomain.BaseDirectory + "audio\\" + Block.Teleport + ".wav";
                    var teleportPlayer = new System.Media.SoundPlayer(teleportPath);
                    teleportPlayer.Play();
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
