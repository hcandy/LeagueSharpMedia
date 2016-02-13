//namespace Yasuo.Skills.Combo
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;

//    using LeagueSharp;
//    using LeagueSharp.Common;

//    using Yasuo.Common.Extensions;
//    using Yasuo.Common.Utility.Enums;

//    class LastBreath : Combo
//    {
//        public LastBreathLogicProvider Provider;

//        public LastBreath()
//        {
//            this.Provider = new LastBreathLogicProvider();
//        }

//        public SkillMode GetSkillMode()
//        {
//            return SkillMode.OnUpdate;
//        }

//        public void Execute(EventArgs args)
//        {
//            List<Obj_AI_Hero> enemies = new List<Obj_AI_Hero>();
//            enemies.AddRange(ObjectManager.Player.GetEnemiesInRange(Variables.Spells[SpellSlot.R].Range).Where(x => Extensions.IsAirbone(x)));

//            Obj_AI_Hero target = this.Provider.GetTarget(enemies);

//            // Auto Ult on X targets with preference
//            if (enemies.Count >= 3)
//            {
//                if (this.Provider.MostDamageDealt(enemies).CountEnemiesInRange(Variables.Spells[SpellSlot.R].Range) >= 3)
//                {
//                    target = this.Provider.MostDamageDealt(enemies);
//                }
//                target = this.Provider.MostKnockedUp(enemies);
//            }


//            if (target != null)
//            {
//                if (this.Provider.ShouldCastNow(target))
//                {
//                    if (ObjectManager.Player.HealthPercent >= 25 && target.HealthPercent <= 50
//                        || target.Health <= ObjectManager.Player.GetDamageSpell(target, SpellSlot.R).CalculatedDamage)
//                    {
//                        this.CastLastBreath(target);
//                    }                   
//                }
//            }
//        }

//        public void ExecuteFarm()
//        {
            
//        }

//        private void CastLastBreath(Obj_AI_Hero target)
//        {
//            if (target.IsValid && !target.IsZombie)
//            {
//                Variables.Spells[SpellSlot.R].Cast(target);
//            }
//        }
//    }
//}
