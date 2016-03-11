namespace Yasuo.Skills.Mixed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Yasuo.Common;
    using Yasuo.Common.Classes;
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Objects;
    using Yasuo.Common.Provider;
    using Yasuo.Common.Utility;

    internal class SweepingBlade : Child<Mixed>
    {
        public SweepingBlade(Mixed parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public List<Obj_AI_Base> BlacklistUnits;

        public Path Path;

        public override string Name => "Sweeping Blade";

        public SweepingBladeLogicProvider ProviderE;

        protected override void OnEnable()
        {
            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            Game.OnUpdate -= this.OnUpdate;
            Drawing.OnDraw -= this.OnDraw;
            base.OnDisable();
        }

        protected override sealed void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);
            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            // TODO: Add completely automatic harass (dash in/out)
            //// Mode
            //this.Menu.AddItem(
            //    new MenuItem(this.Name + "ModeTarget", "Dash to: ").SetValue(new StringList(new[] { "Mouse", "Auto" })));

            // EQ

            #region EQ

            this.Menu.AddItem(
                new MenuItem(this.Name + "EQ", "Try to E for EQ").SetValue(true)
                    .SetTooltip("The assembly will try to E on a minion in order to Q"));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinHitAOE", "Min HitCount for AOE").SetValue(new Slider(1, 1, 15)));

            #endregion

            #region E LastHit

            this.Menu.AddItem(
                new MenuItem(this.Name + "LastHit", "Smart Lasthit").SetValue(true)
                    .SetTooltip("The assembly will only Lasthit a minion if Q is not up and the end position of the dash is not too close to the enemy and is not inside a skillshot"));

            #endregion

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            this.ProviderE = new SweepingBladeLogicProvider();

            base.OnInitialize();
        }

        public void OnUpdate(EventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed)
            {
                return;
            }

            Obj_AI_Base minion = MinionManager.GetMinions(Game.CursorPos, 475).Where(x =>
                                !x.HasBuff("YasuoDashWrapper")
                                && x.Distance(Variables.Player) <= Variables.Spells[SpellSlot.E].Range)
                                .OrderByDescending(x => x.Health)
                                .FirstOrDefault();

            var minions = MinionManager.GetMinions(
                Variables.Player.ServerPosition,
                Variables.Spells[SpellSlot.E].Range,
                MinionTypes.All,
                MinionTeam.Enemy,
                MinionOrderTypes.None);

            if (Menu.Item(this.Name + "LastHit").GetValue<bool>())
            {
                if (minions == null)
                {
                    return;
                }

                var enemies = HeroManager.Enemies.Where(x => x.Health > 0).ToList();
                var possibleExecutions = new List<Obj_AI_Base>();

                foreach (
                    var x in
                        minions.Where(
                            unit =>
                            unit.Health <= this.ProviderE.GetDamage(unit)
                            && unit.Distance(Variables.Player.ServerPosition) <= Variables.Spells[SpellSlot.E].Range))
                {
                    if (enemies.Count(enemy => enemy.Distance(Variables.Player.ServerPosition) <= 1000) > 0)
                    {
                        foreach (var y in enemies.Where(z => z.HealthPercent > 10))
                        {
                            var newPos = Variables.Player.ServerPosition.Extend(
                                x.ServerPosition,
                                Variables.Spells[SpellSlot.E].Range);
                            if (newPos.Distance(y.ServerPosition) < y.AttackRange)
                            {
                                possibleExecutions.Add(x);
                            }
                        }
                    }
                    else
                    {
                        possibleExecutions.Add(minion);
                    }
                }

                if (possibleExecutions.Count == 0)
                {
                    return;
                }

                Execute(possibleExecutions.MinOrDefault(x => x.Distance(Helper.GetMeanVector2(minions))));
            }

            #region EQ

            if (Menu.Item(this.Name + "EQ").GetValue<bool>())
            {
                var possibleEqDashes = new List<Yasuo.Common.Objects.Dash>();
                var unitsEq =
                    ProviderE.GetUnits(Variables.Player.ServerPosition)
                        .Where(x => x.Distance(Variables.Player) <= Variables.Spells[SpellSlot.E].Range);

                if (unitsEq != null && unitsEq.Any())
                {
                    foreach (var unit in unitsEq.Where(x => Variables.Player.ServerPosition.Extend(x.ServerPosition, Variables.Spells[SpellSlot.E].Range).CountEnemiesInRange(350)
                                                    >= Menu.Item(this.Name + "MinHitAOE").GetValue<Slider>().Value).ToList())
                    {
                        possibleEqDashes.Add(new Yasuo.Common.Objects.Dash(unit));
                    }
                }

                if (possibleEqDashes.Any())
                {
                    Execute(possibleEqDashes.MaxOrDefault(x => x.Unit.CountEnemiesInRange(350)).Unit);
                }
            }

            #endregion

            #region Cast on Champion

            var targetE = TargetSelector.SelectedTarget ?? TargetSelector.GetTarget(
                Variables.Spells[SpellSlot.E].Range,
                TargetSelector.DamageType.Physical);

            if (targetE != null
                && targetE.Distance(Variables.Player.ServerPosition) <= Variables.Spells[SpellSlot.E].Range)
            {
                var dash = new Yasuo.Common.Objects.Dash(targetE);

                // 1v1
                if (dash.EndPosition.CountEnemiesInRange(1000) <= 1
                    && dash.EndPosition.Distance(Prediction.GetPrediction(dash.Unit, 500).CastPosition) <= Variables.Player.AttackRange
                    && dash.EndPosition.Distance(Prediction.GetPrediction(dash.Unit, 500).CastPosition) <= Variables.Spells[SpellSlot.Q].Range && Variables.Spells[SpellSlot.Q].IsReady())
                {
                    Execute(targetE);
                }

                Vector3 meanVector = Helper.GetMeanVector3(
                            HeroManager.Enemies.Where(x => x.Distance(Variables.Player.ServerPosition) <= 900)
                            .Select(enemy => enemy.ServerPosition)
                            .ToList());

                if (meanVector != Vector3.Zero)
                {
                    if (meanVector.Distance(Path.DashObject.EndPosition) >= meanVector.Distance(Variables.Player.ServerPosition)
                        && Path.DashObject.DangerValue <= 4
                        && Game.CursorPos.Distance(Path.DashObject.EndPosition) <= Game.CursorPos.Distance(Path.DashObject.StartPosition)
                        && Variables.Player.HealthPercent > Menu.Item(this.Name + "MinOwnHealth").GetValue<Slider>().Value)
                    {
                        Execute(targetE);
                    }
                }
            }

            #endregion

            #region targetedVector Mode

            var targetedVector = Vector3.Zero;

            switch (this.Menu.Item(this.Name + "ModeTarget").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    targetedVector = Game.CursorPos;
                    break;
                case 1:
                    var target = TargetSelector.GetTarget(Variables.Spells[SpellSlot.Q].Range, TargetSelector.DamageType.Physical);

                    if (Menu.Item(this.Name + "Prediction").GetValue<bool>())
                    {
                        if (target != null && target.IsValid)
                        {
                            targetedVector = Prediction.GetPrediction(target, Variables.Player.Distance(target) / 1000).UnitPosition;
                        }
                        else
                        {
                            targetedVector = Game.CursorPos;
                        }

                    }
                    else
                    {
                        targetedVector = target.ServerPosition;
                    }

                    break;
            }

            #endregion

            #region Path Settings

            //if (Menu.Item(this.Name + "PathAroundSkillShots").GetValue<bool>())
            //{
            //    GapClosePath = this.ProviderE.GetPath(targetedVector, aroundSkillshots: true);
            //}
            //else
            {
                this.Path = this.ProviderE.GetPath(targetedVector);
            }

            #endregion

            #region When to Execute and when not

            Drawing.DrawText(500, 600, System.Drawing.Color.Red, "Walk Path Lenght: " + Helper.GetPathLenght(Variables.Player.GetPath(targetedVector)));
            Drawing.DrawText(500, 660, System.Drawing.Color.Red, "Walk Path Time: " + Helper.GetPathLenght(Variables.Player.GetPath(targetedVector)) / Variables.Player.MoveSpeed);

            // if a path is given, and the first unit of the path is in dash range, and the path time is faster than running to the given vector (dashVactor)
            // TODO: Sometimes when the Path is very crowded it happens that the Dash will get executed in the wrong direction. A more accurate pathing algorithm through making unit connections in SweepingBladeLogicProvider faster will fix that
            if (this.Path != null
                && Variables.Player.Distance(this.Path.FirstUnit) <= Variables.Spells[SpellSlot.E].Range
                && this.Path.FasterThanWalking
                )
            {
                #region WallCheck

                if (Path.DashObject.IsWallDash)
                {
                    this.Execute(
                        this.Path.WallDashSavesTime
                            ? this.Path.DashObject.Unit
                            : this.ProviderE.GetAlternativePath(this.Path).DashObject.Unit);
                }

                #endregion

                else if (Path.DashObject.EndPosition.Distance(targetedVector) <= (Variables.Player.Distance(targetedVector)))
                {
                    Drawing.DrawCircle(Path.DashObject.EndPosition, 150, System.Drawing.Color.Red);
                    Execute(this.Path.FirstUnit);
                }
            }

            #endregion
        }

        public void OnDraw(EventArgs args)
        {
            
        }

        private void Execute(Obj_AI_Base target)
        {
            if (target.IsValidTarget() && target != null)
            {
                Variables.Spells[SpellSlot.E].CastOnUnit(target);
            }
        }
    }
}

