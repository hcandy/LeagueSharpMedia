//TODO:  TEMPLATE
/*



namespace Yasuo.Common.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common.Extensions;
    using Yasuo.Common.Provider;
    using Yasuo.Modules.WallDash;

    internal class ClassName : Child<ChildName>
    {
        public ClassName(ChildName parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public override string Name => "ClassName";

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

            
            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public void OnUpdate(EventArgs args)
        {

        }

        public void OnDraw(EventArgs args)
        {

        }
    }
}



*/
