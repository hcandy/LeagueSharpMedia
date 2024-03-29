﻿namespace Yasuo.Modules.WallDash
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common;
    using Yasuo.Common.Classes;
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Provider;

    internal class WallDash : Child<Modules>
    {
        public WallDash(Modules parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public override string Name => "Wall Dash";

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

            this.Menu.AddItem(
                new MenuItem(this.Name + "Keybind", "Keybind").SetValue(new KeyBind(5, KeyBindType.Press)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MouseCheck", "Check for mouse position").SetValue(false));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinWallWidth", "Minimum wall width: ").SetValue(new Slider(150, 10, (int) Variables.Spells[SpellSlot.E].Range / 2)));

            this.Menu.AddItem(new MenuItem(this.Name + "Helper", "How it works")
                .SetTooltip("Hold down the Keybind to let the assembly perform a Dash over a unit that will be a WallDash"));

            //var advanced = new Menu("Advanced Settings", this.Name + "Advanced");

            //advanced.AddItem(
            //    new MenuItem(this.Name + "WidthReduction", "WallWidth %").SetValue(new Slider(100, 0, 200)));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            this.Provider = new SweepingBladeLogicProvider(475);
            base.OnInitialize();
        }

        public void OnUpdate(EventArgs args)
        {
            var units = this.Provider.GetUnits(Variables.Player.ServerPosition);

            if (this.Menu.Item(this.Name + "Keybind").GetValue<KeyBind>().Active)
            {
                Variables.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                foreach (var unit in units.Where(unit => unit.IsWallDash(Variables.Spells[SpellSlot.E].Range)))
                {
                    if (!this.Menu.Item(this.Name + "MouseCheck").GetValue<bool>())
                    {
                        Execute(unit);
                    }
                    // Summary: if Cursor position is near dash end position, dash. That is to prevent dashes over walls that were not intended.
                    else if (Variables.Player.ServerPosition.Extend(unit.ServerPosition, Variables.Spells[SpellSlot.E].Range).Distance(Game.CursorPos) < Variables.Spells[SpellSlot.E].Range)
                    {
                        Execute(unit);
                    }
                }
            }

        }

        public void OnDraw(EventArgs args)
        {
            if (Variables.Player.IsDead || Variables.Player.IsDashing()) return;

            var units = this.Provider.GetUnits(Variables.Player.ServerPosition);
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

