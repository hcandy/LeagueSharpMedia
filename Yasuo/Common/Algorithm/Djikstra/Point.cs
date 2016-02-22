namespace Yasuo.Common.Algorithm.Djikstra
{
    using SharpDX;

    public class Point
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unit">Unit of the Node</param>
        public Point(Vector3 position)
        {
            this.Position = position;
        }

        /// <summary>
        /// Unit of the Node
        /// </summary>
        public Vector3 Position { get; set; }
    }
}
