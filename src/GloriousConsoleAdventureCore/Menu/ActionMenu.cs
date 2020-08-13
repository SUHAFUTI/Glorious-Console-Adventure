using System;
using GloriousConsoleAdventure.Color;
using GloriousConsoleAdventure.Models.Hero;
using GloriousConsoleAdventure.Models.MapModels;

namespace GloriousConsoleAdventure.Menu
{
    public class ActionMenu
    {
        int _menuOffset;
        public ActionMenu(int menuOffset)
        {
            _menuOffset = menuOffset;
        }
        private string Status { get; set; }
        /// <summary>
        /// Renders the action menu on the right
        /// </summary>
        /// <param name="hero">Hero stuff to render</param>
        /// <param name="world">World stuff to render</param>
        
        public void SetStatus(string text)
        {
            Status = text;
        }
        public void ResetStatus()
        {
            Status = string.Empty;
        }
        public void RenderMenu(Hero hero, World world)
        {
            //Todo: We need to move coloring to some global setting
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            //Draw box around menu
            Drawborders(27, _menuOffset - 5);

            //Set cursor to inside the menubox
            Console.SetCursorPosition(_menuOffset, 2);
            Console.WriteLine("{0}", hero.Name);

            //Set cursor to inside the menubox
            Console.SetCursorPosition(_menuOffset, 4);
            Console.WriteLine("Dynamite: {0}", hero.Dynamite);

            Console.SetCursorPosition(_menuOffset, 6);
            Console.WriteLine("Steps taken: {0}", hero.Steps);

            Console.SetCursorPosition(_menuOffset, 25);
            TheArtist.SetPalette(Palettes.StatusBar);
            if (!string.IsNullOrWhiteSpace(Status))
            {
                Console.WriteLine("~  {0}  ~", Status);
                TheArtist.ResetPalette();
            }
            else
            {
                Console.SetCursorPosition(_menuOffset - 1, 25);
                TheArtist.ResetPalette();
                Console.WriteLine("                              ");
            }



            if (hero.Coordinates != null)
            {
#if DEBUG
                //Set cursor to inside the menubox
                Console.SetCursorPosition(_menuOffset, 27);
                Console.WriteLine("Map position: ({0},{1})", hero.Coordinates.X, hero.Coordinates.Y);
                Console.SetCursorPosition(_menuOffset, 28);
                Console.WriteLine("World position: ({0},{1})", world.WhereAmI.X, world.WhereAmI.Y);
#endif
            }
            if (hero.Coins > 0)
            {
                //Set cursor to inside the menubox
                Console.SetCursorPosition(_menuOffset, 8);
                Console.WriteLine("Coins: {0}", hero.Coins);
            }
        }

        /// <summary>
        /// Draws the border around the right action menu
        /// </summary>
        /// <param name="height">How high do you want it?</param>
        private static void Drawborders(int height, int menuOffset)
        {
            var x = menuOffset;
            var y = 1;
            // 205 ═, 201 ╔, 188 ╝, 187 ╗, 200 ╚, 186 ║
            Console.SetCursorPosition(x, y++);
            Console.WriteLine("╔════════════════════════════════════╗");
            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(menuOffset, y++);
                Console.WriteLine("║                                    ║");
            }
            Console.SetCursorPosition(menuOffset, y);
            Console.WriteLine("╚════════════════════════════════════╝");

        }
    }
}
