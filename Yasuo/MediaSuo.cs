using System;

using LeagueSharp;
using LeagueSharp.Common;

using System.Linq;

using LeagueSharp.SDK;

namespace Yasuo
{
    using System.Collections.Generic;
    using System.Reflection;

    using Yasuo.Common;

    class MediaSuo
    {
        /**
        * TODO:
        MOST PRIO: FIX OrbWalker
        * E: Dont Dash into Wall || Dash if E hits wall if distance shorter than before || if Evade E check for Wall || Evade with E behind own wall
        * Q: Q on multiple target || E for Q (ref: Bubba Kush Lee Sin > Flash R into multiple targets
        * E through Trundle / J4 / Anivia Walls
        * Dont E under Tower in Harass/LanceClear/Combo logic
        * Add features
        *
        */

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaSuo"/> class.
        /// </summary>
        public MediaSuo(string Name = null)
        {
            if (Name == null)
            {
                Name = Assembly.GetExecutingAssembly().GetName().ToString();
            }

            Menu = new Menu(Name, Name, true);

            var info = new Menu("Info", Name + " Info", false);         
            info.AddItem(new MenuItem("Version", "Version: " + 1337));
            info.AddItem(new MenuItem("Author", "Author: " + Variables.Author));
            Menu.AddSubMenu(info);




            CustomEvents.Game.OnGameLoad += OnGameLoad;
            CustomEvents.Game.OnGameEnd +=  OnGameEnd;

            Game.OnUpdate += OnUpdate;
        }

        public event EventHandler<Base.UnloadEventArgs> OnUnload;

        public Menu Menu { get; }

        public List<IChild> Features = new List<IChild>();

        /// <summary>
        /// Called when the game loads
        /// </summary>
        /// <param name="args"></param>
        private void OnGameLoad(EventArgs args)
        {
            var orbWalkingMenu = new Menu("Orbwalking", "Orbwalking");
            Menu.AddSubMenu(orbWalkingMenu);

            Variables.Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalking"));
            Menu.AddToMainMenu();
        }

        /// <summary>
        /// Called when the game ends
        /// </summary>
        /// <param name="args"></param>
        private static void OnGameEnd(EventArgs args)
        {
            AppDomain.Unload(AppDomain.CurrentDomain);
        }

        private static void OnUpdate(EventArgs args)
        {   
            Variables.SetSpells();
        }
    }
}
