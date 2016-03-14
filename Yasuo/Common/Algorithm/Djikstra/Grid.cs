namespace Yasuo.Common.Algorithm.Djikstra
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    public class Grid
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Grid(List<Connection> connections)
        {
            this.Connections = connections;
        }

        public List<Connection> Connections { get; set; }

        /// <summary>
        ///     Draws the Grid as lines in the world
        /// </summary>
        /// <param name="width"></param>
        /// <param name="color"></param>
        public void Draw(int width = 1, System.Drawing.Color color = default(System.Drawing.Color))
        {
            try
            {
                if (Connections == null || Connections.Count == 0)
                {
                    return;
                }

                if (color == default(System.Drawing.Color))
                {
                    color = System.Drawing.Color.White;
                }

                foreach (var connection in Connections)
                {
                    connection.Draw(width, color);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }
}
