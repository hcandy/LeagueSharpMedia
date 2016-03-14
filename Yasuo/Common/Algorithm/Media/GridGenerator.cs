using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yasuo.Common.Algorithm.Media
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common.Algorithm.Djikstra;

    /// <summary>
    ///     Grid or "Connectio" Generator: http://i.imgur.com/XomUJvK.png
    /// </summary>
    public class GridGenerator
    {
        public List<Obj_AI_Base> Units { get; private set; }

        public List<Grid> Grids;  

        public GridGenerator(List<Obj_AI_Base> units)
        {
            Units = units;
        }
        
        /// <summary>
        ///     Creates a new grid for every unit in dash range
        /// </summary>
        public void Generate()
        {
            try
            {
                foreach (var startingUnit in this.Units.Where(x => x.ServerPosition.Distance(Variables.Player.ServerPosition) <= Variables.Spells[SpellSlot.E].Range))
                {
                    var connections = new List<Connection>();

                    var thresholderPositions =
                        new List<Point>()
                            {
                            new Point((Variables.Player.ServerPosition.Extend(startingUnit.ServerPosition, Variables.Spells[SpellSlot.E].Range)))
                            };

                    var possibledashes = 0;

                    do
                    {
                        foreach (var dashEndPoint in thresholderPositions.ToList())
                        {
                            var blacklistedMinions = new List<Obj_AI_Base> { startingUnit };

                            foreach (var unit in this.Units.Where(x => x.Distance(dashEndPoint.Position) <= Variables.Spells[SpellSlot.E].Range))
                            {
                                if (blacklistedMinions.Contains(unit))
                                {
                                    continue;
                                }

                                possibledashes++;

                                thresholderPositions.Add(new Point(dashEndPoint.Position.Extend(unit.ServerPosition, Variables.Spells[SpellSlot.E].Range)));
     
                                connections.Add(new Connection(dashEndPoint, unit));

                                blacklistedMinions.Add(unit);
                            }
                        }
                    }
                    while (possibledashes - connections.Count > 0);

                    try
                    {
                        if (connections.Count > 0)
                        {
                            Grids.Add(new Grid(connections));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(@"ERROR: "+ex);
                    }

                    Game.PrintChat("Grid added");

                    if (true)
                    {
                        Drawing.DrawText(1100, 600, System.Drawing.Color.White, "Possible Dashesh: " + possibledashes);
                        Drawing.DrawText(1100, 620, System.Drawing.Color.White, "Positions       : " + thresholderPositions.Count);
                        Drawing.DrawText(1100, 640, System.Drawing.Color.White, "Connections     : " + connections.Count);
                        Drawing.DrawText(1100, 660, System.Drawing.Color.White, "Grids           : " + Grids.Count);
                        //Drawing.DrawText(1500, 1580, System.Drawing.Color.White, "Possible Dashesh: " + possibledashes);

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
