using System;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;

// ReSharper disable ArrangeThisQualifier
namespace Yasuo
{
    class Program
    {
        static void Main(string[] args)
        {
            var instance = new Bootstrap();
        }           
    }
}

//namespace Yasuo
//{
//    class Program
//    {
//        private static StartEvade evade;

//        private static Obj_AI_Hero Player => ObjectManager.Player;

//        public static Spell Q, Q2, W, E, R;

//        private static void Main(string[] args)
//        {
//            evade = new StartEvade();

//            CustomEvents.Game.OnGameLoad += OnLoad;
//            Game.OnUpdate += OnUpdate;
//            Drawing.OnDraw += Drawings.OnDraw;

//            Q = new Spell(SpellSlot.Q, 475);
//            Q2 = new Spell(SpellSlot.Q, 900);
//            W = new Spell(SpellSlot.W, 400);
//            E = new Spell(SpellSlot.E, 475, TargetSelector.DamageType.Magical);
//            R = new Spell(SpellSlot.R, 1300);

//            Q.SetSkillshot(Manager.GetQDelay, 20, float.MaxValue, false, SkillshotType.SkillshotLine);
//            Q2.SetSkillshot(Manager.GetQDelay, 90, 1500, false, SkillshotType.SkillshotLine);
//            E.SetTargetted(0.05f, 1000);
//            R.SetTargetted(0, float.MaxValue);
//        }

//        public static void OnLoad(EventArgs args)
//        {
//            if (ObjectManager.Player.ChampionName.ToLower() == "yasuo")
//            {
//                var MediaSuo = new Bootstrap();
//            }

//        }

//        public static void OnUpdate(EventArgs args)
//        {
//            WindWall.Dodge();

//            if (Player.Mana <= Player.MaxMana)
//            {
//                FlowManager.CheckFlow();
//            }

//            Drawing.DrawText(560, 600, Color.Aqua, "Cursor Pos: " + Game.CursorPos);

//            try
//            {
//                switch (MenuManager.Orbwalker.ActiveMode)
//                {
//                    case Orbwalking.OrbwalkingMode.Combo:
//                        Combo();
//                        break;
//                    case Orbwalking.OrbwalkingMode.Mixed:
//                        Mixed();
//                        break;
//                    case Orbwalking.OrbwalkingMode.LaneClear:
//                        Clear.LaneClear();
//                        break;
//                    case Orbwalking.OrbwalkingMode.LastHit:
//                        LastHit();
//                        break;
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }
//        }

//        public static void Combo()
//        {
//            var target = TargetSelector.GetTarget(2500f, TargetSelector.DamageType.Physical);
//            var SelectedTarget = TargetSelector.GetSelectedTarget();

//            if (TargetSelector.SelectedTarget != null)
//            {
//                target = SelectedTarget; // Prio on Selected target
//            }

//            #region Q
//            // Q   TODO: Maybe add forced EQ with logic (drunk), MinHit in Menu, Dont cast if Canion Minion will die from it (statiks)
//            var MinHit = 2;
//            if (Q.IsReady())
//            {
//                var pred = Program.Q.GetPrediction(target, true, -1F, new[] { CollisionableObjects.YasuoWall });
//                if (!Player.IsDashing())
//                {
//                    if (Q2.CastIfWillHit(target, MinHit) && Manager.HasWhirlwind()) // Multiple Enemies around target
//                    {
//                        var enemiesHit = HeroManager.Enemies.Where(x => Q.WillHit(x, pred.CastPosition)).ToList();

//                        if (enemiesHit.Count() >= MinHit)
//                        {
//                            Q.Cast(pred.CastPosition);
//                        }
//                    }

//                    if (target.CountEnemiesInRange(Math.Min(Player.Distance(target), Q2.Range)) > 1 && Manager.HasWhirlwind()) // x vs x - One Target
//                    {
//                        if (Manager.GetEUnit(target) == null || target.CountEnemiesInRange(500) > 2)
//                        {
//                            Q2.CastIfHitchanceEquals(target, HitChance.High);
//                        }

//                        if (Player.Distance(target) <= E.Range && Player.Distance(target) > E.Range - 375)
//                        {
//                            Game.PrintChat("Multiple = Close target");
//                            E.Cast(target);
//                            Q.Cast(target);
//                        }
//                        else
//                        {
//                            Q.Cast(target);
//                        }
//                    }

//                    if (target.CountEnemiesInRange(Math.Min(Player.Distance(target), Q2.Range)) == 1 && Manager.HasWhirlwind()) // x vs 1 - One target
//                    {
//                        if (Manager.GetEUnit(target) == null || Player.GetBuff("YasuoQ3W").EndTime - Game.Time <= 1000)
//                        {
//                            Q2.CastIfHitchanceEquals(target, HitChance.High);
//                        }

//                        if (Player.Distance(target) <= E.Range && Player.Distance(target) > E.Range - 375)
//                        {
//                            Game.PrintChat("1v1 = close target");
//                            E.Cast(target);
//                            Q.Cast(target);
//                        }
//                        else
//                        {
//                            Q.Cast(target);
//                        }
//                    }
//                }

//                if (Player.IsDashing() && Player.ServerPosition.CountEnemiesInRange(325f) >= 1) // While Dashing (EQ)
//                {
//                    Q.Cast(Player.ServerPosition);
//                }
//            }
//            #endregion
//            #region E
//            // E
//            if (E.IsReady())
//            {
//                if (target != null && Player.Distance(target) >= Q.Range)
//                {
//                    E.Cast(Manager.GetEUnit(target));
//                }
//                if (target == null)
//                {
//                    E.Cast(Manager.GetEUnit());
//                }
//            }
//            #endregion

//            //R
//            var UseR = MenuManager._config.Item("RUse").GetValue<bool>();
//            var MinEnemies = MenuManager._config.Item("useROHP").GetValue<Slider>().Value;
//            if (UseR && R.IsReady())
//            {
//                if (Manager.GetKnockedUp().Count >= MinEnemies && Manager.GetRTimeRemaining(target) == Game.Time)
//                {
//                    R.Cast(Manager.GetKnockedUp().MaxOrDefault(x => x.CountEnemiesInRange(400)));
//                }

//                if (target.CountEnemiesInRange(600) < 1)
//                {
//                    if (Manager.TimeToGapclose(target) > Manager.GetRTimeRemaining(target))
//                    {
//                        E.Cast(Manager.GetEUnit(target, true));
//                    }
//                    else
//                    {
//                        R.Cast(target);
//                    }
//                }
//            }

//        }

//        public static void Mixed()
//        {
//            var target = TargetSelector.GetTarget(2500f, TargetSelector.DamageType.Physical);
//            if (Q.IsReady())
//            {
//                //Q.
//            }
//        }



//        public static void LastHit()
//        {
//        }

//        public static void AutoQ()
//        {
//        }
//    }
//}