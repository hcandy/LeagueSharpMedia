namespace Yasuo.Common.Utility
{
    using System.Linq;
    using System.Runtime.InteropServices;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    public static class Utility
    {

        // TODO: I have no clue if this will work out
        public static bool IsUnderTowerSafe(this Vector3 position)
        {
            var nearestTurret = ObjectManager.Get<Obj_AI_Turret>().MinOrDefault(turret => turret.Distance(position));

            // This position is not in turret range
            if (nearestTurret.Distance(position) > nearestTurret.AttackRange)
            {
                return true;
            }

            // Turret is focusing something else
            if (nearestTurret.Target != null || nearestTurret.Target != Variables.Player)
            {
                return true;
            }

            // We can onehit the turret, there are not much enemies near and we won't die from the next turret shot
            if (nearestTurret.Health < Variables.Player.GetAutoAttackDamage(nearestTurret) 
                && nearestTurret.CountEnemiesInRange(nearestTurret.AttackRange) < 2 
                && Variables.Player.Health > nearestTurret.GetAutoAttackDamage(Variables.Player)
                && position.Distance(nearestTurret.ServerPosition) <= Variables.Player.AttackRange)
            {
                return true;
            }
            return false;
        }
    }
}
