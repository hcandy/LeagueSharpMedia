// TODO: Add Multi Pathing System. The Idea is to get some paths that are equally good and compare them then. This way you could do things like if Path A is safer than Path B in Situation X choose Path A

namespace Yasuo.Skills.Combo
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
    using Yasuo.Common.Pathing;
    using Yasuo.Common.Provider;
    using Yasuo.Common.Utility;
    using Yasuo.Modules;
    using Yasuo.Modules.WallDash;

    using YasuoDash = Common.Pathing.Dash;

    internal class SweepingBlade : Child<Combo>
    {
        public SweepingBlade(Combo parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public List<Obj_AI_Base> BlacklistUnits;

        public Path GapClosePath;

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
                new MenuItem(this.Name + "MinHitAOE", "Min HitCount for AOE").SetValue(new Slider(2, 2, 5)));

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
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                || !Variables.Spells[SpellSlot.E].IsReady())
            {
                GapClosePath = null;
                return;
            }

            #region Cast on Champion

            var targetE = TargetSelector.SelectedTarget ?? TargetSelector.GetTarget(
                Variables.Spells[SpellSlot.E].Range,
                TargetSelector.DamageType.Physical);

            if (targetE != null
                && targetE.Distance(Variables.Player.ServerPosition) <= Variables.Spells[SpellSlot.E].Range)
            {
                YasuoDash dash = new YasuoDash(targetE);

                Vector3 meanVector =
                    Helper.GetMeanVector3(
                        HeroManager.Enemies.Where(x => x.Distance(Variables.Player.ServerPosition) <= 900)
                            .Select(enemy => enemy.ServerPosition)
                            .ToList());

                if (meanVector != Vector3.Zero)
                {
                    if (meanVector.Distance(dash.EndPosition) >= meanVector.Distance(Variables.Player.ServerPosition)
                        && dash.DangerValue <= 4
                        && Game.CursorPos.Distance(dash.EndPosition) <= Game.CursorPos.Distance(dash.StartPosition))
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
                    var target = TargetSelector.GetTarget(
                        Variables.Spells[SpellSlot.Q].Range,
                        TargetSelector.DamageType.Physical);

                    if (Menu.Item(this.Name + "Prediction").GetValue<bool>())
                    {
                        targetedVector =
                            Prediction.GetPrediction(target, Variables.Player.Distance(target) / 1000).UnitPosition;
                    }
                    else
                    {
                        targetedVector = target.ServerPosition;
                    }

                    break;
            }

            #endregion

            #region Path Settings

            if (Menu.Item(this.Name + "PathAroundSkillShots").GetValue<bool>())
            {
                GapClosePath = this.ProviderE.GetPath(targetedVector, aroundSkillshots: true);
            }
            else
            {
                GapClosePath = this.ProviderE.GetPath(targetedVector);
            }

            #endregion

            #region When to Execute and when not

            // if a path is given, and the first unit of the path is in dash range, and the path time is faster than running to the given vector (dashVactor)
            if (GapClosePath != null
                && Variables.Player.Distance(GapClosePath.FirstUnit) <= Variables.Spells[SpellSlot.E].Range
                && GapClosePath.PathTime
                <= Helper.GetPathLenght(Variables.Player.GetPath(targetedVector)) / Variables.Player.MoveSpeed)
            {
                #region WallCheck

                // if WallDash
                if (GapClosePath.FirstUnit.IsWallDash(Variables.Spells[SpellSlot.E].Range))
                {
                    if (
                        Helper.GetPathLenght(
                            Variables.Player.GetPath(
                                Variables.Player.ServerPosition.Extend(
                                    GapClosePath.FirstUnit.ServerPosition,
                                    Variables.Spells[SpellSlot.E].Range),
                                targetedVector)) < Helper.GetPathLenght(Variables.Player.GetPath(targetedVector)))
                    {
                        Game.PrintChat(
                            "Next Dash is a walldash, and the new distance to the Aimed Vector is lower than before.");

                        Execute(GapClosePath.FirstUnit);
                    }
                    //TODO: else, find new path
                }
                else
                {
                    Execute(GapClosePath.FirstUnit);
                }

                #endregion
            }

            #endregion
        }

        public void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender != Variables.Player || args.SData.Name != "YasuoDashWrapper")
            {
                return;
            }

            if (this.GapClosePath != null && this.GapClosePath.Units.Contains(args.Target))
            {
                this.GapClosePath.RemoveUnit((Obj_AI_Base)args.Target);
            }
        }

        public void OnDraw(EventArgs args)
        {
            this.GapClosePath?.Draw();
        }

        private void Execute(Obj_AI_Base unit)
        {
            try
            {
                if (!unit.IsValidTarget()
                    || !ProviderTurret.IsSafe(Variables.Player.ServerPosition.Extend(unit.ServerPosition, Variables.Spells[SpellSlot.E].Range))
                    || unit.HasBuff("YasuoDashWrapper"))
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