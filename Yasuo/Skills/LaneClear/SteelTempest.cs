namespace Yasuo.Skills.LaneClear
{
    using System;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common;
    using Yasuo.Common.Classes;
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Predictions;
    using Yasuo.Skills.Combo;
    using Yasuo.Common.Provider;

    using HitChance = Yasuo.Common.Predictions.HitChance;
    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class SteelTempest : Child<LaneClear>
    {
        public SteelTempest(LaneClear parent)
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
                        "If predicted hit count > slider, it will try to hit multiple, else it will aim for a single minion"));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinHitAOE", "Min HitCount for AOE").SetValue(new Slider(2, 2, 5)));

            this.Menu.AddItem(
    new MenuItem(this.Name + "EQ", "Do EQ").SetValue(true)
        .SetTooltip(
            "If this is enabled, the assembly will try to hit minions while dashing"));

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
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear || !Variables.Spells[SpellSlot.Q].IsReady())
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                Variables.Player.ServerPosition,
                Variables.Spells[SpellSlot.Q].Range,
                MinionTypes.All,
                MinionTeam.Enemy,
                MinionOrderTypes.None);

            if (minions.Count == 0 || minions == null)
            {
                return;
            }
            
            #region EQ

            // EQ > Synergyses with the E function in SweepingBlade/LogicProvider.cs
            if (Menu.Item(this.Name + "EQ").GetValue<bool>() &&
                (Variables.Player.IsDashing() && minions.Where(x => x.Health <= ProviderQ.GetDamage(x)).Count(x => x.Distance(Variables.Player) <= 375) > 2))
            {
                Execute(minions.Where(x => x.Distance(Variables.Player) <= 375).MinOrDefault(x => x.Health));
            }

            #endregion

            else
            {
                if (Variables.Player.Spellbook.IsAutoAttacking || Variables.Player.Spellbook.IsCharging || Variables.Player.Spellbook.IsChanneling) return;
                
                if (Menu.Item(this.Name + "AOE").GetValue<bool>())
                {
                    Execute(minions.MinOrDefault(x => x.Health));
                }
            }



        }

        private static void Execute(Obj_AI_Base unit, bool AOE = false)
        {
            if (AOE)
            {
                Variables.Spells[SpellSlot.Q].CastOnBestTarget(aoe: true);
            }
            else
            {
                
            }
        }
    }
}
