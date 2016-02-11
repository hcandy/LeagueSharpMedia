namespace Yasuo.Common.Utility.Djikstra
{
    class Edge
    {
        private Node origin;
        private Node destination;
        private double distance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="origin">Start Node</param>
        /// <param name="destination">End Node</param>
        /// <param name="distance">Distance</param>
        public Edge(Node origin, Node destination, double distance)
        {
            this.origin = origin;
            this.destination = destination;
            this.distance = distance;
        }

        /// <summary>
        /// Start Node
        /// </summary>
        public Node Origin
        {
            get { return this.origin; }
            set { this.origin = value; }
        }

        /// <summary>
        /// End Node
        /// </summary>
        public Node Destination
        {
            get { return this.destination; }
            set { this.destination = value; }
        }

        /// <summary>
        /// Distance
        /// </summary>
        public double Distance
        {
            get { return this.distance; }
            set { this.distance = value; }
        }
    }
}
