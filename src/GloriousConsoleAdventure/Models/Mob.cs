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
        public string Name { get; set; }
        public Coordinate Coordinates { get; set; }
        public Stats Stats { get; set; }
    }
}
