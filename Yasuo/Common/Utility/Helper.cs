﻿namespace Yasuo.Common.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using LeagueSharp;
    using LeagueSharp.Common;
    using SDK = LeagueSharp.SDK;

    using Microsoft.SqlServer.Server;

    using SharpDX;

    using Yasuo.Common.Extensions;
    using Yasuo.Common.Provider;
    using Yasuo.Skills.Combo;

    public class Helper
    {
        public static SweepingBladeLogicProvider ProviderE = new SweepingBladeLogicProvider();

        #region general

        public static string GetCurrentMethod() => MethodBase.GetCurrentMethod().Name;

        #endregion

        #region vectors

        public static float GetPathLenght(Vector3[] Path)
        {
            float result = 0f;

            for (int i = 0; i < Path.Count(); i++)
            {
                if (i + 1 != Path.Count())
                {
                    result += Path[i].Distance(Path[i + 1]);
                }
            }
            return result;
        }

        /// <summary>
        ///     Returns the center from a given list of units
        /// </summary>
        /// <param name="units"></param>
        /// <returns>Vector2</returns>
        public static Vector2 GetMeanVector2(List<Obj_AI_Base> units)
        {
            if (units.Count == 0)
            {
                return Vector2.Zero;
            }
            float x = 0, y = 0;

            foreach (var unit in units)
            {
                x += unit.ServerPosition.X;
                y += unit.ServerPosition.Y;
            }

            return new Vector2(x / units.Count, y / units.Count);
        }

        /// <summary>
        ///     Returns the center from a given list of vectors
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns>Vector2</returns>
        public static Vector2 GetMeanVector2(List<Vector2> vectors)
        {
            if (vectors.Count == 0)
            {
                return Vector2.Zero;
            }

            float x = 0, y = 0;

            foreach (var vector in vectors)
            {
                x += vector.X;
                y += vector.Y;
            }

            return new Vector2(x / vectors.Count, y / vectors.Count);
        }

        /// <summary>
        ///     Returns the center from a given list of units
        /// </summary>
        /// <param name="units"></param>
        /// <returns>Vector3</returns>
        public static Vector3 GetMeanVector3(List<Obj_AI_Base> units)
        {
            if (units.Count == 0)
            {
                return Vector3.Zero;
            }
            float x = 0, y = 0, z = 0;

            foreach (var unit in units)
            {
                x += unit.ServerPosition.X;
                y += unit.ServerPosition.Y;
                z += unit.ServerPosition.Z;
            }

            return new Vector3(x / units.Count, y / units.Count, z / units.Count);
        }

        /// <summary>
        ///     Returns the center from a given list of vectors
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns>Vector2</returns>
        public static Vector3 GetMeanVector3(List<Vector3> vectors)
        {
            if (vectors.Count == 0)
            {
                return Vector3.Zero;
            }

            float x = 0, y = 0, z = 0;

            foreach (var vector in vectors)
            {
                x += vector.X;
                y += vector.Y;
                z += vector.Z;
            }

            return new Vector3(x / vectors.Count, y / vectors.Count, z / vectors.Count);
        }

        #endregion

        #region spells

        /// <summary>
        ///     Returns the Q Cast Delay
        /// </summary>
        public static float GetQDelay
        {
            get
            {
                if (Variables.Player.IsDashing())
                {
                    return 500;
                }
                return (float)(1 - Math.Min((ObjectManager.Player.AttackSpeedMod - 1) / 0.172, 0.66f));
            }
        }

        internal static float DistanceToTarget(Obj_AI_Base unit)
        {
            return unit.Distance(TargetSelector.GetSelectedTarget());
        }

        /// <summary>
        ///     Returns Spell Range by Spell Name
        /// </summary>
        /// <param name="SpellName"></param>
        /// <returns>float</returns>
        internal static float GetSpellRange(string SpellName)
        {
            if (SpellName != null)
            {
                return SDK.SpellDatabase.GetByName(SpellName).Range;
            }
            return 0;
        }

        /// <summary>
        ///     Returns Spell Range by Missile Name
        /// </summary>
        /// <param name="MissileName"></param>
        /// <returns>float</returns>
        internal static float GetSpellRange2(string MissileName)
        {
            if (MissileName != null)
            {
                return SDK.SpellDatabase.GetByMissileName(MissileName).Range;
            }
            return 0;
        }

        /// <summary>
        ///     Returns Spell Width based on Spell Name
        /// </summary>
        /// <param name="SpellName"></param>
        /// <returns>float</returns>
        internal static float GetSpellWidth(string SpellName)
        {
            if (SpellName == "YasuoWMovingWall")
            {
                return (250 + (50 * Variables.Spells[SpellSlot.W].Level));
            }
            if (SpellName == "YasuoQ")
            {
                return 20;
            }
            if (SpellName == "YasuoQ2")
            {
                return 90;
            }
            return SpellName != null ? SDK.SpellDatabase.GetByName(SpellName).Width : 0;
        }

        /// <summary>
        ///     Returns Missile Speed based on Spell Name
        /// </summary>
        /// <param name="SpellName"></param>
        /// <returns>float</returns>
        internal static float GetMissileSpeed(string SpellName)
        {
            switch (SpellName)
            {
                case "YasuoDash":
                    return 1000 + (ObjectManager.Player.MoveSpeed - 345);
                case "YasuoQ":
                    return float.MaxValue;
                case "YasuoQ2":
                    return 1400;
                default:
                    return SDK.SpellDatabase.GetByName(SpellName).MissileSpeed;
            }
        }

        /// <summary>
        ///     Returns the Spell Delay based on Spell Name
        /// </summary>
        /// <param name="SpellName"></param>
        /// <returns>float</returns>
        internal static float GetSpellDelay(string SpellName)
        {
            if (SpellName != null)
            {
                return SDK.SpellDatabase.GetByName(SpellName).Delay;
            }
            return 0;
        }

        #endregion
    }
}
