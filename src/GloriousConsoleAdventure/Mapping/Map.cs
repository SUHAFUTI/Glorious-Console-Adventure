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

        //Experimental
        public void PlaceExit(Block[,] exittingMap, Direction exitDirection)
        {
            switch (exitDirection)
            {
                case Direction.North:
                    for (int i = 0; i < MapWidth - 1; i++)
                    {
                        Map[i, MapHeight - 1] = exittingMap[i, 1];
                        if (Map[i, MapHeight - 1] == Block.EmptySpace)
                        {
                            if (Map[i, MapHeight - 2] == Block.Wall)
                                Map[i, MapHeight - 2] = Block.EmptySpace;

                        }
                    }
                    break;
                case Direction.South:
                    for (int i = 0; i < MapWidth; i++)
                    {
                        Map[i, 1] = exittingMap[i, MapHeight - 1];
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
        /// Obsolete
        /// </summary>
        [Obsolete("Please use PlaceRandomBlock with a Block.Coin")]
        public void PlaceRandomCoin()
        {
            var randX = _rand.Next(1, MapWidth);
            var randY = _rand.Next(1, MapHeight);
            while (IsWall(randX, randY))
            {
                randX = _rand.Next(1, MapWidth);
                randY = _rand.Next(1, MapHeight);
            }
            Map[randX, randY] = Block.Coin;
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
                    if (column == 0 && row != 10)
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
        /// <returns>Coordinate of first valid position</returns>
        public int[] GetValidStartLocation()
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
