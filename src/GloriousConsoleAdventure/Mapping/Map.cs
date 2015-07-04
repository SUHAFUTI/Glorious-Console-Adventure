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
using GloriousConsoleAdventure.Enums;
using GloriousConsoleAdventure.Helpers;

namespace GloriousConsoleAdventure.Mapping
{
    public class MapHandler
    {
        private readonly Random _rand = MagicNumberHat.Random;
        public Dictionary<Direction, Guid> AdjacentMaps { get; set; }
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public int PercentAreWalls { get; set; }
        public Guid Id { get; set; }
        public Block[,] Map;

        /// <summary>
        /// Handles map related stuff
        /// </summary>
        /// <param name="mapWidth">Width of the map</param>
        /// <param name="mapHeight">Height of the map</param>
        /// <param name="percentWalls">How much of the map should be walls</param>
        /// <param name="randomBlocks">List of random blocks to include</param>
        public MapHandler(int mapWidth, int mapHeight, int percentWalls = 40, List<Block> randomBlocks = null)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            PercentAreWalls = percentWalls;
            AdjacentMaps = new Dictionary<Direction, Guid>();
            Id = Guid.NewGuid();
            Map = new Block[MapWidth, MapHeight];
            RandomFillMap();
            MakeCaverns();
            if (randomBlocks != null)
            {
                randomBlocks.ForEach(PlaceRandomBlock);
            }
        }
        /// <summary>
        /// Creates cavarns on current map
        /// </summary>
        public void MakeCaverns()
        {
            // By initilizing column in the outter loop, its only created ONCE
            for (int column = 0, row = 0; row <= MapHeight - 1; row++)
            {
                for (column = 0; column <= MapWidth - 1; column++)
                {
                    Map[column, row] = PlaceWallLogic(column, row);
                }
            }
        }
        /// <summary>
        /// Places walls based on the 4/5 rule
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns></returns>
        public Block PlaceWallLogic(int x, int y)
        {
            int numWalls = GetAdjacentWalls(x, y, 1, 1);


            if (Map[x, y] == Block.Wall)
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
        public void CloneExit(Block[,] exittingMap, Direction exitDirection)
        {
            switch (exitDirection)
            {
                case Direction.North:
                    for (int i = 0; i < MapWidth - 1; i++)
                    {
                        Map[i, MapHeight - 1] = exittingMap[i, 1];
                        if (Map[i, MapHeight - 1] == Block.EmptySpace)
                        {
                            //if the block above is a wall, remove it to make way, should perhaps check one or two tiles more
                            if (Map[i, MapHeight - 2] == Block.Wall)
                                Map[i, MapHeight - 2] = Block.EmptySpace;

                        }
                    }
                    break;
                case Direction.South:
                    for (int i = 0; i < MapWidth; i++)
                    {
                        Map[i, 1] = exittingMap[i, MapHeight - 1];
                        //if the block below is a wall, remove it to make way, should perhaps check one or two tiles more
                        if (Map[i, 1] == Block.EmptySpace)
                        {
                            if (Map[i, 2] == Block.Wall)
                                Map[i, 2] = Block.EmptySpace;

                        }
                    }
                    break;
                case Direction.East:
                    //Traverse the column
                    for (var y = 0; y < MapHeight; y++)
                    {
                        //Map exit from exitmap
                        Map[1, y] = exittingMap[MapWidth - 1, y];
                        //if the block to the left is a wall, remove it to make way, should perhaps check one or two tiles more
                        if (Map[1, y] == Block.EmptySpace)
                        {
                            if (Map[2, y] == Block.Wall)
                                Map[2, y] = Block.EmptySpace;
                        }
                    }
                    break;
                case Direction.West:
                    //Traverse the column
                    for (var y = 0; y < MapHeight; y++)
                    {
                        //Map exit from exitmap
                        Map[MapWidth -1, y] = exittingMap[1 - 1, y];
                        //if the block to the left is a wall, remove it to make way, should perhaps check one or two tiles more
                        if (Map[MapWidth- 1, y] == Block.EmptySpace)
                        {
                            if (Map[MapWidth -2, y] == Block.Wall)
                                Map[MapWidth -2, y] = Block.EmptySpace;
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// Traverses map in a given direction and makes an exit at first possible empty tile.
        /// </summary>
        /// <param name="direction">Which direction the exit should appear</param>
        public void GenerateExit(Direction direction)
        {
            var foundExit = false;
            switch (direction)
            {
                case Direction.North:
                    //Traverse rows one by one
                    for (var y = 0; y < MapHeight; y++)
                    {
                        //and do it untill we find an exit
                        if (foundExit) break;
                        //Check every coordinate in that row
                        for (var x = 0; x < MapWidth - 1; x++)
                        {
                            //We only want one exit?
                            if (foundExit) break;
                            //If its empty 
                            if (Map[x, y] == Block.EmptySpace)
                            {
                                //Carve exit upwards
                                for (var i = y; i >= 0; i--)
                                {
                                    Map[x, i] = Block.EmptySpace;
                                }
                                //Let the column loop know we found an exit
                                foundExit = true;
                            }
                        }
                    }
                    break;
                case Direction.South:
                    //Traverse rows one by one
                    for (var y = MapHeight - 1; y > 0; y--)
                    {
                        //and do it untill we find an exit
                        if (foundExit) break;
                        //Check every coordinate in that row
                        for (var x = 0; x < MapWidth - 1; x++)
                        {
                            //We only want one exit?
                            if (foundExit) break;
                            //If its empty 
                            if (Map[x, y] == Block.EmptySpace)
                            {
                                //Carve exit downwards
                                for (var i = y; i < MapHeight; i++)
                                {
                                    Map[x, i] = Block.EmptySpace;
                                }
                                //Let the column loop know we found an exit
                                foundExit = true;
                            }
                        }
                    }
                    break;
                case Direction.West:
                    //Traverse columns one by one starting from left
                    for (var x = 0; x < MapWidth; x++)
                    {
                        //and do it untill we find an exit
                        if (foundExit) break;
                        //Check every coordinate in that column
                        for (var y = 0; y < MapHeight - 1; y++)
                        {
                            //We only want one exit?
                            if (foundExit) break;
                            //If its empty 
                            if (Map[x, y] == Block.EmptySpace)
                            {
                                //Carve exit left
                                for (var i = x; i >= 0; i--)
                                {
                                    Map[i, y] = Block.EmptySpace;
                                }
                                //Let the column loop know we found an exit
                                foundExit = true;
                            }
                        }
                    }
                    break;
                case Direction.East:
                    //Traverse columns one by one starting from the right
                    for (var x = MapWidth - 1; x < MapWidth; x--)
                    {
                        //and do it untill we find an exit
                        if (foundExit) break;
                        //Check every coordinate in that column
                        for (var y = 0; y < MapHeight - 1; y++)
                        {
                            //We only want one exit?
                            if (foundExit) break;
                            //If its empty 
                            if (Map[x, y] == Block.EmptySpace)
                            {
                                //Carve exit right
                                for (var i = x; i <= MapWidth - 1; i++)
                                {
                                    Map[i, y] = Block.EmptySpace;
                                }
                                //Let the column loop know we found an exit
                                foundExit = true;
                            }
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// Generate random exits on map
        /// </summary>
        /// <param name="exits">Amount of exits</param>
        public void GenerateRandomExits(int exits)
        {
            for (var i = 0; i == exits; i++)
            {
                var values = Enum.GetValues(typeof(Direction));
                var exit = (Direction)values.GetValue(MagicNumberHat.Random.Next(values.Length));
                GenerateExit(exit);
            }
        }
        /// <summary>
        /// Places a random block on the map
        /// </summary>
        /// <param name="block">Takes a block</param>
        public void PlaceRandomBlock(Block block)
        {
            var randX = _rand.Next(1, MapWidth);
            var randY = _rand.Next(1, MapHeight);
            while (IsWall(randX, randY))
            {
                randX = _rand.Next(1, MapWidth);
                randY = _rand.Next(1, MapHeight);
            }

            Map[randX, randY] = block;
        }
        /// <summary>
        /// Returns how many walls are near a coordinate with given scope
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="scopeX">scope x direction</param>
        /// <param name="scopeY">scope y direction</param>
        /// <returns>amount of walls</returns>
        public int GetAdjacentWalls(int x, int y, int scopeX, int scopeY)
        {
            int startX = x - scopeX;
            int startY = y - scopeY;
            int endX = x + scopeX;
            int endY = y + scopeY;

            int iX = startX;
            int iY = startY;

            int wallCounter = 0;

            for (iY = startY; iY <= endY; iY++)
            {
                for (iX = startX; iX <= endX; iX++)
                {
                    if (!(iX == x && iY == y))
                    {
                        if (IsWall(iX, iY))
                        {
                            wallCounter += 1;
                        }
                    }
                }
            }
            return wallCounter;
        }
        /// <summary>
        /// Get blocktype at coordinate
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns>blocktype</returns>
        public Block GetCurrentBlock(int x, int y)
        {
            return Map[x, y];
        }
        /// <summary>
        /// Sets the map cordinates to EmptySpace
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ClearBlock(int x, int y)
        {
            Map[x, y] = Block.EmptySpace;
        }
        /// <summary>
        /// Checks if given coordinates is a wall
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coodinate</param>
        /// <returns>true if wall</returns>
        public bool IsWall(int x, int y)
        {
            // Consider out-of-bound a wall
            if (IsOutOfBounds(x, y))
            {
                return true;
            }

            if (Map[x, y] == Block.Wall)
            {
                return true;
            }

            if (Map[x, y] == Block.EmptySpace)
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
        /// <returns></returns>
        public bool IsMapExit(int x, int y)
        {
            if (y == 0 || y == MapHeight || x == 0 || x == MapWidth)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Checks if coordinates is out of bounds
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coorindate</param>
        /// <returns>true if out of bounds</returns>
        bool IsOutOfBounds(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return true;
            }
            else if (x > MapWidth - 1 || y > MapHeight - 1)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Generates a blank map
        /// </summary>
        public void BlankMap()
        {
            for (int column = 0, row = 0; row < MapHeight; row++)
            {
                for (column = 0; column < MapWidth; column++)
                {
                    Map[column, row] = Block.EmptySpace;
                }
            }
        }
        /// <summary>
        /// Randomly fills map
        /// </summary>
        public void RandomFillMap()
        {
            // New, empty map
            Map = new Block[MapWidth, MapHeight];

            int mapMiddle = 0; // Temp variable
            for (int column = 0, row = 0; row < MapHeight; row++)
            {
                //Border creation
                for (column = 0; column < MapWidth; column++)
                {
                    if (column == 0)
                    {
                        Map[column, row] = Block.Wall;
                    }
                    else if (row == 0)
                    {
                        Map[column, row] = Block.Wall;
                    }
                    else if (column == MapWidth - 1)
                    {
                        Map[column, row] = Block.Wall;
                    }
                    else if (row == MapHeight - 1)
                    {
                        Map[column, row] = Block.Wall;
                    }
                    // Else, fill with a wall a random percent of the time
                    else
                    {
                        mapMiddle = (MapHeight / 2);

                        if (row == mapMiddle)
                        {
                            Map[column, row] = 0;
                        }
                        else
                        {
                            Map[column, row] = RandomPercent(PercentAreWalls);
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
        /// <returns>Coordinate of first valid position</returns>
        public int[] GetValidStartLocation(int x, int y)
        {
            for (int column = y, row = x; row < MapHeight; row++)
            {
                for (column = y; column < MapWidth; column++)
                {
                    if (Map[column, row] == 0) return new[] { column, row };
                }
            }
            return null;

        }
    }
}
