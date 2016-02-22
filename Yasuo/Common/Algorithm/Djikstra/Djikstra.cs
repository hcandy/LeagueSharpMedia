namespace Yasuo.Common.Algorithm.Djikstra
{
    using System;
    using System.Collections.Generic;

    using SharpDX;

    class Dijkstra
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connections">List of all Connections</param>
        /// <param name="points">List of all Points</param>
        public Dijkstra(List<Point> points, List<Connection> connections)
        {
            try
            {
                this.Connections = connections;
                this.Points = points;
                this.Base = new List<Point>();
                this.Dist = new Dictionary<Vector3, float>();
                this.Previous = new Dictionary<Vector3, Point>();

                foreach (var n in this.Points)
                {
                    this.Previous.Add(n.Position, null);
                    this.Base.Add(n);
                    this.Dist.Add(n.Position, float.MaxValue);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"[Djikstra]: " +ex);
            }


        }

        public List<Point> Points { get; set; }

        public List<Connection> Connections { get; set; }

        public List<Point> Base { get; set; }

        public Dictionary<Vector3, float> Dist { get; set; }

        public Dictionary<Vector3, Point> Previous { get; set; }

        /// <summary>
        /// Calculates the shortest distance from the Start node to all other nodes
        /// </summary>
        /// <param name="start">Startknoten</param>
        public void CalculateDistance(Point start)
        {
            this.Dist[start.Position] = 0;

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
                        var alt = this.Dist[u.Position] +
                                this.GetDistanceBetween(u, v);
                        if (alt < this.Dist[v.Position])
                        {
                            this.Dist[v.Position] = alt;
                            this.Previous[v.Position] = u;
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

            while (this.Previous[d.Position] != null)
            {
                d = this.Previous[d.Position];
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
            var distance = float.MaxValue;
            Point smallest = null;

            foreach (var n in this.Base)
            {
                if (this.Dist[n.Position] < distance)
                {
                    distance = this.Dist[n.Position];
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
        public float GetDistanceBetween(Point o, Point d)
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
