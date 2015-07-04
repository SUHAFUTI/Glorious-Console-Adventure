using GloriousConsoleAdventure.Mapping;

namespace GloriousConsoleAdventure.Models.Hero
{
    public class Hero : Mob
    {
        public int Steps { get; set; }
        public int Coins { get; set; }
        public Hero(string name)
        {
            Name = name;
        }
    }
}
