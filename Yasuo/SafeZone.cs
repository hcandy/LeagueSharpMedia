//              ('-.      .-')                             
//             ( OO ).-. ( OO ).                           
//  ,--.   ,--./ . --. /(_)---\_) ,--. ,--.    .-'),-----. 
//   \  `.'  / | \-.  \ /    _ |  |  | |  |   ( OO'  .-.  '
// .-')     /.-'-'  |  |\  :` `.  |  | | .-') /   |  | |  |
//(OO  \   /  \| |_.'  | '..`''.) |  |_|( OO )\_) |  |\|  |
// |   /  /\_  |  .-.  |.-._)   \ |  | | `-' /  \ |  | |  |
// `-./  /.__) |  | |  |\       /('  '-'(_.-'    `'  '-'  '
//   `--'      `--' `--' `-----'   `-----'         `-----' 
//
// Thanks to HyunMi helping me with math, cause I suck. Also thanks to Asuna helping me to understand it. :)

namespace Yasuo
{
    using System;
    using SharpDX;

    using LeagueSharp;
    using LeagueSharp.Common;


    public static class SafeZone
    {
        /// <summary>
        /// Gets the yasuo windwall safezone
        /// </summary>
        /// <param name="casterPosition">The Vector3 position of the enemy</param>
        /// <param name="spellRange">The range of the spell he is going to cast</param>
        /// <returns>Returns the zone of safety behind yasuo's windwall where the enemy's spell will be blocked, limited by his spell's range</returns>
        // Test result: Returns a Z. Good job HyunMi. Nice rework. Kappa

        public static Geometry.Polygon Safezone(Vector3 casterPosition, float spellRange)
        {
            Vector3 wwCenter = ObjectManager.Player.ServerPosition.Extend(casterPosition, 300);

            Vector3 wwPerpend = (wwCenter - ObjectManager.Player.ServerPosition).Normalized();
            wwPerpend.X = -wwPerpend.X;

            Vector3 leftInnerBound = wwCenter + 250 * wwPerpend;
            Vector3 rightInnerBound = wwCenter - 250 * wwPerpend;

            Vector3 leftOuterBound = casterPosition.Extend(leftInnerBound, spellRange);
            Vector3 rightOuterBound = casterPosition.Extend(rightInnerBound, spellRange);

            Geometry.Polygon safeZone = new Geometry.Polygon();
            safeZone.Add(leftInnerBound);
            safeZone.Add(rightInnerBound);
            safeZone.Add(leftOuterBound);
            safeZone.Add(rightOuterBound);
            safeZone.Add(new Geometry.Polygon.Arc(leftOuterBound, casterPosition, Angle(casterPosition, leftOuterBound, rightOuterBound), spellRange));

            return safeZone;
        }

        /// <summary>
        /// Gets the yasuo windwall safezone with extra out values for debugging
        /// </summary>
        /// <param name="casterPosition">The Vector3 position of the enemy who is going to cast the spell</param>
        /// <param name="spellRange">The range of said spell</param>
        /// <param name="spellWidth">The width of said spell</param>
        /// <param name="allyPosition">The position of an ally</param>
        /// <param name="safezone">The zone of safety behind yasuo's windwall limited by the range of the enemy's spell</param>
        /// <param name="windWallEdges">The left and right bounds of the to be placed windwall</param>
        /// <param name="spellPathToAlly">A rectangle that represents the spell that the enemy is going to send to your ally</param>
        /// <returns>Return a bool wheter or not this ally is safe from that spell or not</returns>
        /// <remarks>All out parameters will be set to null and the function will return false upon an exception</remarks>
        public static bool SafezoneDebug(Vector3 casterPosition, float spellRange, float spellWidth, Vector3 allyPosition, out Geometry.Polygon safezone, out Tuple<Vector3, Vector3> windWallEdges, out Geometry.Polygon.Rectangle spellPathToAlly)
        {
            try
            {
                Vector3 wwCenter = ObjectManager.Player.ServerPosition.Extend(casterPosition, 300);

                Vector3 wwPerpend = (wwCenter - ObjectManager.Player.ServerPosition).Normalized();
                wwPerpend.X = -wwPerpend.X;

                Vector3 leftInnerBound = wwCenter + 250 * wwPerpend;
                Vector3 rightInnerBound = wwCenter - 250 * wwPerpend;

                Vector3 leftOuterBound = casterPosition.Extend(leftInnerBound, spellRange);
                Vector3 rightOuterBound = casterPosition.Extend(rightInnerBound, spellRange);

                Geometry.Polygon safeZone = new Geometry.Polygon();
                safeZone.Add(leftInnerBound);
                safeZone.Add(rightInnerBound);
                safeZone.Add(leftOuterBound);
                safeZone.Add(rightOuterBound);
                safeZone.Add(new Geometry.Polygon.Arc(leftOuterBound, casterPosition, Angle(casterPosition, leftOuterBound, rightOuterBound), spellRange));

                safezone = safeZone;
                windWallEdges = new Tuple<Vector3, Vector3>(leftInnerBound, rightInnerBound);
                spellPathToAlly = new Geometry.Polygon.Rectangle(casterPosition, allyPosition, spellWidth);
                return safeZone.IsInside(allyPosition);
            }
            catch (Exception)
            {
                safezone = null;
                windWallEdges = null;
                spellPathToAlly = null;
                return false;
            }
        }

