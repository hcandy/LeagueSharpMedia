namespace Yasuo.Common.Extensions
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Modes.Weights;

    using SharpDX;

    using Yasuo.Modules;
    using Yasuo.Modules.Protector;

    using Geometry = LeagueSharp.Common.Geometry;

    static class Extensions
    {
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

        public static Vector2 MisslePosition(this Skillshot skillshot)
        {
            if (!skillshot.HasMissile)
            {
                return Vector2.Zero;
            }

            var speed = skillshot.SData.MissileSpeed;
            var distance = skillshot.StartPosition.Distance(skillshot.EndPosition);
            var starttime = skillshot.StartTime;
            var direction = skillshot.Direction;

            var traveleddistance = (Game.Time - starttime) * speed;

            return traveleddistance <= distance ? skillshot.StartPosition.Extend(direction, traveleddistance) : Vector2.Zero;
        }

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
