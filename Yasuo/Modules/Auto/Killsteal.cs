namespace Yasuo.Modules.Auto
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common.Classes;

    internal class KillSteal : Child<Modules>
    {
        public KillSteal(Modules parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public override string Name => "Killsteal";

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

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinHealthPercentage", "Min Own Health %").SetValue(new Slider(15, 0, 99)));

            #region E

            this.Menu.AddItem(new MenuItem(this.Name + "NoEintoEnemies", "Don't E into enemies").SetValue(true));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MaxEnemiesAroundE", "Max Enemies Around DashEnd Position").SetValue(
                    new Slider(2, 1, 5)));

            #endregion

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public void OnUpdate(EventArgs args)
        {
            // TODO: Add Logic for every spell.
        }

        public void Execute(Obj_AI_Base target, SpellSlot spellslot)
        {
            Variables.Spells[spellslot].Cast(target);
        }
    }
}

