namespace Yasuo.Common.Utility.Djikstra
{
    using LeagueSharp;

    public class Point
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unit">Unit of the Node</param>
        public Point(Obj_AI_Base unit)
        {
            this.Unit = unit;
        }

        /// <summary>
        /// Unit of the Node
        /// </summary>
        public Obj_AI_Base Unit { get; set; }
    }
}
