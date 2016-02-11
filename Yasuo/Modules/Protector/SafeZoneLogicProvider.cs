//namespace Yasuo.Modules.Protector
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;

//    using LeagueSharp;
//    using LeagueSharp.Common;

//    using SharpDX;

//    using Yasuo.Common;
//    using Yasuo.Evade;

//    using Geometry = LeagueSharp.Common.Geometry;

//    class SafeZoneLogicProvider
//    {
//        public Dictionary<Skillshot, Obj_AI_Base> possibleCollisions;

//        public SafeZoneLogicProvider(List<Skillshot> skillshots, List<Obj_AI_Base> units)
//        {
//            foreach (var unit in units)
//            {
//                foreach (var skillshot in skillshots)
//                {
//                    var eta = (int) Math.Min(skillshot.SpellData.Range / skillshot.SpellData.MissileSpeed, 
//                                                unit.Distance(skillshot.GetMissilePosition(0) / skillshot.SpellData.MissileSpeed));
//                    if (skillshot.IsAboutToHit(eta + Game.Ping, unit))
//                    {
//                        possibleCollisions.Add(skillshot, unit);
//                    }
//                }
//            }

//            if (this.possibleCollisions == null)
//            {
//                return;
//            }

//            foreach (var collision in this.possibleCollisions)
//            {
//                var SZ = new SafeZone(
//                    collision.Key.Start,
//                    collision.Key.SpellData.Range,
//                    collision.Key.SpellData.Radius);

//                if (SZ.IsInside(collision.Value))
//                {
//                    Variables.Spells[SpellSlot.W].Cast(SZ.Start);
//                }
//            }
//        }

        


//        // TODO: Develope Target Selector for protecting allies. Maybe abuse our current TS for that. 
//        public static Obj_AI_Base GetUnit(List<Obj_AI_Base> units)
//        {
            
//            return units.Where(x => x is Obj_AI_Hero).MaxOrDefault(x => TargetSelector.GetPriority((Obj_AI_Hero) x));
//        }
//    }
//}
