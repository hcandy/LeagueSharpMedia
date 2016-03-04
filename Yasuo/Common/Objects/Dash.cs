// TODO: Add Dash End Positions as list. Maybe think about positive things when I change Obj_AI_Base to Connection or Point.
// TODO: Rework Calculations based on Dash End Positions.

namespace Yasuo.Common.Pathing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    public class Dash
    {
        public Vector3 StartPosition;

        public Vector3 EndPosition;

        public Dash(Obj_AI_Base unit)
        {
            this.SetDashLength();
            this.SetDangerValue();
            StartPosition = Variables.Player.ServerPosition;
            EndPosition = Variables.Player.ServerPosition.Extend(unit.ServerPosition, DashLenght);
        }

        public int DangerValue { get; private set; }

        public float DashTime { get; private set; }

        public float DashLenght { get; private set; }

        // TODO: Add Path in Skillshot, Add Enemies Around, Add Allies Around, Add Minions Around (?)
        public void SetDangerValue()
        {
            DangerValue = 0;
        }

        public void SetDashTime()
        {
            DashTime = DashLenght / Variables.Spells[SpellSlot.E].Speed;
        }

        public void SetDashLength()
        {
            DashLenght += Variables.Spells[SpellSlot.E].Range;
        }

        public void Draw()
        {
            var color = Color.White;

            if (EndPosition.CountEnemiesInRange(375) > 0)
            {
                color = Color.Red;
            }
                Drawing.DrawLine(
                    Drawing.WorldToScreen(StartPosition),
                    Drawing.WorldToScreen(EndPosition),
                    4f,
                    color);


            Drawing.DrawCircle(EndPosition, 375, color);
        }

    }
}
