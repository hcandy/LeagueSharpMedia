namespace Yasuo.Common.Provider
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

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

        public List<Obj_AI_Hero> GetEnemiesAround(Vector3 position)
        {
            var result = new List<Obj_AI_Hero>();

            result.AddRange(position.GetEnemiesInRange(475).Where(x => x.IsAirbone()));

            return result;
        }

        public Obj_AI_Hero LeastKnockUpTime(Obj_AI_Hero target)
        {
            var buffTime = this.GetEnemiesAround(target.ServerPosition).ToDictionary(x => target, x => target.RemainingAirboneTime());

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
            if (!target.IsAirbone() || !target.IsValid)
            {
                return false;
            }

            //Instant Ult in 1 v 1 because armor pen and less time for enemies to get spells up
            if (target.CountEnemiesInRange(900) == 0)
            {
                var gapClosePath = new SweepingBladeLogicProvider(target.Distance(Variables.Player)).GetPath(target.ServerPosition);

                Game.PrintChat("[Ult] PathTime: "+gapClosePath.PathTime);
                Game.PrintChat("[Ult] AirboneTime: "+target.RemainingAirboneTime());
                if (gapClosePath.PathTime >= target.RemainingAirboneTime())
                {
                    return true;
                }
            }

            return target.RemainingAirboneTime() <= Game.Ping + buffer;
        }

        public Vector3 GetExecutionPosition(Obj_AI_Hero target)
        {
            var endPosition = Vector3.Zero;
            if (target.IsValid && target != null)
            {
                if (target.UnderTurret(true))
                {
                    var turret =
                        ObjectManager.Get<Obj_AI_Turret>()
                            .Where(x => !x.IsAlly && x.Health > 0)
                            .MinOrDefault(x => x.Distance(Variables.Player));

                    if (turret.IsValid && turret != null)
                    {
                        var y = target.Distance(turret);

                        endPosition = turret.ServerPosition.Extend(
                            target.ServerPosition,
                            turret.AttackRange + target.BoundingRadius - y);
                    }
                }
                else
                {
                    endPosition = target.ServerPosition;
                }
            }
            return endPosition;
        }
    }
}
