namespace GloriousConsoleAdventure.Models.MapModels
{
    /// <summary>
    /// Represents a map coordinate
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// New Coordinate
        /// </summary>
        public Coordinate()
        {
        }

        /// <summary>
        /// New Coordinate
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Coordinate)) return false;
            return Equals((Coordinate)obj);
        }

        /// <summary>
        /// Equal operator
        /// </summary>
        /// <param name="other">coordinate to check against</param>
        /// <returns>True of theres a match</returns>
        protected bool Equals(Coordinate other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Overrides get hash code
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
        public int X { get; set; } //Left
        public int Y { get; set; } //Top
    }

}