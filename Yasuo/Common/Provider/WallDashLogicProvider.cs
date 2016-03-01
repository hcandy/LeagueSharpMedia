//TODO: Make class more accessable. And easier to use. Maybe add some Extensions?

namespace Yasuo.Common.Provider
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    static class WallDashLogicProvider
    {
        public static bool IsWallDash(this Obj_AI_Base target, float dashRange, float minWallWidth = 50)
        {
            return IsWallDash(target.ServerPosition, dashRange, minWallWidth);
        }

        public static bool IsWallDash(this Vector3 position, float dashRange, float minWallWidth = 50)
        {
            var dashEndPos = Variables.Player.Position.Extend(position, dashRange);
            var firstWallPoint = GetFirstWallPoint(ObjectManager.Player.Position, dashEndPos);

            if (firstWallPoint.Equals(Vector3.Zero))
            {
                // No Wall
                return false;
            }

            if (dashEndPos.IsWall())
            // End Position is in Wall
            {
                var wallWidth = GetWallWidth(firstWallPoint, dashEndPos);

                if (wallWidth > minWallWidth
                    && wallWidth - firstWallPoint.Distance(dashEndPos) < wallWidth * 0.4f)
                {
                    return true;
                }
            }
            else
            // End Position is not a Wall
            {
                return true;
            }
            return false;
        }

        public static Vector3 GetFirstWallPoint(Vector3 start, Vector3 end, int step = 1)
        {
            if (start.IsValid() && end.IsValid())
            {
                var distance = start.Distance(end);
                for (var i = 0; i < distance; i = i + step)
                {
                    var newPoint = start.Extend(end, i);
                    if (NavMesh.GetCollisionFlags(newPoint) == CollisionFlags.Wall)
                    {
                        Drawing.DrawLine(Drawing.WorldToScreen(start), Drawing.WorldToScreen(end), 4f, Color.White);
                        Drawing.DrawCircle(newPoint, 50, Color.Aqua);
                        return newPoint;
                    }
                }
            }

            return Vector3.Zero;
        }

        public static float GetWallWidth(Vector3 start, Vector3 direction, int maxWallWidth = 1000, int step = 1)
        {
            var thickness = 0f;

            if (!start.IsValid() || !direction.IsValid())
            {
                return thickness;
            }

            for (var i = 0; i < maxWallWidth; i = i + step)
            {
                if (NavMesh.GetCollisionFlags(start.Extend(direction, i)) == CollisionFlags.Wall || start.Extend(direction, i).IsWall())
                {
                    thickness += step;
                }
                else
                {
                    return thickness;
                }
            }
            return thickness;
        }
    }
}
