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

    using Yasuo.Common.Provider;
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Utility;

    using Color = System.Drawing.Color;

    class LastBreath
    {
        public LastBreathLogicProvider ProviderR;

        public TurretLogicProvider ProviderTurret;

        public Vector3 StartPosition;

        public Vector3 EndPosition;

        public Obj_AI_Hero Target;

        public LastBreath(Obj_AI_Hero unit)
        {
            ProviderR = new LastBreathLogicProvider();
            ProviderTurret = new TurretLogicProvider();

            this.Target = unit;
            this.StartPosition = Variables.Player.ServerPosition;

            this.SetEndPosition();
            this.SetAffectedEnemies();
            this.SetTravelDistance();
            this.SetDangerValue();
            this.SetKnockUpAmount();
            this.SetDamageDealt();
        }

        public int DangerValue { get; private set; }

        public float TravelDistance { get; private set; }

        public int EnemiesInUlt { get; private set; }

        public float DamageDealt { get; private set; }

        public List<Obj_AI_Hero> AffectedEnemies { get; private set; } 

        private void SetDangerValue()
        {
            try
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => !x.IsAirbone() && x.Distance(EndPosition) <= 750))
                {
                    DangerValue += 1;
                }

                if (!ProviderTurret.IsSafe(EndPosition))
                {
                    DangerValue += 5;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"[SetDangerValue:] "+ex); 
            }


            // TODO: Add Skillshots
        }

        private void SetTravelDistance()
        {
            this.TravelDistance = this.StartPosition.Distance(this.EndPosition);
        }

        private void SetEndPosition()
        {
            ProviderR.GetExecutionPosition(Target);
        }

        private void SetKnockUpAmount()
        {
            if (AffectedEnemies != null)
            {
                EnemiesInUlt = this.AffectedEnemies.Count;
            }
        }

        private void SetDamageDealt()
        {
            if (AffectedEnemies != null)
            {
                foreach (var enemy in this.AffectedEnemies)
                {
                    DamageDealt += Variables.Spells[SpellSlot.R].GetDamage(enemy);
                }
            }
            Game.PrintChat("Predicted Dmg dealt: "+DamageDealt);
        }

        private void SetAffectedEnemies()
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsAirbone() && x.Distance(EndPosition) <= 475))
            {
                this.AffectedEnemies.Add(enemy);
            }
            Game.PrintChat("Enemies knocked up: "+AffectedEnemies.Count);
        }

        public void Draw()
        {
            var color = Color.DeepSkyBlue;

            Drawing.DrawLine(
                Drawing.WorldToScreen(this.StartPosition),
                Drawing.WorldToScreen(this.EndPosition.Extend(Variables.Player.ServerPosition, 200)),
                1f,
                Color.White);


            Drawing.DrawCircle(this.EndPosition, 200, Color.White);

            foreach (var enemy in this.AffectedEnemies)
            {
                Drawing.DrawCircle(enemy.Position, enemy.BoundingRadius, color);
            }
        }

    }
}
