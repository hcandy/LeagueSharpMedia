using LeagueSharp;
using LeagueSharp.Common;

using SharpDX;


namespace Yasuo
{
    static class Wall
    {
        private static Obj_AI_Hero Player => ObjectManager.Player;

        public static bool IsWallDash(this Obj_AI_Base unit, float dashRange, float minWallWidth = 50)
        {
            return CanWallDash(unit, dashRange, minWallWidth);
        }

        public static bool CanWallDash(Obj_AI_Base target, float dashRange, float minWallWidth = 50)
        {
            var dashEndPos = ObjectManager.Player.Position.Extend(target.Position, dashRange);
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
                        return newPoint;
                    }
                }
            }
            return Vector3.Zero;
        }

        public static float GetWallWidth(Vector3 start, Vector3 direction, int maxWallWidth = 1000, int step = 1)
        {
            var thickness = 0f;

            if (start.IsValid() && direction.IsValid())
            {
                for (var i = 0; i < maxWallWidth; i = i + step)
                {
                    if (NavMesh.GetCollisionFlags(start.Extend(direction, i)) == CollisionFlags.Wall)
                    {
                        thickness += step;
                    }
                    else
                    {
                        Game.PrintChat("Thickness: "+thickness);
                        return thickness;
                    }
                }
            }
            //Drawing.DrawText(450, 450, Color.White, "Wall Thickness: " +thickness);
            return thickness;
        }
    }
}
