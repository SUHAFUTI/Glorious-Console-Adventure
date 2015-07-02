using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloriousConsoleAdventure
{
    public static class TheCartographer
    {
        public static void DrawThisMapPlease(MapHandler map)
        {
            map.MakeCaverns();
            map.PlaceRandomCoin();
            map.PrintMap();
        }
    }
}
