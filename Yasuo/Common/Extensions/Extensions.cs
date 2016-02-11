namespace Yasuo.Common.Extensions
{
    using System;
    using System.Linq;

    using LeagueSharp;

    using Yasuo.Modules;
    using Yasuo.Modules.Protector;

    static class Extensions
    {
        // TODO: REMOVE WALLDASH
        /// <summary>
        /// Returns if Dash over unit is a wall dash
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="dashRange"></param>
        /// <param name="minWallWidth"></param>
        /// <returns>bool</returns>
        //public static bool IsWallDash(this Obj_AI_Base unit, float dashRange, float minWallWidth = 50)
        //{
        //    return WallDash.CanWallDash(unit, dashRange, minWallWidth);
        //}

        /// <summary>
        /// Returns the remaining airbone time from unit
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static float RemainingAirboneTime(this Obj_AI_Base unit)
        {
            float result = 0;

            foreach (var buff in unit.Buffs.Where(buff => buff.Type == BuffType.Knockback || buff.Type == BuffType.Knockup))
            {
                result = buff.EndTime - Game.Time;
            }

            return result;
        }

        /// <summary>
        /// Returns true if unit is airbone
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static bool IsAirbone(this Obj_AI_Base unit) => unit.HasBuffOfType(BuffType.Knockup) || unit.HasBuffOfType(BuffType.Knockback);

        public static bool HasQ3(this Obj_AI_Hero hero) => ObjectManager.Player.HasBuff("YasuoQ3W");

        /// <exception cref="Exception">A delegate callback throws an exception. </exception>
        public static void RaiseEvent(this EventHandler @event, object sender, EventArgs e)
        {
            if (@event != null)
            {
                @event(sender, e);
            }
        }

        /// <exception cref="Exception">A delegate callback throws an exception. </exception>
        public static void RaiseEvent<T>(this EventHandler<T> @event, object sender, T e) where T : EventArgs
        {
            if (@event != null)
            {
                @event(sender, e);
            }
        }
    }
}
