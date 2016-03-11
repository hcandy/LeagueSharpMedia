// TODO: Add Multi Pathing System. The Idea is to get some paths that are equally good and compare them then. This way you could do things like if Path A is safer than Path B in Situation X choose Path A

namespace Yasuo.Skills.Combo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.ExceptionServices;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Yasuo.Common;
    using Yasuo.Common.Classes;
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Objects;
    using Yasuo.Common.Provider;
    using Yasuo.Common.Utility;
    using Yasuo.Modules;
    using Yasuo.Modules.WallDash;

    internal class SweepingBlade : Child<Combo>
    {
        public SweepingBlade(Combo parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public List<Obj_AI_Base> BlacklistUnits;

        public Path Path;

        public override string Name => "Sweeping Blade";

        public SweepingBladeLogicProvider ProviderE;

        public TurretLogicProvider ProviderTurret;

        protected override void OnEnable()
        {
            Game.OnUpdate += this.OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += this.OnProcessSpellCast;
            Drawing.OnDraw += this.OnDraw;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            Game.OnUpdate -= this.OnUpdate;
            Obj_AI_Base.OnProcessSpellCast -= this.OnProcessSpellCast;
            Drawing.OnDraw -= this.OnDraw;
            base.OnDisable();
        }

        protected override sealed void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);
            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            // Blacklist
            var blacklist = new Menu("Blacklist", this.Name + "Blacklist");

            if (HeroManager.Enemies.Count == 0)
            {
                blacklist.AddItem(new MenuItem(blacklist.Name + "null", "No enemies found"));
            }
            else
            {
                foreach (var x in HeroManager.Enemies)
                {
                    blacklist.AddItem(new MenuItem(blacklist.Name + x.ChampionName, x.ChampionName).SetValue(false));
                }
                MenuExtensions.AddToolTip(
                    blacklist,
                    "Setting a champion to 'on', will make the script not using Q for him anymore");
            }
            this.Menu.AddSubMenu(blacklist);

            // Spell Settings

            // Mode
            this.Menu.AddItem(
                new MenuItem(this.Name + "ModeTarget", "Dash to: ").SetValue(new StringList(new[] { "Mouse", "Enemy" }))
                    .SetTooltip("The assembly will try to E on a minion in order to Q"));

            this.Menu.AddItem(
                new MenuItem(this.Name + "PathAroundSkillShots", "[Disabled] Try to Path around Skillshots").SetValue(
                    true).SetTooltip("if this is enabled, the assembly will path around a skillshot if a path is given"));

            // EQ

            #region EQ

            this.Menu.AddItem(
                new MenuItem(this.Name + "EQ", "Try to E for EQ").SetValue(true)
                    .SetTooltip("The assembly will try to E on a minion in order to Q"));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinHitAOE", "Min HitCount for AOE").SetValue(new Slider(1, 1, 5)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinOwnHealth", "Min Player Health%").SetValue(new Slider(15, 1, 100))
                    .SetTooltip("The assembly will try to E on a minion in order to Q"));

            #endregion

            // Prediction

            this.Menu.AddItem(
                new MenuItem(this.Name + "Prediction", "Predict enemy position").SetValue(true)
                    .SetTooltip(
                        "The assembly will try to E to the enemy predicted position. This will not work if Mode is set to Mouse."));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        protected override void OnInitialize()
        {
            this.ProviderE = new SweepingBladeLogicProvider();
            this.ProviderTurret = new TurretLogicProvider();

            base.OnInitialize();
        }

        public void OnUpdate(EventArgs args)
        {
            try
            {
                if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                || !Variables.Spells[SpellSlot.E].IsReady())
                {
                    return;
                }


                #region EQ

                if (Menu.Item(this.Name + "EQ").GetValue<bool>())
                {
                    var possibleEqDashes = new List<Yasuo.Common.Objects.Dash>();
                    var unitsEq =
                        ProviderE.GetUnits(Variables.Player.ServerPosition)
                            .Where(x => x.Distance(Variables.Player) <= Variables.Spells[SpellSlot.E].Range);

                    var objAiBases = unitsEq as IList<Obj_AI_Base> ?? unitsEq.ToList();
                    if (objAiBases.Any())
                    {
                        foreach (var unit in objAiBases.Where(x => Variables.Player.ServerPosition.Extend(x.ServerPosition, Variables.Spells[SpellSlot.E].Range).CountEnemiesInRange(350)
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
            catch (Exception ex)
            {
                
                Console.WriteLine(ex);
            }
            
        }

        public void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender != Variables.Player || args.SData.Name != "YasuoDashWrapper")
            {
                return;
            }

            if (this.Path != null && this.Path.Units.Contains(args.Target))
            {
                this.Path.RemoveUnit((Obj_AI_Base)args.Target);
            }
        }

        public void OnDraw(EventArgs args)
        {
            //this.GapClosePath?.RealPath.Draw();
            this.Path?.Draw();
            //this.Path?.DashObject?.Draw();
        }

        private void Execute(Obj_AI_Base unit)
        {
            try
            {
                if (unit != null && !unit.IsValidTarget() || unit.HasBuff("YasuoDashWrapper") 
                    || ProviderTurret.IsSafePosition(Variables.Player.ServerPosition.Extend(unit.ServerPosition, Variables.Spells[SpellSlot.R].Range)))
                {
                    return;
                }

                Variables.Spells[SpellSlot.E].CastOnUnit(unit);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Skills/Combo/SweepingBlade/Execute(): " + ex);
            }
        }
    }
}