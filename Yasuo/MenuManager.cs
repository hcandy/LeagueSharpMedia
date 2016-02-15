using System.Drawing;
using LeagueSharp.Common;


namespace Yasuo
{
    using System.Collections.Generic;

    using LeagueSharp;

    public class MenuManager
    {
        private Menu RootMenu { get; }

        public MenuManager()
        {
            if (RootMenu == null)
            {
                RootMenu = new Menu(ObjectManager.Player.CharData.BaseSkinName, "media.yasuo", true);
            }
        }

        public Orbwalking.Orbwalker Orbwalker;
        public Menu Config { get; set; }

        //TODO: Add Evade Menus
        public void GenerateMenu()
        {

            Config = RootMenu;
            Config.AddSubMenu(DrawingMenu());
            Config.AddSubMenu(ComboMenu());
            Config.AddSubMenu(HarassMenu());
            Config.AddSubMenu(FarmMenu());
            Config.AddSubMenu(MiscMenu());
            Config.AddSubMenu(OrbWalkingMenu());
           
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
        }

        private List<Menu> subMenus = new List<Menu>()
        {
            DrawingMenu(),

        }; 

        public void Add(Menu subMenu)
        {
            Config.AddSubMenu(subMenu);
        }

        public static Menu OrbWalkingMenu()
        {
            var orbWalkingMenu = new Menu("Orbwalking", "Orbwalking");
            return orbWalkingMenu;
        }

        public Menu ComboMenu()
        {
            var combo1Menu = new Menu("Combo Settings", "combospells");
            {
                //Q
                combo1Menu.AddItem(new MenuItem("QUse", "Use Q (Steel Tempest)").SetValue(true));
                combo1Menu.AddItem(new MenuItem("QMode", "      Q Mode").SetValue(new StringList(new[] { "Auto", "Assisted" })));
                combo1Menu.AddItem(new MenuItem("QE", "      Use Q while E ").SetValue(true));
                combo1Menu.AddItem(new MenuItem("QEminHitCount", "      Min Hitcount QE").SetValue(new Slider(1, 1, 5)));
                combo1Menu.AddItem(new MenuItem("Seperator1", ""));
                combo1Menu.AddItem(new MenuItem("BroScience", "Activate Broscience").SetValue(false));

                //W
                combo1Menu.AddItem(new MenuItem("WUse", "Use W (Wind Wall)").SetValue(true));
                combo1Menu.AddItem(new MenuItem("WMode", "      W Mode").SetValue(new StringList(new[] { "Auto", "Only On Dangerous", "Disable" })));
                combo1Menu.AddItem(new MenuItem("PH1", "      Insert List here"));
                combo1Menu.AddItem(new MenuItem("Seperator2", ""));

                //E
                combo1Menu.AddItem(new MenuItem("EUse", "Use E (Sweeping Blade)").SetValue(true));
                combo1Menu.AddItem(new MenuItem("EMode", "      E Mode").SetValue(new StringList(new[] { "Auto", "Assisted", "To Mouse" })));
                combo1Menu.AddItem(new MenuItem("EPrefer", "      E Mode").SetValue(new StringList(new[] { "Nearest", "Furthest", "Shortest/Fastest Way" })));
                combo1Menu.AddItem(new MenuItem("EStacks", "      Stack E").SetValue(true));
                combo1Menu.AddItem(new MenuItem("EDistance", "      Min Distance to enemy").SetValue(new Slider(250, 100, 2500)));
                combo1Menu.AddItem(new MenuItem("Seperator3", ""));

                //R
                combo1Menu.AddItem(new MenuItem("RUse", "Use R (Last Breath)").SetValue(true));
                combo1Menu.AddItem(new MenuItem("RDelay", "      R Mode").SetValue(new StringList(new[] { "Auto", "Instant", "Delayed" })));
                combo1Menu.AddItem(new MenuItem("RDelayMS", "      Delay until R").SetValue(new Slider(50, 10, 2000)));
                combo1Menu.AddItem(new MenuItem("useROHP", "      Only Use R if own HP >= X%").SetValue(new Slider(50, 0, 100)));
                combo1Menu.AddItem(new MenuItem("useRTHP", "      Only Use R if target HP <= X%").SetValue(new Slider(50, 0, 100)));
                combo1Menu.AddItem(new MenuItem("Seperator4", ""));

                //AA
                combo1Menu.AddItem(new MenuItem("AAMode", "Prefer AA over Q").SetValue(false));
                combo1Menu.AddItem(new MenuItem("AAQ", "      Disable Q If Enemy in AA Range").SetValue(true).SetTooltip("In the lategame the Q does less DMG than AA the enemy"));
                combo1Menu.AddItem(new MenuItem("AAASPref", "      Disable Q If AS is higher than").SetValue(new Slider(200, 0, 250)));
            }
            return combo1Menu;
        }

