using GloriousConsoleAdventure.Models.MapModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloriousConsoleAdventureCore.Interfaces
{
    public interface ICreature
    {
        Coordinate Position { get; set; }
        string Name { get; set; }

    }
}
