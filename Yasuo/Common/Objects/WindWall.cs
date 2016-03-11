namespace Yasuo.Common.Objects
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal class WindWall
    {
        //public SafeZoneLogicProvider Provider;

        public Vector2 Start;

        public float Range;

        public float AdditionalWidth;

        private Geometry.Polygon polygon;

        public List<Obj_AI_Base> AlliesInside;

        public List<Obj_AI_Base> EnemiesInside;

        public Vector2 CastPosition;

        public WindWall(Vector2 start, float range, float additionalWidth)
        {
            this.Start = start;

            this.Range = range;

            this.AdditionalWidth = additionalWidth;

            this.Create();

            this.CheckAllies();
        }

        private void Create()
        {
            var wwCenter = Geometry.Extend(Variables.Player.ServerPosition, this.Start.To3D(), 300);
            this.CastPosition = wwCenter.To2D();

            var wwPerpend = (wwCenter - Variables.Player.ServerPosition).Normalized();
            wwPerpend.X = -wwPerpend.X;

            var leftInnerBound = Geometry.Extend(
                wwCenter,
                wwPerpend,
                (Variables.Spells[SpellSlot.W].Width / 2) + this.AdditionalWidth);
            var rightInnerBound = Geometry.Extend(
                wwCenter,
                wwPerpend,
                -(Variables.Spells[SpellSlot.W].Width / 2) - this.AdditionalWidth);

            var leftOuterBound = Geometry.Extend(this.Start, leftInnerBound.To2D(), this.Range);
            var rightOuterBound = Geometry.Extend(this.Start, rightInnerBound.To2D(), this.Range);

            var safeZone = new Geometry.Polygon();
            safeZone.Add(leftInnerBound);
            safeZone.Add(rightInnerBound);
            safeZone.Add(leftOuterBound);
            safeZone.Add(rightOuterBound);
            safeZone.Add(new Geometry.Polygon.Arc(leftOuterBound, this.Start, 250 * (float)Math.PI / 180, this.Range));

            this.polygon = safeZone;
        }

        private void CheckAllies()
        {
            foreach (var ally in HeroManager.Allies)
            {
                if (this.polygon.IsInside(ally.ServerPosition.To2D()))
                {
                    this.AlliesInside.Add(ally);
                }
            }
        }

        public void Draw()
        {
            this.polygon.Draw(Color.Aqua, 2);
        }
    }
}