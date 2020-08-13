using System;
using System.Collections.Generic;
using System.Linq;
using GloriousConsoleAdventure.Color;
using GloriousConsoleAdventure.Enums;
using GloriousConsoleAdventure.Helpers;

namespace GloriousConsoleAdventure.Models.MapModels
{
    /// <summary>
    /// Basic map entity. Holds all the information for a map.
    /// </summary>
    public class Map
    {
        private readonly Random _rand = MagicNumberHat.Random;
        public Block[,] MapBlocks { get; set; }
        public Guid Id { get; set; }
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public Palettes MapPalette { get; set; }
        public List<BlockTile> ActionBlocks { get; set; }
        public Dictionary<Direction, Coordinate> Exits { get; set; }
        public int WallPercentage { get; set; }
        public List<Map> MapStructures { get; set; }
        public Coordinate ParentMap { get; set; }

        public Map(int mapWidth, int mapHeight, int percentWalls = 40, List<Block> randomBlocks = null, Palettes mapPalette = Palettes.Cave, bool emptyMap = false)
        {
            Exits = new Dictionary<Direction, Coordinate>();
            MapStructures = new List<Map>();
            Id = new Guid();
            MapHeight = mapHeight;
            MapWidth = mapWidth;
            MapPalette = mapPalette;
            ActionBlocks = new List<BlockTile>();
            if (emptyMap)
            {
                percentWalls = 0;
            }
            RandomFillMap(percentWalls, mapWidth, mapHeight);

            if (!emptyMap)
                MakeCaverns();

            if (randomBlocks != null)
            {
                foreach (var randomBlock in randomBlocks)
                {
                    PlaceRandomBlock(randomBlock);
                }
            }
        }
        /// <summary>
        /// Randomly fills map
        /// </summary>
        private void RandomFillMap(int wallPercent, int mapWidth, int mapHeight)
        {

            MapBlocks = new Block[MapWidth, MapHeight];
            for (int row = 0; row < MapHeight; row++)
            {
                int column;
                //Border creation
                for (column = 0; column < MapWidth; column++)
                {
                    if (column == 0)
                    {
                        MapBlocks[column, row] = Block.Wall;
                    }
                    else if (row == 0)
                    {
                        MapBlocks[column, row] = Block.Wall;
                    }
                    else if (column == MapWidth - 1)
                    {
                        MapBlocks[column, row] = Block.Wall;
                    }
                    else if (row == MapHeight - 1)
                    {
                        MapBlocks[column, row] = Block.Wall;
                    }
                    // Else, fill with a wall a random percent of the time
                    else
                    {
                        int mapMiddle = (MapHeight / 2);

                        if (row == mapMiddle)
                        {
                            MapBlocks[column, row] = 0;
                        }
                        else
                        {
                            MapBlocks[column, row] = RandomPercent(wallPercent);
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
        private Block RandomPercent(int percent)
        {
            if (percent >= _rand.Next(1, 101))
            {
                return Block.Wall;
            }
            return Block.EmptySpace;
        }
        /// <summary>
        /// Creates cavarns on a given map
        /// </summary>
        /// <param name="map">Map to make caverns on</param>
        private void MakeCaverns()
        {
            // By initilizing column in the outter loop, its only created ONCE
            for (int column = 0, row = 0; row <= MapHeight - 1; row++)
            {
                for (column = 0; column <= MapWidth - 1; column++)
                {
                    MapBlocks[column, row] = PlaceWallLogic(column, row);
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
        private Block PlaceWallLogic(int x, int y)
        {
            int numWalls = GetAdjacentBlocks(x, y, 1, 1, Block.Wall);


            if (MapBlocks[x, y] == Block.Wall)
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
        /// Returns how blocks of given type is within given scope of x,y
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="scopeX">scope x direction</param>
        /// <param name="scopeY">scope y direction</param>
        /// <param name="block">blocktype to check for</param>
        /// <param name="map">map to check</param>
        /// <returns>amount of adjacent blocks</returns>
        private int GetAdjacentBlocks(int x, int y, int scopeX, int scopeY, Block block)
        {
            var startX = x - scopeX;
            var startY = y - scopeY;
            var endX = x + scopeX;
            var endY = y + scopeY;
            var blockCounter = 0;

            int iY;
            for (iY = startY; iY <= endY; iY++)
            {

                int iX;
                for (iX = startX; iX <= endX; iX++)
                {
                    if (iX == x && iY == y) continue;
                    if (block == Block.Wall)
                    {
                        if (IsWall(iX, iY))
                        {
                            blockCounter++;
                        }
                    }
                    else if (MapBlocks[iX, iY] == block)
                    {
                        blockCounter++;
                    }
                }
            }
            return blockCounter;
        }
        /// <summary>
        /// Checks if given coordinates is a wall
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coodinate</param>
        /// <param name="map">map to check</param>
        /// <returns>true if wall</returns>
        private bool IsWall(int x, int y)
        {
            // Consider out-of-bound a wall
            if (IsOutOfBounds(x, y))
            {
                return true;
            }

            if (MapBlocks[x, y] == Block.Wall)
            {
                return true;
            }

            if (MapBlocks[x, y] == Block.EmptySpace)
            {
                return false;
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
        private bool IsOutOfBounds(int x, int y)
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
        /// Places a random block on the map
        /// </summary>
        /// <param name="block">Blocktype to place</param>
        /// <param name="map">Map to place block in</param>
        /// <returns>Updated map</returns>
        private void PlaceRandomBlock(Block block)
        {
            var randX = _rand.Next(1, MapWidth);
            var randY = _rand.Next(1, MapHeight);
            while (IsWall(randX, randY))
            {
                randX = _rand.Next(1, MapWidth);
                randY = _rand.Next(1, MapHeight);
            }

            MapBlocks[randX, randY] = block;
            Palettes palette;
            Enum.TryParse(block.ToString(), out palette);

            ActionBlocks.Add(new BlockTile
            {
                Block = block,
                Coordinate = new Coordinate { X = randX, Y = randY },
                Palette = palette
            });
        }

        /// <summary>
        /// Traverses map in a given direction and makes an exit at first possible empty tile.
        /// </summary>
        /// <param name="direction">Which direction the exit should appear</param>
        /// <param name="map">Map to generate exit on</param>
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
                            if (MapBlocks[x, y] == Block.EmptySpace)
                            {
                                //Carve exit upwards
                                for (var i = y; i >= 0; i--)
                                {
                                    MapBlocks[x, i] = Block.EmptySpace;
                                }
                                //Let the column loop know we found an exit
                                foundExit = true;
                                Exits.Add(direction, new Coordinate(x, y));
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
                            if (MapBlocks[x, y] == Block.EmptySpace)
                            {
                                //Carve exit downwards
                                for (var i = y; i < MapHeight; i++)
                                {
                                    MapBlocks[x, i] = Block.EmptySpace;
                                }
                                //Let the column loop know we found an exit
                                foundExit = true;
                                Exits.Add(direction, new Coordinate(x, y));
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
                            if (MapBlocks[x, y] == Block.EmptySpace)
                            {
                                //Carve exit left
                                for (var i = x; i >= 0; i--)
                                {
                                    MapBlocks[i, y] = Block.EmptySpace;
                                }
                                //Let the column loop know we found an exit
                                foundExit = true;
                                Exits.Add(direction, new Coordinate(x, y));
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
                            if (MapBlocks[x, y] == Block.EmptySpace)
                            {
                                //Carve exit right
                                for (var i = x; i <= MapWidth - 1; i++)
                                {
                                    MapBlocks[i, y] = Block.EmptySpace;
                                }
                                //Let the column loop know we found an exit
                                foundExit = true;
                                Exits.Add(direction, new Coordinate(x, y));
                            }
                        }
                    }
                    break;
            }
        }
        public List<Coordinate> BlastCross(Coordinate explosion)
        {
            var result = new List<Coordinate>();

            if (!ActionBlocks.Any(x => Equals(x.Coordinate, new Coordinate(explosion.X, explosion.Y + 1))))
            {
                result.Add(new Coordinate(explosion.X, explosion.Y + 1));
                MapBlocks[explosion.X, explosion.Y + 1] = Block.EmptySpace;
            }
            if (!ActionBlocks.Any(x => Equals(x.Coordinate, new Coordinate(explosion.X, explosion.Y - 1))))
            {
                result.Add(new Coordinate(explosion.X, explosion.Y - 1));
                MapBlocks[explosion.X, explosion.Y - 1] = Block.EmptySpace;
            }
            if (!ActionBlocks.Any(x => Equals(x.Coordinate, new Coordinate(explosion.X + 1, explosion.Y))))
            {
                result.Add(new Coordinate(explosion.X + 1, explosion.Y));
                MapBlocks[explosion.X + 1, explosion.Y] = Block.EmptySpace;
            }
            if (!ActionBlocks.Any(x => Equals(x.Coordinate, new Coordinate(explosion.X - 1, explosion.Y))))
            {
                result.Add(new Coordinate(explosion.X - 1, explosion.Y));
                MapBlocks[explosion.X - 1, explosion.Y] = Block.EmptySpace;
            }

            return result;
        }

        /// <summary>
        /// Checks if given coordinates is a map exit
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coodinate</param>
        /// <param name="map">Map thecheck if given coordinate is exit</param>
        /// <returns></returns>
        public bool IsMapExit(Coordinate coordinate)
        {
            if (coordinate.Y == 0 || coordinate.Y == MapHeight || coordinate.X == 0 || coordinate.X == MapWidth)
            {
                //Check if the block actually is empty before deeming it an exit
                //I'm not sure why this isn't an issue, with max values 
                if (coordinate.Y == 0 || coordinate.X == 0)
                    return MapBlocks[coordinate.X, coordinate.Y] == Block.EmptySpace;
                return true;
            }
            return false;
        }
        public BlockTile GetActionBlock(Coordinate coordinate)
        {
            return ActionBlocks.FirstOrDefault(x => x.Coordinate.Equals(coordinate));
        }

    }
}
