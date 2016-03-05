namespace Yasuo.Common.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using SDK = LeagueSharp.SDK;

    using SharpDX;

    using Yasuo.Common.Algorithm.Djikstra;
    using Yasuo.Common.Pathing;
    using Yasuo.Common.Utility;

    using Point = Yasuo.Common.Algorithm.Djikstra.Point;

    // TODO: Add Waypoint System

    public class SweepingBladeLogicProvider
    {
        public static float CalculationRange;

        public SweepingBladeLogicProvider(float calculationRange = 3000)
        {
            CalculationRange = calculationRange;
        }

        /// <summary>
        ///     Returns the Position that is best to Gapclose to a given position
        /// </summary>
        /// <param name="position">The vector to dash to</param>
        /// <param name="minions"></param>
        /// <param name="champions"></param>
        /// <returns>Obj_AI_Base</returns>
        public Path GetPath(Vector3 position, bool minions = true, bool champions = true, bool aroundSkillshots = false)
        {
            try
            {
                var units = this.GetUnits(ObjectManager.Player.ServerPosition.To2D(), minions, champions, aroundSkillshots);
                var connections = new List<Connection>();

                if (units == null || units.Count == 0)
                {
                    return null;
                }

                var points = units.Select(Position => new Point(Position.ServerPosition)).ToList();
                points.Add(new Point(Variables.Player.ServerPosition));

                // TODO: Make that more dynamic (distance to next Position based on current player distance to possible first Position), what that does is a more correct pathing
                // NOTE: That would need a multipathing system that uses Djikstra Algorithm for every minion in E range and determines the shortest path based on that outcome.
                foreach (var point in points)
                {
                    foreach (var neighbour in points)
                    {
                        if (point.Position.Distance(neighbour.Position) <= Variables.Spells[SpellSlot.E].Range)
                        {
                            connections.Add(new Connection(point, neighbour));
                        }
                    }
                }

                // Create new Object of the Djikstra class with values from above
                var calculator = new Dijkstra(points, connections);

                // Set starting point, Obj_Ai_Base Player in this case
                calculator.CalculateDistance(points.FirstOrDefault(x => x.Position == Variables.Player.ServerPosition));

                // Set end point and return result as path
                var path = calculator.GetPathTo(points.MinOrDefault(x => x.Position.Distance(position)));

                var result = new Path(path.Select(x => x.Position).ToList(), Variables.Player.ServerPosition, position);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"[GetPath]: " +ex);
            }
            return null;

        }

        // TODO: Not sure if .ToList() is a performance issue. Maybe work with a lock. 
        // TODO: But that could be worse.. maybe add a second List and loop through it to prevent modifying the old list while looping
        /// <summary>
        ///     Returns a list containing all units
        /// </summary>
        /// <param name="startPosition">start point (vector)</param>
        /// <param name="minions">bool</param>
        /// <param name="champions">bool</param>
        /// <returns>List(Obj_Ai_Base)</returns>
        public List<Obj_AI_Base> GetUnits(Vector2 startPosition, bool minions = true, bool champions = true, bool noSkillshots = false)
        {
            // Add all units
            var units = new List<Obj_AI_Base>();

            if (minions)
            {
                units.AddRange(
                    MinionManager.GetMinions(
                        ObjectManager.Player.ServerPosition,
                        CalculationRange,
                        MinionTypes.All,
                        MinionTeam.NotAlly));
            }

            if (champions)
            {
                units.AddRange(HeroManager.Enemies);
            }

            if (noSkillshots)
            {
                foreach (var x in units.Where(x => x.isInSkillshot()).ToList())
                {
                    units.Remove(x);
                }
            }

            foreach (var x in units.Where(x => !x.IsValid || x.HasBuff("YasuoDashWrapper") || x.IsDead || x.Health == 0).ToList())
            {
                units.Remove(x);
            }

            return units;
        }

        public double GetDamage(Obj_AI_Base unit)
        {
            return Variables.Player.CalcDamage(
                unit,
                Damage.DamageType.Magical,
                (50 + 20 * Variables.Spells[SpellSlot.E].Level) * (1 + Math.Max(0, Variables.Player.GetBuffCount("YasuoDashScalar") * 0.25))
                + 0.6 * Variables.Player.FlatMagicDamageMod);
        }
    }
}
