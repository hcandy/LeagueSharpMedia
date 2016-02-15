namespace Yasuo.Skills.Combo
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.SDK.MoreLinq;

    using SharpDX;

    using Yasuo.Common.Pathing;
    using Yasuo.Common.Utility;
    using Yasuo.Common.Utility.Djikstra;

    using Point = Yasuo.Common.Utility.Djikstra.Point;

    //TODO: Add Waypoint System
    public class SweepingBladeLogicProvider
    {
        public static float CalculationRange;

        public SweepingBladeLogicProvider(float calculationRange = 3000)
        {
            CalculationRange = calculationRange;
        }

        /// <summary>
        ///     Returns the unit that is best to Gapclose to a given position
        /// </summary>
        /// <param name="position">The vector to dash to</param>
        /// <param name="minions"></param>
        /// <param name="champions"></param>
        /// <returns>Obj_AI_Base</returns>
        public Path GetPath(Vector2 position, bool minions = true, bool champions = true)
        {
            var units = GetUnits(ObjectManager.Player.ServerPosition.To2D(), minions, champions);
            var connections = new List<Connection>();

            if (units == null || units.Count == 0)
            {
                return null;
            }

            var points = units.Select(unit => new Point(unit)).ToList();
            points.Add(new Point(Variables.Player));

            foreach (var point in points)
            {
                foreach (var neighbour in points)
                {
                    if (point.Unit.Distance(neighbour.Unit) <= Variables.Spells[SpellSlot.E].Range)
                    {
                        connections.Add(new Connection(point, neighbour));
                    }
                }
            }

            // Create new Object of the Djikstra class with values from above
            var calculator = new Dijkstra(points, connections);

            // Set starting point, Obj_Ai_Base Player in this case
            calculator.CalculateDistance(points.FirstOrDefault(x => x.Unit.NetworkId == Variables.Player.NetworkId));

            // Set end point and return result as path
            var path = calculator.GetPathTo(points.MinOrDefault(x => x.Unit.Distance(position)));

            var result = new Path(path.Select(x => x.Unit).ToList(), Variables.Player.ServerPosition.To2D(), position);

            Console.WriteLine(@"Pathfinding results: "+result.ReturnUnit());
            return result;

        }

        /// <summary>
        ///     Returns a list containing all units
        /// </summary>
        /// <param name="startPosition">start point (vector)</param>
        /// <param name="minions">bool</param>
        /// <param name="champions">bool</param>
        /// <returns>List(Obj_Ai_Base)</returns>
        public static List<Obj_AI_Base> GetUnits(Vector2 startPosition, bool minions = true, bool champions = true)
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

            if (units.Count == 0)
            {
                return null;
            }
            else
            {
                foreach (var x in units.Where(x => !x.IsValid || x.HasBuff("YasuoDashWrapper")))
                {
                    units.Remove(x);
                }
                return units;
            }
        }
    }
}
