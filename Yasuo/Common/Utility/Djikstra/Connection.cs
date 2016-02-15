namespace Yasuo.Common.Utility.Djikstra
{
    using LeagueSharp.Common;

    class Connection
    {
        public Connection(Point from, Point to)
        {
            this.From = from;
            this.To = to;
            this.Distance = from.Unit.Distance(to.Unit);
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
        public double Distance { get; set; }
    }
}
