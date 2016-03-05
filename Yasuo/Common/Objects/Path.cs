// TODO: Add Dash End Positions as list. Maybe think about positive things when I change Obj_AI_Base to Connection or Point.
// TODO: Rework Calculations based on Dash End Positions.

namespace Yasuo.Common.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    public class Path
    {
        public Vector3 StartPosition;

        public Vector3 EndPosition;

        public Geometry.Polygon GeometryPath;

        public Path RealPath;

        public List<Vector3> Positions;

        public List<Obj_AI_Base> Units; 

        public Path(List<Vector3> positions, Vector3 startPosition, Vector3 endPosition)
        {
            this.Positions = positions;
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;

            this.SetAll();

            this.FirstUnit = this.Units.FirstOrDefault();
        }

        public Obj_AI_Base FirstUnit { get; private set; }

        public int DangerValue { get; private set; }

        public float WalkTime { get; private set; }

        public float DashTime { get; private set; }

        public float PathTime { get; private set; }

        public float WalkLenght { get; private set; }

        public float DashLenght { get; private set; }

        public float PathLenght { get; private set; }

        public void VecToUnits()
        {
            int count = 0;
            foreach (var position in this.Positions)
            {
                var allminions = MinionManager.GetMinions(position, 50, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.None);
                var position1 = position;
                var minion = allminions.MinOrDefault(x => x.Distance(position1));

                if (minion != null && minion.IsValid)
                {
                    this.Units.Add(minion);
                }

                count++;
            }
            Game.PrintChat("Converted Vectors to Units amount: "+count);
        }

        public void SetDangerValue()
        {
            foreach (var unit in this.Positions.Where(x => x.CountEnemiesInRange(Variables.Spells[SpellSlot.E].Range) > 0))
            {
                foreach (var hero in HeroManager.Enemies.Where(y => y.Distance(unit) <= Variables.Spells[SpellSlot.E].Range))
                {
                    this.DangerValue += (int)TargetSelector.GetPriority(hero);
                }
                this.DangerValue += 1;
            }
        }

        public void SetWalkTime()
        {
            this.WalkTime = this.WalkLenght / Variables.Player.MoveSpeed;
        }

        public void SetDashTime()
        {
            this.DashTime = this.DashLenght / Variables.Spells[SpellSlot.E].Speed;
        }

        public void SetPathTime()
        {
            this.PathTime = this.WalkTime + this.DashTime;
        }

        public void SetDashLength()
        {
            foreach (var unit in this.Positions)
            {
                this.DashLenght += Variables.Spells[SpellSlot.E].Range;
            }
        }

        // TODO: Get lengts inbetween units
        public void SetWalkLength()
        {
            var startDistance = this.FirstUnit.Distance(this.StartPosition);
            var endDistance = this.Positions.LastOrDefault().Distance(this.EndPosition);

            var x = 0f;

            for (int i = 0; i < this.Positions.Count - 1; i++)
            {
                x += this.Positions[i].Distance(this.Positions[i + 1]);
            }

            var inbetweenDistance = (float) x - this.DashLenght;

            if (startDistance <= Variables.Spells[SpellSlot.E].Range)
            {
                this.WalkLenght = startDistance + endDistance;

                if (inbetweenDistance > 0)
                {
                    this.WalkLenght += inbetweenDistance;
                }
            }
        }

        public void SetPathLengtht()
        {
            this.PathLenght = this.WalkLenght + this.PathLenght;
        }

        // TODO: No clue if that works
        public void SetRealPath()
        {
            if (this.StartPosition == null)
            {
                return;
            }
            this.RealPath.StartPosition = Variables.Player.ServerPosition;
            var oldPosition = Variables.Player.ServerPosition;

            for (int i = 0; i < this.Units.Count; i++)
            {
                // new unit
                var unit = this.Units[i + 1];
                var newPosition = oldPosition.Extend(unit.ServerPosition, Variables.Spells[SpellSlot.E].Range);

                // new end position
                if (oldPosition.Distance(unit.ServerPosition) < Variables.Spells[SpellSlot.E].Range)
                {
                    oldPosition = newPosition;
                    this.RealPath.AddPosition(newPosition);
                }

                // last unit reached
                if (i == this.Units.Count - 1)
                {
                    this.RealPath.RemovePosition(newPosition);
                    this.RealPath.EndPosition = newPosition;
                }
            }
        }

        private void SetAll()
        {
            try
            {
                this.VecToUnits();

                this.SetDashLength();
                this.SetWalkLength();
                this.SetPathLengtht();

                this.SetDashTime();
                this.SetWalkTime();
                this.SetPathTime();

                this.SetDangerValue();
            }
            catch (Exception ex)
            {             
                Console.WriteLine(ex);
            }
        }

        public void RemoveUnit(Obj_AI_Base unit)
        {
            if (this.Positions.Contains(unit.ServerPosition))
            {
                this.Positions.Remove(unit.ServerPosition);
            }
        }

        public void AddUnit(Obj_AI_Base unit)
        {
            if (!this.Positions.Contains(unit.ServerPosition))
            {
                this.Positions.Add(unit.ServerPosition);
            }
        }

        public void RemovePosition(Vector3 position)
        {
            if (!this.Positions.Contains(position))
            {
                return;   
            }
            this.Positions.Remove(position);
        }

        public void AddPosition(Vector3 position)
        {
            if (this.Positions.Contains(position))
            {
                return;
            }
            this.Positions.Add(position);
        }

        //TODO: Add color, Line width, end and start point boolean
        public void Draw()
        {
            try
            {
                Drawing.DrawText(500, 500, System.Drawing.Color.White, "Positions: "+this.Positions.Count);
                if (this.Positions != null && this.Positions.Count > 0)
                {
                    for (var i = 0; i < this.Positions.Count; i++)
                    {
                        if (this.Positions.Count > i + 1 && this.Positions[i + 1].IsValid())
                        {
                            Drawing.DrawLine(
                            Drawing.WorldToScreen(this.Positions[i]),
                            Drawing.WorldToScreen(this.Positions[i + 1]),
                                4f,
                            System.Drawing.Color.White);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }


        }

    }
}