        public Menu HarassMenu()
        {

            var harassMenu = new Menu("Harass Settings", "Harasssettings");
            {
                harassMenu.AddItem(new MenuItem("mMin", "Min. Flow").SetValue(new Slider(100)));
                harassMenu.AddItem(new MenuItem("UseQH", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("UseQauto", "      Auto Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("UseQHLH", "      Use Q to Last Hit Minions").SetValue(true));
                harassMenu.AddItem(new MenuItem("UseQHLH3", "      Don't use 3Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("UseEH", "Use E").SetValue(false));
                harassMenu.AddItem(new MenuItem("UseQHS", "      Stack E").SetValue(true));

            }
            return harassMenu;
        }

        public Menu FarmMenu()
        {
            var farmMenu = new Menu("Farming Settings", "farmingsettings");
            var laneMenu = new Menu("Lane Clear", "lanesettings");
            {
                laneMenu.AddItem(
                    new MenuItem("disablelane", "Lane Clear Toggle").SetValue(new KeyBind('T', KeyBindType.Toggle)));
                laneMenu.AddItem(new MenuItem("useEPL", "Min. % Mana For Lane Clear").SetValue(new Slider(50)));
                laneMenu.AddItem(new MenuItem("passiveproc", "Don't Use Spells if Passive Will Proc").SetValue(true));
                laneMenu.AddItem(new MenuItem("useQlc", "Use Q to Last Hit").SetValue(true));
                laneMenu.AddItem(new MenuItem("useWlc", "Use W to Last Hit").SetValue(false));
                laneMenu.AddItem(new MenuItem("useElc", "Use E to Last Hit").SetValue(false));
                laneMenu.AddItem(new MenuItem("useQ2L", "Use Q to Lane Clear").SetValue(true));
                laneMenu.AddItem(new MenuItem("useW2L", "Use W to Lane Clear").SetValue(false));
                laneMenu.AddItem(new MenuItem("useE2L", "Use E to Lane Clear").SetValue(false));
                laneMenu.AddItem(new MenuItem("useRl", "Use R to Lane Clear").SetValue(false));
                laneMenu.AddItem(new MenuItem("rMin", "Min. Minions to Use R").SetValue(new Slider(3, 1, 20)));
            }

            var jungleMenu = new Menu("Jungle Settings", "junglesettings");
            {
                jungleMenu.AddItem(new MenuItem("useJM", "Min. % Mana for Jungle Clear").SetValue(new Slider(50)));
                jungleMenu.AddItem(new MenuItem("useQj", "Use Q").SetValue(true));
                jungleMenu.AddItem(new MenuItem("useWj", "Use W").SetValue(true));
                jungleMenu.AddItem(new MenuItem("useEj", "Use E").SetValue(true));
                jungleMenu.AddItem(new MenuItem("useRj", "Use R").SetValue(true));
            }


            var lastMenu = new Menu("Last Hit Settings", "lastsettings");
            {
                lastMenu.AddItem(new MenuItem("useQl2h", "Use Q to Last Hit").SetValue(true));
                lastMenu.AddItem(new MenuItem("useWl2h", "Use W to Last Hit").SetValue(false));
                lastMenu.AddItem(new MenuItem("useEl2h", "Use E to Last Hit").SetValue(false));
            }

            farmMenu.AddSubMenu(laneMenu);
            farmMenu.AddSubMenu(jungleMenu);
            farmMenu.AddSubMenu(lastMenu);
            return farmMenu;
        }

        public Menu WallMenu()
        {
            var wallMenu = new Menu("Wall Jump", "wallsettings");
            {
                wallMenu.AddItem(new MenuItem("walljumpkey", "Wall Jump Key").SetValue(new KeyBind('T', KeyBindType.Press)));
                wallMenu.AddItem(new MenuItem("useWWJ", "Use W for Vision").SetValue(true));
                wallMenu.AddItem(new MenuItem("useWAWJ", "Use Trinket for Vision").SetValue(true));
                wallMenu.AddItem(new MenuItem("checkDangerWJ", "Don't Wall Jump into Skillshot").SetValue(true));
                wallMenu.AddItem(new MenuItem("dangerlevenWJ", "Danger Level Skillshot >").SetValue(new Slider(2, 1, 5)));
            }
            return wallMenu;
        }

        public Menu EvadeMenu()
        {
            var evadeMenu = new Menu("Evade", "evade");
            {
                evadeMenu.AddItem(new MenuItem("WUse", "Use W (WindWall)").SetValue(true));
                evadeMenu.AddItem(new MenuItem("EUse", "Use E (Sweeping Blade)").SetValue(true));
            }
            return evadeMenu;
        }

        public Menu MiscMenu()
        {
            var miscMenu = new Menu("Miscellaneous", "miscsettings");

            var passiveMenu = new Menu("Auto Passive", "passivesettings");
            {
                passiveMenu.AddItem(new MenuItem("ManapSlider", "Min. % Mana"))
                    .SetValue(new Slider(30));
                passiveMenu.AddItem(
                    new MenuItem("autoPassive", "Stack Passive").SetValue(new KeyBind('Z', KeyBindType.Toggle)));
                passiveMenu.AddItem(new MenuItem("stackSlider", "Keep Passive Count At"))
                    .SetValue(new Slider(3, 1, 4));
                passiveMenu.AddItem(new MenuItem("autoPassiveTimer", "Refresh Passive Every (s)"))
                    .SetValue(new Slider(5, 1, 10));
                passiveMenu.AddItem(new MenuItem("stackMana", "Min. % Mana")).SetValue(new Slider(50));
            }

            var itemMenu = new Menu("Items", "itemsettings");
            {
                itemMenu.AddItem(new MenuItem("tearS", "Auto Stack Tear").SetValue(new KeyBind('G', KeyBindType.Toggle)));
                itemMenu.AddItem(new MenuItem("tearoptions", "Stack Tear Only at Fountain").SetValue(false));
                itemMenu.AddItem(new MenuItem("tearSM", "Min % Mana to Stack Tear").SetValue(new Slider(95)));
                itemMenu.AddItem(new MenuItem("staff", "Use Seraph's Embrace").SetValue(true));
                itemMenu.AddItem(new MenuItem("staffhp", "Seraph's When % HP <").SetValue(new Slider(30)));
                itemMenu.AddItem(new MenuItem("muramana", "Use Muramana").SetValue(true));
            }

            var hpMenu = new Menu("Auto Potions", "hpsettings");
            {
                hpMenu.AddItem(new MenuItem("autoPO", "Enable Consumable Usage").SetValue(true));
                hpMenu.AddItem(new MenuItem("HP", "Auto Health Potions")).SetValue(true);
                hpMenu.AddItem(new MenuItem("HPSlider", "Min. % Health for Potion")).SetValue(new Slider(30));
                hpMenu.AddItem(new MenuItem("MANA", "Auto Mana Potion").SetValue(true));
                hpMenu.AddItem(new MenuItem("MANASlider", "Min. % Mana for Potion")).SetValue(new Slider(30));
                hpMenu.AddItem(new MenuItem("Biscuit", "Auto Biscuit").SetValue(true));
                hpMenu.AddItem(new MenuItem("bSlider", "Min. % Health for Biscuit")).SetValue(new Slider(30));
                hpMenu.AddItem(new MenuItem("flask", "Auto Flask").SetValue(true));
                hpMenu.AddItem(new MenuItem("fSlider", "Min. % Health for Flask")).SetValue(new Slider(30));
            }

            var eventMenu = new Menu("Events", "eventssettings");
            {
                eventMenu.AddItem(new MenuItem("useW2I", "Interrupt with W").SetValue(true));
                eventMenu.AddItem(new MenuItem("useQW2D", "W/Q on Dashing").SetValue(true));
                eventMenu.AddItem(new MenuItem("level", "Auto Level-Up").SetValue(true));
                eventMenu.AddItem(new MenuItem("autow", "Auto W Enemy Under Turret").SetValue(true));
            }

            var ksMenu = new Menu("Kill Steal", "kssettings");
            {
                ksMenu.AddItem(new MenuItem("KS", "Killsteal")).SetValue(true);
                ksMenu.AddItem(new MenuItem("useQ2KS", "Use Q to KS").SetValue(true));
                ksMenu.AddItem(new MenuItem("useW2KS", "Use W to KS").SetValue(true));
                ksMenu.AddItem(new MenuItem("useE2KS", "Use E to KS").SetValue(true));
            }

            var chase = new Menu("Chase Target", "Chase Target");
            {
                chase.AddItem(new MenuItem("chase", "Activate Chase")).SetValue(new KeyBind('A', KeyBindType.Press));
                chase.AddItem(new MenuItem("usewchase", "Use W")).SetValue(true);
                chase.AddItem(new MenuItem("chaser", "Use [R]")).SetValue(false);
            }

            miscMenu.AddSubMenu(passiveMenu);
            miscMenu.AddSubMenu(itemMenu);
            miscMenu.AddSubMenu(hpMenu);
            miscMenu.AddSubMenu(eventMenu);
            miscMenu.AddSubMenu(ksMenu);
            miscMenu.AddSubMenu(chase);
            return miscMenu;
        }

        public static Menu DrawingMenu()
        {
            var drawMenu = new Menu("Drawing Settings", "Drawings");
            drawMenu.AddItem(new MenuItem("Draw", "Display Drawings").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawoptions", "Drawing Mode Mode").SetValue(new StringList(new[] { "Normal Mode", "Colorblind Mode" })));
            drawMenu.AddItem(new MenuItem("Seperator1", ""));
            drawMenu.AddItem(new MenuItem("qDraw", "Draw Q").SetValue(true));
            drawMenu.AddItem(new MenuItem("eDraw", "Draw E").SetValue(true));
            drawMenu.AddItem(new MenuItem("wDraw", "Draw W").SetValue(true));
            drawMenu.AddItem(new MenuItem("rDraw", "Draw R").SetValue(true));

            return drawMenu;
        }
    }
}
