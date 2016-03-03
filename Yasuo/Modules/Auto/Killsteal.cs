//TODO:  Djikstra

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
                new MenuItem(this.Name + "MinHealthPercentage", "Min Own Health %").SetValue(new Slider(35, 0, 99)));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public void OnUpdate(EventArgs args)
        {

        }

        public void Execute(Obj_AI_Base target, SpellSlot spellslot)
        {
            Variables.Spells[spellslot].Cast(target);
        }
    }
}

