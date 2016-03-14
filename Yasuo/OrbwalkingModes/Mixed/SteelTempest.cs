namespace Yasuo.OrbwalkingModes.Mixed
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common.Classes;
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Predictions;
    using Yasuo.Common.Provider;

    using HitChance = Yasuo.Common.Predictions.HitChance;

    internal class SteelTempest : Child<Mixed>
    {
        public SteelTempest(Mixed parent)
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

            // Spell Settings
            // Hit Multiple
            this.Menu.AddItem(
                new MenuItem(this.Name + "AOE", "Try to hit multiple").SetValue(true)
                    .SetTooltip(
                        "If predicted hit count > slider, it will try to hit multiple, else it will aim for a single champion"));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinHitAOE", "Min HitCount for AOE").SetValue(new Slider(1, 1, 5)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "LastHit", "LastHit with Q").SetValue(true)
                    .SetTooltip("If this is enabled, the assembly will lasthit minions with Steel Tempest"));

            this.Menu.AddItem(
                new MenuItem(this.Name + "LastHitNoQ3", "Do not LastHit with charged Q").SetValue(true)
                    .SetTooltip(
                        "if this is enabled, the assembly won't lasthit minions with Steel Tempest when it has 3 Stacks (tornado)"));

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
            if (Variables.Player.IsDashing() && pred.UnitPosition.Distance(ObjectManager.Player.ServerPosition) <= Variables.Spells[SpellSlot.Q].Range)
            {
                this.Execute(target);
            }
            if (!Variables.Player.IsDashing())
            {
                if (this.Menu.Item(this.Name + "AOE").GetValue<bool>()
                    && Variables.Player.CountEnemiesInRange(Variables.Spells[SpellSlot.Q].Range)
                    >= this.Menu.Item(this.Name + "MinHitAOE").GetValue<Slider>().Value)
                {
                    this.Execute(target, Variables.Player.HasQ3(), true);
                }
                this.Execute(target, Variables.Player.HasQ3());
            }

            #endregion
        }

        private void Execute(Obj_AI_Base target, bool hasQ3 = false, bool aoe = false)
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
