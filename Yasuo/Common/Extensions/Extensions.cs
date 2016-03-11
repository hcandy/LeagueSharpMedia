namespace Yasuo.Common.Extensions
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Modes.Weights;

    using SharpDX;

    using Yasuo.Modules;
    using Yasuo.Modules.Protector;

    using Geometry = LeagueSharp.Common.Geometry;
    using MinionTypes = LeagueSharp.Common.MinionTypes;

    static class Extensions
    {

        public static int CountMinionsInRange(this Vector3 position, float range)
        {
            var minionList = MinionManager.GetMinions(position, range);

            return minionList?.Count ?? 0;
        }
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

        // BUG: Returns wrong value do to SDK not working.
        /// <summary>
        /// Returns the missile position after time time.
        /// </summary>
        public static Vector2 MissilePosition(this Skillshot skillshot, bool allowNegative = false, float delay = 0)
        {
            if (!skillshot.HasMissile)
            {
                return Vector2.Zero;
            }

            if (skillshot.SData.SpellType == SpellType.SkillshotLine
                || skillshot.SData.SpellType == SpellType.SkillshotMissileLine)
            {
                Game.PrintChat("test");
                var t = Math.Max(0, Utils.TickCount + delay - skillshot.StartTime - skillshot.SData.Delay);
                t = (int)Math.Max(0, Math.Min(skillshot.EndPosition.Distance(skillshot.StartPosition), t * skillshot.SData.MissileSpeed / 1000));
                return skillshot.StartPosition + skillshot.Direction * t;
            }
            return Vector2.Zero;
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
