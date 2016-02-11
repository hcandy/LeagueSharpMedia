namespace Yasuo.Common.Utility.Djikstra
{
    using System.Collections.Generic;

    using LeagueSharp;

    class Dijkstra
    {
        private List<Node> nodes;
        private List<Edge> edges;
        private List<Node> _base;
        private Dictionary<Obj_AI_Base, double> dist;
        private Dictionary<Obj_AI_Base, Node> previous;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="edges">List of all Edges</param>
        /// <param name="nodes">List of all Nodes</param>
        public Dijkstra(List<Edge> edges, List<Node> nodes)
        {

            this.Edges = edges;
            this.Nodes = nodes;
            this.Base = new List<Node>();
            this.Dist = new Dictionary<Obj_AI_Base, double>();
            this.Previous = new Dictionary<Obj_AI_Base, Node>();

            // Adding Nodes
            foreach (Node n in this.Nodes)
            {
                this.Previous.Add(n.Unit, null);
                this.Base.Add(n);
                this.Dist.Add(n.Unit, double.MaxValue);
            }
        }

        /// <summary>
        /// Calculates the shortest distance from the Start node to all other nodes
        /// </summary>
        /// <param name="start">Startknoten</param>
        public void CalculateDistance(Node start)
        {
            this.Dist[start.Unit] = 0;

            while (this.Base.Count > 0)
            {
                Node u = this.GetNodeWithSmallestDistance();
                if (u == null)
                {
                    this.Base.Clear();
                }
                else
                {
                    foreach (Node v in this.GetNeighbors(u))
                    {
                        double alt = this.Dist[u.Unit] +
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
        public List<Node> GetPathTo(Node d)
        {
            List<Node> path = new List<Node>();

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
        public Node GetNodeWithSmallestDistance()
        {
            double distance = double.MaxValue;
            Node smallest = null;

            foreach (Node n in this.Base)
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
        public List<Node> GetNeighbors(Node n)
        {
            List<Node> neighbors = new List<Node>();

            foreach (Edge e in this.Edges)
            {
                if (e.Origin.Equals(n) && this.Base.Contains(n))
                {
                    neighbors.Add(e.Destination);
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
        public double GetDistanceBetween(Node o, Node d)
        {
            foreach (Edge e in this.Edges)
            {
                if (e.Origin.Equals(o) && e.Destination.Equals(d))
                {
                    return e.Distance;
                }
            }

            return 0;
        }

        /// <summary>
        /// List of all Nodes in the base
        /// </summary>
        public List<Node> Nodes
        {
            get { return this.nodes; }
            set { this.nodes = value; }
        }

        /// <summary>
        /// List of all edges
        /// </summary>
        public List<Edge> Edges
        {
            get { return this.edges; }
            set { this.edges = value; }
        }

        /// <summary>
        /// Count of un processed Nodes
        /// </summary>
        public List<Node> Base
        {
            get { return this._base; }
            set { this._base = value; }
        }

        /// <summary>
        /// Distance of the edges
        /// </summary>
        public Dictionary<Obj_AI_Base, double> Dist
        {
            get { return this.dist; }
            set { this.dist = value; }
        }

        /// <summary>
        /// Previous Node
        /// </summary>
        public Dictionary<Obj_AI_Base, Node> Previous
        {
            get { return this.previous; }
            set { this.previous = value; }
        }
    }
}
