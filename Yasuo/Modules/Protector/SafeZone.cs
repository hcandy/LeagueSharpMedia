// TODO: REWORK

namespace Yasuo.Modules.Protector
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using Geometry = LeagueSharp.Common.Geometry;

    class SafeZone
    {
        //public SafeZoneLogicProvider Provider;

        public Vector2 Start;

        public float Range;

        public float AdditionalWidth;

        private Geometry.Polygon Polygon;

        public List<Obj_AI_Base> AlliesInside;

        public List<Obj_AI_Base> EnemiesInside;

        public SafeZone(Vector2 start, float range, float additionalWidth)
        {
            this.Start = start;

            this.Range = range;

            this.AdditionalWidth = additionalWidth;

            this.Create();
        }

        private void Create()
        {
            var wwCenter = Geometry.Extend(ObjectManager.Player.ServerPosition, this.Start.To3D(), 300);

            var wwPerpend = (wwCenter - ObjectManager.Player.ServerPosition).Normalized();
            wwPerpend.X = -wwPerpend.X;

            var leftInnerBound = Geometry.Extend(wwCenter, wwPerpend, 250 + this.AdditionalWidth);
            var rightInnerBound = Geometry.Extend(wwCenter, wwPerpend, -250 + this.AdditionalWidth);

            var leftOuterBound = Geometry.Extend(this.Start, leftInnerBound.To2D(), this.Range);
            var rightOuterBound = Geometry.Extend(this.Start, rightInnerBound.To2D(), this.Range);

            var safeZone = new Geometry.Polygon();
            safeZone.Add(leftInnerBound);
            safeZone.Add(rightInnerBound);
            safeZone.Add(leftOuterBound);
            safeZone.Add(rightOuterBound);
            safeZone.Add(new Geometry.Polygon.Arc(leftOuterBound, this.Start, 250 * (float)Math.PI / 180, this.Range));

            Polygon = safeZone;
        }

        public void Draw()
        {
            Polygon.Draw(Color.Aqua, 2);
        }
    }
}