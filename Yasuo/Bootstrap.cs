﻿// TODO: Add automatically Version changing based on the current date.

namespace Yasuo
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.SDK;

    using Yasuo.Common;
    using Yasuo.Common.Classes;
    using Yasuo.Common.Utility;
    using Yasuo.Modules;
    using Yasuo.Modules.Protector;
    using Yasuo.Modules.WallDash;
    using Yasuo.Skills.Combo;
    using Yasuo.Skills.LaneClear;
    using Yasuo.Skills.JungleClear;
    using Yasuo.Skills.Mixed;

    using Notifications = LeagueSharp.Common.Notifications;

    internal class Bootstrap
    {
        public Bootstrap()
        {
            try
            {
                Variables.Assembly = new Assembly("Yasuo");

                LoadedSuccessfully(Variables.Name + " by "+Variables.Author, 1337, 2500);

                #region parents

                // Orbwalking Modes
                var combo = new Combo();
                var laneclear = new LaneClear();
                var jungleclear = new JungleClear();
                var mixed = new Mixed();

                // Extra Features
                var module = new Modules.Modules();
                var protector = new Protector();

                #endregion

                // TODO: Add Evade Skills (Sweeping Blade/Wind Wall)
                // TODO: Add AntiGapCloser Skills (SweepingBlade)
                #region features

                CustomEvents.Game.OnGameLoad += delegate
                    {
                        Variables.Assembly.Features.AddRange(
                            new List<IChild>
                                {
                                    // Orbwalking Modes
                                    new Skills.Combo.SteelTempest(combo),
                                    new Skills.Combo.SweepingBlade(combo),
                                    new Skills.Combo.LastBreath(combo),
                                    new Skills.Combo.Flash(combo),

                                    new Skills.LaneClear.SteelTempest(laneclear),
                                    new Skills.LaneClear.SweepingBlade(laneclear),

                                    new Skills.JungleClear.SteelTempest(jungleclear),
                                    new Skills.JungleClear.SweepingBlade(jungleclear),

                                    new Skills.Mixed.SteelTempest(mixed),
                                    new Skills.Mixed.SweepingBlade(mixed),

                                    // Extra Features - Disabled due to SDK/Core problems
                                    //new WallDash(module),
                                    //new WindWallProtector(protector)
                                });

                        foreach (var feature in Variables.Assembly.Features)
                        {
                            feature.HandleEvents();
                        }
                    };

                #endregion
            }

            catch (Exception ex)
            {
                Console.WriteLine(@"Fehler beim Laden: "+ex);
            }
        }

        private void LoadedSuccessfully(String name, int version, int displayTime)
        {
            // TODO: Add a little banner, like OKTW but way smaller and not centered. Also it does not load on f5 or f8.
            Notifications.AddNotification(
                string.Format("[{0}] {1} - loaded successfully!", name, version),
                displayTime,
                true);
            //Menu.GetMenu(Variables.Name, "Info").AddItem(new MenuItem("AmountFeatures", "Amount of features: " + Variables.Assembly.Features.Count));
            if (Game.Time < 1000 * 60)
                // Assuming Game.Time 1000 = 1s
            {
                Console.WriteLine(@"Load Banner");
            }
        }
    }
}
