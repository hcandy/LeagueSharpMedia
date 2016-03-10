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
    using Yasuo.Common.Objects;

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
                    blacklist.AddItem(new MenuItem(blacklist.Name + x.ChampionName, x.ChampionName).SetValue(false));
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
            try
            {
                if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo || !Variables.Spells[SpellSlot.R].IsReady())
                {
                    return;
                }

                var enemies = new List<Obj_AI_Hero>();
                enemies.AddRange(HeroManager.Enemies.Where(enemy => enemy.IsAirbone()));

                var possibleExecutions = new List<Common.Objects.LastBreath>();
                var validatedExecutions = new List<Common.Objects.LastBreath>();
                if (enemies.Count > 0)
                {
                    possibleExecutions.AddRange(enemies.Select(enemy => new Yasuo.Common.Objects.LastBreath(enemy)));
                }

                var execution = new Yasuo.Common.Objects.LastBreath(null);

                // Menu: Min Hit AOE && AOE
                if (Menu.Item(Name + "AOE").GetValue<bool>())
                {
                    validatedExecutions.AddRange(possibleExecutions.Where(entry => entry.EnemiesInUlt >= this.Menu.Item(this.Name + "MinHitAOE").GetValue<Slider>().Value));
                }
                else
                {
                    validatedExecutions = possibleExecutions;
                }

                // TODO: Add a lot more stuff here
                #region TargetSelector

                if (validatedExecutions.Count > 0)
                {
                    execution = validatedExecutions.MaxOrDefault(x => x.DamageDealt);
                }

                #endregion

                if (execution == null || !Provider.ShouldCastNow(execution.Target))
                {
                    return;
                }

                // Menu: Min Player Health
                if (Variables.Player.HealthPercent <= this.Menu.Item(this.Name + "MinPlayerHealth").GetValue<Slider>().Value)
                {
                    return;
                }

                // Menu: Overkill Check TODO: Improve that
                if (this.Menu.Item(this.Name + "OverkillCheck").GetValue<bool>())
                {
                    var healthAll = 0f;
                    var damageAll = 0f;

                    if (execution.AffectedEnemies.Count > 0)
                    {
                        healthAll += execution.AffectedEnemies.Sum(enemy => enemy.Health);
                    }
                    else
                    {
                        healthAll = execution.Target.Health;
                    }

                    foreach (var spell in Variables.Spells.Where(x => x.Value.IsReady() && x.Value.Slot != SpellSlot.R && x.Value.Slot != SpellSlot.W))
                    {
                        foreach (var enemy in this.Provider.GetEnemiesAround(execution.EndPosition))
                        {
                            damageAll += spell.Value.GetDamage(enemy);
                        }
                    }

                    if (healthAll > damageAll)
                    {
                        Game.PrintChat(@"healthAll:" + healthAll);
                        Game.PrintChat(@"damageAll:" + damageAll);

                        this.Execute(execution.Target);
                    }
                }
                else
                {
                    this.Execute(execution.Target);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
        }

        private void Execute(Obj_AI_Hero target)
        {
            if (target.IsValid && !target.IsZombie && target.IsAirbone())
            {
                Variables.Spells[SpellSlot.R].CastOnUnit(target);
            }
        }
    }
}

