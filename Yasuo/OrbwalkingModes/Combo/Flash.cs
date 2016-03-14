namespace Yasuo.OrbwalkingModes.Combo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common.Classes;
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Provider;

    internal class Flash : Child<Combo>
    {
        public Flash(Combo parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public override string Name => "Flash";

        public FlashLogicProvider Provider;

        protected override void OnEnable()
        {
            Game.OnUpdate += this.OnUpdate;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            Game.OnUpdate -= this.OnUpdate;
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
                    blacklist.AddItem(new MenuItem(blacklist.Name + x.ChampionName, x.ChampionName).SetValue(false));
                }
                MenuExtensions.AddToolTip(
                    blacklist,
                    "Setting a champion to 'on', will make the script not using Flash for him anymore");
            }
            this.Menu.AddSubMenu(blacklist);

            this.Menu.AddItem(new MenuItem(this.Name + "EQFlash", "Do EQ Flash").SetValue(true)
                .SetTooltip("If enabled the assembly will perfom an EQ Flash Combo when it can hit X enemies"));
            this.Menu.AddItem(new MenuItem(this.Name + "MinHitCount", "Min HitCount EQ Flash").SetValue(new Slider(3, 1, 5)));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            this.Provider = new FlashLogicProvider();

            base.OnInitialize();
        }

        public void OnUpdate(EventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            foreach (var minion in 
                MinionManager.GetMinions(
                    Variables.Player.ServerPosition, Variables.Spells[SpellSlot.E].Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.None)
                        .Where(x => !x.HasBuff("YasuoDashWrapper")).ToList())
            {
                //TODO: Add logic.
            }

        }

        private void CastLastBreath(Obj_AI_Hero target)
        {
            if (target.IsValid && !target.IsZombie)
            {
                Variables.Spells[SpellSlot.R].Cast(target);
            }
        }
    }
}

