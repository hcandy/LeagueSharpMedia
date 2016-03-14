namespace Yasuo.Common.Algorithm.Djikstra
{
    using System.Drawing;
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    public class Connection
    {
        public Connection(Point from,  Obj_AI_Base over)
        {
            this.From = from;
            this.Over = over;
            this.To = new Point(from.Position.Extend(over.ServerPosition, Variables.Spells[SpellSlot.E].Range));
            this.Distance = From.Position.Distance(To.Position);
        }

        /// <summary>
        /// Start Node
        /// </summary>
        public Point From { get; set; }

        /// <summary>
        /// End Node
        /// </summary>
        public Point To { get; set; }

        public Obj_AI_Base Over { get; set; }

        /// <summary>
        /// Distance
        /// </summary>
        public float Distance { get; set; }

        public void Draw(int width = 1, System.Drawing.Color color = default(System.Drawing.Color))
        {
            Drawing.DrawLine(Drawing.WorldToScreen(From.Position), Drawing.WorldToScreen(To.Position), width, color);
        }
    }
}
