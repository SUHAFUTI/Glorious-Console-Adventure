using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GloriousConsoleAdventure.Models.Hero;

namespace GloriousConsoleAdventure
{
    public class ActionMenu
    {
        public static void RenderMenu(Hero hero)
        {
            //Todo: We need to move coloring to some global setting
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            //Draw box around menu
            Drawborders(27);

            //Set cursor to inside the menubox
            Console.SetCursorPosition(46, 2);
            Console.WriteLine("{0}", hero.Name);
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
