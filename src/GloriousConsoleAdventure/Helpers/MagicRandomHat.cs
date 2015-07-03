using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloriousConsoleAdventure.Helpers
{
    public static class MagicNumberHat
    {
        private static Random _random;

        public static Random Random
        {
            get { return _random ?? (_random = new Random()); }
        }
    }
}
