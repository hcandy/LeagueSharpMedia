namespace Yasuo.Common.Algorithm.Djikstra
{
    using LeagueSharp.Common;

    class Connection
    {
        public Connection(Point from, Point to)
        {
            this.From = from;
            this.To = to;
            this.Distance = from.Position.Distance(to.Position);
        }

        /// <summary>
        /// Start Node
        /// </summary>
        public Point From { get; set; }

        /// <summary>
        /// End Node
        /// </summary>
        public Point To { get; set; }

        /// <summary>
        /// Distance
        /// </summary>
        public float Distance { get; set; }
    }
}
