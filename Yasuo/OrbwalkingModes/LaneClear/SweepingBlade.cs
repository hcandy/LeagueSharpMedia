// TODO: Add new Dash Object to make things easier

namespace Yasuo.OrbwalkingModes.LaneClear
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common.Classes;
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Provider;
    using Yasuo.Common.Utility;

    internal class SweepingBlade : Child<LaneClear>
    {
        #region Fields

        public List<Obj_AI_Base> BlacklistUnits;

        public SweepingBladeLogicProvider ProviderE;

        public TurretLogicProvider ProviderTurret;

        #endregion

        #region Constructors and Destructors

        public SweepingBlade(LaneClear parent)
            : base(parent)
        {
            this.OnLoad();
        }

        #endregion

        #region Public Properties

        public override string Name => "Sweeping Blade";

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
            this.ProviderE = new SweepingBladeLogicProvider();
            this.ProviderTurret = new TurretLogicProvider();

            base.OnInitialize();
        }

        protected override sealed void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);
            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            // Mode
            this.Menu.AddItem(
                new MenuItem(this.Name + "ModeTarget", "Dash to: ").SetValue(new StringList(new[] { "Mouse", "Auto" })));

            // EQ

            #region EQ

            this.Menu.AddItem(
                new MenuItem(this.Name + "EQ", "Try to E for EQ").SetValue(true)
                    .SetTooltip("The assembly will try to E on a minion in order to Q"));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinHitAOE", "Min HitCount for AOE").SetValue(new Slider(1, 1, 15)));

            #endregion

            #region E LastHit

            this.Menu.AddItem(
                new MenuItem(this.Name + "LastHit", "Smart Lasthit").SetValue(true)
                    .SetTooltip(
                        "The assembly will only Lasthit a minion if Q is not up and the end position of the dash is not too close to the enemy and is not inside a skillshot"));

            #endregion

            #region Misc

            this.Menu.AddItem(
                new MenuItem(this.Name + "NoWallJump", "Anti WallDash").SetValue(true)
                    .SetTooltip(
                        "if this is enabled, the assembly won't use Sweeping Blade on a unit if it is a walljump. This is especially useful when doing jungle clear"));

            #endregion

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        void OnUpdate(EventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != LeagueSharp.Common.Orbwalking.OrbwalkingMode.LaneClear || !Variables.Spells[SpellSlot.E].IsReady())
            {
                return;
            }

            Obj_AI_Base minion = null;
            var minions = MinionManager.GetMinions(
                Variables.Player.ServerPosition,
                Variables.Spells[SpellSlot.E].Range,
                MinionTypes.All,
                MinionTeam.Enemy,
                MinionOrderTypes.None);

            switch (this.Menu.Item(this.Name + "ModeTarget").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    minion =
                        MinionManager.GetMinions(Game.CursorPos, 475)
                            .Where(
                                x =>
                                !x.HasBuff("YasuoDashWrapper")
                                && x.Distance(Variables.Player) <= Variables.Spells[SpellSlot.E].Range)
                            .OrderByDescending(x => x.Health)
                            .FirstOrDefault();
                    break;
                case 1:
                    minion =
                        MinionManager.GetMinions(Variables.Player.ServerPosition, 475)
                            .Where(x => !x.HasBuff("YasuoDashWrapper"))
                            .OrderByDescending(x => x.Health)
                            .FirstOrDefault();
                    break;
            }

            if (minion == null)
            {
                return;
            }

            // if EQ will hit more than X units
            if (this.Menu.Item(this.Name + "EQ").GetValue<bool>()
                && Variables.Player.ServerPosition.Extend(minion.ServerPosition, Variables.Spells[SpellSlot.E].Range)
                       .CountMinionsInRange(375) > this.Menu.Item(this.Name + "MinHitAOE").GetValue<Slider>().Value
                && Variables.Player.Health > 100)
            {
                if (Variables.Spells[SpellSlot.Q].IsReady() && Variables.Spells[SpellSlot.Q].Level > 0)
                {
                    this.Execute(minion);
                }
            }

            // Smart Last Hit
            if (this.Menu.Item(this.Name + "LastHit").GetValue<bool>())
            {
                if (minions == null)
                {
                    return;
                }

                var enemies = HeroManager.Enemies.Where(x => x.Health > 0).ToList();
                var possibleExecutions = new List<Obj_AI_Base>();

                foreach (
                    var x in
                        minions.Where(
                            unit =>
                            unit.Health <= this.ProviderE.GetDamage(unit)
                            && unit.Distance(Variables.Player.ServerPosition) <= Variables.Spells[SpellSlot.E].Range))
                {
                    if (enemies.Count(enemy => enemy.Distance(x.ServerPosition) <= 1000) > 0)
                    {
                        foreach (var y in enemies.Where(z => z.HealthPercent > 10))
                        {
                            var newPos = Variables.Player.ServerPosition.Extend(
                                x.ServerPosition,
                                Variables.Spells[SpellSlot.E].Range);
                            if (newPos.Distance(y.ServerPosition) < y.AttackRange + 475)
                            {
                                possibleExecutions.Add(x);
                            }
                        }
                    }
                    else
                    {
                        possibleExecutions.Add(minion);
                    }
                }

                if (possibleExecutions.Count == 0)
                {
                    return;
                }

                this.Execute(possibleExecutions.MinOrDefault(x => x.Distance(Helper.GetMeanVector2(minions))));
            }
        }

        private void Execute(Obj_AI_Base unit)
        {
            // Check if Dash End position is safe under turret
            if (unit.IsValidTarget() && unit != null
                && this.ProviderTurret.IsSafePosition(
                    Variables.Player.ServerPosition.Extend(unit.ServerPosition, Variables.Spells[SpellSlot.E].Range)))
            {
                if (this.Menu.Item(this.Name + "NoWallJump").GetValue<bool>())
                {
                    if (unit.IsWallDash(Variables.Spells[SpellSlot.E].Range, 20))
                    {
                        return;
                    }
                }
                Variables.Spells[SpellSlot.E].CastOnUnit(unit);
            }
        }

        #endregion
    }
}