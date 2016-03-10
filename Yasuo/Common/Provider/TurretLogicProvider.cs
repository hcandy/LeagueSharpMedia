namespace Yasuo.Common.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Yasuo.Common.Extensions;

    // Credits to Mayomie for OnTarget
    class TurretLogicProvider
    {
        private static Dictionary<int, Obj_AI_Turret> turretCache = new Dictionary<int, Obj_AI_Turret>();

        private static Dictionary<int, AttackableUnit> turretTarget = new Dictionary<int, AttackableUnit>();

        public TurretLogicProvider() 
        {
            Obj_AI_Base.OnTarget += OnTarget;

            InitializeCache();
        }

        public void OnTarget(Obj_AI_Base sender, Obj_AI_BaseTargetEventArgs args)
        {
            var turret = sender as Obj_AI_Turret;
            if (turret != null && turretCache.ContainsKey(sender.NetworkId))
            {
                turretTarget[sender.NetworkId] = args.Target;
            }
        }

        // TODO: FIX: http://i.imgur.com/aUnV8NA.png
        public bool IsSafePosition(Vector3 position)
        {
            try
            {
                var turret = turretCache.Where(x => x.Value.Health > 0).MinOrDefault(x => x.Value.Distance(position)).Value;

                if (!turret.IsValid || !position.UnderTurret(true) || (int)turret.Health == 0 || turret.IsDead)
                {
                    return true;
                }

                if (turretTarget != null)
                {
                    var target = turretTarget[turret.NetworkId];

                    // We can onehit the turret, there are not much enemies near and we won't die from the next turret shot
                    if (turret.Health + turret.PhysicalShield <= Variables.Player.GetAutoAttackDamage(turret)
                        && turret.CountEnemiesInRange(turret.AttackRange) < 2
                        && Variables.Player.Health > turret.GetAutoAttackDamage(Variables.Player)
                        && position.Distance(turret.ServerPosition) <= Variables.Player.AttackRange)
                    {
                        return true;
                    }

                    if (target != null && !target.IsMe
                        && this.CountAttackableUnitsInRange(turret.Position, turret.AttackRange) > 1
                        && target.Health >= turret.GetAutoAttackDamage((Obj_AI_Base)target))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                
            }
            return false;

        }

        private int CountAttackableUnitsInRange(Vector3 position, float range)
        {
            if (!position.IsValid())
            {
                return 0;
            }

            var minions = MinionManager.GetMinions(position, range).Where(minion => minion.IsValidTarget());
            var heroes = HeroManager.Allies.Where(ally => ally.Distance(position) <= range + ally.BoundingRadius && ally.IsValidTarget());

            return minions.Count() + heroes.Count();
        }

        private static void InitializeCache()
        {
            foreach (var obj in ObjectManager.Get<Obj_AI_Turret>().Where(turret => !turret.IsAlly && turret.Health > 0))
            {
                if (!turretCache.ContainsKey(obj.NetworkId))
                {
                    turretCache.Add(obj.NetworkId, obj);
                    turretTarget.Add(obj.NetworkId, null);
                }
            }
        }
    }
}
