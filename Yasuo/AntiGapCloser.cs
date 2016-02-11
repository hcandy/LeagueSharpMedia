//namespace Yasuo
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;
//    using System.Threading.Tasks;
//    using LeagueSharp;
//    using LeagueSharp.Common;

//    class AntiGapCloser
//    {
//        private static Obj_AI_Hero Player => ObjectManager.Player;

//        public static void OnCreate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
//        {
//            if (args.End.Distance(Player.ServerPosition) <= Program.E.Range - 320)
//            {
//                var obj = new List<Obj_AI_Base>();
//                obj.AddRange(MinionManager.GetMinions(Player.ServerPosition, Program.E.Range, MinionTypes.All, MinionTeam.Enemy));
//                obj.AddRange(MinionManager.GetMinions(Player.ServerPosition, Program.E.Range, MinionTypes.All, MinionTeam.Neutral));
//                obj.AddRange(HeroManager.Enemies.Where(i => i.IsValidTarget(Program.E.Range)));

//                if (obj != null)
//                {
//                    obj.Where(x => x.IsValidTarget() && !x.IsZombie)
//                        .OrderByDescending(x => x.Distance(sender.ServerPosition))
//                        .LastOrDefault();
//                }
//            }
            
//        }
//    }
//}