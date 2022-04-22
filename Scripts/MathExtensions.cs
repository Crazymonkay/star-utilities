namespace RoadTurtleGames.StarUtilities
{
    public static class MathExtensions
    {
        public static bool IsInRange(float val, float min, float max)
        {
            return val >= min && val < max;
        }
    }
}
