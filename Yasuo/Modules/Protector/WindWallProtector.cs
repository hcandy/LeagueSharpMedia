namespace Yasuo.Modules.Protector
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SDK = LeagueSharp.SDK;

    using Yasuo.Common;
    using Yasuo.Common.Extensions;

    internal class WindWallProtector : Child<Protector>
    {
        public WindWallProtector(Protector parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public override string Name => "Wind Wall";

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

        public bool CheckForCollision(GameObjectProcessSpellCastEventArgs args)
        {
            for (int i = 0; i < 5; i++)
            {
                if (SDK.SpellDatabase.GetByName(args.SData.Name).CollisionObjects[i]
                    == SDK.CollisionableObjects.YasuoWall)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnUpdate(EventArgs args)
        {
            foreach (var skillshot in SDK.Tracker.DetectedSkillshots)
            {
                foreach (var ally in HeroManager.Allies)
                {
                    var time = (int) skillshot.MisslePosition().Distance(ally.ServerPosition) / skillshot.SData.MissileSpeed;

                    if (skillshot.IsAboutToHit(ally, time))
                    {
                        Game.PrintChat("Ally is about to get hit by a skillshot in: " +time);
                        SafeZone = new SafeZone(Variables.Player.ServerPosition.To2D(), skillshot.SData.Range, skillshot.SData.Radius);
                    }
                }
            }
        }

        public void OnDraw(EventArgs args)
        {
            SafeZone.Draw();
        }
    }
}