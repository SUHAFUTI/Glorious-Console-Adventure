using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Mapping;
using GloriousConsoleAdventure.Models.MapModels;

namespace GloriousConsoleAdventure.Models
{
    public class Mob
    {
        private Coordinate _coordinate;
        public string Name { get; set; }

        public Coordinate Coordinates
        {
            get { return _coordinate; }
            set
            {
                if (Coordinates != null) PreviousCoordinate = _coordinate;
                _coordinate = value;
            }
        }
        public Stats Stats { get; set; }
        public Coordinate PreviousCoordinate { get; private set; }
    }
}
