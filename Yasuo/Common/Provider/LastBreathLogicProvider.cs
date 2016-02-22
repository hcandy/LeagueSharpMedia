namespace Yasuo.Common.Provider
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common.Extensions;

    class LastBreathLogicProvider
    {
        public Obj_AI_Hero MostKnockedUp(List<Obj_AI_Hero> enemies) => enemies.MaxOrDefault(x => x.CountEnemiesInRange(Variables.Spells[SpellSlot.R].Range));

        public Obj_AI_Hero MostDamageDealt(List<Obj_AI_Hero> enemies)
        {
            Dictionary<Obj_AI_Hero, float> damage = null;

            var dmgTemp = 0f;

            foreach (var x in enemies)
            {
                dmgTemp += enemies.Where(z => x.Distance(z) <= Variables.Spells[SpellSlot.R].Range).Sum(y => Variables.Spells[SpellSlot.R].GetDamage(y));
                dmgTemp += Variables.Spells[SpellSlot.R].GetDamage(x);

                damage.Add(x, dmgTemp);
                dmgTemp = 0;
            }

            return damage.MaxOrDefault(x => x.Value).Key;
        }

        public List<Obj_AI_Hero> GetEnemiesAround(Obj_AI_Hero target)
        {
            var result = new List<Obj_AI_Hero>();

            result.AddRange(target.GetEnemiesInRange(Variables.Spells[SpellSlot.R].Range).Where(x => x.IsAirbone()));

            return result;
        }

        public Obj_AI_Hero LeastKnockUpTime(Obj_AI_Hero target)
        {
            var buffTime = this.GetEnemiesAround(target).ToDictionary(x => target, x => target.RemainingAirboneTime());

            return buffTime.MinOrDefault(x => x.Value).Key;
        }

        // TODO: Add that
        public Obj_AI_Hero MostSafety(List<Obj_AI_Hero> enemies)
        {
            Obj_AI_Hero mostAlliesAround = null;
            Obj_AI_Hero leastEnemiesAround = null;
            mostAlliesAround = enemies.MaxOrDefault(x => x.CountAlliesInRange(500));
            leastEnemiesAround = enemies.MinOrDefault(x => x.CountEnemiesInRange(500));

            // 5 enemies are knocked up, no safety check needed
            if (enemies.Count >= 5)
            {
                return this.MostKnockedUp(enemies);
            }

            return null;
        }

        // TODO: Add winding up and Q and E time into consideration
        public bool ShouldCastNow(Obj_AI_Hero target, int buffer = 10)
        {
            if (!target.IsAirbone())
            {
                return false;
            }

            //Instant Ult in 1 v 1 because armor pen and less time for enemies to get spells up
            if (target.CountEnemiesInRange(900) == 0)
            {
                var GapClosePath = new SweepingBladeLogicProvider(target.Distance(Variables.Player)).GetPath(target.ServerPosition);

                if (GapClosePath.PathTime >= target.RemainingAirboneTime())
                {
                    return true;
                }
            }

            if (target.RemainingAirboneTime() <= Game.Ping + buffer)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
