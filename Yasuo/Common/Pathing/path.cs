// TODO: Add Dash End Positions as list. Maybe think about positive things when I change Obj_AI_Base to Connection or Point.

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
        public Vector2 StartPosition;

        public Vector2 EndPosition;

        public List<Obj_AI_Base> Units;

        public Path(List<Obj_AI_Base> units, Vector2 startPosition, Vector2 endPosition)
        {
            Units = units;
            StartPosition = startPosition;
            EndPosition = endPosition;

            FirstUnit = this.ReturnUnit();
        }
        public Obj_AI_Base FirstUnit { get; private set; }

        public int DangerValue { get; private set; }

        public float WalkTime { get; private set; }

        public float DashTime { get; private set; }

        public float PathTime { get; private set; }

        public float WalkLenght { get; private set; }

        public float DashLenght { get; private set; }

        public float PathLenght { get; private set; }

        public void SetDangerValue()
        {
            foreach (var unit in Units.Where(x => x.CountEnemiesInRange(Variables.Spells[SpellSlot.E].Range) > 0))
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
            foreach (var unit in Units)
            {
                DashLenght += 475;
            }
        }

        //TODO: Get lengts inbetween units
        public void SetWalkLength()
        {
            var StartDistance = this.ReturnUnit().Distance(StartPosition);
            var EndDistance = this.ReturnLastUnit().Distance(EndPosition);

            if (StartDistance <= Variables.Spells[SpellSlot.E].Range)
            {
                WalkLenght = StartDistance + EndDistance;
            }
        }

        public void SetPathLengtht()
        {
            PathLenght = WalkLenght + PathLenght;
        }

        public void SetAll()
        {
            try
            {
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

        public Obj_AI_Base ReturnUnit()
        {
            return Units.FirstOrDefault(x => x.Name != Variables.Player.Name);
        }

        public Obj_AI_Base ReturnLastUnit()
        {
            return Units.LastOrDefault();
        }

        public void RemoveUnit(Obj_AI_Base unit)
        {
            Units.Remove(unit);
        }

        public void AddUnit(Obj_AI_Base unit)
        {
            Units.Add(unit);
        }

    }
}
