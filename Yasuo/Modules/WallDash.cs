//TODO:  Djikstra

namespace Yasuo.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Yasuo.Common;
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Utility;
    using Yasuo.Skills.Combo;

    internal class WallDash : Child<Modules>
    {
        public WallDash(Modules parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public List<Obj_AI_Base> BlacklistUnits;

        public override string Name => "Wall Dash";

        public Skills.Combo.SweepingBladeLogicProvider Provider;

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


            this.Menu.AddItem(
                new MenuItem(this.Name + "Keybind", "Keybind").SetValue(new KeyBind(5, KeyBindType.Press)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MouseCheck", "Check for mouse position").SetValue(false));

            this.Menu.AddItem(
    new MenuItem(this.Name + "MinWallWidth", "Minimum wall width: ").SetValue(new Slider(50, 10, (int) Variables.Spells[SpellSlot.E].Range / 2)));

            this.Menu.AddItem(new MenuItem(this.Name + "Helper", "How it works")
                .SetTooltip("Hold down "+Menu.Item(this.Name+"Keybind").GetValue<KeyBind>()+ " to let the assembly perform a Dash over a unit that will be a WallDash"));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            Provider = new SweepingBladeLogicProvider();
            base.OnInitialize();
        }

        public void OnUpdate(EventArgs args)
        {
            if (Variables.Player.IsDead || Variables.Player.IsDashing()) return;

            var MouseCheck = Menu.Item(this.Name + "MouseCheck").GetValue<bool>();

            var units = Yasuo.Skills.Combo.SweepingBladeLogicProvider.GetUnits(Variables.Player.ServerPosition.To2D(), true, true);

            if (Menu.Item(this.Name + "Keybind").GetValue<KeyBind>().Active)
            {
                Variables.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                foreach (var unit in units.Where(unit => unit.IsWallDash(Variables.Spells[SpellSlot.E].Range)))
                {
                    if (!MouseCheck)
                    {
                        Execute(unit);
                    }
                    else if (Game.CursorPos.Distance(unit.ServerPosition) < 500)
                    {
                        Execute(unit);
                    }
                }
            }

        }

        public void OnDraw(EventArgs args)
        {
            
        }

        private static void Execute(Obj_AI_Base target)
        {
            Game.PrintChat("[WallDash] Casting: " + target.Name);
            if (target.IsValidTarget())
            {
                Variables.Spells[SpellSlot.E].CastOnUnit(target);
            }
        }
    }
}

