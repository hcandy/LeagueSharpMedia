namespace Yasuo.Modules.Evade
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.SDK;

    using Yasuo.Common.Provider;
    using Yasuo.Common.Classes;
    using Yasuo.Common.Objects;

    using Variables = Yasuo.Variables;

    internal class SweepingBlade : Child<Modules>
    {
        public SweepingBlade(Modules parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public Path Path;

        public SweepingBladeLogicProvider Provider;

        public override string Name => "Sweeping Blade";

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

            
            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public void OnUpdate(EventArgs args)
        {
            if (Tracker.DetectedSkillshots != null)
            switch (Variables.Orbwalker.ActiveMode)
            {
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.Combo:
                    break;
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.LaneClear:
                    break;
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.LastHit:
                    break;
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.Mixed:
                    break;
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.None:
                    break;
            }
        }

        public void OnDraw(EventArgs args)
        {

        }
    }
}
