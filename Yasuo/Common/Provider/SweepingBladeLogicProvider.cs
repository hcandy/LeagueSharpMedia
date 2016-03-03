namespace Yasuo.Common.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

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

                //TODO: Make that more dynamic (distance to next Position based on current player distance to possible first Position), what that does is a more correct pathing
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

        /// <summary>
        ///     Returns a list containing all units
        /// </summary>
        /// <param name="startPosition">start point (vector)</param>
        /// <param name="minions">bool</param>
        /// <param name="champions">bool</param>
        /// <returns>List(Obj_Ai_Base)</returns>
        public List<Obj_AI_Base> GetUnits(Vector2 startPosition, bool minions = true, bool champions = true, bool noSkillshots = false)
        {
            try
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
                    foreach (var x in units.Where(x => x.isInSkillshot()))
                    {
                        units.Remove(x);
                    }
                }

                foreach (var x in units.Where(x => !x.IsValid || x.HasBuff("YasuoDashWrapper")))
                {
                    units.Remove(x);
                }

                return units;
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"[GetUnits]: "+ex);
            }
            return null;
        }
    }
}
