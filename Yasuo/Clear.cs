//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Yasuo
//{
//    using System.Drawing;
//    using System.Security.Cryptography.X509Certificates;

//    using Evade.Helpers;

//    using LeagueSharp;
//    using LeagueSharp.Common;

//    class Clear
//    {
//        private static Obj_AI_Hero Player => ObjectManager.Player;

//        public static void LaneClear()
//        {
//            // general
//            var minion = new List<Obj_AI_Base>();
//            minion.AddRange(MinionManager.GetMinions(Player.ServerPosition, 1000, MinionTypes.All, MinionTeam.Enemy));
//            minion.AddRange(MinionManager.GetMinions(Player.ServerPosition, 1000, MinionTypes.All, MinionTeam.Neutral));

//            minion = minion.Where(x => x.IsValid)
//                    .OrderByDescending(x => x.Health)
//                    .ToList();

//            if (minion.Count == 0)
//            {
//                return;
//            }

//            var minionE = minion.Where(x => !Manager.HasDashWrapper(x))
//                                .FirstOrDefault(x => Manager.GetEDamage(x) >= x.Health + x.GetAutoAttackDamage(x) && Player.Distance(x) <= Program.E.Range);
//            var minionQ = minion.FirstOrDefault(x => Manager.GetQDamage(x) >= x.Health + x.GetAutoAttackDamage(x) && Player.Distance(x) <= Program.Q2.Range);
//            var minionEq = minion.FirstOrDefault(x => (Manager.GetQDamage(x) + Manager.GetEDamage(x)) >= (x.Health + x.GetAutoAttackDamage(x)) && Player.Distance(x) <= 375);
//            Drawing.DrawText(500, 400, Color.Aqua, "Minion Count: " + minion.Count);
//            Drawing.DrawText(500, 425, Color.Aqua, "Minion Count E: " + minionE);
//            Drawing.DrawText(500, 450, Color.Aqua, "Minion Count Q: " + minionQ);
//            Drawing.DrawText(500, 475, Color.Aqua, "Minion Count EQ: " + minionEq);

//            // E
//            if (Program.E.IsReady() && minionE != null)
//            {
//                var nearestEnemy = HeroManager.Enemies.MinOrDefault(x => Player.Distance(x));

//                if (!Manager.GetEndPositionDash(minionE).UnderTurret(true) // Turret Check
//                    && !Manager.GetEndPositionDash(minionE).To2D().CheckDangerousPos(50)) // Danger Check (Evade)
//                    //&& Manager.GetEndPositionDash(MinionE).Distance(minion.MaxOrDefault(x => x.Distance(Player)).ServerPosition) <= Player.Distance(minion.MaxOrDefault(x => x.Distance(Player)).ServerPosition)
//                    //&& Manager.GetEndPositionDash(MinionE).Distance(NearestEnemy.ServerPosition) >= 675f|| NearestEnemy.HealthPercent <= 20 || NearestEnemy == null) // Enemy Check
//                {
//                    Game.PrintChat("E: LastHit");
//                    Program.E.Cast(minionE);
//                }
//            }

//            // Q
//            if (Program.Q.IsReady())
//            {
//                //Stack Whirlwind
//                if (!Manager.HasWhirlwind() && Program.Q.IsInRange(minion.MinOrDefault(x => x.Health)))
//                {
//                    Game.PrintChat("Stack Q");
//                    Program.Q.Cast(minion.MinOrDefault(x => x.Health));
//                }

//                if (Manager.HasWhirlwind())
//                {
//                    var minions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValid && x.Distance(Player) > Player.AttackRange
//                    && x.IsValidTarget(Program.Q2.Range) && Manager.GetQDamage(x) >= x.Health - 75); //75 = Buffer to not let minion Health drop critical or unkillable

//                    // Line
//                    var lineFarmLocation = MinionManager.GetBestLineFarmLocation(minions.Select(x => x.ServerPosition.To2D()).ToList(), Program.Q2.Width, Program.Q2.Range);
//                    if (lineFarmLocation.MinionsHit >= 3) // Config.Item.MinHit
//                    {
//                        Game.PrintChat("Q2 Cast Farm Line Location, Count:" + lineFarmLocation.MinionsHit);
//                        Program.Q2.Cast(lineFarmLocation.Position);
//                    }

//                    // Circular


//                    //var circleFarmLocation = MinionManager.GetBestCircularFarmLocation(minions.Select(x => x.ServerPosition.To2D()).ToList(), 375, 375);

//                    //if (circleFarmLocation.MinionsHit >= 3 && Manager.GetEUnit(circleFarmLocation.Position.To3D()) != null)
//                    //// Config.Item.MinHit
//                    //{
//                    //    if (Manager.GetEUnit(circleFarmLocation.Position.To3D()).Distance(circleFarmLocation.Position)
//                    //        < 500)
//                    //    {
//                    //        Game.PrintChat("EQ Cast Farm Circle Location");
//                    //        Program.E.Cast(Manager.GetEUnit(circleFarmLocation.Position.To3D()));
//                    //        Program.Q.Cast(Player);
//                    //    }
//                    //}
//                }
//            }
//        }
//    }
//}
