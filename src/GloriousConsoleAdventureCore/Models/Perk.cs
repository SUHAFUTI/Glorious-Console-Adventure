using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloriousConsoleAdventure.Models
{
    /// <summary>
    /// Interface for perks
    /// </summary>
    public interface IPerk
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Modifier { get; set; }
    }
}
