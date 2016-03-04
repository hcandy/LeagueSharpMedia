namespace Yasuo.Skills.Mixed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common;
    using Yasuo.Common.Classes;
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Pathing;
    using Yasuo.Common.Provider;

    internal class SweepingBlade : Child<Mixed>
    {
        public SweepingBlade(Mixed parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public List<Obj_AI_Base> BlacklistUnits;

        public Path GapClosePath;

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
                new MenuItem(this.Name + "MinHitAOE", "Min HitCount for AOE").SetValue(new Slider(3, 2, 15)));

            #endregion

            #region E LastHit

            this.Menu.AddItem(
                new MenuItem(this.Name + "LastHit", "Smart Lasthit").SetValue(true)
                    .SetTooltip("The assembly will only Lasthit a minion if Q is not up and the end position of the dash is not too close to the enemy and is not inside a skillshot"));

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
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed)
            {
                return;
            }

            Obj_AI_Base minion = null;

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
            if (
                Variables.Player.ServerPosition.Extend(minion.ServerPosition, Variables.Spells[SpellSlot.E].Range)
                    .CountMinionsInRange(Variables.Spells[SpellSlot.E].Range)
                > this.Menu.Item(this.Name + "MinHitAOE").GetValue<int>())
            {
                Execute(minion);
            }
        }

        public void OnDraw(EventArgs args)
        {
            
        }

        private static void Execute(Obj_AI_Base target)
        {
            if (target.IsValidTarget())
            {
                Variables.Spells[SpellSlot.E].CastOnUnit(target);
            }
        }
    }
}

