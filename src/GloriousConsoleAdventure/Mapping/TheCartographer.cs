using GloriousConsoleAdventure.Enums;

namespace GloriousConsoleAdventure.Mapping
{
    public static class TheCartographer
    {
        public static void DrawThisMapPlease(MapHandler map)
        {
            map.MakeCaverns();
            map.PlaceRandomBlock(Block.Coin);
            map.PrintMap();
        }
    }
}
