﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using LeagueSharp;

//namespace Yasuo
//{
//    using LeagueSharp.Common;

//    using SharpDX;

//    class FlowManager
//    {
//        public static Obj_AI_Hero Player => ObjectManager.Player;

//        public static float GetUnitsUntilMaxFlow()
//        {
//            return Manager.Player.Level >= 13 ? 4600f : (Manager.Player.Level >= 7 ? 5200f : 5900f);
//        }

//        private static float lastReset; //Game.Time

//        private static Vector3 lastPosition = Vector3.Zero;

//        public static float CurrentUnits; //Distance traveled

//        private static readonly DateTime AssemblyLoadTime = DateTime.Now;

//        public static float GetRemainingUnits()
//        {
//            return FlowManager.GetUnitsUntilMaxFlow() - CurrentUnits;
//        }

//        public static void CheckFlow()
//        {
//            if (Manager.Player.Mana >= Manager.Player.MaxMana)
//            {
//                Reset();
//                return;
//            }
//            if (!lastPosition.Equals(Vector3.Zero))
//            {
//                CurrentUnits += Manager.Player.Position.Distance(lastPosition);
//            }
//            lastPosition = Manager.Player.Position;
//            if (CurrentUnits >= FlowManager.GetUnitsUntilMaxFlow())
//            {
//                Reset();
//            }
//        }

//        private static float GetTick()
//        {
//            return (int)DateTime.Now.Subtract(AssemblyLoadTime).TotalMilliseconds;
//        }

//        public static void Reset()
//        {
//            lastReset = GetTick();
//            CurrentUnits = 0;
//            lastPosition = Vector3.Zero;
//        }
//    }
//}
