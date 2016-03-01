// TODO: Add Dash End Positions as list. Maybe think about positive things when I change Obj_AI_Base to Connection or Point.
// TODO: Rework Calculations based on Dash End Positions.

namespace Yasuo.Common.Pathing
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
            StartPosition = startPosition;
            EndPosition = endPosition;

            FirstUnit = this.ReturnFirstPosition();
        }

        public Vector3 FirstUnit { get; private set; }

        public int DangerValue { get; private set; }

        public float WalkTime { get; private set; }

        public float DashTime { get; private set; }

        public float PathTime { get; private set; }

        public float WalkLenght { get; private set; }

        public float DashLenght { get; private set; }

        public float PathLenght { get; private set; }

        public void VecToUnits()
        {
            foreach (var position in Positions)
            {
                var allminions = MinionManager.GetMinions(position, 50, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.None);
                var minion = allminions.MinOrDefault(x => x.Distance(position));

                if (minion != null)
                {
                    Units.Add(minion);
                }
            }
        }

        public void SetDangerValue()
        {
            foreach (var unit in this.Positions.Where(x => x.CountEnemiesInRange(Variables.Spells[SpellSlot.E].Range) > 0))
            {
                foreach (var hero in HeroManager.Enemies.Where(y => y.Distance(unit) <= Variables.Spells[SpellSlot.E].Range))
                {
                    DangerValue += (int)TargetSelector.GetPriority(hero);
                }
                DangerValue += 1;
            }
        }

        public void SetWalkTime()
        {
            WalkTime = WalkLenght / Variables.Player.MoveSpeed;
        }

        public void SetDashTime()
        {
            DashTime = DashLenght / Variables.Spells[SpellSlot.E].Speed;
        }

        public void SetPathTime()
        {
            PathTime = WalkTime + DashTime;
        }

        public void SetDashLength()
        {
            foreach (var unit in this.Positions)
            {
                DashLenght += 475;
            }
        }

        //TODO: Get lengts inbetween units
        public void SetWalkLength()
        {
            var StartDistance = this.ReturnFirstPosition().Distance(StartPosition);
            var EndDistance = Positions.LastOrDefault().Distance(EndPosition);

            var x = 0f;

            for (int i = 0; i < this.Positions.Count - 1; i++)
            {
                x += this.Positions[i].Distance(this.Positions[i + 1]);
            }

            var inbetweenDistance = (float) x - DashLenght;

            if (StartDistance <= Variables.Spells[SpellSlot.E].Range)
            {
                WalkLenght = StartDistance + EndDistance;

                if (inbetweenDistance > 0)
                {
                    WalkLenght += inbetweenDistance;
                }
            }
        }

        public void SetPathLengtht()
        {
            PathLenght = WalkLenght + PathLenght;
        }

        public void SetRealPath()
        {
            if (StartPosition == null)
            {
                return;
            }
            RealPath.StartPosition = Variables.Player.ServerPosition;
            for (int i = 0; i < this.Units.Count; i++)
            {
                var oldPosition = Variables.Player.ServerPosition.Extend(unit.ServerPosition, Variables.Spells[SpellSlot.E].Range);
                var unit = this.Units[i + 1];
                if (this.Units[i].Distance(Variables.Player.ServerPosition) <= Variables.Spells[SpellSlot.E].Range)
                {
                    
                    RealPath.AddPosition(oldPosition);
                }
                if (i == Units.Count - 1)
                {
                    RealPath.RemovePosition(oldPosition);
                    RealPath.EndPosition = oldPosition;
                }
            }
        }

        public void SetAll()
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

        public Vector3 ReturnFirstPosition()
        {
            if (Positions != null)
            {
                return this.Positions.FirstOrDefault(x => x != Variables.Player.ServerPosition);
            }
            return Vector3.Zero;
        }

        public void RemoveUnit(Obj_AI_Base unit)
        {
            if (Positions.Contains(unit.ServerPosition))
            {
                this.Positions.Remove(unit.ServerPosition);
            }
        }

        public void AddUnit(Obj_AI_Base unit)
        {
            if (!Positions.Contains(unit.ServerPosition))
            {
                this.Positions.Add(unit.ServerPosition);
            }
        }

        public void RemovePosition(Vector3 position)
        {
            if (!Positions.Contains(position))
            {
                return;   
            }
            Positions.Remove(position);
        }

        public void AddPosition(Vector3 position)
        {
            if (Positions.Contains(position))
            {
                return;
            }
            Positions.Add(position);
        }

        //TODO: Add color, Line width, end and start point boolean
        public void Draw()
        {
            
        }

    }
}
