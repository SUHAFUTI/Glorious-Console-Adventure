/* * Dungeon generation 
 * Based on poc made by Adam Rakaska 
 *  http://www.csharpprogramming.tips
 *    http://www.adam-rakaska.codes
 * Original mapping   
 * http://www.csharpprogramming.tips/2013/07/Rouge-like-dungeon-generation.html
 * Article regarding mapmaking
 * http://www.roguebasin.com/index.php?title=Cellular_Automata_Method_for_Generating_Random_Cave-Like_Levels
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using GloriousConsoleAdventure.Color;
using GloriousConsoleAdventure.Enums;
using GloriousConsoleAdventure.Helpers;
using GloriousConsoleAdventure.Models.MapModels;

namespace GloriousConsoleAdventure.Mapping
{
    public class MapHandler
    {
        private readonly Random _rand = MagicNumberHat.Random;

        public Palettes MapPalette { get; set; }
        /// <summary>
        /// Creates a new map
        /// </summary>
        /// <param name="mapWidth">Width</param>
        /// <param name="mapHeight">Height</param>
        /// <param name="percentWalls">Percent walls</param>
        /// <param name="randomBlocks">Randomblocks to include, default is null</param>
        /// <param name="mapPalette">Palette to use</param>
        /// <returns>Generated map</returns>
        public Map CreateMap(int mapWidth, int mapHeight, int percentWalls = 40, List<Block> randomBlocks = null, Palettes mapPalette = Palettes.Cave)
        {
            var map = new Map
            {
                Id = new Guid(),
                MapHeight = mapHeight,
                MapWidth = mapWidth,
                MapPalette = mapPalette,
                ActionBlocks = new List<BlockTile>()
            };
            RandomFillMap(map, percentWalls);
            MakeCaverns(map);
            if (randomBlocks != null)
            {
                foreach (var randomBlock in randomBlocks)
                {
                    PlaceRandomBlock(randomBlock, map);
                }
            }
            return map;
        }

        /// <summary>
        /// Creates cavarns on a given map
        /// </summary>
        /// <param name="map">Map to make caverns on</param>
        public void MakeCaverns(Map map)
        {
            // By initilizing column in the outter loop, its only created ONCE
            for (int column = 0, row = 0; row <= map.MapHeight - 1; row++)
            {
                for (column = 0; column <= map.MapWidth - 1; column++)
                {
                    map.MapBlocks[column, row] = PlaceWallLogic(column, row, map);
                }
            }
        }

        /// <summary>
        /// Places walls based on the 4/5 rule
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="map">Map to run the rules on</param>
        /// <returns>Which block to place</returns>
        public Block PlaceWallLogic(int x, int y, Map map)
        {
            int numWalls = GetAdjacentBlocks(x, y, 1, 1, Block.Wall, map);


            if (map.MapBlocks[x, y] == Block.Wall)
            {
                if (numWalls >= 4)
                {
                    return Block.Wall;
                }
                if (numWalls < 2)
                {
                    return Block.EmptySpace;
                }

            }
            else
            {
                if (numWalls >= 5)
                {
                    return Block.Wall;
                }
            }
            return Block.EmptySpace;
        }

        /// <summary>
        /// Clone an exit from an existing map to a new one for consistency between an exit and an entrance
        /// </summary>
        /// <param name="exittingMap">Map to clone from</param>
        /// <param name="exitDirection">Direction from which we came</param>
        /// <param name="map">Map to update</param>
        public static void CloneExit(Map exittingMap, Direction exitDirection, Map map)
        {
            switch (exitDirection)
            {
                case Direction.North:
                    for (int x = 0; x < map.MapWidth - 1; x++)
                    {
                        map.MapBlocks[x, map.MapHeight - 1] = exittingMap.MapBlocks[x, 0];
                        if (map.MapBlocks[x, map.MapHeight - 1] == Block.EmptySpace)
                        {
                            //Register exit in new map, is in opposite direction.
                            if (!map.Exits.ContainsKey(Direction.South))
                                map.Exits.Add(Direction.South, new Coordinate(x, map.MapHeight - 1));

                            var b = 2;
                            //Keep excavating until we hit EmptySpace to ensure an exit
                            //TODO: Needs to be smarter, sometimes hits a 1,1 hole and is happy. Not a real exit
                            while (map.MapBlocks[x, map.MapHeight - b] == Block.Wall)
                            {
                                map.MapBlocks[x, map.MapHeight - b++] = Block.EmptySpace;
                            }
                        }
                    }
                    break;
                case Direction.South:
                    for (int x = 0; x < map.MapWidth; x++)
                    {
                        map.MapBlocks[x, 0] = exittingMap.MapBlocks[x, map.MapHeight - 1];
                        //if the block below is a wall, remove it to make way, should perhaps check one or two tiles more
                        if (map.MapBlocks[x, 0] == Block.EmptySpace)
                        {
                            //Register exit in new map, is in opposite direction.
                            if (!map.Exits.ContainsKey(Direction.North))
                                map.Exits.Add(Direction.North, new Coordinate(x, 0));

                            var b = 1;
                            //Keep excavating until we hit EmptySpace to ensure an exit
                            //TODO: Needs to be smarter, sometimes hits a 1,1 hole and is happy. Not a real exit
                            while (map.MapBlocks[x, b] == Block.Wall)
                            {
                                map.MapBlocks[x, b++] = Block.EmptySpace;
                            }
                        }
                    }
                    break;
                case Direction.East:
                    //Traverse the column
                    for (var y = 0; y < map.MapHeight; y++)
                    {
                        //Map exit from exitmap
                        map.MapBlocks[0, y] = exittingMap.MapBlocks[map.MapWidth - 1, y];
                        //if the block to the left is a wall, remove it to make way, should perhaps check one or two tiles more
                        if (map.MapBlocks[0, y] == Block.EmptySpace)
                        {
                            //Register exit in new map, is in opposite direction.
                            if (!map.Exits.ContainsKey(Direction.West))
                                map.Exits.Add(Direction.West, new Coordinate(0, y));

                            var b = 1;
                            //Keep excavating until we hit EmptySpace to ensure an exit
                            //TODO: Needs to be smarter, sometimes hits a 1,1 hole and is happy. Not a real exit
                            while (map.MapBlocks[b, y] == Block.Wall)
                            {
                                map.MapBlocks[b++, y] = Block.EmptySpace;
                            }
                        }
                    }
                    break;
                case Direction.West:
                    //Traverse the column
                    for (var y = 0; y < map.MapHeight; y++)
                    {
                        //Map exit from exitmap
                        map.MapBlocks[map.MapWidth - 1, y] = exittingMap.MapBlocks[0, y];
                        //if the block to the left is a wall, remove it to make way, should perhaps check one or two tiles more
                        if (map.MapBlocks[map.MapWidth - 1, y] == Block.EmptySpace)
                        {
                            //Register exit in new map, is in opposite direction.
                            if (!map.Exits.ContainsKey(Direction.East))
                                map.Exits.Add(Direction.East, new Coordinate(map.MapWidth - 1, y));

                            var b = 2;
                            //Keep excavating until we hit EmptySpace to ensure an exit
                            //TODO: Needs to be smarter, sometimes hits a 1,1 hole and is happy. Not a real exit
                            while (map.MapBlocks[map.MapWidth - b, y] == Block.Wall)
                            {
                                map.MapBlocks[map.MapWidth - b++, y] = Block.EmptySpace;
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Traverses map in a given direction and makes an exit at first possible empty tile.
        /// </summary>
        /// <param name="direction">Which direction the exit should appear</param>
        /// <param name="map">Map to generate exit on</param>
        public static void GenerateExit(Direction direction, Map map)
        {
            var foundExit = false;
            switch (direction)
            {
                case Direction.North:
                    //Traverse rows one by one
                    for (var y = 0; y < map.MapHeight; y++)
                    {
                        //and do it untill we find an exit
                        if (foundExit) break;
                        //Check every coordinate in that row
                        for (var x = 0; x < map.MapWidth - 1; x++)
                        {
                            //We only want one exit?
                            if (foundExit) break;
                            //If its empty 
                            if (map.MapBlocks[x, y] == Block.EmptySpace)
                            {
                                //Carve exit upwards
                                for (var i = y; i >= 0; i--)
                                {
                                    map.MapBlocks[x, i] = Block.EmptySpace;
                                }
                                //Let the column loop know we found an exit
                                foundExit = true;
                                map.Exits.Add(direction, new Coordinate(x, y));
                            }
                        }
                    }
                    break;
                case Direction.South:
                    //Traverse rows one by one
                    for (var y = map.MapHeight - 1; y > 0; y--)
                    {
                        //and do it untill we find an exit
                        if (foundExit) break;
                        //Check every coordinate in that row
                        for (var x = 0; x < map.MapWidth - 1; x++)
                        {
                            //We only want one exit?
                            if (foundExit) break;
                            //If its empty 
                            if (map.MapBlocks[x, y] == Block.EmptySpace)
                            {
                                //Carve exit downwards
                                for (var i = y; i < map.MapHeight; i++)
                                {
                                    map.MapBlocks[x, i] = Block.EmptySpace;
                                }
                                //Let the column loop know we found an exit
                                foundExit = true;
                                map.Exits.Add(direction, new Coordinate(x, y));
                            }
                        }
                    }
                    break;
                case Direction.West:
                    //Traverse columns one by one starting from left
                    for (var x = 0; x < map.MapWidth; x++)
                    {
                        //and do it untill we find an exit
                        if (foundExit) break;
                        //Check every coordinate in that column
                        for (var y = 0; y < map.MapHeight - 1; y++)
                        {
                            //We only want one exit?
                            if (foundExit) break;
                            //If its empty 
                            if (map.MapBlocks[x, y] == Block.EmptySpace)
                            {
                                //Carve exit left
                                for (var i = x; i >= 0; i--)
                                {
                                    map.MapBlocks[i, y] = Block.EmptySpace;
                                }
                                //Let the column loop know we found an exit
                                foundExit = true;
                                map.Exits.Add(direction, new Coordinate(x, y));
                            }
                        }
                    }
                    break;
                case Direction.East:
                    //Traverse columns one by one starting from the right
                    for (var x = map.MapWidth - 1; x < map.MapWidth; x--)
                    {
                        //and do it untill we find an exit
                        if (foundExit) break;
                        //Check every coordinate in that column
                        for (var y = 0; y < map.MapHeight - 1; y++)
                        {
                            //We only want one exit?
                            if (foundExit) break;
                            //If its empty 
                            if (map.MapBlocks[x, y] == Block.EmptySpace)
                            {
                                //Carve exit right
                                for (var i = x; i <= map.MapWidth - 1; i++)
                                {
                                    map.MapBlocks[i, y] = Block.EmptySpace;
                                }
                                //Let the column loop know we found an exit
                                foundExit = true;
                                map.Exits.Add(direction, new Coordinate(x, y));
                            }
                        }
                    }
                    break;
            }
        }

        public static void BlastCrossBombermanStyle(Coordinate coordinate, Map map)
        {
            map.MapBlocks[coordinate.X, coordinate.Y + 1] = Block.EmptySpace;
            map.MapBlocks[coordinate.X, coordinate.Y - 1] = Block.EmptySpace;
            map.MapBlocks[coordinate.X + 1, coordinate.Y] = Block.EmptySpace;
            map.MapBlocks[coordinate.X - 1, coordinate.Y] = Block.EmptySpace;
        }

        /// <summary>
        /// Generate random exits on map
        /// </summary>
        /// <param name="map">Map to generate exits on</param>
        /// <param name="exits">Amount of exits</param>
        public static void GenerateRandomExitDirection(Map map, int exits)
        {
            var usedExits = new List<Direction>();
            var values = Enum.GetValues(typeof(Direction));
            Direction exit = Direction.North;
            var i = 0;

            while (i < exits)
            {
                do
                {
                    exit = (Direction)values.GetValue(MagicNumberHat.Random.Next(values.Length));

                } while (usedExits.Contains(exit));
                GenerateExit(exit, map);
                i++;
            }


        }

        /// <summary>
        /// Places a random block on the map
        /// </summary>
        /// <param name="block">Blocktype to place</param>
        /// <param name="map">Map to place block in</param>
        /// <returns>Updated map</returns>
        public void PlaceRandomBlock(Block block, Map map)
        {
            var randX = _rand.Next(1, map.MapWidth);
            var randY = _rand.Next(1, map.MapHeight);
            while (IsWall(randX, randY, map))
            {
                randX = _rand.Next(1, map.MapWidth);
                randY = _rand.Next(1, map.MapHeight);
            }

            map.MapBlocks[randX, randY] = block;
            Palettes palette;
            Enum.TryParse(block.ToString(), out palette);

            map.ActionBlocks.Add(new BlockTile
            {
                Block = block,
                Coordinate = new Coordinate { X = randX, Y = randY },
                Palette = palette
            });
        }

        /// <summary>
        /// Returns how blocks of given type is within given scope of x,y
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="scopeX">scope x direction</param>
        /// <param name="scopeY">scope y direction</param>
        /// <param name="block">blocktype to check for</param>
        /// <param name="map">map to check</param>
        /// <returns>amount of adjacent blocks</returns>
        public int GetAdjacentBlocks(int x, int y, int scopeX, int scopeY, Block block, Map map)
        {
            var startX = x - scopeX;
            var startY = y - scopeY;
            var endX = x + scopeX;
            var endY = y + scopeY;

            var iX = startX;
            var iY = startY;

            var blockCounter = 0;

            for (iY = startY; iY <= endY; iY++)
            {
                for (iX = startX; iX <= endX; iX++)
                {
                    if (iX == x && iY == y) continue;
                    if (block == Block.Wall)
                    {
                        if (IsWall(iX, iY, map))
                        {
                            blockCounter++;
                        }
                    }
                    else
                    {
                        if (map.MapBlocks[iX, iY] == block)
                        {
                            blockCounter++;
                        }
                    }
                }
            }
            return blockCounter;
        }

        /// <summary>
        /// Get blocktype at coordinate
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="map">map to get block from</param>
        /// <returns>blocktype</returns>
        public Block GetCurrentBlock(int x, int y, Map map)
        {
            return map.MapBlocks[x, y];
        }

        /// <summary>
        /// Sets the map cordinates to EmptySpace
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="map">Map to set coordinates on</param>
        public void ClearBlock(Coordinate coordinate, Map map)
        {
            var removeBlock = map.ActionBlocks.First(b => b.Coordinate.Equals(coordinate));
            map.ActionBlocks.Remove(removeBlock);
            map.MapBlocks[coordinate.X, coordinate.Y] = Block.EmptySpace;
        }

        /// <summary>
        /// Checks if given coordinates is a wall
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coodinate</param>
        /// <param name="map">map to check</param>
        /// <returns>true if wall</returns>
        public bool IsWall(int x, int y, Map map)
        {
            // Consider out-of-bound a wall
            if (IsOutOfBounds(x, y, map))
            {
                return true;
            }

            if (map.MapBlocks[x, y] == Block.Wall)
            {
                return true;
            }

            if (map.MapBlocks[x, y] == Block.EmptySpace)
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Checks if given coordinates is a map exit
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coodinate</param>
        /// <param name="map">Map thecheck if given coordinate is exit</param>
        /// <returns></returns>
        public bool IsMapExit(int x, int y, Map map)
        {
            if ((y == 0 || y == map.MapHeight || x == 0 || x == map.MapWidth))
            {
                //Check if the block actually is empty before deeming it an exit
                //I'm not sure why this isn't an issue, with max values 
                if (y == 0 || x == 0)
                    return map.MapBlocks[x, y] == Block.EmptySpace;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if coordinates is out of bounds
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coorindate</param>
        /// <param name="map">map to check</param>
        /// <returns>true if out of bounds</returns>
        static bool IsOutOfBounds(int x, int y, Map map)
        {
            if (x < 0 || y < 0)
            {
                return true;
            }
            else if (x > map.MapWidth - 1 || y > map.MapHeight - 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Takes provided map and blanks it out
        /// </summary>
        /// <param name="map">Map to blank out</param>
        public void BlankMap(Map map)
        {
            for (int column = 0, row = 0; row < map.MapHeight; row++)
            {
                for (column = 0; column < map.MapWidth; column++)
                {
                    map.MapBlocks[column, row] = Block.EmptySpace;
                }
            }
        }

        /// <summary>
        /// Randomly fills map
        /// </summary>
        public void RandomFillMap(Map map, int wallPercent)
        {
            map.MapBlocks = new Block[map.MapWidth, map.MapHeight];

            int mapMiddle = 0; // Temp variable
            for (int column = 0, row = 0; row < map.MapHeight; row++)
            {
                //Border creation
                for (column = 0; column < map.MapWidth; column++)
                {
                    if (column == 0)
                    {
                        map.MapBlocks[column, row] = Block.Wall;
                    }
                    else if (row == 0)
                    {
                        map.MapBlocks[column, row] = Block.Wall;
                    }
                    else if (column == map.MapWidth - 1)
                    {
                        map.MapBlocks[column, row] = Block.Wall;
                    }
                    else if (row == map.MapHeight - 1)
                    {
                        map.MapBlocks[column, row] = Block.Wall;
                    }
                    // Else, fill with a wall a random percent of the time
                    else
                    {
                        mapMiddle = (map.MapHeight / 2);

                        if (row == mapMiddle)
                        {
                            map.MapBlocks[column, row] = 0;
                        }
                        else
                        {
                            map.MapBlocks[column, row] = RandomPercent(wallPercent);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns either a wall or empty with percent as seed
        /// </summary>
        /// <param name="percent">How high chance there should be for a wall</param>
        /// <returns>Either a wall or empty</returns>
        Block RandomPercent(int percent)
        {
            if (percent >= _rand.Next(1, 101))
            {
                return Block.Wall;
            }
            return Block.EmptySpace;
        }

        /// <summary>
        /// Used to get a valid start position. E.g. NOT in a wall! 
        /// </summary>
        /// <param name="x">x offset on where to start looking</param>
        /// <param name="y">y offset on where to start looking</param>
        /// <param name="map">map to find start location from</param>
        /// <returns>Coordinate of first valid position</returns>
        public int[] GetValidStartLocation(int x, int y, Map map)
        {
            for (int column = y, row = x; row < map.MapHeight; row++)
            {
                for (column = y; column < map.MapWidth; column++)
                {
                    if (map.MapBlocks[column, row] == 0) return new[] { column, row };
                }
            }
            return null;

        }
    }
}
