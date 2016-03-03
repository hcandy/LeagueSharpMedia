namespace Yasuo.Common.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using SDK = LeagueSharp.SDK;

    using SharpDX;
    using Variables = Yasuo.Variables;

    class WindWallLogicProvider
    {
        // TODO: Maybe Block spells that aiming an enemy and are blockable i.e: Lux W
        // TODO: E when W not needed
        // TODO: E behind W when skillshot is targeted (ie. cait ult) will hit you or next AA will kill you or do much dmg
        // TODO: Anti Gragas Insec
        // TODO: Add delay to Special Interactions
        // TODO: Crit in AA
        // TODO: 1v1 SafeZone logic
        // TODO: Clean up code
        // TODO: Annie Stun, Katarina Ult

        public static Vector3 GetCastPosition(SDK.Skillshot skillshot, Vector3 to)
        {

            return Vector3.Zero;
        }

        private List<Vector3> GetPossiblePoints(Vector2 finalPos, int steps = 50)
        {
            var position = Variables.Player.ServerPosition;
            var radius = Variables.Spells[SpellSlot.W].Range;

            List<Vector3> points = new List<Vector3>();
            for (var i = 1; i <= steps; i++)
            {
                var angle = i * 2 * Math.PI / steps;
                var point = new Vector3(position.X + radius * (float)Math.Cos(angle), position.Y + radius * (float)Math.Sin(angle), position.Z);

                if (point.Distance(Variables.Player.Position.Extend(finalPos.To3D(), radius)) < 430)
                {
                    points.Add(point);
                    //Utility.DrawCircle(point, 20, System.Drawing.Color.Aqua, 1, 1);
                }
            }

            points.RemoveAt(0);
            points.RemoveAt(1);
            return points;
        }
    }
}