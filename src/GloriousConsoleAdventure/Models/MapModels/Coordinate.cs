namespace GloriousConsoleAdventure.Models.MapModels
{
    /// <summary>
    /// Represents a map coordinate
    /// </summary>
    public class Coordinate
    {
        public Coordinate()
        {
        }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Coordinate)) return false;
            return Equals((Coordinate) obj);
        }

        protected bool Equals(Coordinate other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X*397) ^ Y;
            }
        }

        public int X { get; set; } //Left
        public int Y { get; set; } //Top
    }

}