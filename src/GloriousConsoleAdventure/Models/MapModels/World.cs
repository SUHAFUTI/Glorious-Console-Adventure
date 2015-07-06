using System.Collections.Generic;

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

        public Coordinate WhereAmI { get; set; }
    }


}
