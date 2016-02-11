//namespace Yasuo
//{
//    using System;
//    using System.CodeDom;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Security.Cryptography.X509Certificates;

//    using LeagueSharp;
//    using LeagueSharp.Common;
//    using SharpDX;
//    using Color = System.Drawing.Color;
//    using Evade;

//    using Yasuo.Evade.Helpers;

//    //TODO: Getting rid of Manager.cs and moving it to proper classes
//    class Manager
//    {
//        public static Obj_AI_Hero Player => ObjectManager.Player;

//        #region Q


//        /// <summary>
//        /// Returns true if Player has Statiks Shivt
//        /// </summary>
//        private static bool HaveStatik => Player.GetBuffCount("ItemStatikShankCharge") == 100;

//        /// <summary>
//        /// Returns the Q damage on target
//        /// </summary>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public static double GetQDamage(Obj_AI_Base target)
//        {
//            var dmgItem = 0d;
//            if (Items.HasItem(3057) && (Items.CanUseItem(3057) || Player.HasBuff("Sheen")))
//            {
//                dmgItem = Player.BaseAttackDamage;
//            }
//            if (Items.HasItem(3078) && (Items.CanUseItem(3078) || Player.HasBuff("Sheen")))
//            {
//                dmgItem = Player.BaseAttackDamage * 2;
//            }
//            var damageModifier = 1d;
//            var reduction = 0d;
//            var result = dmgItem
//                         + Player.TotalAttackDamage * (Player.Crit >= 0.85f ? (Items.HasItem(3031) ? 1.875 : 1.5) : 1);
//            if (Items.HasItem(3153))
//            {
//                var dmgBotrk = Math.Max(0.08 * target.Health, 10);
//                result += target is Obj_AI_Minion ? Math.Min(dmgBotrk, 60) : dmgBotrk;
//            }
//            var targetHero = target as Obj_AI_Hero;
//            if (targetHero != null)
//            {
//                if (Items.HasItem(3047, targetHero))
//                {
//                    damageModifier *= 0.9d;
//                }
//                if (targetHero.ChampionName == "Fizz")
//                {
//                    reduction += 4 + (targetHero.Level - 1 / 3) * 2;
//                }
//                var mastery = targetHero.Masteries.FirstOrDefault(i => i.Page == MasteryPage.Defense && i.Id == 68);
//                if (mastery != null && mastery.Points >= 1)
//                {
//                    reduction += 1 * mastery.Points;
//                }
//            }
//            return Player.CalcDamage(target, Damage.DamageType.Physical, 20 * Program.Q.Level + (result - reduction) * damageModifier)
//                   + (HaveStatik ? Player.CalcDamage(target, Damage.DamageType.Magical, 100 * (Player.Crit >= 0.85f ? (Items.HasItem(3031) ? 2.25 : 1.8) : 1)): 0);
//        }

//        /// <summary>
//        /// Returns true if Player has Whirlwind
//        /// </summary>
//        /// <returns></returns>
//        public static bool HasWhirlwind() => Player.HasBuff("YasuoQ3W");

//        //public static bool WhirlwindIsAboutToEnd()
//        //{
//        //    if (HasWhirlwind())
//        //    {
//        //        if (Player.HasBuff("YasuoQ3W"))
//        //    }
//        //}


//        #endregion

//        #region E

//        /// <summary>
//        /// Returns the target that will put you closes to the target with certain conditions
//        /// </summary>
//        /// <param name="target"></param>
//        /// <param name="underTower"></param>
//        /// <returns></returns>
//        public static Obj_AI_Base GetEUnit(Obj_AI_Base target = null, bool underTower = true)
//        {

//            var pos = target != null
//              ? Prediction.GetPrediction(target, Program.E.Delay, 0, GetESpeed()).UnitPosition
//              : Game.CursorPos;

//            if (target != null && Game.CursorPos.Distance(target.Position) >= 700)
//            {
//                pos = Game.CursorPos;
                
//            }

//            var obj = new List<Obj_AI_Base>();
//            obj.AddRange(MinionManager.GetMinions(Player.ServerPosition, Program.E.Range, MinionTypes.All, MinionTeam.Enemy));
//            obj.AddRange(MinionManager.GetMinions(Player.ServerPosition, Program.E.Range, MinionTypes.All, MinionTeam.Neutral));
//            obj.AddRange(HeroManager.Enemies.Where(i => i.IsValidTarget(Program.E.Range)));


