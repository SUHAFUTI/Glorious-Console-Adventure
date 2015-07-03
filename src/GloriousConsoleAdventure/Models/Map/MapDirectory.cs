using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloriousConsoleAdventure.Models.Map
{
    public class AdjacentMapDirectory
    {
        public Guid North { get; set; }
        public Guid South { get; set; }
        public Guid East { get; set; }
        public Guid West { get; set; }
    }
}
