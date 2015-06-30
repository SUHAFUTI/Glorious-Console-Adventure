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

namespace GloriousConsoleAdventure
{
    public class MapHandler
    {
        Random rand = new Random();


        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public int PercentAreWalls { get; set; }

        public int[,] Map;
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
        public int PlaceWallLogic(int x, int y)
        {
            int numWalls = GetAdjacentWalls(x, y, 1, 1);


            if (Map[x, y] == 1)
            {
                if (numWalls >= 4)
                {
                    return 1;
                }
                if (numWalls < 2)
                {
                    return 0;
                }

            }
            else
            {
                if (numWalls >= 5)
                {
                    return 1;
                }
            }
            return 0;
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

        public bool IsWall(int x, int y)
        {
            // Consider out-of-bound a wall
            if (IsOutOfBounds(x, y))
            {
                return true;
            }

            if (Map[x, y] == 1)
            {
                return true;
            }

            if (Map[x, y] == 0)
            {
                return false;
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
        public void PrintMap()
        {
            Console.Clear();
            Console.Write(MapToString());
        }

        string MapToString(bool debug = false)
        {
            string returnString = "";
            if (debug) returnString = string.Join(" ", // Seperator between each element
                                            "Width:",
                                            MapWidth.ToString(),
                                            "\tHeight:",
                                            MapHeight.ToString(),
                                            "\t% Walls:",
                                            PercentAreWalls.ToString(),
                                            Environment.NewLine
                                           );
            //0 empty, 1 wall, 
            List<string> mapSymbols = new List<string>();
            mapSymbols.Add(" ");
            //alt + 219 = █
            mapSymbols.Add("█");
            mapSymbols.Add("+");

            for (int column = 0, row = 0; row < MapHeight; row++)
            {
                for (column = 0; column < MapWidth; column++)
                {
                    returnString += mapSymbols[Map[column, row]];
                }
                returnString += Environment.NewLine;
            }
            return returnString;
        }
        public MapHandler(int mapWidth, int mapHeight, int[,] map, int percentWalls = 40)
        {
            this.MapWidth = mapWidth;
            this.MapHeight = mapHeight;
            this.PercentAreWalls = percentWalls;
            this.Map = map;
        }
        public MapHandler(int mapWidth, int mapHeight, int percentWalls = 40)
        {
            this.MapWidth = mapWidth;
            this.MapHeight = mapHeight;
            this.PercentAreWalls = percentWalls;
            Map = new int[MapWidth, MapHeight];
            RandomFillMap();
        }

        public MapHandler()
        {
            MapWidth = 40;
            MapHeight = 21;
            PercentAreWalls = 40;

            Map = new int[MapWidth, MapHeight];

            RandomFillMap();
        }

        public void BlankMap()
        {
            for (int column = 0, row = 0; row < MapHeight; row++)
            {
                for (column = 0; column < MapWidth; column++)
                {
                    Map[column, row] = 0;
                }
            }
        }

        public void RandomFillMap()
        {
            // New, empty map
            Map = new int[MapWidth, MapHeight];

            int mapMiddle = 0; // Temp variable
            for (int column = 0, row = 0; row < MapHeight; row++)
            {
                for (column = 0; column < MapWidth; column++)
                {
                    // If coordinants lie on the the edge of the map (creates a border)
                    if (column == 0)
                    {
                        Map[column, row] = 1;
                    }
                    else if (row == 0)
                    {
                        Map[column, row] = 1;
                    }
                    else if (column == MapWidth - 1)
                    {
                        Map[column, row] = 1;
                    }
                    else if (row == MapHeight - 1)
                    {
                        Map[column, row] = 1;
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

        int RandomPercent(int percent)
        {
            if (percent >= rand.Next(1, 101))
            {
                return 1;
            }
            return 0;
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
