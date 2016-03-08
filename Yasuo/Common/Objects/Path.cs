// TODO: Add Dash End Positions as list. Maybe think about positive things when I change Obj_AI_Base to Connection or Point.
// TODO: Rework Calculations based on Dash End Positions.

namespace Yasuo.Common.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common.Utility;

    using SharpDX;

    using Yasuo.Common.Provider;

    using Color = System.Drawing.Color;

    public class Path
    {
        public Vector3 StartPosition;

        public Vector3 EndPosition;

        public Geometry.Polygon GeometryPath;

        public Path RealPath;

        public List<Vector3> Positions = new List<Vector3>();

        public List<Obj_AI_Base> Units;

        public Dash DashObject;

        private SweepingBladeLogicProvider ProviderE;

        private readonly FlowLogicProvider ProviderFlow;

        public Path(List<Obj_AI_Base> units, Vector3 startPosition, Vector3 endPosition)
        {
            if (units.Contains(Variables.Player))
            {
                units.Remove(Variables.Player);
            }

            ProviderE = new SweepingBladeLogicProvider(startPosition.Distance(endPosition) + 150);
            ProviderFlow = new FlowLogicProvider();

            this.Units = units;
            
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
            
            if (Units != null && Units.Count > 0)
            {
                this.FirstUnit = this.Units.MinOrDefault(x => x.Distance(Variables.Player));
                this.SetAll();
            }

            DashObject = new Dash(FirstUnit);

            Drawing.DrawText(500, 540, Color.Red, "PathLength: "+PathLenght);
            Drawing.DrawText(500, 560, Color.Red, "DashLengt: " + DashLenght);
            Drawing.DrawText(500, 580, Color.Red, "WalkLenght: " + WalkLenght);

            Drawing.DrawText(500, 640, Color.Red, "Time: " + PathTime);
        }

        public Obj_AI_Base FirstUnit { get; private set; }

        public bool FasterThanWalking { get; private set; }

        public bool WallDashSavesTime { get; private set; }

        public bool GetsShield { get; private set; }

        public int DangerValue { get; private set; }

        public float WalkTime { get; private set; }

        public float DashTime { get; private set; }

        public float PathTime { get; private set; }

        public float WalkLenght { get; private set; }

        public float DashLenght { get; private set; }

        public float PathLenght { get; private set; }

        public void UnitsToVec()
        {
            if (Units != null)
            {
                Drawing.DrawText(500, 520, Color.AliceBlue, "" + Units.Count);

                foreach (var unit in this.Units.ToList())
                {
                    if (!Positions.Contains(unit.ServerPosition))
                    {
                        Positions.Add(unit.ServerPosition);
                    }
                }
                Drawing.DrawText(500, 500, Color.AliceBlue, "" + Positions.Count);
            }

        }

        // TODO: Add Skillshots in Path (Based on Danger Level)
        public void SetDangerValue()
        {
            foreach (var unit in this.Units.Where(x => x.CountEnemiesInRange(Variables.Spells[SpellSlot.E].Range) > 0))
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
            this.DashTime = this.DashLenght / (1000 + Variables.Player.MoveSpeed);
        }

        public void SetPathTime()
        {
            Drawing.DrawText(500, 700, System.Drawing.Color.Red, "Pathing: Walktime: " + WalkTime);
            Drawing.DrawText(500, 720, System.Drawing.Color.Red, "Pathing: Dashtime: " + DashTime);
            this.PathTime = this.WalkTime + this.DashTime;
        }

        public void SetDashLength()
        {
            foreach (var unit in this.Units)
            {
                this.DashLenght += Variables.Spells[SpellSlot.E].Range;
            }
        }

        // TODO: Get lengts inbetween units
        public void SetWalkLength()
        {
            var startDistance = this.Units.FirstOrDefault().Distance(this.StartPosition);
            var endDistance = this.Units.LastOrDefault().Distance(this.EndPosition);

            var x = 0f;

            for (int i = 0; i < this.Units.Count - 1; i++)
            {
                x += this.Units[i].Distance(this.Units[i + 1]);
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
            this.PathLenght = this.WalkLenght + this.DashLenght;
        }

        // TODO: No clue if that works
        public void SetRealPath()
        {
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
                    this.RealPath.EndPosition = newPosition;
                }
            }
        }

        private void SetAll()
        {
            try
            {
                this.UnitsToVec();

                this.SetDashLength();
                this.SetWalkLength();
                this.SetPathLengtht();

                this.SetDashTime();
                this.SetWalkTime();
                this.SetPathTime();

                this.SetDangerValue();
                this.CompareDashWalkTime();

                this.CheckWallDashTimeSaving();
            }
            catch (Exception ex)
            {             
                Console.WriteLine(ex);
            }
        }

        public void CompareDashWalkTime()
        {
            var playerPathLenght = Helper.GetPathLenght(Variables.Player.GetPath(EndPosition)) / Variables.Player.MoveSpeed;

            if (playerPathLenght > PathTime)
            {
                this.FasterThanWalking = false;
                Drawing.DrawText(500, 800, Color.Red, "FasterThanWalking");
            }
            else
            {
                Drawing.DrawText(500, 800, Color.Green, "FasterThanWalking");
                FasterThanWalking = true;
            }
        }

        public void CheckForWallDashes()
        {
            
        }

        public void CheckWallDashTimeSaving()
        {
            if (DashObject.IsWallDash)
            {
                var PathSpeedWalking = Helper.GetPathLenght(Variables.Player.GetPath(EndPosition)) / Variables.Player.MoveSpeed;
                var PathSpeedDashing = this.PathTime;

                if (PathSpeedWalking <= PathSpeedDashing)
                {
                    DashObject.WallDashSavesTime = true;
                    WallDashSavesTime = true;
                }
            }
        }

        public void CheckForShield()
        {
            if (PathLenght <= ProviderFlow.GetRemainingUnits())
            {
                this.GetsShield = true;
            }
            else
            {
                GetsShield = false;
            }
        }

        public void RemoveUnit(Obj_AI_Base unit)
        {
            if (this.Units.Contains(unit))
            {
                this.Units.Remove(unit);
            }
        }

        public void AddUnit(Obj_AI_Base unit)
        {
            if (!this.Units.Contains(unit))
            {
                this.Units.Add(unit);
            }
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
                if (this.Units != null && this.Units.Count > 0
                    && this.Positions != null && this.Positions.Count > 0)
                {
                    Drawing.DrawCircle(FirstUnit.Position, 50, Color.Aqua);

                    for (var i = 0; i < this.Positions.Count; i++)
                    {
                        if (this.Positions.Count > i + 1)
                        {
                            Drawing.DrawLine(
                            Drawing.WorldToScreen(this.Positions[i]),
                            Drawing.WorldToScreen(this.Positions[i + 1]),
                                2f,
                            Color.White);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


        }
    }
}
