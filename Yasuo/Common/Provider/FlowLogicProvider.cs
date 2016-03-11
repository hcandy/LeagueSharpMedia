// OBSERVATION: After some Event I don't know yet, its not accurate anymore, until Max Flow.
namespace Yasuo.Common.Provider
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Common.Utility;

    using SharpDX;

    class FlowLogicProvider
    {
        public float GetUnitsUntilMaxFlow()
        {
            return Variables.Player.Level >= 13 ? 4600f : (Variables.Player.Level >= 7 ? 5200f : 5900f);
        }

        private float lastReset; // Game.Time

        private Vector3 lastPosition = Vector3.Zero;

        public float CurrentUnits; // Distance traveled



        public float GetRemainingUnits()
        {
            return this.GetUnitsUntilMaxFlow() - CurrentUnits;
        }

        public void CheckFlow()
        {
            if ((int)Variables.Player.Mana == (int)Variables.Player.MaxMana)
            {
                Reset();
                return;
            }

            if (!lastPosition.Equals(Vector3.Zero))
            {
                CurrentUnits += Variables.Player.Position.Distance(lastPosition);
            }
            lastPosition = Variables.Player.Position;

            if (CurrentUnits >= this.GetUnitsUntilMaxFlow())
            {
                Reset();
            }
        }



        private void Reset()
        {
            lastReset = Helper.GetTick();
            CurrentUnits = 0;
            lastPosition = Vector3.Zero;
        }
    }
}