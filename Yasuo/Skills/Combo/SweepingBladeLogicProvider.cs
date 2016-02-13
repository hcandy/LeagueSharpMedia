namespace Yasuo.Skills.Combo
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Yasuo.Common.Utility;
    using Yasuo.Common.Utility.Djikstra;

    //TODO: Add Waypoint System
    public class SweepingBladeLogicProvider
    {
        public Helper Helper { get; }

        public SweepingBladeLogicProvider()
        {
            this.Helper = this.Helper;
        }

        /// <summary>
        /// The MAXIMAL calculation distance. Every unit in a range of 3000 units will be taken into consideration.
        /// </summary>
        public static float CalculationDistance = 3000f;

        /// <summary>
        /// Returns the unit that is best to Gapclose to a given position
        /// </summary>
        /// <param name="position">The vector to dash to</param>
        /// <param name="minions"></param>
        /// <param name="champions"></param>
        /// <param name="noLowHealth"></param>
        /// <returns>Obj_AI_Base</returns>
        public Obj_AI_Base GetBestUnit(Vector2 position, bool minions = true, bool champions = true, bool noLowHealth = false)
        {
            return this.GetBestPath(position, minions, champions, noLowHealth).FirstOrDefault();
        }

        /// <summary>
        /// Returns the unit that is best to Gapclose to a given position
        /// </summary>
        /// <param name="position">The vector to dash to</param>
        /// <param name="minions"></param>
        /// <param name="champions"></param>
        /// <param name="noLowHealth"></param>
        /// <returns>Obj_AI_Base</returns>
        public List<Obj_AI_Base> GetBestPath(Vector2 position, bool minions = true, bool champions = true, bool noLowHealth = false)
        {
            var dictNodes = GetUnits(ObjectManager.Player, minions, champions, noLowHealth).ToDictionary(x => x, x => new Node(x));

            var nodes = dictNodes.Values.ToList();

            var edges = new List<Edge>();

            var _path = new List<Obj_AI_Base>();

            foreach (var x in dictNodes.OfType<Obj_AI_Base>())
            {
                edges.AddRange(dictNodes.OfType<Obj_AI_Base>().Where(y => x.Distance(y) <= 900).Select(y => new Edge(dictNodes[x], dictNodes[y], x.Distance(y))));
            }

            // Create new Object of the Djikstra class with values from above
            Dijkstra d = new Dijkstra(edges, nodes);

            // Set starting point, Obj_Ai_Base Player in this case
            d.CalculateDistance(dictNodes[ObjectManager.Player]);

            List<Node> path = d.GetPathTo(dictNodes[TargetSelector.GetSelectedTarget()]);

            return path.OfType<Obj_AI_Base>().ToList();
        }

        /// <summary>
        /// Returns a list containing all units
        /// </summary>
        /// <param name="startPosition">start point (vector)</param>
        /// <param name="minions">bool</param>
        /// <param name="champions">bool</param>
        /// <param name="notInSkillshot">bool</param>
        /// <param name="noLowHealth">bool</param>
        /// <returns>List(Obj_Ai_Base)</returns>
        /// 
        public static List<Obj_AI_Base> GetUnits(Obj_AI_Base startPosition, bool minions = true, bool champions = true, bool notInSkillshot = true, bool noLowHealth = false)
        {
            // Add all units
            var units = new List<Obj_AI_Base>();

            // Add Player and Target since they are start and end point
            units.Add(ObjectManager.Player);
            units.Add(TargetSelector.GetSelectedTarget());

            if (minions)
            {
                units.AddRange(MinionManager.GetMinions(ObjectManager.Player.ServerPosition, CalculationDistance, MinionTypes.All, MinionTeam.NotAlly));
            }

            if (champions)
            {
                units.AddRange(HeroManager.Enemies);
            }

            //if (notInSkillshot)
            //{
            //    foreach (var x in units)
            //    {
            //        if () //unit in SKS
            //        {
            //            units.Remove(x);
            //        }
            //    }
            //}

            if (noLowHealth)
            {
                foreach (var x in units.Where(x => x.Health <= Variables.Spells[SpellSlot.E].GetDamage(x) + 50))
                {
                    units.Remove(x);
                }
            }

            //if (Waypoints() != null)
            //{
            //    units.AddRange(Waypoints());
            //}

            return units.Count == 0 ? null : units;
        }

        public static Vector3 GetEndPosition(Obj_AI_Base target)
        {
            var rawEndPos = Vector3.Zero;
            var realEndPos = Vector3.Zero;

            rawEndPos = ObjectManager.Player.ServerPosition.Extend(target.ServerPosition, Variables.Spells[SpellSlot.E].Range);

            if (rawEndPos.IsWall())
            {
                if (Wall.CanWallDash(target, Variables.Spells[SpellSlot.E].Range))
                    realEndPos = Wall.GetFirstWallPoint(ObjectManager.Player.ServerPosition, rawEndPos, 2);
            }
            else
            {
                realEndPos = rawEndPos;
            }

            return realEndPos;
        }



        /// <summary>
        /// Returns the best unit to dodge
        /// </summary>
        /// <param name="position"></param>
        /// <param name="minions"></param>
        /// <param name="champions"></param>
        /// <returns></returns>
        public Obj_AI_Base GetDodgeUnit(Vector2 position, bool minions = true, bool champions = true)
        {
            var units = GetUnits(ObjectManager.Player, minions, champions, false, false);
            units.Remove(ObjectManager.Player);

            Obj_AI_Base result = null;

            // We are playing aggresive, so we try to dodge over a minion which will put us closest to the target
            if (Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = TargetSelector.SelectedTarget;
                if (target != null)
                {
                    result = this.GetBestUnit(Prediction.GetPrediction(target, Variables.Spells[SpellSlot.E].Speed).UnitPosition.To2D());
                }

            }

            return result;
        }

        /// <summary>
        /// Returns the time of execution for Sweeping Blade (E)
        /// </summary>
        /// <returns></returns>
        public float GetExecutionTime()
        {
            var speed = 1025;
            var distance = 475;

            return distance / speed;
        }

        /// <summary>
        /// Returns the estimated time of arrival
        /// </summary>
        /// <param name="additionalDelay">Adds additional time</param>
        /// <param name="additionalDistance">Adds additional distance</param>
        /// <returns></returns>
        public float TimeOfArival(List<Obj_AI_Base> path, float additionalTime = 0f, float additionalDistance = 0f)
        {
            var distance = 0f;
            var speed = ObjectManager.Player.MoveSpeed;

            var dashedtime = 0f;

            if (path != null)
            {
                dashedtime = path.Sum(unit => this.GetExecutionTime());

                for (int i = 0; i < path.Count; i++)
                {
                    distance += path[i].Distance(path[i + 1]);
                }
            }

            var time = ((distance + additionalDistance) / speed) + additionalTime;
            time -= dashedtime;

            return time;
        }
    }
}
