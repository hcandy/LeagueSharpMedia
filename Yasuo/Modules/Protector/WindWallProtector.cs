namespace Yasuo.Modules.Protector
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using SDK = LeagueSharp.SDK;

    using Yasuo.Common;
    using Yasuo.Common.Classes;
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Provider;
    using Yasuo.Skills.Combo;

    internal class WindWallProtector : Child<Protector>
    {
        public WindWallProtector(Protector parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public override string Name => "Wind Wall";

        public SweepingBladeLogicProvider Provider;

        public SafeZone SafeZone;

        public SDK.Tracker Tracker;

        //public SafeZoneLogicProvider Provider;

        protected override void OnEnable()
        {
            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            Game.OnUpdate -= OnUpdate;
            Drawing.OnDraw -= this.OnDraw;
            base.OnDisable();
        }

        protected override void OnInitialize()
        {
            Tracker = new SDK.Tracker();
            Provider = new SweepingBladeLogicProvider(Variables.Spells[SpellSlot.E].Range * 2);
            base.OnInitialize();
        }

        protected override void OnLoad()
        {
            Menu = new Menu(Name, Name);

            var whitelist = new Menu("Whitelist", Name + "Whitelist");

            foreach (var x in HeroManager.Allies)
            {
                whitelist.AddItem(new MenuItem(whitelist.Name + x.Name, x.Name).SetValue(true));
            }

            var blacklist = new Menu("Blacklist", Name + "Blacklist");

            foreach (var x in HeroManager.Enemies)
            {
                blacklist.AddItem(new MenuItem(blacklist.Name + x.Name, x.Name).SetValue(true));
            }


            Menu.AddSubMenu(whitelist);
            Menu.AddSubMenu(blacklist);

            Menu.AddItem(new MenuItem(Name + "Enabled", "Enabled").SetValue(true));

            Parent.Menu.AddSubMenu(Menu);
        }

        public void OnUpdate(EventArgs args)
        {
            if (SDK.Tracker.DetectedSkillshots == null)
            {
                return;
            }
            
            if (SDK.Tracker.DetectedSkillshots.Count > 0)
            {
                Game.PrintChat("[WindWallProtector]: Skillshots detected: " +SDK.Tracker.DetectedSkillshots.Count);
            }


            foreach (var skillshot in SDK.Tracker.DetectedSkillshots)
            {
                var time = (int) skillshot.MisslePosition().Distance(Variables.Player.ServerPosition) / skillshot.SData.MissileSpeed;
                Game.PrintChat(time.ToString());

                foreach (var ally in HeroManager.Allies)
                {
                    if (skillshot.IsAboutToHit(ally, 1000))
                    {
                        var GapClosePath = this.Provider.GetPath(ally.ServerPosition);

                        Game.PrintChat("Someone is about to get hit by a skillshot in: " + time);

                        if (GapClosePath != null && GapClosePath.PathTime < time)
                        {
                            Game.PrintChat("Can gapclose in time to protect ally");
                        }
                        
                        this.SafeZone = new SafeZone(skillshot.StartPosition, skillshot.SData.Range, skillshot.SData.Radius);

                        if (this.SafeZone != null && this.SafeZone.AlliesInside.Contains(ally))
                        {
                            this.Execute(this.SafeZone.CastPosition);
                        }
                    }
                }
                    
            }
        }

        public void OnDraw(EventArgs args)
        {
            this.SafeZone?.Draw();
        }

        public void Execute(Vector2 CastPosition)
        {
            if (Variables.Spells[SpellSlot.W].IsReady())
            {
                Variables.Spells[SpellSlot.W].Cast(CastPosition);
            }
        }
    }
}