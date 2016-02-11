//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Policy;
//using System.Text;
//using System.Threading.Tasks;
//using LeagueSharp;
//using LeagueSharp.Common;
//using SharpDX;
//using Color = System.Drawing.Color;

//namespace Yasuo
//{
//    internal class Drawings
//    {
//        private static Obj_AI_Hero Player => ObjectManager.Player;

//        public static void OnDraw(EventArgs args)
//        {
//            if (Player.IsDead) return;

//            if (TargetSelector.GetSelectedTarget() != null)
//            {
//                SafeZone.Safezone(TargetSelector.GetSelectedTarget().ServerPosition, 1000f).Draw(System.Drawing.Color.White, 2);
//            }
//            //Drawing.DrawText(900,900, Color.AntiqueWhite, "isWall points.count = "+Wall.isWallJump(MinionManager.GetMinions(500, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.Health).Where(x => x.IsValid).FirstOrDefault(x => x.Distance(Player.ServerPosition) < 500), 500));
//            //Drawing.DrawCircle(Game.CursorPos, 250f, Color.Aqua);

//            //Max Flow Distance
//            if (Player.Mana <= Player.MaxMana)
//            {
//                Render.Circle.DrawCircle(Player.Position, FlowManager.GetRemainingUnits(), Color.White);
//            }

//            if (Program.Q.Level >= 1)
//            {
//                Render.Circle.DrawCircle(Player.Position, Program.Q.Range, Color.White);
//            }


//            if (Program.W.Level >= 1)
//            {
//                Render.Circle.DrawCircle(Player.Position, Program.W.Range, Color.White);
//            }

//            if (Program.R.IsReady() && Program.R.Level >= 1)
//            {
//                Render.Circle.DrawCircle(Player.Position, Program.R.Range, Color.White);
//            }

//            if (Manager.GetEUnit() != null)
//            {
//                Render.Circle.DrawCircle(Manager.GetEUnit().Position, 60f, Color.White);

//                //for (int i = 0; i < Manager.EPath().Count(); i++)
//                //{
//                //    // Assign string reference based on induction variable.
//                //    Obj_AI_Base value = Manager.EPath()[i];
//                //    Drawing.DrawLine(Drawing.WorldToScreen(value.Position), Drawing.WorldToScreen(Manager.EPath()[i+1].Position), 4f, Color.White);
//                //}

//            }
//        }
//    }
//}
