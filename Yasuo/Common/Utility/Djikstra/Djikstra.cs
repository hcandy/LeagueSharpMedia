namespace Yasuo.Common.Utility.Djikstra
{
    using System.Collections.Generic;

    using LeagueSharp;

    class Dijkstra
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connections">List of all Connections</param>
        /// <param name="points">List of all Points</param>
        public Dijkstra(List<Point> points, List<Connection> connections)
        {

            this.Connections = connections;
            this.Points = points;
            this.Base = new List<Point>();
            this.Dist = new Dictionary<Obj_AI_Base, double>();
            this.Previous = new Dictionary<Obj_AI_Base, Point>();

            // Adding Nodes
            foreach (var n in this.Points)
            {
                this.Previous.Add(n.Unit, null);
                this.Base.Add(n);
                this.Dist.Add(n.Unit, double.MaxValue);
            }
        }

        public List<Point> Points { get; set; }

        public List<Connection> Connections { get; set; }

        public List<Point> Base { get; set; }

        public Dictionary<Obj_AI_Base, double> Dist { get; set; }

        public Dictionary<Obj_AI_Base, Point> Previous { get; set; }

        /// <summary>
        /// Calculates the shortest distance from the Start node to all other nodes
        /// </summary>
        /// <param name="start">Startknoten</param>
        public void CalculateDistance(Point start)
        {
            this.Dist[start.Unit] = 0;

            while (this.Base.Count > 0)
            {
                var u = this.GetNodeWithSmallestDistance();
                if (u == null)
                {
                    this.Base.Clear();
                }
                else
                {
                    foreach (var v in this.GetNeighbors(u))
                    {
                        var alt = this.Dist[u.Unit] +
                                this.GetDistanceBetween(u, v);
                        if (alt < this.Dist[v.Unit])
                        {
                            this.Dist[v.Unit] = alt;
                            this.Previous[v.Unit] = u;
                        }
                    }
                    this.Base.Remove(u);
                }
            }
        }

        /// <summary>
        /// Calculates the Path to the Node d (d = target)
        /// </summary>
        /// <param name="d">Targeted Node</param>
        /// <returns></returns>
        public List<Point> GetPathTo(Point d)
        {
            var path = new List<Point>();

            path.Insert(0, d);

            while (this.Previous[d.Unit] != null)
            {
                d = this.Previous[d.Unit];
                path.Insert(0, d);
            }

            return path;
        }

        /// <summary>
        /// Gets the Node with the shortest distance
        /// </summary>
        /// <returns></returns>
        public Point GetNodeWithSmallestDistance()
        {
            var distance = double.MaxValue;
            Point smallest = null;

            foreach (var n in this.Base)
            {
                if (this.Dist[n.Unit] < distance)
                {
                    distance = this.Dist[n.Unit];
                    smallest = n;
                }
            }

            return smallest;
        }

        /// <summary>
        /// Gives all neighbors that are still in the base
        /// </summary>
        /// <param name="n">Nodes</param>
        /// <returns></returns>
        public List<Point> GetNeighbors(Point n)
        {
            var neighbors = new List<Point>();

            foreach (var e in this.Connections)
            {
                if (e.From.Equals(n) && this.Base.Contains(n))
                {
                    neighbors.Add(e.To);
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Gives the distance between 2 nodes
        /// </summary>
        /// <param name="o">Start Node</param>
        /// <param name="d">End Node</param>
        /// <returns></returns>
        public double GetDistanceBetween(Point o, Point d)
        {
            foreach (var e in this.Connections)
            {
                if (e.From.Equals(o) && e.To.Equals(d))
                {
                    return e.Distance;
                }
            }

            return 0;
        }


    }
}
