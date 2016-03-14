namespace Yasuo.Modules.Protector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using SDK = LeagueSharp.SDK;

    using Yasuo.Common;
    using Yasuo.Common.Classes;
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Objects;
    using Yasuo.Common.Provider;

    using Color = System.Drawing.Color;

    internal class WindWallProtector : Child<Protector>
    {
        public WindWallProtector(Protector parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public override string Name => "Wind Wall";

        private List<GameObject> detectedObjects; 

        public SweepingBladeLogicProvider Provider;

        public WindWall WindWall;

        public SDK.Tracker Tracker;

        //public SafeZoneLogicProvider Provider;

        protected override void OnEnable()
        {
            Game.OnUpdate += this.OnUpdate;
            //GameObject.OnCreate += this.OnCreate;
            Drawing.OnDraw += this.OnDraw;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            Game.OnUpdate -= this.OnUpdate;
            //GameObject.OnCreate -= this.OnCreate;
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
            if (SDK.Tracker.DetectedSkillshots == null || SDK.Tracker.DetectedSkillshots.Count == 0)
            {
                return;
            }
            
            //var endPos = missile.EndPosition;

            //if (missile.StartPosition.Distance(endPos) < missile.SData.CastRange)
            //{
            //    endPos = missile.StartPosition.Extend(missile.EndPosition, missile.SData.CastRange);
            //}

            //var time = 1000 * missile.Position.Distance(endPos) / missile.SData.MissileSpeed - Game.Ping / 2 + missile.SData.CastFrame / 30f;

            foreach (var skillshot in SDK.Tracker.DetectedSkillshots)
            {
                //TODO: Does not work because SDK is missing too much. Solution: Pull Request or finding an alternative way of doing this
                
                foreach (var ally in HeroManager.Allies)
                {
                    var endPos = skillshot.EndPosition;

                    if (skillshot.StartPosition.Distance(endPos) < skillshot.SData.Range)
                    {
                        endPos = skillshot.StartPosition.Extend(skillshot.EndPosition, skillshot.SData.Range);
                    }

                    int time = (int)(1000 * skillshot.MissilePosition(false).Distance(endPos) / skillshot.SData.MissileSpeed - Game.Ping / 2);

                    if (skillshot.IsAboutToHit(ally, time))
                    {
                        var gapClosePath = this.Provider.GetPath(ally.ServerPosition);

                        if (gapClosePath != null && gapClosePath.PathTime < time)
                        {
                            //Game.PrintChat("Can gapclose in time to protect ally");
                        }
                        
                        this.WindWall = new WindWall(skillshot.StartPosition, skillshot.SData.Range, skillshot.SData.Radius);

                        if (this.WindWall != null && this.WindWall.AlliesInside.Contains(ally))
                        {
                            this.Execute(this.WindWall.CastPosition);
                        }
                    }
                }
                    
            }
        }

        public void OnDraw(EventArgs args)
        {
            foreach (var skillshot in SDK.Tracker.DetectedSkillshots)
            {
                Render.Circle.DrawCircle(skillshot.MissilePosition(false).To3D(), 50, Color.White);
                Drawing.DrawCircle(skillshot.MissilePosition(false).To3D(), 50, Color.White);

                Drawing.DrawText(550, 550, Color.AliceBlue, "Missile Position: " + skillshot.MissilePosition(false));
            }
        }

        public void Execute(Vector2 castPosition)
        {
            if (Variables.Spells[SpellSlot.W].IsReady() 
                && castPosition.IsValid() && castPosition != Vector2.Zero)
            {
                Variables.Spells[SpellSlot.W].Cast(castPosition);
            }
        }
    }
}