//TODO: REWORK

//namespace Yasuo.Modules
//{
//    using System;
//    using System.Collections.Generic;

//    using LeagueSharp;
//    using LeagueSharp.Common;
//    using Geometry = LeagueSharp.Common.Geometry;

//    using SharpDX;

//    using Yasuo.Evade;
//    using Yasuo.Modules.Protector;

//    using SKD = LeagueSharp.SDK;

//    internal class SafeZone
//    {
//        public SafeZoneLogicProvider Provider;

//        public Vector2 Start;

//        public float Range;

//        public float AdditionalWidth;

//        public List<Obj_AI_Base> AlliesInside;

//        public List<Obj_AI_Base> EnemiesInside;

//        public SafeZone(Vector2 start, float range, float additionalWidth)
//        {
//            this.Start = start;

//            Range = range;

//            AdditionalWidth = additionalWidth;

//            this.Create();

//            foreach (var ally in HeroManager.Allies)
//            {
//                AlliesInside.Add(ally);
//            }

//            foreach (var enemy in HeroManager.Enemies)
//            {
//                EnemiesInside.Add(enemy);
//            }
//        }

//        public Geometry.Polygon Create()
//        {
//            var wwCenter = ObjectManager.Player.ServerPosition.Extend(this.Start.To3D(), 300);

//            var wwPerpend = (wwCenter - ObjectManager.Player.ServerPosition).Normalized();
//            wwPerpend.X = -wwPerpend.X;

//            var leftInnerBound = wwCenter.Extend(wwPerpend, 250 + AdditionalWidth);
//            var rightInnerBound = wwCenter.Extend(wwPerpend, -250 + AdditionalWidth);

//            var leftOuterBound = this.Start.Extend(leftInnerBound.To2D(), Range);
//            var rightOuterBound = this.Start.Extend(rightInnerBound.To2D(), Range);

//            var safeZone = new Geometry.Polygon();
//            safeZone.Add(leftInnerBound);
//            safeZone.Add(rightInnerBound);
//            safeZone.Add(leftOuterBound);
//            safeZone.Add(rightOuterBound);
//            safeZone.Add(new Geometry.Polygon.Arc(leftOuterBound, this.Start, Angle(this.Start.To3D(), leftOuterBound.To3D(), rightOuterBound.To3D()), Range));

//            return safeZone;
//        }

//        public void Update()
//        {
//            this.Create();
//        }

//        private static float Angle(Vector3 a, Vector3 b, Vector3 c)
//        {
//            var lenA = b.Distance(c);
//            var lenB = a.Distance(c);
//            var lenC = b.Distance(a);
//            return ((float)Math.Cosh((lenB * lenB) + (lenC * lenC) - (lenA * lenA)) / (2 * lenB * lenC));
//        }

//        public bool IsInside(Obj_AI_Base unit)
//        {
//            return this.Create().IsInside(unit);
//        }

//        public void Draw()
//        {
            
//        }
//    }
//}
