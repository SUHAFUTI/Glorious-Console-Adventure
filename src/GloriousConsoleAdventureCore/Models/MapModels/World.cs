using System;
using System.Collections.Generic;
using GloriousConsoleAdventure.Enums;

namespace GloriousConsoleAdventure.Models.MapModels
{
    /// <summary>
    /// Holds all generated maps and their coordinates
    /// </summary>
    public class World
    {
        /// <summary>
        /// Holds the collection of generated maps at give coordinate
        /// </summary>
        public Dictionary<Coordinate, Map> MapGrid { get; set; }

        /// <summary>
        /// Where am I in this vast world!?!
        /// </summary>
        public Coordinate WhereAmI { get; set; }

        public Dictionary<Direction, Map> GetDestinationsAdjacentMaps(Coordinate goingTo)
        {
            var result = new Dictionary<Direction, Map>();
            Coordinate destinationCoordinate;
            //South
            destinationCoordinate = new Coordinate(goingTo.X, goingTo.Y + 1);
            if (MapGrid.ContainsKey(destinationCoordinate))
                result.Add(Direction.South, MapGrid[destinationCoordinate]);

            //North
            destinationCoordinate = new Coordinate(goingTo.X, goingTo.Y - 1);
            if (MapGrid.ContainsKey(destinationCoordinate))
                result.Add(Direction.North, MapGrid[destinationCoordinate]);

            //East
            destinationCoordinate = new Coordinate(goingTo.X + 1, goingTo.Y);
            if (MapGrid.ContainsKey(destinationCoordinate))
                result.Add(Direction.East, MapGrid[destinationCoordinate]);

            //West
            destinationCoordinate = new Coordinate(goingTo.X - 1, goingTo.Y);
            if (MapGrid.ContainsKey(destinationCoordinate))
                result.Add(Direction.West, MapGrid[destinationCoordinate]);

            return result;
        }

    }
}
