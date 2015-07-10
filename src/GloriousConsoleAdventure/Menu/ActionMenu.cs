using System;
using GloriousConsoleAdventure.Models.Hero;
using GloriousConsoleAdventure.Models.MapModels;

namespace GloriousConsoleAdventure.Menu
{
    public class ActionMenu
    {
        public static void RenderMenu(Hero hero, World world)
        {
            //Todo: We need to move coloring to some global setting
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            //Draw box around menu
            Drawborders(27);

            //Set cursor to inside the menubox
            Console.SetCursorPosition(46, 2);
            Console.WriteLine("{0}", hero.Name);

            //Set cursor to inside the menubox
            Console.SetCursorPosition(46, 4);
            Console.WriteLine("Steps taken: {0}", hero.Steps);

            if (hero.Coordinates != null)
            {
                //Set cursor to inside the menubox
                Console.SetCursorPosition(46, 6);
#if DEBUG
                Console.WriteLine("Map position: ({0},{1})", hero.Coordinates.X, hero.Coordinates.Y);
                Console.SetCursorPosition(46, 8);
                Console.WriteLine("World position: ({0},{1})", world.WhereAmI.X, world.WhereAmI.Y);
#endif
            }
            if (hero.Coins > 0)
            {
                //Set cursor to inside the menubox
                Console.SetCursorPosition(46, 8);
                Console.WriteLine("Coins: {0}", hero.Coins);
            }
        }

        private static void Drawborders(int height)
        {
            int x = 41;
            int y = 1;
            // 205 ═, 201 ╔, 188 ╝, 187 ╗, 200 ╚, 186 ║
            Console.SetCursorPosition(x, y++);
            Console.WriteLine("╔════════════════════════════════════╗");
            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(41, y++);
                Console.WriteLine("║                                    ║");
            }
            Console.SetCursorPosition(41, y);
            Console.WriteLine("╚════════════════════════════════════╝");

        }
    }
}