        private static float Angle(Vector3 a, Vector3 b, Vector3 c)
        {
            float lenA = b.Distance(c);
            float lenB = a.Distance(c);
            float lenC = b.Distance(a);
            return ((float)Math.Cosh((lenB * lenB) + (lenC * lenC) - (lenA * lenA)) / (2 * lenB * lenC));
        }

        private static Vector3 Move(this Vector3 origin, Vector3 moveTo, float distance)
        {
            float t = distance / origin.Distance(moveTo);
            return new Vector3(origin.X + ((moveTo.X - origin.X) * t), origin.Y + ((moveTo.Y - origin.Y) * t), origin.Z + ((moveTo.Z - origin.Z) * t));
        }

        /* k Thnx bea */
        /*
                public static Geometry.Polygon GetSafeZone(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args = null) //my methos uses sender and args instead of everything at once. Outcome is the same I guess.
                {
                    var PredictedWWPos = Manager.Player.ServerPosition.Extend(sender.ServerPosition, 300).To2D();
                    var Direction = (PredictedWWPos - Manager.Player.ServerPosition.To2D()).Normalized().Perpendicular();

                    var WWRPos = PredictedWWPos + (250) * Direction; // 250 === 500 / 2
                    var WWLPos = PredictedWWPos - (250) * Direction; // 250 === 500 / 2

                    float Radius;

                    Radius = args?.SData.CastRadius ?? 1500f;

                    var WWRPosExtend = sender.ServerPosition.To2D().Extend(WWRPos, Radius);
                    var WWLPosExtend = sender.ServerPosition.To2D().Extend(WWLPos, Radius);

                    var SafeZone = new Geometry.Polygon();
                    {
                        SafeZone.Add(WWLPos);
                        SafeZone.Add(WWRPos);
                        SafeZone.Add(WWLPosExtend);
                        SafeZone.Add(WWRPosExtend);
                        SafeZone.Add(GibTheArkOfNoah(WWLPosExtend, WWRPosExtend, sender.ServerPosition.To2D(), Radius)); // <------ (╯°□°)╯︵ <pǝɹǝƃuop ǝƃɐssǝɯ>
                    }

                    return SafeZone;
                }

                public static Geometry.Polygon.Arc GibTheArkOfNoah(Vector2 startArc, Vector2 endArc, Vector2 center, float radius)
                {
                    float angle = PointsAngle(center, startArc, endArc);
                    return new Geometry.Polygon.Arc(startArc, center, angle, radius);
                }

                public static float PointsAngle(Vector2 a, Vector2 b, Vector2 c)
                {
                    Vector2 lenA = b.Distance(c), lenB = a.Distance(c), lenC = b.Distance(a);
                    var angle = (float)Math.Cosh((lenB * lenB) + (lenC * lenC) - (lenA * lenA)) / (2 * lenB * lenC);
                    return angle;
                }

                public static float DegreeToRadian(float angle)
                {
                    return Math.PI * angle / 180f;
                }

                public static float RadianToDegree(float angle)
                {
                    return angle * (180f / Math.PI);
                }

                public static void OnDraw(EventArgs args)
                {
                    if (Player.IsDead && Player.IsZombie) return;

                    if (TargetSelector.GetSelectedTarget() != null)
                    {
                        WindWall.SafeZone(TargetSelector.GetSelectedTarget()).Draw(System.Drawing.Color.AntiqueWhite, 5);
                    }
                }
                */
    }
}
