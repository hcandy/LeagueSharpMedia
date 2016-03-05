// TODO: Add Dash End Positions as list. Maybe think about positive things when I change Obj_AI_Base to Connection or Point.
// TODO: Rework Calculations based on Dash End Positions.

namespace Yasuo.Common.Objects
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    class Dash
    {
        public Vector3 StartPosition;

        public Vector3 EndPosition;

        public Dash(Obj_AI_Base unit)
        {
            this.SetDashLength();
            this.SetDangerValue();
            this.StartPosition = Variables.Player.ServerPosition;
            this.EndPosition = Variables.Player.ServerPosition.Extend(unit.ServerPosition, this.DashLenght);
        }

        public int DangerValue { get; private set; }

        public float DashTime { get; private set; }

        public float DashLenght { get; private set; }

        // TODO: Add Path in Skillshot, Add Enemies Around, Add Allies Around, Add Minions Around (?)
        public void SetDangerValue()
        {
            this.DangerValue = 0;
        }

        public void SetDashTime()
        {
            this.DashTime = this.DashLenght / Variables.Spells[SpellSlot.E].Speed;
        }

        public void SetDashLength()
        {
            this.DashLenght += Variables.Spells[SpellSlot.E].Range;
        }

        public void Draw()
        {
            var color = Color.White;

            if (this.EndPosition.CountEnemiesInRange(375) > 0)
            {
                color = Color.Red;
            }
                Drawing.DrawLine(
                    Drawing.WorldToScreen(this.StartPosition),
                    Drawing.WorldToScreen(this.EndPosition),
                    4f,
                    color);


            Drawing.DrawCircle(this.EndPosition, 375, color);
        }

    }
}
