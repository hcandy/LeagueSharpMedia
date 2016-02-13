// TODO: Add automatically Version changing based on the current date.

namespace Yasuo
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common;
    using Yasuo.Common.Utility;
    using Yasuo.Skills;
    using Yasuo.Skills.Combo;

    internal class Bootstrap
    {
        public Bootstrap()
        {
            try
            {
                Variables.Assembly = new MediaSuo("Yasuo");

                LoadedSuccessfully(Variables.Name + " by "+Variables.Author, 1337, 2500);

                #region parents

                var combo = new Combo();
                //var Items = new Yasuo.Items.Item();

                #endregion


                #region features

                CustomEvents.Game.OnGameLoad += delegate
                    {
                        Variables.Assembly.Features.AddRange(
                            new List<IChild>
                                {
                                    new SteelTempest(combo),
                                    new SweepingBlade(combo)
                                });

                        /*      new WindWall(evade),
                        
                        new LastBreath(combo)  */

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

            if (Game.Time < 1000 * 60)
                // Assuming Game.Time 1000 = 1s
            {
                Console.WriteLine(@"Load Banner");
            }
        }
    }
}
