namespace Yasuo.Skills.Combo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common;
    using Yasuo.Common.Classes;
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Predictions;
    using Yasuo.Common.Provider;

    using HitChance = Yasuo.Common.Predictions.HitChance;

    internal class LastBreath : Child<Combo>
    {
        public LastBreath(Combo parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public override string Name => "Last Breath";

        public LastBreathLogicProvider Provider;

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
                    blacklist.AddItem(new MenuItem(blacklist.Name + x.Name, x.Name).SetValue(false));
                }
                MenuExtensions.AddToolTip(
                    blacklist,
                    "Setting a champion to 'on', will make the script not using R for him anymore");
            }
            this.Menu.AddSubMenu(blacklist);

            // Spell Settings
            // Hit Multiple
            this.Menu.AddItem(
                new MenuItem(this.Name + "AOE", "Try to hit multiple").SetValue(true)
                    .SetTooltip(
                        "If hit count > slider, it will try to hit multiple, else it will aim for a single champion"));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinHitAOE", "Min HitCount for AOE").SetValue(new Slider(2, 2, 5)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinPlayerHealth", "Min Player Health (%)").SetValue(new Slider(0)));

            this.Menu.AddItem(new MenuItem(this.Name + "OverkillCheck", "Overkill Check").SetValue(true)
                .SetTooltip("if EQ or Q or E will be enough to kill the target Ultimate won't execute"));




            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            this.Provider = new LastBreathLogicProvider();

            base.OnInitialize();
        }

        public void OnUpdate(EventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            List<Obj_AI_Hero> enemies = new List<Obj_AI_Hero>();
            enemies.AddRange(ObjectManager.Player.GetEnemiesInRange(Variables.Spells[SpellSlot.R].Range).Where(x => x.IsAirbone()));

            Obj_AI_Hero target = null;

            // Targetselector
            if (Menu.Item(Name + "AOE").GetValue<bool>())
            {
                if (enemies.Count >= Menu.Item(Name + "MinHitAOE").GetValue<int>())
                {
                    if (this.Provider.MostDamageDealt(enemies).CountEnemiesInRange(Variables.Spells[SpellSlot.R].Range) >= 2)
                    {
                        target = this.Provider.MostDamageDealt(enemies);
                    }
                    target = this.Provider.MostKnockedUp(enemies);
                }
            }

            if (target == null || !Provider.ShouldCastNow(target))
            {
                return;
            }

            if (Variables.Player.HealthPercent >= Menu.Item(Name +"MinPlayerHealth").GetValue<int>())
            {
                if (Menu.Item(Name + "OverkillCheck").GetValue<bool>())
                {
                    var healthAll = Provider.GetEnemiesAround(target).Sum(x => x.Health);
                    var damageAll = 0f;

                    foreach (var spell in Variables.Spells.Where(x => x.Value.IsReady() && x.Value.Slot != SpellSlot.R))
                    {
                        foreach (var enemy in Provider.GetEnemiesAround(target))
                        {
                            damageAll += spell.Value.GetDamage(enemy);
                        }
                    }

                    if (healthAll > damageAll)
                    {
                        CastLastBreath(target);
                    }
                }
                else
                {
                    this.CastLastBreath(target);
                }
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

