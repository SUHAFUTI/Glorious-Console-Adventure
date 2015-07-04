/*´MFEH: I totally stole this! 
 * 
 * Automata procedual dungeon generation proof-of-concept
 *
 *
 * Developed by Adam Rakaska 
 *  http://www.csharpprogramming.tips
 *    http://www.adam-rakaska.codes
 *    
 * http://www.csharpprogramming.tips/2013/07/Rouge-like-dungeon-generation.html
 * 
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

        public MapHandler(int mapWidth, int mapHeight, int percentWalls = 40, List<Block> randomBlocks = null)
        {
            this.MapWidth = mapWidth;
            this.MapHeight = mapHeight;
            this.PercentAreWalls = percentWalls;
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

        //TODO: East and West
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
                    break;
                case Direction.West:
                    break;
            }
        }

        public void GenerateExit(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    var empties = new List<int>();
                    for (int x = 0; x < MapWidth - 1; x++)
                    {
                        if (Map[x, MapHeight - 4] == Block.EmptySpace)
                        {
                            empties.Add(x);

                            ////if the block above is a wall, remove it to make way, should perhaps check one or two tiles more
                            //if (Map[x, MapHeight - 2] == Block.Wall)
                            //    Map[x, MapHeight - 2] = Block.EmptySpace;
                        }
                    }
                    if (!empties.Any())
                    {
                        for (int i = MapHeight - 5; i < MapHeight - 1; i++)
                        {
                            Map[15, i] = Block.EmptySpace;
                            Map[16, i] = Block.EmptySpace;
                        }
                    }
                    else
                    {
                        for (int i = MapHeight - 4; i < MapHeight - 1; i++)
                        {
                            Map[empties[0], i] = Block.EmptySpace;
                            Map[empties[0], i] = Block.EmptySpace;
                        }
                    }

                    break;
                case Direction.South:
                    for (int i = 0; i < MapWidth; i++)
                    {

                        //if the block below is a wall, remove it to make way, should perhaps check one or two tiles more
                        if (Map[i, 1] == Block.EmptySpace)
                        {
                            if (Map[i, 2] == Block.Wall)
                                Map[i, 2] = Block.EmptySpace;

                        }
                    }
                    break;
                case Direction.East:
                    break;
                case Direction.West:
                    break;
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

        //returns true if the position is a mapexit
        public bool IsMapExit(int x, int y)
        {
            if (y == 0 || y == MapHeight || x == 0 || x == MapWidth)
            {
                return true;
            }
            return false;
        }

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

        public void RandomFillMap()
        {
            // New, empty map
            Map = new Block[MapWidth, MapHeight];

            int mapMiddle = 0; // Temp variable
            for (int column = 0, row = 0; row < MapHeight; row++)
            {
                for (column = 0; column < MapWidth; column++)
                {
                    // If coordinants lie on the the edge of the map (creates a border)
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
            for (int column = 0, row = 0; row < MapHeight; row++)
            {
                for (column = 0; column < MapWidth; column++)
                {
                    if (Map[column, row] == 0) return new[] { column, row };
                }
            }
            return null;

        }
    }
}
