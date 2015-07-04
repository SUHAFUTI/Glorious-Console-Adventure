using GloriousConsoleAdventure.Mapping;

namespace GloriousConsoleAdventure.Models.Hero
{
    public class Hero
    {
        public int Steps { get; set; }
        public int Coins { get; set; }
        public Hero(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public Coordinate Coordinates { get; set; }
    }
}
