namespace Yasuo.Skills.LaneClear
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.SDK.Core.Utils;

    using Yasuo.Common.Classes;
    using Yasuo.Common.Provider;
    using Yasuo.Common.Utility;

    using SkillshotType = LeagueSharp.SDK.SkillshotType;

    internal class SteelTempest : Child<LaneClear>
    {
        #region Fields

        public SweepingBladeLogicProvider ProviderE;

        public SteelTempestLogicProvider ProviderQ;

        #endregion

        #region Constructors and Destructors

        public SteelTempest(LaneClear parent)
            : base(parent)
        {
            this.OnLoad();
        }

        #endregion

        #region Public Properties

        public override string Name => "Steel Tempest";

        #endregion

        #region Public Methods and Operators

        #endregion

        #region Methods

        protected override void OnDisable()
        {
            Game.OnUpdate -= this.OnUpdate;
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            Game.OnUpdate += this.OnUpdate;
            base.OnEnable();
        }

        protected override void OnInitialize()
        {
            this.ProviderQ = new SteelTempestLogicProvider();
            this.ProviderE = new SweepingBladeLogicProvider();

            base.OnInitialize();
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
                new MenuItem(this.Name + "CenterCheck", "Check for the minions mean vector").SetValue(true)
                    .SetTooltip(
                        "if this is enabled, the assembly will try to not use stacked/charged Q inside many minions and will either wait until the buff runs out or until you are further away from the minions to hit more."));

            this.Menu.AddItem(
                new MenuItem(this.Name + "EQ", "Do EQ").SetValue(true)
                    .SetTooltip("If this is enabled, the assembly will try to hit minions while dashing"));

            // Prediction Mode
            //this.Menu.AddItem(new MenuItem(this.Name + "Prediction", "Prediction").SetValue(new StringList(Variables.Predictions, 0)));
            //Menu.AddItem(new MenuItem(Name + "Prediction Mode", "Prediction Mode").SetValue(new Slider(5, 0, 0)));
            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        public void OnUpdate(EventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != LeagueSharp.Common.Orbwalking.OrbwalkingMode.LaneClear || !Variables.Spells[SpellSlot.Q].IsReady())
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
            if (Menu.Item(this.Name + "EQ").GetValue<bool>()
                && (Variables.Player.IsDashing()
                    && minions.Where(x => x.Health <= ProviderQ.GetDamage(x))
                           .Count(x => x.Distance(Variables.Player) <= 375) > 2))
            {
                Execute(minions);
            }

            #endregion

            #region Unstacked Q and Stacked Q

            else
            {
                if (Variables.Player.Spellbook.IsAutoAttacking || Variables.Player.Spellbook.IsCharging
                    || Variables.Player.Spellbook.IsChanneling)
                {
                    return;
                }

                // Mass lane clear logic
                if (ProviderQ.HasQ3())
                {
                    // if AOE is enabled and more than X units are around us.
                    if (Menu.Item(this.Name + "AOE").GetValue<bool>()
                        && Menu.Item(this.Name + "MinHitAOE").GetValue<Slider>().Value
                        <= minions.Where(x => !x.InAutoAttackRange()).ToList().Count)
                    {
                        // Check for the minions centered position and wait until we are a bit away
                        if (Menu.Item(this.Name + "CenterCheck").GetValue<bool>()
                            && Variables.Player.Distance(Helper.GetMeanVector2(minions)) > 450
                            || minions.Where(x => !x.InAutoAttackRange()).ToList().Count > 15
                            || ProviderQ.BuffTime() <= 10)
                        {
                            minions = minions.Where(x => !x.InAutoAttackRange()).ToList();
                            Execute(minions, true);
                        }
                        else if (!Menu.Item(this.Name + "CenterCheck").GetValue<bool>())
                        {
                            minions = minions.Where(x => !x.InAutoAttackRange()).ToList();
                            Execute(minions, true);
                        }
                    }
                }

                // Stack Logic
                // TODO: Add Health Prediction
                else
                {
                    Execute(minions, tryStacking: true);
                }
            }

            #endregion
        }

        private void Execute(List<Obj_AI_Base> units, bool aoe = false, bool circular = false, bool tryStacking = false)
        {
            if (aoe)
            {
                var pred = MinionManager.GetBestLineFarmLocation(
                    units.Select(m => m.ServerPosition.To2D()).ToList(),
                    Variables.Spells[SpellSlot.Q].Width,
                    Variables.Spells[SpellSlot.Q].Range);

                Variables.Spells[SpellSlot.Q].Cast(pred.Position);
            }
            if (circular)
            {
                Variables.Spells[SpellSlot.Q].Cast(
                    units.Where(x => x.Distance(Variables.Player) <= 375).MinOrDefault(x => x.Health));
            }
            if (tryStacking)
            {
                var predPositions = Minion.GetMinionsPredictedPositions(
                    units,
                    Variables.Spells[SpellSlot.Q].Delay,
                    Variables.Spells[SpellSlot.Q].Width,
                    Variables.Spells[SpellSlot.Q].Speed,
                    Variables.Player.ServerPosition,
                    Variables.Spells[SpellSlot.Q].Range,
                    false,
                    SkillshotType.SkillshotLine);

                var pred = predPositions.MaxOrDefault(x => x.Distance(Variables.Player));
                Variables.Spells[SpellSlot.Q].Cast(pred);
            }
        }

        #endregion
    }
}