//            return
//                obj.Where(
//                    x =>
//                    GetEndPositionDash(x).Distance(pos) < Player.ServerPosition.Distance(pos) && !HasDashWrapper(x)
//                    && UnderTowerSafe(x) && Program.E.IsInRange(x) && x.IsValidTarget() && !GetEndPositionDash(x).To2D().CheckDangerousPos(50))
//                    .MinOrDefault(x => pos.Distance(GetEndPositionDash(x)));
//        }

//        /// <summary>
//        /// Returns the Unit that will put you closes to the Vector3 with certain conditions
//        /// </summary>
//        /// <param name="position"></param>
//        /// <param name="underTower"></param>
//        /// <returns></returns>
//        public static Obj_AI_Base GetEUnit(Vector3 position, bool underTower = true)
//        {

//            var pos = position;

//            var obj = new List<Obj_AI_Base>();
//            obj.AddRange(MinionManager.GetMinions(Player.ServerPosition, Program.E.Range, MinionTypes.All, MinionTeam.Enemy));
//            obj.AddRange(MinionManager.GetMinions(Player.ServerPosition, Program.E.Range, MinionTypes.All, MinionTeam.Neutral));
//            obj.AddRange(HeroManager.Enemies.Where(i => i.IsValidTarget(Program.E.Range)));


//            return
//                obj.Where(
//                    x =>
//                    GetEndPositionDash(x).Distance(pos) < Player.ServerPosition.Distance(pos) && !HasDashWrapper(x)
//                    && UnderTowerSafe(x) && Program.E.IsInRange(x) && x.IsValidTarget() && !GetEndPositionDash(x).To2D().CheckDangerousPos(50))
//                    .MinOrDefault(x => pos.Distance(GetEndPositionDash(x)));
//        }

//        /// <summary>
//        /// Returns true if target has the Dash Mark
//        /// </summary>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public static bool HasDashWrapper(Obj_AI_Base target) => target.HasBuff("YasuoDashWrapper");

//        /// <summary>
//        /// Returns true if endposition is under tower
//        /// </summary>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public static bool UnderTower(Obj_AI_Base target) => (GetEndPositionDash(target).UnderTurret(true));

//        /// <summary>
//        /// Returns true if endposition is under tower and min 1 Ally is under tower too
//        /// </summary>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public static bool UnderTowerSafe(Obj_AI_Base target) => !UnderTower(target) && GetEndPositionDash(target).CountAlliesInRange(1000f) >= 0;

//        /// <summary>
//        /// Returns the E speed
//        /// </summary>
//        /// <returns></returns>
//        public static int GetESpeed() => (int)(Player.MoveSpeedFloorMod + 500);

//        /// <summary>
//        /// Returns the amount of Damage on target
//        /// </summary>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public static double GetEDamage(Obj_AI_Base target) => Player.CalcDamage(target, Damage.DamageType.Magical, (50 + 20 * Program.E.Level) * (1 + Math.Max(0, GetEStacks * 0.25)) + 0.6 * Player.TotalMagicalDamage);

//        /// <summary>
//        /// Returns the amount of E Stacks
//        /// </summary>
//        public static int GetEStacks => Player.GetBuffCount("YasuoDashScalar");

//        /// <summary>
//        /// Returns the end position after dash as Vector3
//        /// </summary>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public static Vector3 GetEndPositionDash(Obj_AI_Base target) => Player.ServerPosition.Extend(target.ServerPosition, Program.E.Range);

//        /// <summary>
//        /// Creates a path between Player and Target
//        /// </summary>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public static List<Vector2> GetDashPath(Obj_AI_Base target)
//        {
//            List<Vector2> wayPoints = new List<Vector2>();
//            wayPoints.Add(Player.ServerPosition.To2D());
//            wayPoints.Add(GetEndPositionDash(target).To2D());
//            return wayPoints;
//        }
//        #endregion
//        #region R
//        /// <summary>
//        /// Returns a list of all knocked up enemys
//        /// </summary>
//        /// <returns></returns>
//        public static List<Obj_AI_Hero> GetKnockedUp()
//        {
//            return (List<Obj_AI_Hero>)HeroManager.Enemies.Where(x => x.HasBuffOfType(BuffType.Knockup) || x.HasBuffOfType(BuffType.Knockback));
//        }

