using GloriousConsoleAdventure.Color;
using GloriousConsoleAdventure.Enums;
using GloriousConsoleAdventure.Helpers;
using GloriousConsoleAdventure.Mapping;
using GloriousConsoleAdventure.Menu;
using GloriousConsoleAdventure.Models.Hero;
using GloriousConsoleAdventure.Models.MapModels;
using GloriousConsoleAdventureCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloriousConsoleAdventureCore
{
    public class GameEngine
    {
        private MapHandler _mapHandler;
        private Map _currentMap;
        private Hero _hero;
        private World _world;
        private ActionMenu _actionMenu;
        private TheArtist _artist;
        private SoundPlayer _soundPlayer;

        public GameEngine(TheArtist artist, SoundPlayer soundPlayer, MapHandler mapHandler, ActionMenu actionMenu)
        {
            _artist = artist;
            _soundPlayer = soundPlayer;
            _mapHandler = mapHandler;
            _actionMenu = actionMenu;
            
        }
        public const int MapHeight = 60;
        public const int MapWidth = 80;
        private static readonly List<Block> RandomBlockConfiguration = new List<Block>
        {
            Block.Coin,
            Block.Teleport,
            Block.Teleport
        };
        

        public void Game()
        {
            Console.CursorVisible = false;
            Console.OutputEncoding = Encoding.UTF8;
            
            //Init hero
            _hero = new Hero("Herald Grimrian");
            _hero.Dynamite = 5;
            //Height has to be 1 more than map
            Console.SetWindowSize(80, 31);

            //Create map
            _currentMap = new Map(MapWidth, MapHeight, 40, RandomBlockConfiguration);
            //Init new world from 0,0
            _world = new World { WhereAmI = new Coordinate(0, 0), MapGrid = new Dictionary<Coordinate, Map>() };
            _world.MapGrid.Add(new Coordinate(0, 0), _currentMap);

            //Generate one exit in each direction
            foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>())
            {
                _currentMap.GenerateExit(direction);
            }

            //Draw the map
            _artist.DrawGame(_currentMap, _hero, _world);
            //Let's get it on!
            InitGame(_mapHandler.GetValidStartLocation(15, 15, _currentMap));

            //Main loop. Check if arrow keys are pressed and move accordingly.
            ConsoleKeyInfo keyInfo;
            while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Escape)
            {
                System.Threading.Thread.Sleep(30); //Low tech game tick
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
                        if (_hero.Dynamite > 0)
                        {
                            _soundPlayer.PlaySound("explosion");
                            _actionMenu.SetStatus("Kaboom");
                            _hero.Dynamite--;
                            var blastCross = _currentMap.BlastCross(_hero.Coordinates);
                            blastCross.ForEach(x => _artist.Paint(Palettes.Cave, x, Block.EmptySpace));
                            _artist.Paint(Palettes.Hero, _hero.Coordinates, Block.Hero);
                        }
                        else
                        {
                            _actionMenu.SetStatus("Your spell fizzles");
                            _actionMenu.RenderMenu(_hero, _world);
                        }
                        break;
                }
            }
        }
        private void MoveHero(int x, int y)
        {
            _actionMenu.ResetStatus();
            var heroCoordinate = new Coordinate()
            {
                X = _hero.Coordinates.X + x,
                Y = _hero.Coordinates.Y + y
            };

            if (_currentMap.IsMapExit(heroCoordinate))
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
                        var isChildMap = _currentMap.ParentMap != null && _world.MapGrid.ContainsKey(_currentMap.ParentMap);
                        coordinate = isChildMap ? _currentMap.ParentMap : new Coordinate(_world.WhereAmI.X, _world.WhereAmI.Y - 1);
                        if (_world.MapGrid.ContainsKey(coordinate))
                        {
                            if (isChildMap)
                            {
                                //move hero to house entrance.
                            }
                            _currentMap = _world.MapGrid[coordinate];
                            _artist.DrawGame(_currentMap, _hero, _world);
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
                            _artist.DrawGame(_currentMap, _hero, _world);
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
                            _artist.DrawGame(_currentMap, _hero, _world);
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
                            _artist.DrawGame(_currentMap, _hero, _world);
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
                _artist.Paint(Palettes.Hero, heroCoordinate, Block.Hero);
                _hero.Coordinates = heroCoordinate;
                _hero.Steps++;
            }

            if (CanMove(heroCoordinate, _currentMap))
            {
                RemoveHero();
                _artist.Paint(Palettes.Hero, heroCoordinate, Block.Hero);
                _hero.Coordinates = heroCoordinate;
                BlockAction(heroCoordinate, _currentMap);
                _hero.Steps++;
            }

            var actionBlock = _currentMap.GetActionBlock(heroCoordinate);
            if (actionBlock != null && actionBlock.Block == Block.Interactive)
            {
                var house = _mapHandler.CreateMap(10, 5, 0, null, Palettes.Grass);
                house.ParentMap = _world.WhereAmI;
                _mapHandler.GenerateExit(Direction.South, house);
                _hero.Coordinates = house.Exits.First().Value;
                _currentMap.MapStructures.Add(house);
                _artist.CloneExitsAndDrawThisMapPlease(house, new Dictionary<Direction, Map>(), _hero, _world);
                _currentMap = house;
                _actionMenu.SetStatus("INTERACTIVE BLOCK!");
            }

            _actionMenu.RenderMenu(_hero, _world);
        }

        /// <summary>
        /// Generates the next map and sets it to current map.
        /// </summary>
        /// <param name="coordinate">Co-ordinates where the new map should be placed</param>
        /// <param name="exitDirection">Where does the hero come from</param>
        private void GenerateNextMap(Coordinate coordinate)
        {
            var nextmap = new Map(MapWidth, MapHeight, 40, RandomBlockConfiguration);
            var adjacentMaps = _world.GetDestinationsAdjacentMaps(coordinate);
            _artist.CloneExitsAndDrawThisMapPlease(nextmap, adjacentMaps, _hero, _world);
            _world.MapGrid.Add(coordinate, nextmap);
            var houseSpot = _mapHandler.GetValidStartLocation(MagicNumberHat.Random.Next(10, 17), MagicNumberHat.Random.Next(12, 17), nextmap);
            _artist.DrawHouse(new Coordinate(houseSpot[0], houseSpot[1]), nextmap);
            //Set current map to the next
            _currentMap = nextmap;
        }

        /// <summary>
        /// This is a method that checks if a block is hit and what action to run
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="map">Map to run blockaction on</param>
        private void BlockAction(Coordinate coordinate, Map map)
        {
            var block = _mapHandler.GetCurrentBlock(coordinate.X, coordinate.Y, map);

            switch (block)
            {
                case Block.Coin:

                    _soundPlayer.PlaySound("coin");
                    _mapHandler.ClearBlock(coordinate, map);
                    _hero.Coins++;
                    break;
                case Block.Teleport:
                    _soundPlayer.PlaySound("teleport");
                    //Now we know where to move the bastard
                    var tp = map.ActionBlocks.FirstOrDefault(a => a.Block == Block.Teleport && !a.Coordinate.Equals(coordinate));
                    if (tp != null)
                    {
                        RemoveHero();
                        _artist.Paint(Palettes.Hero, tp.Coordinate, Block.Hero);
                        _hero.Coordinates = tp.Coordinate;
                    }
                    break;
            }

            var heroTeleporter = map.ActionBlocks.FirstOrDefault(a => a.Block == Block.Teleport && a.Coordinate.Equals(_hero.PreviousCoordinate));
            if (heroTeleporter != null)
            {
                _artist.Paint(heroTeleporter);
            }
        }

        /// <summary>
        /// Overpaint the old hero
        /// </summary>
        public void RemoveHero()
        {
            TheArtist.Delete(_hero.Coordinates);
        }

        /// <summary>
        /// Make sure that the new coordinate is not placed outside the
        /// console window (since that will cause a runtime crash
        /// </summary>
        public bool CanMove(Coordinate c, Map map)
        {
            var actionBlock = map.ActionBlocks.FirstOrDefault(x => x.Coordinate.Equals(c));
            if (actionBlock != null && (actionBlock.Block == Block.Occupied || actionBlock.Block == Block.Interactive))
                return false;

            if (_mapHandler.IsWall(c.X, c.Y, map)) return false;
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
        private void InitGame(IReadOnlyList<int> startPosition = null)
        {
            //If we don't have a start position default to 0,0
            if (startPosition == null)
            {
                _hero.Coordinates = new Coordinate()
                {
                    X = 0,
                    Y = 0
                };
            }
            //otherwise give Hero default coordinates
            else
            {
                _hero.Coordinates = new Coordinate
                {
                    X = startPosition[0],
                    Y = startPosition[1]
                };
            }

            MoveHero(0, 0);
            _actionMenu.RenderMenu(_hero, _world);

        }

    }
}
