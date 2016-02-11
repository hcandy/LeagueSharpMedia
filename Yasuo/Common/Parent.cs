namespace Yasuo.Common
{
    using LeagueSharp.Common;

    public abstract class Parent : Base
    {
        protected Parent()
        {
            OnLoad();
        }

        public override bool Enabled => !this.Unloaded && this.Menu != null && this.Menu.Item(this.Name + "Enabled").GetValue<bool>();

        public void OnLoad()
        {
            Menu = new Menu(Name, Name);

            Menu.AddItem(new MenuItem(Name + "Enabled", "Enabled").SetValue(true));

            Variables.Assembly.Menu.AddSubMenu(Menu);

            OnInitialize();            
        }
    }
}