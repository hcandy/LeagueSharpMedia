namespace Yasuo.Common.Classes
{
    using LeagueSharp.Common;

    public abstract class Parent : Base
    {
        protected Parent()
        {
            this.OnLoad();
        }

        public override bool Enabled => !this.Unloaded && this.Menu != null && this.Menu.Item(this.Name + "Enabled").GetValue<bool>();

        public void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            Variables.Assembly.Menu.AddSubMenu(this.Menu);

            this.OnInitialize();            
        }
    }
}