//        /// <summary>
//        /// Returns true if R will put you out of the turret range
//        /// </summary>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public static bool UnderTowerR(Obj_AI_Hero target)
//        {
//            var dist = 400; //Yasuo Ult width. Maybe incorrect. Notice: Yas Ult places you outside of turret Range if Enemy is in a certain Radius under Tower
//            return ObjectManager.Get<Obj_AI_Turret>().Any(x => x.IsValidTarget(950 - dist, true, target.ServerPosition));
//        }

//        /// <summary>
//        /// Returns the remaining R time
//        /// </summary>
//        /// <param name="target"></param>
//        /// <param name="delay"></param>
//        /// <returns></returns>
//        public static float GetRTimeRemaining(Obj_AI_Hero target, float delay = 0f)
//        {
//            var targetBuff = target.Buffs.MinOrDefault(x => x.IsValid && x.Type == BuffType.Knockup || x.Type == BuffType.Knockback);

//            return targetBuff.EndTime - Game.Time - delay;
//        }

//        /// <summary>
//        /// Returns the time you need to Gapclose to your target
//        /// </summary>
//        /// <param name="target"></param>
//        /// <param name="buffer"></param>
//        /// <returns></returns>
//        public static int TimeToGapclose(Obj_AI_Hero target, float buffer = 0f)
//        {
//            var eTarget = 0;
//            if (true)
//            {
//                eTarget = 1;
//            }
//            var pos = target.ServerPosition;
//            var obj = new List<Obj_AI_Base>();
//            obj.AddRange(MinionManager.GetMinions(Player.ServerPosition, Program.E.Range, MinionTypes.All, MinionTeam.Enemy));
//            obj.AddRange(MinionManager.GetMinions(Player.ServerPosition, Program.E.Range, MinionTypes.All, MinionTeam.Neutral));
//            // obj.AddRange(HeroManager.Enemies.Where(i => i.IsValidTarget(Program.E.Range)));

//            obj.Where(x =>
//                GetEndPositionDash(x).Distance(pos) < Player.ServerPosition.Distance(pos) && !HasDashWrapper(x)
//                && UnderTowerSafe(x) && Program.R.IsInRange(x) && x.IsValidTarget()
//                && GetEndPositionDash(x).To2D().CheckDangerousPos(50));

//            if ((obj.Count + eTarget * 1000) >= (GetRTimeRemaining(target) + buffer)) // One dash needs 1 Second to get executed, therefore for 1 unit you need to add 1 second
//            {
//                return obj.Count;
//            }
//            return (int)(GetRTimeRemaining(target) + 1);
//        }

//        /// <summary>
//        /// Returns true if Player can Gapclose to a knocked up enemy
//        /// </summary>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public static bool CanGapcloseToUlt(Obj_AI_Base target)
//        {
//            var pos = target.ServerPosition;
//            var obj = new List<Obj_AI_Base>();
//            obj.AddRange(MinionManager.GetMinions(Player.ServerPosition, Program.E.Range, MinionTypes.All, MinionTeam.Enemy));
//            obj.AddRange(MinionManager.GetMinions(Player.ServerPosition, Program.E.Range, MinionTypes.All, MinionTeam.Neutral));
//            obj.AddRange(HeroManager.Enemies.Where(i => i.IsValidTarget(Program.E.Range)));

//            var unit = obj.Where(x => GetEndPositionDash(x).Distance(pos) < Player.ServerPosition.Distance(pos) && !HasDashWrapper(x)
//                && UnderTowerSafe(x) && Program.E.IsInRange(x) && x.IsValidTarget()
//                && GetEndPositionDash(x).To2D().CheckDangerousPos(50))
//                .MinOrDefault(x => pos.Distance(GetEndPositionDash(x)));

//            return GetEndPositionDash(unit).Distance(target.ServerPosition) <= Program.R.Range; //Will E on a minion put you in R Range
//        }
//        #endregion


//        #region P

//        #endregion
//    }
//}