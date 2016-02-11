

namespace Yasuo.Skills.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common;
    using Yasuo.Common.Extensions;
    using Yasuo.Skills.Combo;

    using SDK = LeagueSharp.SDK;

    internal class SteelTempest : Child<Spells>
    {
        public SteelTempest(Spells parent) : base(parent)
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

        // TODO: Add Spell specific settings
        protected override sealed void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            // Blacklist
            var blacklist = new Menu("Blacklist", this.Name + "Blacklist");

            foreach (var x in HeroManager.Enemies)
            {
                blacklist.AddItem(new MenuItem(blacklist.Name + x.Name, x.Name).SetValue(false));
            }
            MenuExtensions.AddToolTip(
                blacklist,
                "Setting a champion to 'on', will make the script not using Q for him anymore");
            this.Menu.AddSubMenu(blacklist);

            // Spell Settings
            // Hit Multiple
            this.Menu.AddItem(new MenuItem(this.Name + "AOE", "Try to hit multiple").SetValue(true));
            this.Menu.AddItem(new MenuItem(this.Name + "MinHitAOE", "Min HitCount for AOE").SetValue(new Slider(2, 2, 5)));
            MenuExtensions.AddToolTip(
                this.Menu, 
                "If predicted hit count > slider, it will try to hit multiple, else it will aim for a single champion");

            // Prediction Mode
            this.Menu.AddItem(new MenuItem(this.Name + "Prediction", "Prediction").SetValue(new StringList(Variables.Predictions, 3)));
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
            var target = TargetSelector.GetSelectedTarget();

            var predOKTW = Yasuo.Common.Predictions.PredictionOKTW.GetPrediction(target, Variables.Spells[SpellSlot.Q].Delay);

            if (target == null || !target.IsValidTarget()) return;

            #region EQ

            if (!Variables.Spells[SpellSlot.Q].IsReady())
            {
                return;
            }
            
            // EQ > Synergyses with the E function in SweepingBlade/LogicProvider.cs
            if (Variables.Player.IsDashing()  
                && predOKTW.UnitPosition.Distance(ObjectManager.Player.ServerPosition) <= 375)
            {
                CastSteelTempest(false);
            }
            else
            {
                if (Variables.Player.HasQ3())
                {
                    CastSteelTempest(true);
                }
                else
                {
                    CastSteelTempest(false);
                }
            }


                #endregion
        }

        private static void CastSteelTempest(bool HasQ3 = false)
        {
            var target = TargetSelector.GetSelectedTarget();

            var predOKTW = Yasuo.Common.Predictions.PredictionOKTW.GetPrediction(target, Variables.Spells[SpellSlot.Q].Delay);

            if (HasQ3)
            {
                Variables.Spells[SpellSlot.Q].CastOnBestTarget(aoe: true);
            }
            else
            {
                if (predOKTW.Hitchance >= Yasuo.Common.Predictions.HitChance.High)
                {
                    Variables.Spells[SpellSlot.Q].Cast(predOKTW.CastPosition);
                }
            }
        }
    }
}
