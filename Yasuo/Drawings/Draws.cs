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

        public FlowLogicProvider Provider;

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

            
            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            Provider = new FlowLogicProvider();
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
            if (Provider.GetRemainingUnits() > 0)
            {
                Render.Circle.DrawCircle(Variables.Player.Position, Provider.GetRemainingUnits(), Color.White);
            }

            #endregion

            #region Spells

            //if (Variables.Spells[SpellSlot.Q].Level >= 1)
            //{
            //    Render.Circle.DrawCircle(Variables.Player.Position, Program.Q.Range, Color.White);
            //}


            //if (Variables.Spells[SpellSlot.W].Level > 1)
            //{
            //    Render.Circle.DrawCircle(Variables.Player.Position, Program.W.Range, Color.White);
            //}

            //if (Variables.Spells[SpellSlot.R].Level > 1)
            //{
            //    Render.Circle.DrawCircle(Variables.Player.Position, Program.R.Range, Color.White);
            //}

            foreach (var spell in Variables.Spells.Where(spell => spell.Value.Level > 0).Where(spell => spell.Value.Range > 0))
            {
                Render.Circle.DrawCircle(Variables.Player.ServerPosition, spell.Value.Range, Color.White);
            }

            #endregion
        }
    }
}