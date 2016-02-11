namespace Yasuo.Common.Utility.Djikstra
{
    using LeagueSharp;

    public class Node
    {
        private Obj_AI_Base unit;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unit">Unit of the Node</param>
        public Node(Obj_AI_Base unit)
        {
            this.unit = unit;
        }

        /// <summary>
        /// Unit of the Node
        /// </summary>
        public Obj_AI_Base Unit
        {
            get { return this.unit; }
            set { this.unit = value; }
        }
    }
}
