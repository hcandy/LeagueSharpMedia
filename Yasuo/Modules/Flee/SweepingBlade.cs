// TODO: Add Multi Pathing System. The Idea is to get some paths that are equally good and compare them then. This way you could do things like if Path A is safer than Path B in Situation X choose Path A

namespace Yasuo.Modules.Flee
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Yasuo.Common.Classes;
    using Yasuo.Common.Objects;
    using Yasuo.Common.Provider;
    using Yasuo.Common.Utility;

    internal class SweepingBlade : Child<Flee>
    {
        public SweepingBlade(Flee parent)
            : base(parent)
        {
            this.OnLoad();
        }

        public List<Obj_AI_Base> BlacklistUnits;

        public Path Path;

        public override string Name => "Flee";

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

            // Spell Settings
            this.Menu.AddItem(
                new MenuItem(this.Name + "StackQ", "Keybind").SetValue(new KeyBind(5, KeyBindType.Press)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "PathAroundSkillShots", "[Disabled] Try to Path around Skillshots").SetValue(
                    true).SetTooltip("if this is enabled, the assembly will path around a skillshot if a path is given"));




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
                if (!this.Menu.Item(this.Name + "Keybind").GetValue<KeyBind>().Active
                    || !Variables.Spells[SpellSlot.E].IsReady())
                {
                    return;
                }

                Variables.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                var targetedVector = Game.CursorPos;

                if (targetedVector != Vector3.Zero)
                {
                    this.Path = this.ProviderE.GetPath(targetedVector);
                }


                #region When to Execute and when not

                // if a path is given, and the first unit of the path is in dash range, and the path time is faster than running to the given vector (dashVactor)
                // TODO: Sometimes when the Path is very crowded it happens that the Dash will get executed in the wrong direction. A more accurate pathing algorithm through making unit connections in SweepingBladeLogicProvider faster will fix that
                if (this.Path != null
                    && Variables.Player.Distance(this.Path.FirstUnit) <= Variables.Spells[SpellSlot.E].Range
                    && this.Path.FasterThanWalking
                    )
                {
                    #region WallCheck

                    if (this.Path.DashObject.IsWallDash)
                    {
                        this.Execute(
                            this.Path.WallDashSavesTime
                                ? this.Path.DashObject.Unit
                                : this.ProviderE.GetAlternativePath(this.Path).DashObject.Unit);
                    }

                    #endregion

                    else if (this.Path.DashObject.EndPosition.Distance(targetedVector) <= (Variables.Player.Distance(targetedVector)))
                    {
                        Drawing.DrawCircle(this.Path.DashObject.EndPosition, 150, System.Drawing.Color.Red);
                        this.Execute(this.Path.FirstUnit);
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
                if (unit != null && !unit.IsValidTarget() || unit.HasBuff("YasuoDashWrapper"))
                {
                    return;
                }

                Variables.Spells[SpellSlot.E].CastOnUnit(unit);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Modules/Flee/SweepingBlade/Execute(): " + ex);
            }
        }
    }
}