using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloriousConsoleAdventure.Models
{
    /// <summary>
    /// Base class of perks
    /// </summary>
    public class Perk
    {
        public string Name { get; set; }
        public string Description { get; set; }
        //Todo: Add stat buff or other? 
    }
}
