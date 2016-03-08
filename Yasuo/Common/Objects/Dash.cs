// TODO: Add Dash End Positions as list. Maybe think about positive things when I change Obj_AI_Base to Connection or Point.
// TODO: Rework Calculations based on Dash End Positions.

namespace Yasuo.Common.Objects
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common.Utility;

    using Yasuo.Common.Provider;
    using SharpDX;

    using Color = System.Drawing.Color;

    public class Dash
    {
        public Vector3 StartPosition;

        public Vector3 EndPosition;

        public Obj_AI_Base Unit;

        public Dash(Obj_AI_Base unit)
        {
            Unit = unit;

            this.StartPosition = Variables.Player.ServerPosition;
            this.EndPosition = Variables.Player.ServerPosition.Extend(unit.ServerPosition, Variables.Spells[SpellSlot.E].Range);

            this.SetDashLength();
            this.SetDangerValue();

            this.CheckWallDash();
        }

        public int DangerValue { get; private set; }

        public float DashTime { get; private set; }

        public float DashLenght { get; private set; }

        public bool IsWallDash { get; private set; }

        public bool WallDashSavesTime { get; protected internal set; }

        // TODO: Add Path in Skillshot (Based on Skillshot Danger value) , Add Enemies Around (Based on Priority), Add Allies Around, Add Minions Around (?)
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
            if (EndPosition.IsWall() && !this.IsWallDash)
            {
                EndPosition = WallDashLogicProvider.GetFirstWallPoint(StartPosition, EndPosition);
            }
            this.DashLenght = Variables.Spells[SpellSlot.E].Range;
        }

        public void CheckWallDash(float minWallWidth = 50)
        {
            if (this.Unit.IsWallDash(this.DashLenght, minWallWidth))
            {
                IsWallDash = true;
            }
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

            Render.Circle.DrawCircle(this.EndPosition, 350, color);
        }

    }
}
