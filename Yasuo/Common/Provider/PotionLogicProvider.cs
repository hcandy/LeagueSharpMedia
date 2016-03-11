// TODO: Much.
namespace Yasuo.Common.Provider
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.Common.Data;

    class PotionLogicProvider
    {
        public int PotionValue(int amount)
        {
            var value = 150;
            var mod = (int) Variables.Player.FlatHPRegenMod;
            
            // TODO: Add Resolve Masteries

            return (value * mod) * amount;
        }

        public float HealthRecovered(int time = int.MaxValue, int amount = 0)
        {
            // No potions
            if (!Items.HasItem(2003, Variables.Player))
            {
                return 0;
            }

            //TODO: amount = item count
            // Time is relative, so its amount * bufftime
            if (time == int.MaxValue)
            {
                time = amount * 15000;
            }

            if (amount == 0)
            {

            }
            return PotionValue(amount);
        }
    }
}
