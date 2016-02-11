//namespace Yasuo.Skills.Combo
//{
//    using System.Collections.Generic;
//    using System.Linq;

//    using LeagueSharp;
//    using LeagueSharp.Common;

//    using Yasuo.Common.Extensions;
//    using Yasuo.Common.Utility.Enums;

//    class LastBreathLogicProvider
//    {
//        public static LastBreathSettings Settings;

//        public LastBreathLogicProvider()
//        {
//            Settings = new LastBreathSettings();
//        }

//        public Obj_AI_Hero GetTarget(List<Obj_AI_Hero> enemies)
//        {
//            // Target Selector
    
//            return null;
//        }

//        public Obj_AI_Hero MostKnockedUp(List<Obj_AI_Hero> enemies) => enemies.MaxOrDefault(x => x.CountEnemiesInRange(Variables.Spells[SpellSlot.R].Range));

//        public Obj_AI_Hero MostDamageDealt(List<Obj_AI_Hero> enemies)
//        {
//            Dictionary<Obj_AI_Hero, float> damage = null;

//            var dmgTemp = 0f;

//            foreach (var x in enemies)
//            {
//                dmgTemp += enemies.Where(z => x.Distance(z) <= Variables.Spells[SpellSlot.R].Range).Sum(y => Variables.Spells[SpellSlot.R].GetDamage(y));
//                dmgTemp += Variables.Spells[SpellSlot.R].GetDamage(x);

//                damage.Add(x, dmgTemp);
//                dmgTemp = 0;
//            }

//            return damage.MaxOrDefault(x => x.Value).Key;
//        }

//        public List<Obj_AI_Hero> GetEnemiesAround(Obj_AI_Hero target)
//        {
//            var result = new List<Obj_AI_Hero>();

//            result.AddRange(target.GetEnemiesInRange(Variables.Spells[SpellSlot.R].Range).Where(x => x.IsAirbone()));

//            return result;
//        } 

//        public Obj_AI_Hero LeastKnockUpTime(Obj_AI_Hero target)
//        {
//            var buffTime = this.GetEnemiesAround(target).ToDictionary(x => target, x => target.RemainingAirboneTime());

//            return buffTime.MinOrDefault(x => x.Value).Key;
//        }

//        public Obj_AI_Hero MostSafety(List<Obj_AI_Hero> enemies)
//        {      
//            Obj_AI_Hero mostAlliesAround = null;
//            Obj_AI_Hero leastEnemiesAround = null;
//            mostAlliesAround = enemies.MaxOrDefault(x => x.CountAlliesInRange(500));
//            leastEnemiesAround = enemies.MinOrDefault(x => x.CountEnemiesInRange(500));

//            // 5 enemies are knocked up, no safety check needed
//            if (enemies.Count >= 5)
//            {
//                return this.MostKnockedUp(enemies);
//            }

//            switch (Settings.PlayStyle())
//            {
//                case PlayStyle.Aggressive:
//                    return leastEnemiesAround;
//                case PlayStyle.Passive:
//                    return mostAlliesAround;
//                case PlayStyle.Automatic:
//                    if (ObjectManager.Player.Mana == ObjectManager.Player.MaxMana
//                        || ObjectManager.Player.HealthPercent >= 50)
//                    {
//                        return leastEnemiesAround;
//                    }
//                return mostAlliesAround;
//            }

//            return null;
//        }

//        public bool ShouldCastNow(Obj_AI_Hero target)
//        {
//            if (!target.IsAirbone())
//            {
//                return false;
//            }

//            //Instant Ult in 1 v 1 because armor pen and less time for enemies to get spells up
//            if (target.CountEnemiesInRange(900) == 0)
//            {
//                return !Variables.Spells[SpellSlot.W].IsReady(2000) || !Variables.Spells[SpellSlot.W].IsReady(2000);
//            }

//            return target.RemainingAirboneTime() <= Game.Ping + 10;
//        }
//    }
//}
