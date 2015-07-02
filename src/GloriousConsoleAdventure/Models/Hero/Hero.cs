using GloriousConsoleAdventure.Mapping;

namespace GloriousConsoleAdventure.Models.Hero
{
    public class Hero
    {
        public Hero(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public Coordinate Coordinates { get; set; }
    }
}
