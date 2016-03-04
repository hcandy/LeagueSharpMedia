// TODO: Add new Dash Object to make things easier

namespace Yasuo.Skills.LaneClear
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Yasuo.Common;
    using Yasuo.Common.Classes;
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Pathing;
    using Yasuo.Common.Utility;
    using Yasuo.Modules.WallDash;
    using Yasuo.Skills.Combo;
    using Yasuo.Common.Provider;

    internal class SweepingBlade : Child<LaneClear>
    {
        public SweepingBlade(LaneClear parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public List<Obj_AI_Base> BlacklistUnits;

        public override string Name => "Sweeping Blade";

        public SweepingBladeLogicProvider Provider;

        protected override void OnEnable()
        {
            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            Game.OnUpdate -= this.OnUpdate;
            Drawing.OnDraw -= this.OnDraw;
            base.OnDisable();
        }

        protected override sealed void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);
            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));
            
            // Mode
            this.Menu.AddItem(
                new MenuItem(this.Name + "ModeTarget", "Dash to: ").SetValue(new StringList(new[] { "Mouse", "Auto" })));

            // EQ

            #region EQ

            this.Menu.AddItem(
                new MenuItem(this.Name + "EQ", "Try to E for EQ").SetValue(true)
                    .SetTooltip("The assembly will try to E on a minion in order to Q"));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinHitAOE", "Min HitCount for AOE").SetValue(new Slider(1, 2, 15)));

            #endregion

            #region E LastHit

            this.Menu.AddItem(
                new MenuItem(this.Name + "LastHit", "Smart Lasthit").SetValue(true)
                    .SetTooltip("The assembly will only Lasthit a minion if Q is not up and the end position of the dash is not too close to the enemy and is not inside a skillshot"));

            #endregion

            #region Misc

            this.Menu.AddItem(
                new MenuItem(this.Name + "NoWallJump", "Anti WallDash").SetValue(true)
                    .SetTooltip("if this is enabled, the assembly won't use Sweeping Blade on a unit if it is a walljump. This is especially useful when doing jungle clear"));

            #endregion

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            this.Provider = new SweepingBladeLogicProvider();

            base.OnInitialize();
        }

        public void OnUpdate(EventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variables.Spells[SpellSlot.E].IsReady())
            {
                return;
            }

            Obj_AI_Base minion = null;
            List<Obj_AI_Base> minions = MinionManager.GetMinions(
                Variables.Player.ServerPosition,
                Variables.Spells[SpellSlot.E].Range,
                MinionTypes.All,
                MinionTeam.Enemy,
                MinionOrderTypes.None);

            switch (this.Menu.Item(this.Name + "ModeTarget").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    minion = MinionManager.GetMinions(Game.CursorPos, 475).Where(x => !x.HasBuff("YasuoDashWrapper") && x.Distance(Variables.Player) <= Variables.Spells[SpellSlot.E].Range)
                        .OrderByDescending(x => x.Health).FirstOrDefault();
                    break;
                case 1:
                    minion = MinionManager.GetMinions(Variables.Player.ServerPosition, 475).Where(x => !x.HasBuff("YasuoDashWrapper"))
                        .OrderByDescending(x => x.Health).FirstOrDefault();
                    break;
            }

            if (minion == null)
            {
                return;
            }

            // if EQ will hit more than X units
            if (Menu.Item(this.Name + "EQ").GetValue<bool>() && 
                Variables.Player.ServerPosition.Extend(minion.ServerPosition, Variables.Spells[SpellSlot.E].Range)
                    .CountMinionsInRange(375) > Menu.Item(Name + "MinHitAOE").GetValue<Slider>().Value
                    && Variables.Player.Health > 100)
            {
                if (Variables.Spells[SpellSlot.Q].IsReady() && Variables.Spells[SpellSlot.Q].Level > 0)
                {
                    Execute(minion);
                }

            }

            // Smart Last Hit
            if (Menu.Item(this.Name + "LastHit").GetValue<bool>())
            {
                if (minions == null)
                {
                    return;
                }

                var enemies = HeroManager.Enemies.Where(x => x.Health > 0).ToList();
                List<Obj_AI_Base> possibleExecutions = new List<Obj_AI_Base>();

                foreach (var x in minions.Where(unit => unit.Health <= Provider.GetDamage(unit) 
                                                && unit.Distance(Variables.Player.ServerPosition) <= Variables.Spells[SpellSlot.E].Range))
                {
                    if (enemies.Count(enemy => enemy.Distance(Variables.Player.ServerPosition) <= 1000) > 0)
                    {
                        foreach (var y in enemies.Where(z => z.HealthPercent > 10))
                        {
                            var newPos = Variables.Player.ServerPosition.Extend(
                                x.ServerPosition,
                                Variables.Spells[SpellSlot.E].Range);
                            if (newPos.Distance(y.ServerPosition) < y.AttackRange)
                            {
                                possibleExecutions.Add(x);
                            }
                        }
                    }
                    else
                    {
                        possibleExecutions.Add(minion);
                    }

                        
                }

                if (possibleExecutions.Count == 0)
                {
                    return;
                }

                Execute(possibleExecutions.MinOrDefault(x => x.Distance(Helper.GetMeanVector2(minions))));
            }


        }

        public void OnDraw(EventArgs args)
        {
            List<Obj_AI_Base> minions = MinionManager.GetMinions(
                Variables.Player.ServerPosition,
                1000,
                MinionTypes.All,
                MinionTeam.Enemy,
                MinionOrderTypes.None);
            var Meanvector = Helper.GetMeanVector3(minions);
            Drawing.DrawCircle(Meanvector, 150, System.Drawing.Color.White);
        }

        private void Execute(Obj_AI_Base unit)
        {
            if (unit.IsValidTarget() && unit != null
                && Helper.IsUnderTowerSafe(Variables.Player.ServerPosition.Extend(unit.ServerPosition, Variables.Spells[SpellSlot.E].Range)))
            {
                if (Menu.Item(this.Name + "NoWallJump").GetValue<bool>())
                {
                    if (unit.IsWallDash(Variables.Spells[SpellSlot.E].Range, 20))
                    {
                        return;
                    }
                }
                Variables.Spells[SpellSlot.E].CastOnUnit(unit);
            }
        }
    }
}

