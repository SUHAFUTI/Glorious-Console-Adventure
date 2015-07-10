using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Color;
using GloriousConsoleAdventure.Enums;
using GloriousConsoleAdventure.Mapping;
using GloriousConsoleAdventure.Menu;
using GloriousConsoleAdventure.Models.Hero;
using GloriousConsoleAdventure.Models.MapModels;

namespace GloriousConsoleAdventure
{
    class Program
    {
        private const int MapHeight = 30;
        private const int MapWidth = 40;
        private static readonly List<Block> RandomBlockConfiguration = new List<Block>
        {
            Block.Coin,
            Block.Teleport,
            Block.Teleport
        };

        //static readonly MapHandler Map = new MapHandler(MapWidth, MapHeight, 40, RandomBlockConfiguration);
        private static readonly MapHandler MapHandler = new MapHandler();
        private static Map _currentMap;
        public static Hero Hero { get; set; }
        //public static Dictionary<Guid, MapHandler> MapDictionary;
        private static World _world;

        static void Main(string[] args)
        {
            //Remove cursor
            Console.CursorVisible = false;
            //Init hero
            Hero = new Hero("Herald Grimrian");
            //Height has to be 1 more than map
            Console.SetWindowSize(80, 31);

            //Create map
            _currentMap = MapHandler.CreateMap(MapWidth, MapHeight, 40, RandomBlockConfiguration);
            //Init new world from 0,0
            _world = new World { WhereAmI = new Coordinate(0, 0), MapGrid = new Dictionary<Coordinate, Map>() };
            _world.MapGrid.Add(new Coordinate(0, 0), _currentMap);


            MapHandler.GenerateExit(Direction.North, _currentMap);
            MapHandler.GenerateExit(Direction.South, _currentMap);
            MapHandler.GenerateExit(Direction.East, _currentMap);
            MapHandler.GenerateExit(Direction.West, _currentMap);

            //_currentMap = MapHandler.GenerateRandomExits(_currentMap, 4);
            TheCartographer.DrawThisMapPlease(_currentMap, Hero, _world);
            InitGame(MapHandler.GetValidStartLocation(15, 15, _currentMap));
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

            if (MapHandler.IsMapExit(heroCoordinate.X, heroCoordinate.Y, _currentMap))
            {
                var exitDirection = Direction.North;
                var entryDirection = Direction.South;
                //If X is 0 we go west
                if (heroCoordinate.X == 0)
                {
                    exitDirection = Direction.West;
                    entryDirection = Direction.East;
                    heroCoordinate.X = MapWidth - 1;
                }
                //If Y is 0 we go North
                if (heroCoordinate.Y == 0)
                {
                    exitDirection = Direction.North;
                    entryDirection = Direction.South;
                    heroCoordinate.Y = MapHeight - 1;
                }
                //If x is the width of the map we go east
                if (heroCoordinate.X == _currentMap.MapWidth)
                {
                    exitDirection = Direction.East;
                    entryDirection = Direction.West;
                    heroCoordinate.X = 1;
                }
                //If x is the height we go South
                if (heroCoordinate.Y == _currentMap.MapHeight)
                {
                    entryDirection = Direction.North;
                    heroCoordinate.Y = 1;
                }
                Coordinate coordinate;
                switch (entryDirection)
                {
                    case Direction.North:
                        coordinate = new Coordinate(_world.WhereAmI.X, _world.WhereAmI.Y - 1);
                        if (_world.MapGrid.ContainsKey(coordinate))
                        {
                            _currentMap = _world.MapGrid[coordinate];
                            TheCartographer.DrawThisMapPlease(_currentMap, Hero, _world);
                        }
                        else
                        {
                            GenerateNextMap(coordinate, Direction.South);
                        }
                        break;
                    case Direction.South:
                        coordinate = new Coordinate(_world.WhereAmI.X, _world.WhereAmI.Y + 1);
                        if (_world.MapGrid.ContainsKey(coordinate))
                        {
                            _currentMap = _world.MapGrid[coordinate];
                            TheCartographer.DrawThisMapPlease(_currentMap, Hero, _world);
                        }
                        else
                        {
                            GenerateNextMap(coordinate, Direction.North);
                        }
                        break;
                    case Direction.East:
                        coordinate = new Coordinate(_world.WhereAmI.X + 1, _world.WhereAmI.Y);
                        if (_world.MapGrid.ContainsKey(coordinate))
                        {
                            _currentMap = _world.MapGrid[coordinate];
                            TheCartographer.DrawThisMapPlease(_currentMap, Hero, _world);
                        }
                        else
                        {
                            GenerateNextMap(coordinate, Direction.West);
                        }
                        break;
                    case Direction.West:
                        coordinate = new Coordinate(_world.WhereAmI.X - 1, _world.WhereAmI.Y);
                        if (_world.MapGrid.ContainsKey(coordinate))
                        {
                            _currentMap = _world.MapGrid[coordinate];
                            TheCartographer.DrawThisMapPlease(_currentMap, Hero, _world);
                        }
                        else
                        {
                            GenerateNextMap(coordinate, Direction.East);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                //Update coordinates on where I am in the world
                _world.WhereAmI = coordinate;
                TheArtist.Paint(Palettes.Hero, heroCoordinate, " ");
                Hero.Coordinates = heroCoordinate;
                Hero.Steps++;
            }

            if (CanMove(heroCoordinate, _currentMap))
            {
                RemoveHero();
                TheArtist.Paint(Palettes.Hero, heroCoordinate, " ");
                Hero.Coordinates = heroCoordinate;
                BlockAction(heroCoordinate, _currentMap);
                Hero.Steps++;
            }

            ActionMenu.RenderMenu(Hero, _world);
        }

        private static void GenerateNextMap(Coordinate coordinate, Direction exitDirection)
        {
            var nextmap = MapHandler.CreateMap(MapWidth, MapHeight, 40, RandomBlockConfiguration);
            MapHandler.GenerateRandomExitDirection(nextmap, 1);
            TheCartographer.CloneExitsAndDrawThisMapPlease(nextmap, _currentMap.MapBlocks, exitDirection, Hero, _world);
            _world.MapGrid.Add(coordinate, nextmap);
            //Set current map to the next
            _currentMap = nextmap;
        }

        /// <summary>
        /// This is a method that checks if a block is hit and what action to run
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="map">Map to run blockaction on</param>
        private static void BlockAction(Coordinate coordinate, Map map)
        {
            var block = MapHandler.GetCurrentBlock(coordinate.X, coordinate.Y, map);

            switch (block)
            {
                case Block.Coin:
                    var coinPath = AppDomain.CurrentDomain.BaseDirectory + "audio\\" + Block.Coin + ".wav";
                    var coinPlayer = new System.Media.SoundPlayer(coinPath);
                    coinPlayer.Play();
                    MapHandler.ClearBlock(coordinate.X, coordinate.Y, map);
                    Hero.Coins++;
                    break;
                case Block.Teleport:
                    var teleportPath = AppDomain.CurrentDomain.BaseDirectory + "audio\\" + Block.Teleport + ".wav";
                    var teleportPlayer = new System.Media.SoundPlayer(teleportPath);
                    teleportPlayer.Play();
                    MapHandler.ClearBlock(coordinate.X, coordinate.Y, map);
                    //TODO teleport
                    break;
            }

        }
        /// <summary>
        /// Overpaint the old hero
        /// </summary>
        static void RemoveHero()
        {
            TheArtist.Delete(Hero.Coordinates);
        }
        /// <summary>
        /// Make sure that the new coordinate is not placed outside the
        /// console window (since that will cause a runtime crash
        /// </summary>
        static bool CanMove(Coordinate c, Map map)
        {
            if (MapHandler.IsWall(c.X, c.Y, map)) return false;
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
            ActionMenu.RenderMenu(Hero, _world);

        }
    }
}
