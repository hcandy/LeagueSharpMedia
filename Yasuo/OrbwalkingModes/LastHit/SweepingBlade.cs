namespace Yasuo.OrbwalkingModes.LastHit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common.Classes;
    using Yasuo.Common.Provider;
    using Yasuo.Common.Utility;

    internal class SweepingBlade : Child<LastHit>
    {
        public SweepingBlade(LastHit parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public override string Name => "Sweeping Blade";

        public SweepingBladeLogicProvider Provider;

        public TurretLogicProvider ProviderTurret;

        protected override void OnEnable()
        {
            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            Game.OnUpdate -= this.OnUpdate;
            Drawing.OnDraw -= this.OnDraw;
            base.OnDisable();
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
                    .SetTooltip("The assembly will only Lasthit a minion if Q is not up and the end position of the dash is not too close to the enemy and is not inside a skillshot"));

            #endregion

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            this.Provider = new SweepingBladeLogicProvider();
            this.ProviderTurret = new TurretLogicProvider();

            base.OnInitialize();
        }

        public void OnUpdate(EventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LastHit
                || !Variables.Spells[SpellSlot.E].IsReady())
            {
                return;
            }

            Obj_AI_Base minion = null;
            List<Obj_AI_Base> minions = MinionManager.GetMinions(
                Variables.Player.ServerPosition,
                Variables.Spells[SpellSlot.E].Range,
                MinionTypes.All,
                MinionTeam.Enemy,
                MinionOrderTypes.None);

            switch (this.Menu.Item(this.Name + "ModeTarget").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    minion = MinionManager.GetMinions(Game.CursorPos, 475).Where(x => !x.HasBuff("YasuoDashWrapper") && x.Distance(Variables.Player) <= Variables.Spells[SpellSlot.E].Range)
                        .OrderByDescending(x => x.Health).FirstOrDefault();
                    break;
                case 1:
                    minion = MinionManager.GetMinions(Variables.Player.ServerPosition, 475).Where(x => !x.HasBuff("YasuoDashWrapper"))
                        .OrderByDescending(x => x.Health).FirstOrDefault();
                    break;
            }

            if (minion == null)
            {
                return;
            }

            // if EQ will hit more than X units and X units die
            if (this.Menu.Item(this.Name + "EQ").GetValue<bool>())
            {
                var minionsEq =
                    MinionManager.GetMinions(
                        Variables.Player.ServerPosition.Extend(
                            minion.ServerPosition,
                            Variables.Spells[SpellSlot.E].Range),
                        375).Where(x => x.Health <= this.Provider.GetDamage(x));

                if (minionsEq != null && minionsEq.Count() >= this.Menu.Item(this.Name + "MinHitAOE").GetValue<Slider>().Value)
                {
                    Execute(minion);
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
                List<Obj_AI_Base> possibleExecutions = new List<Obj_AI_Base>();

                foreach (var x in minions)
                {
                    foreach (var y in enemies.Where(z => z.HealthPercent > 10))
                    {
                        var newPos = Variables.Player.ServerPosition.Extend(x.ServerPosition, Variables.Spells[SpellSlot.E].Range);
                        if (newPos.Distance(y.ServerPosition) < y.AttackRange)
                        {
                            possibleExecutions.Add(x);
                        }
                    }
                        
                }

                if (possibleExecutions.Count < 0)
                {
                    return;
                }

                Execute(possibleExecutions.Where(x => this.ProviderTurret.IsSafePosition(Variables.Player.ServerPosition.Extend(x.ServerPosition, Variables.Spells[SpellSlot.E].Range))).MinOrDefault(x => x.Distance(Helper.GetMeanVector2(minions))));
            }


        }

        public void OnDraw(EventArgs args)
        {
            
        }

        private static void Execute(Obj_AI_Base target)
        {
            if (target.IsValidTarget())
            {
                Variables.Spells[SpellSlot.E].CastOnUnit(target);
            }
        }
    }
}

