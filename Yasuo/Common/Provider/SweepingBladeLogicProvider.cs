﻿namespace Yasuo.Common.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    using LeagueSharp;
    using LeagueSharp.Common;
    using SDK = LeagueSharp.SDK;

    using SharpDX;

    using Yasuo.Common.Algorithm.Djikstra;
    using Yasuo.Common.Algorithm.Media;
    using Yasuo.Common.Objects;
    using Yasuo.Common.Utility;

    using Color = System.Drawing.Color;
    using Point = Yasuo.Common.Algorithm.Djikstra.Point;

    public class SweepingBladeLogicProvider
    {
        public static float CalculationRange;

        public SweepingBladeLogicProvider(float calculationRange = 3000)
        {
            CalculationRange = calculationRange;
        }

        /// <summary>
        ///     Returns a path object that represents the shortest possible path to a given location
        /// </summary>
        /// <param name="endPosition">The vector to dash to</param>
        /// <param name="minions"></param>
        /// <param name="champions"></param>
        /// <returns>Obj_AI_Base</returns>
        public Path GetPath(Vector3 endPosition, bool minions = true, bool champions = true, bool noSkillshots = false)
        {
            try
            {
                var units = this.GetUnits(Variables.Player.ServerPosition, minions, champions);

                if (units == null || units.Count == 0)
                {
                    return null;
                }

                var points = units.Select(position => new Point(position.ServerPosition)).ToList();
                points.Add(new Point(Variables.Player.ServerPosition));

                var possibleGrids = new List<Grid>();
                var possiblePaths = new List<Path>();

                // TODO: Make that more dynamic (distance to next Position based on current player distance to possible first Position), what that does is a more correct pathing
                // NOTE: That would need a multipathing system that uses Djikstra Algorithm for every minion in E range and determines the shortest path based on that outcome.
                // ANOTHER NOTE: The solution is to take every minion around you and create an own grid for it. Then choose the shortest path of that grid. Then return the shortest path.

                
                #region Grid/Connection Generator V2 (Real Multipathing System)

                var generator = new GridGenerator(units);

                generator.Generate();

                possibleGrids = generator.Grids;

                #endregion


                #region Calculator

                if (possibleGrids == null)
                {
                    Game.PrintChat("Grids are null " +Game.ClockTime);
                }

                if (possibleGrids != null && possibleGrids.Count > 0)
                {
                    foreach (var calculator in possibleGrids.Select(grid => new Dijkstra(points, grid.Connections)))
                    {
                        // Set starting point, Obj_Ai_Base Player in this case
                        calculator.CalculateDistance(
                            points.FirstOrDefault(x => x.Position == Variables.Player.ServerPosition));

                        // Set end point and return result as path
                        var path = calculator.GetPathTo(points.MinOrDefault(x => x.Position.Distance(endPosition)));
                        var pathToUnits = new List<Obj_AI_Base>();

                        if (path != null)
                        {
                            pathToUnits.AddRange(
                                path.ToList()
                                    .Select(
                                        point => this.GetUnits(point.Position).MinOrDefault(x => x.Distance(point.Position))));
                        }

                        possiblePaths.Add(new Path(pathToUnits.ToList(), Variables.Player.ServerPosition, endPosition));
                    }
                }

                #endregion

                #region Debug

                if (Variables.Debug)
                {
                    Drawing.DrawText(1500, 500, Color.AliceBlue, "Grids: " + possibleGrids.Count);
                    Drawing.DrawText(1500, 520, Color.AliceBlue, "Paths: " + possiblePaths.Count);

                    var Versatz = 600;
                    foreach (var var in possibleGrids)
                    {
                        Drawing.DrawText(1500, Versatz, Color.AliceBlue, "Grids Units: " + var.Connections.Count);
                        Versatz += 20;
                    }
                }

                #endregion

                var result = possiblePaths.Where(path => path.FasterThanWalking).MinOrDefault(path => path.PathTime);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"[GetPath]: " + ex);
            }
            return null;

        }

        // TODO: REWRITE ALTERNATIVE PATH (using Grid Generator to remove only the old path)
        /*
        public Path GetAlternativePath(Path oldPath, bool minions = true, bool champions = true, bool noSkillshots = true, bool noWallDashes = true)
        {
            try
            {
                var units = this.GetUnits(Variables.Player.ServerPosition, minions, champions);
                var connections = new List<Connection>();

                if (units == null || units.Count == 0)
                {
                    return null;
                }

                var points = units.Select(position => new Point(position.ServerPosition)).ToList();
                points.Add(new Point(Variables.Player.ServerPosition));

                

                // OLD LOGIC
                //foreach (var point in points.ToList())
                //{
                //    foreach (var neighbour in points.Where(neighbour => point.Position.Distance(neighbour.Position) <= Variables.Spells[SpellSlot.E].Range * 1.5).ToList())
                //    {
                //        if (noSkillshots && WallDashLogicProvider.GetFirstWallPoint(point.Position, neighbour.Position, 1) == Vector3.Zero)
                //        {
                //            Console.WriteLine("Added");
                //            connections.Add(new Connection(point, neighbour));
                //        }
                //        else
                //        {
                //            Console.WriteLine("Added else");
                //            connections.Add(new Connection(point, neighbour));
                //        }
                //    }
                //}
                Console.WriteLine("" + connections.Count);


                // Create new Object of the Djikstra class with values from above
                var calculator = new Dijkstra(points, connections);

                // Set starting point, Obj_Ai_Base Player in this case
                calculator.CalculateDistance(points.FirstOrDefault(x => x.Position == Variables.Player.ServerPosition));

                // Set end point and return result as path
                var path = calculator.GetPathTo(points.MinOrDefault(x => x.Position.Distance(oldPath.EndPosition)));
                var pathToUnits = new List<Obj_AI_Base>();

                //if (path != null)
                //{
                //    pathToUnits.AddRange(path.Where(x => this.GetUnits(x.Position.To2D()).Count > 0));
                //}

                if (path != null)
                {
                    pathToUnits.AddRange(path.ToList().Select(point => this.GetUnits(point.Position).MinOrDefault(x => x.Distance(point.Position))));
                }

                var result = new Path(pathToUnits.ToList(), Variables.Player.ServerPosition, oldPath.EndPosition);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"[GetAlternativePath]: " + ex);
            }
            return null;
        }
        */

        /// <summary>
        ///     Returns a list containing all units
        /// </summary>
        /// <param name="startPosition">start point (vector)</param>
        /// <param name="minions">bool</param>
        /// <param name="champions">bool</param>
        /// <returns>List(Obj_Ai_Base)</returns>
        public List<Obj_AI_Base> GetUnits(Vector3 startPosition, bool minions = true, bool champions = true)
        {
            try
            {
                // Add all units
                var units = new List<Obj_AI_Base>();

                if (minions)
                {
                    units.AddRange(
                        MinionManager.GetMinions(
                            startPosition,
                            CalculationRange,
                            MinionTypes.All,
                            MinionTeam.NotAlly));
                }

                if (champions)
                {
                    units.AddRange(HeroManager.Enemies);
                }

                foreach (var x in units.Where(x => !x.IsValid || x.HasBuff("YasuoDashWrapper") || x.IsDead || x.Health == 0 || x.IsMe || x.Distance(Variables.Player.ServerPosition) > CalculationRange).ToList())
                {
                    units.Remove(x);
                }

                return units;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        /// <summary>
        ///     Credits: Brian
        ///     Returns the correct amount of Damage on unit
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
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
