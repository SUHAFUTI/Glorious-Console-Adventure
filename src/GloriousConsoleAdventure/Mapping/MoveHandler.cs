using System.Linq;
using GloriousConsoleAdventure.Enums;
using GloriousConsoleAdventure.Models.MapModels;

namespace GloriousConsoleAdventure.Mapping
{
    /// <summary>
    /// Handles all movement
    /// </summary>
    public static class MoveHandler
    {
        public static void TeleportPlayer(Map map, Coordinate coordinate)
        {
            var teleporters = map.ActionBlocks.Where(b => b.Block == Block.Teleport);
            var teleportTo = teleporters.First(b => !b.Coordinate.Equals(coordinate));
            coordinate = teleportTo.Coordinate;
        }
    }
}
