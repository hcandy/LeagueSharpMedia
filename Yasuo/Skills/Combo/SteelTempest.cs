//TODO: Q After W/AA, Enhance Q Logic in general

namespace Yasuo.Skills.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common;
    using Yasuo.Common.Classes;
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Predictions;
    using Yasuo.Common.Provider;

    using HitChance = Yasuo.Common.Predictions.HitChance;

    internal class SteelTempest : Child<Combo>
    {
        public SteelTempest(Combo parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public override string Name => "Steel Tempest";

        public SteelTempestLogicProvider ProviderQ;

        public SweepingBladeLogicProvider ProviderE;

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
                    "Setting a champion to 'on', will make the script not using Q for him anymore");
            }
            this.Menu.AddSubMenu(blacklist);

            // Spell Settings
            // Hit Multiple
            this.Menu.AddItem(
                new MenuItem(this.Name + "AOE", "Try to hit multiple").SetValue(true)
                    .SetTooltip(
                        "If predicted hit count > slider, it will try to hit multiple, else it will aim for a single champion"));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinHitAOE", "Min HitCount for AOE").SetValue(new Slider(2, 2, 5)));

            // Prediction Mode
            //this.Menu.AddItem(new MenuItem(this.Name + "Prediction", "Prediction").SetValue(new StringList(Variables.Predictions, 0)));
            //Menu.AddItem(new MenuItem(Name + "Prediction Mode", "Prediction Mode").SetValue(new Slider(5, 0, 0)));
            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            this.ProviderQ = new SteelTempestLogicProvider();
            this.ProviderE = new SweepingBladeLogicProvider();

            base.OnInitialize();
        }

        public void OnUpdate(EventArgs args)
        {

            var target = TargetSelector.GetTarget(
                Variables.Spells[SpellSlot.Q].Range,
                TargetSelector.DamageType.Physical);
            var pred = PredictionOktw.GetPrediction(target, Variables.Spells[SpellSlot.Q].Delay);

            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo 
                || target == null || !target.IsValidTarget())
            {
                return;
            }

            #region EQ

            if (!Variables.Spells[SpellSlot.Q].IsReady())
            {
                return;
            }

            // EQ > Synergyses with the E function in SweepingBlade/LogicProvider.cs
            if (Variables.Player.IsDashing() && target.Distance(ObjectManager.Player.ServerPosition) <= 375)
            {
                Execute(target);
            }
            if (!Variables.Player.IsDashing())
            {
                if (Menu.Item(Name + "AOE").GetValue<bool>()
                    && Variables.Player.CountEnemiesInRange(Variables.Spells[SpellSlot.Q].Range)
                    >= Menu.Item(Name + "MinHitAOE").GetValue<Slider>().Value)
                {
                    Execute(target, Variables.Player.HasQ3(), true);
                }
                Execute(target, Variables.Player.HasQ3());
            }

            #endregion
        }

        private static void Execute(Obj_AI_Base target, bool hasQ3 = false, bool aoe = false)
        {
            var pred = PredictionOktw.GetPrediction(target, Variables.Spells[SpellSlot.Q].Delay);

            if (hasQ3)
            {
                if (aoe)
                {
                    Variables.Spells[SpellSlot.Q].CastOnBestTarget(aoe: true);
                }
                else
                {
                    Variables.Spells[SpellSlot.Q].Cast(pred.CastPosition);
                }
            }
            else
            {
                if (pred.Hitchance >= HitChance.High)
                {
                    Variables.Spells[SpellSlot.Q].Cast(pred.CastPosition);
                }
            }
        }
    }
}
