namespace Yasuo.Drawings
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Yasuo.Common.Classes;
    using Yasuo.Common.Provider;
    using Yasuo.Modules;

    using Color = System.Drawing.Color;

    internal class Draws : Child<Drawings>
    {
        public Draws(Drawings parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public FlashLogicProvider ProviderF;
        public FlowLogicProvider ProviderP;
        public LastBreathLogicProvider ProviderR; 
        public PotionLogicProvider ProviderPotion;
        public SteelTempestLogicProvider ProviderQ;
        public WindWallLogicProvider ProviderW;
        public SweepingBladeLogicProvider ProviderE;

        public override string Name => "Drawings";

        protected override void OnEnable()
        {
            Drawing.OnDraw += this.OnDraw;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            Game.OnUpdate -= this.OnDraw;
            base.OnDisable();
        }

        protected override sealed void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);
            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            foreach (var spell in Variables.Spells)
            {
                Menu.AddItem(
                    new MenuItem(this.Name + spell.Key.ToString(), spell.Key.ToString() + " Enabled").SetValue(true)
                        .SetTooltip("Disabling this spell will disable drawings for this spell"));
            }

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            this.ProviderF = new FlashLogicProvider();
            this.ProviderP = new FlowLogicProvider();
            this.ProviderR = new LastBreathLogicProvider();
            this.ProviderPotion = new PotionLogicProvider();
            this.ProviderQ = new SteelTempestLogicProvider();
            this.ProviderW = new WindWallLogicProvider();
            this.ProviderE = new SweepingBladeLogicProvider();
            base.OnInitialize();
        }

        void OnDraw(EventArgs args)
        {
            if (Variables.Player.IsDead) return;

            #region SafeZone

            //if (TargetSelector.GetSelectedTarget() != null)
            //{
            //    SafeZone.Safezone(TargetSelector.GetSelectedTarget().ServerPosition, 1000f).Draw(System.Drawing.Color.White, 2);
            //}

            #endregion

            #region Yasuo Passive Shield (Flow)

            //Max Flow Distance
            if (this.ProviderP.GetRemainingUnits() > 0)
            {
                Render.Circle.DrawCircle(Variables.Player.Position, this.ProviderP.GetRemainingUnits(), Color.White);
            }

            #endregion

            #region Spells

            //if Skillshot has a Range greater than Zero and Level is Higher than Zero. Need to connect that with the Menu.
            foreach (var spell in Variables.Spells.Where(spell => spell.Value.Level > 0 && spell.Value.Range > 0))
            {
                if (Menu.Item(this.Name + spell.Key.ToString()).GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Variables.Player.Position, spell.Value.Range, Color.White);
                }
            }

            #endregion
        }
    }
}