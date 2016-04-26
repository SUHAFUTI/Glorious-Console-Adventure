﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Color;
using GloriousConsoleAdventure.Enums;
using GloriousConsoleAdventure.Helpers;
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
        private static readonly MapHandler MapHandler = new MapHandler();
        private static Map _currentMap;
        public static Hero Hero { get; set; }
        private static World _world;

        static void Main(string[] args)
        {
            //Remove cursor
            Console.CursorVisible = false;
            //Init hero
            Hero = new Hero("Herald Grimrian");
            Hero.Dynamite = 5;
            //Height has to be 1 more than map
            Console.SetWindowSize(80, 31);

            //Create map
            _currentMap = MapHandler.CreateMap(MapWidth, MapHeight, 40, RandomBlockConfiguration);
            //Init new world from 0,0
            _world = new World { WhereAmI = new Coordinate(0, 0), MapGrid = new Dictionary<Coordinate, Map>() };
            _world.MapGrid.Add(new Coordinate(0, 0), _currentMap);

            //Generate one exit in each direction
            MapHandler.GenerateExit(Direction.North, _currentMap);
            MapHandler.GenerateExit(Direction.South, _currentMap);
            MapHandler.GenerateExit(Direction.East, _currentMap);
            MapHandler.GenerateExit(Direction.West, _currentMap);

            //Draw the map
            TheCartographer.DrawGame(_currentMap, Hero, _world);
            //Let's get it on!
            InitGame(MapHandler.GetValidStartLocation(15, 15, _currentMap));

            //Main loop. Check if arrow keys are pressed and move accordingly.
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
                    case ConsoleKey.Delete:
                        if (Hero.Dynamite > 0)
                        {
                            ActionMenu.Status = "KABOOOM!";
                            Hero.Dynamite--;
                            MapHandler.BlastCrossBombermanStyle(Hero.Coordinates, _currentMap);
                            TheCartographer.DrawGame(_currentMap, Hero, _world);
                            TheArtist.Paint(Palettes.Hero, Hero.Coordinates, Block.EmptySpace);
                        }
                        else
                        {
                            ActionMenu.Status = "Your spell fizzles";
                            ActionMenu.RenderMenu(Hero, _world);
                        }
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
            ActionMenu.Status = String.Empty;
            var heroCoordinate = new Coordinate()
            {
                X = Hero.Coordinates.X + x,
                Y = Hero.Coordinates.Y + y
            };

            if (MapHandler.IsMapExit(heroCoordinate.X, heroCoordinate.Y, _currentMap))
            {
                var entryDirection = Direction.South;
                //If X is 0 we go west
                if (heroCoordinate.X == 0)
                {
                    entryDirection = Direction.East;
                    heroCoordinate.X = MapWidth - 1;
                }
                //If Y is 0 we go North
                if (heroCoordinate.Y == 0)
                {
                    entryDirection = Direction.South;
                    heroCoordinate.Y = MapHeight - 1;
                }
                //If x is the width of the map we go east
                if (heroCoordinate.X == _currentMap.MapWidth)
                {
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
                            TheCartographer.DrawGame(_currentMap, Hero, _world);
                        }
                        else
                        {
                            GenerateNextMap(coordinate);
                        }
                        break;
                    case Direction.South:
                        coordinate = new Coordinate(_world.WhereAmI.X, _world.WhereAmI.Y + 1);
                        if (_world.MapGrid.ContainsKey(coordinate))
                        {
                            _currentMap = _world.MapGrid[coordinate];
                            TheCartographer.DrawGame(_currentMap, Hero, _world);
                        }
                        else
                        {
                            GenerateNextMap(coordinate);
                        }
                        break;
                    case Direction.East:
                        coordinate = new Coordinate(_world.WhereAmI.X + 1, _world.WhereAmI.Y);
                        if (_world.MapGrid.ContainsKey(coordinate))
                        {
                            _currentMap = _world.MapGrid[coordinate];
                            TheCartographer.DrawGame(_currentMap, Hero, _world);
                        }
                        else
                        {
                            GenerateNextMap(coordinate);
                        }
                        break;
                    case Direction.West:
                        coordinate = new Coordinate(_world.WhereAmI.X - 1, _world.WhereAmI.Y);
                        if (_world.MapGrid.ContainsKey(coordinate))
                        {
                            _currentMap = _world.MapGrid[coordinate];
                            TheCartographer.DrawGame(_currentMap, Hero, _world);
                        }
                        else
                        {
                            GenerateNextMap(coordinate);
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

            var actionBlock = _currentMap.GetActionBlock(heroCoordinate);
            if (actionBlock != null && actionBlock.Block == Block.Interactive)
                ActionMenu.Status = "INTERACTIVE BLOCK!";
            
            ActionMenu.RenderMenu(Hero, _world);
        }

        /// <summary>
        /// Generates the next map and sets it to current map.
        /// </summary>
        /// <param name="coordinate">Co-ordinates where the new map should be placed</param>
        /// <param name="exitDirection">Where does the hero come from</param>
        private static void GenerateNextMap(Coordinate coordinate)
        {
            var nextmap = MapHandler.CreateMap(MapWidth, MapHeight, 40, RandomBlockConfiguration);
            MapHandler.GenerateRandomExitDirection(nextmap, 1);
            var adjacentMaps = _world.GetDestinationsAdjacentMaps(coordinate);
            TheCartographer.CloneExitsAndDrawThisMapPlease(nextmap, adjacentMaps, Hero, _world);
            _world.MapGrid.Add(coordinate, nextmap);
            var houseSpot = MapHandler.GetValidStartLocation(MagicNumberHat.Random.Next(10, 17), MagicNumberHat.Random.Next(12, 17), nextmap);
            TheArtist.DrawHouse(new Coordinate(houseSpot[0], houseSpot[1]),nextmap);
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
                    MapHandler.ClearBlock(coordinate, map);
                    Hero.Coins++;
                    break;
                case Block.Teleport:
                    var teleportPath = AppDomain.CurrentDomain.BaseDirectory + "audio\\" + Block.Teleport + ".wav";
                    var teleportPlayer = new System.Media.SoundPlayer(teleportPath);
                    teleportPlayer.Play();
                    //Now we know where to move the bastard
                    var tp = map.ActionBlocks.FirstOrDefault(a => a.Block == Block.Teleport && !a.Coordinate.Equals(coordinate));
                    if (tp != null)
                    {
                        RemoveHero();
                        TheArtist.Paint(Palettes.Hero, tp.Coordinate, Block.EmptySpace);
                        Hero.Coordinates = tp.Coordinate;
                    }
                    break;
            }

            var heroTeleporter = map.ActionBlocks.FirstOrDefault(a => a.Block == Block.Teleport && a.Coordinate.Equals(Hero.PreviousCoordinate));
            if (heroTeleporter != null)
            {
                TheArtist.Paint(heroTeleporter);
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
            var actionBlock = map.ActionBlocks.FirstOrDefault(x => x.Coordinate.Equals(c));
            if (actionBlock != null && (actionBlock.Block == Block.Occupied || actionBlock.Block == Block.Interactive))
                return false;

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
        static void InitGame(IReadOnlyList<int> startPosition = null)
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
