//TODO:  Djikstra

namespace Yasuo.Skills.Combo
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Yasuo.Common;
    using Yasuo.Common.Extensions;

    internal class SweepingBlade : Child<Combo>
    {
        public SweepingBlade(Combo parent)
            : base(parent)
        {
            this.OnLoad();
        }

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

            // Blacklist
            var blacklist = new Menu("Blacklist", this.Name + "Blacklist");

            if (HeroManager.Enemies.Count == 0)
            {
                blacklist.AddItem(new MenuItem(blacklist.Name + "null", "No enemies found"));
            }
            else
            {
                foreach (var x in HeroManager.Enemies)
                {
                    blacklist.AddItem(new MenuItem(blacklist.Name + x.Name, x.Name).SetValue(false));
                }
                MenuExtensions.AddToolTip(
                    blacklist,
                    "Setting a champion to 'on', will make the script not using Q for him anymore");
            }
            this.Menu.AddSubMenu(blacklist);

            // Spell Settings

            // Mode
            this.Menu.AddItem(
                new MenuItem(this.Name + "ModeTarget", "Dash to: ").SetValue(
                    new StringList(new[] { "Mouse", "Enemy" }, 0))
                    .SetTooltip("The assembly will try to E on a minion in order to Q"));

            new MenuItem(this.Name + "ModeAlgo", "Algorithmus: ").SetValue(
                new StringList(new[] { "Media (Pre-Calc Path)", "Valvrave/Brian Sharp", "YasuoPro" }, 0))
                .SetTooltip(
                    "Setting the Algorithmus to Media will make " + Variables.Name
                    + " calculate a path around Skillshots or Dangerous Zones.");

            // EQ

            #region EQ

            this.Menu.AddItem(
                new MenuItem(this.Name + "EQ", "Try to E for EQ").SetValue(true)
                    .SetTooltip("The assembly will try to E on a minion in order to Q"));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinHitAOE", "Min HitCount for AOE").SetValue(new Slider(2, 2, 5)));

            #endregion

            // Prediction

            this.Menu.AddItem(
                new MenuItem(this.Name + "Prediction", "Predict enemy position").SetValue(true)
                    .SetTooltip(
                        "The assembly will try to E to the enemy predicted position. This will not work if Mode is set to Mouse."));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            this.Provider = new SweepingBladeLogicProvider();

            base.OnInitialize();
        }

        public void OnUpdate(EventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            var dashVector = Vector2.Zero;

            switch (this.Menu.Item(this.Name + "ModeTarget").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    dashVector = Game.CursorPos.To2D();
                    break;
                case 1:
                    dashVector = TargetSelector.GetTarget(
                Variables.Spells[SpellSlot.Q].Range,
                TargetSelector.DamageType.Physical).ServerPosition.To2D(); ;
                    break;
            }

                var unit = Provider.GetBestUnit(dashVector, true);
                if (unit != null)
                {
                    Game.PrintChat("Execute on: "+unit.Name);
                    Execute(unit);
                }
        }

        public void OnDraw(EventArgs args)
        {
            //var target = TargetSelector.GetTarget(
            //    Variables.Spells[SpellSlot.Q].Range,
            //    TargetSelector.DamageType.Physical);

            //var unit = Provider.GetBestUnit(target.ServerPosition.To2D(), true);
            //if (unit != null)
            //{
            //    Drawing.DrawCircle(unit.ServerPosition, 150, Color.Aqua);
            //}
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

