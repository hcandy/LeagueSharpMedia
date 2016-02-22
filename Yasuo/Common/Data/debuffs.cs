namespace Yasuo.Common.Data
{
    using System.Collections.Generic;

    using LeagueSharp;

    class Debuffs
    {
        public static List<DebuffStruct> debuffs = new List<DebuffStruct>
        {
                                       
        };

        public enum PotionMode
        {
            Instant, Delayed
        }

        public struct DebuffStruct
        {
            public readonly string BuffName;

            public readonly int DamageValue;

            public readonly int Time;

            public DebuffStruct(string buffName, int time, int damageValue)
            {
                BuffName = buffName;
                Time = time;
                DamageValue = damageValue;
            }
        }
    }
}
