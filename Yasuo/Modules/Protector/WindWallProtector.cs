//namespace Yasuo.Modules.Protector
//{
//    using System;
//    using System.Linq;

//    using LeagueSharp;
//    using LeagueSharp.Common;

//    using SDK = LeagueSharp.SDK;
//    using Yasuo.Common;
//    using Yasuo.Common.Utility;
//    using Yasuo.Common.Utility.Enums;
//    using Yasuo.Common.Extensions;
//    using Yasuo.Evade;
//    using Yasuo.Modules.Protector;

//    internal class WindWallProtector : Child<Protector>
//    {
//        public WindWallProtector(Protector parent) : base(parent)
//        {
//            this.OnLoad();
//        }

//        public override string Name => "SafeZone";

//        public SafeZoneLogicProvider Provider;

//        protected override void OnEnable()
//        {
//            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
//            Game.OnUpdate += OnUpdate;
//            base.OnEnable();
//        }

//        protected override void OnDisable()
//        {
//            Obj_AI_Base.OnProcessSpellCast -= OnProcessSpellCast;
//            Game.OnUpdate -= OnUpdate;
//            base.OnDisable();
//        }

//        protected override void OnLoad()
//        {
//            Menu = new Menu(Name, Name);

//            var championMenu = new Menu("Blacklist", Name + "Blacklist");

//            foreach (var x in HeroManager.Allies)
//            {
//                championMenu.AddItem(new MenuItem(championMenu.Name + x.Name, x.Name).SetValue(true));
//            }


//            Menu.AddSubMenu(championMenu);

//            Menu.AddItem(new MenuItem(Name + "Enabled", "Enabled").SetValue(false));

//            Parent.Menu.AddSubMenu(Menu);
//        }

//        protected override void OnInitialize()
//        {
//            Provider = new SafeZoneLogicProvider();
//            base.OnInitialize();
//        }

//        public void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
//        {
//            //if (sender.IsAlly && !sender.IsMe)
//            //{
//            //    return;
//            //}
//            //if (sender.IsMe && args.SData.Name == "WindWall" || args.SData.Name.Contains(SDK.SpellDatabase.GetByMissileName("WindWall").ToString()))
//            //{
//            //    Provider.
//            //}

//            //var spell = SDK.SpellDatabase.GetByName(args.SData.Name);
//            //if (sender.IsEnemy && CheckForCollision(args))
//            //{
//            //    Provider.
//            //}
//        }

//        public bool CheckForCollision(GameObjectProcessSpellCastEventArgs args)
//        {
//            for (int i = 0; i < 5; i++)
//            {
//                if (SDK.SpellDatabase.GetByName(args.SData.Name).CollisionObjects[i]
//                    == SDK.CollisionableObjects.YasuoWall)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        public void OnUpdate(EventArgs args)
//        {
            
//        }





//        /* internal class SteelTempest : Child<Spells>
//    {
//        public SteelTempest(Spells parent) : base(parent)
//        {
//            OnLoad();
//        }

//        public override string Name => "Steel Tempest";

//        public SteelTempestLogicProvider ProviderQ;

//        public SweepingBladeLogicProvider ProviderE;

//        protected override void OnEnable()
//        {
//            Game.OnUpdate += OnUpdate;
//            base.OnEnable();
//        }

//        protected override void OnDisable()
//        {
//            Game.OnUpdate -= OnUpdate;
//            base.OnDisable();
//        }

//        // TODO: Add Spell specific settings
//        protected sealed override void OnLoad()
//        {

//                Menu = new Menu(Name, Name);

//                var championMenu = new Menu("Blacklist", Name + "Blacklist");

//                foreach (var x in HeroManager.Enemies)
//                {
//                    championMenu.AddItem(new MenuItem(championMenu.Name + x.Name, x.Name).SetValue(true));
//                }
//                Menu.AddSubMenu(championMenu);

//                Menu.AddItem(new MenuItem(Name + "Enabled", "Enabled").SetValue(false));

//                Parent.Menu.AddSubMenu(Menu);

//        }

//        protected override void OnInitialize()
//        {
//                ProviderQ = new SteelTempestLogicProvider();
//                ProviderE = new SweepingBladeLogicProvider();

//                base.OnInitialize();
//        }

//        public void OnUpdate(EventArgs args)
//        {
//            var target = TargetSelector.GetSelectedTarget();
//            var targetPred = Prediction.GetPrediction(target, ProviderE.GetSpeed());

//            if (target != null)
//            {
//                if (target.IsValidTarget())
//                {
//                    #region EQ

//                    // EQ > Synergyses with the E function in SweepingBlade/LogicProvider.cs
//                    if (Manager.Player.IsDashing()
//                        && targetPred.UnitPosition.Distance(ObjectManager.Player.ServerPosition) <= 375)
//                    {
//                        CastSteelTempest(Manager.Player);
//                    }

//                    if (Manager.Player.IsDashing() && Variables.Spells[SpellSlot.Q].Cooldown < ProviderE.)
//                    {
//                    #endregion

//                        if (!Manager.Player.IsDashing()) 
//                    }
//                }
//            }
//        }

//        public void ExecuteFarm()
//        {
//            //TODO: Jungle Clear && Lane Clear
//        }

//        private static void CastSteelTempest(Obj_AI_Base target)
//        {
           
//        } */

//        /* k Thnx bea */
//        /*
//                public static Geometry.Polygon GetSafeZone(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args = null) //my methos uses sender and args instead of everything at once. Outcome is the same I guess.
//                {
//                    var PredictedWWPos = Manager.Player.ServerPosition.Extend(sender.ServerPosition, 300).To2D();
//                    var Direction = (PredictedWWPos - Manager.Player.ServerPosition.To2D()).Normalized().Perpendicular();

//                    var WWRPos = PredictedWWPos + (250) * Direction; // 250 === 500 / 2
//                    var WWLPos = PredictedWWPos - (250) * Direction; // 250 === 500 / 2

//                    float Radius;

//                    Radius = args?.SData.CastRadius ?? 1500f;

//                    var WWRPosExtend = sender.ServerPosition.To2D().Extend(WWRPos, Radius);
//                    var WWLPosExtend = sender.ServerPosition.To2D().Extend(WWLPos, Radius);

//                    var SafeZone = new Geometry.Polygon();
//                    {
//                        SafeZone.Add(WWLPos);
//                        SafeZone.Add(WWRPos);
//                        SafeZone.Add(WWLPosExtend);
//                        SafeZone.Add(WWRPosExtend);
//                        SafeZone.Add(GibTheArkOfNoah(WWLPosExtend, WWRPosExtend, sender.ServerPosition.To2D(), Radius)); // <------ (╯°□°)╯︵ <pǝɹǝƃuop ǝƃɐssǝɯ>
//                    }

//                    return SafeZone;
//                }

//                public static Geometry.Polygon.Arc GibTheArkOfNoah(Vector2 startArc, Vector2 endArc, Vector2 center, float radius)
//                {
//                    float angle = PointsAngle(center, startArc, endArc);
//                    return new Geometry.Polygon.Arc(startArc, center, angle, radius);
//                }

//                public static float PointsAngle(Vector2 a, Vector2 b, Vector2 c)
//                {
//                    Vector2 lenA = b.Distance(c), lenB = a.Distance(c), lenC = b.Distance(a);
//                    var angle = (float)Math.Cosh((lenB * lenB) + (lenC * lenC) - (lenA * lenA)) / (2 * lenB * lenC);
//                    return angle;
//                }

//                public static float DegreeToRadian(float angle)
//                {
//                    return Math.PI * angle / 180f;
//                }

//                public static float RadianToDegree(float angle)
//                {
//                    return angle * (180f / Math.PI);
//                }

//                public static void OnDraw(EventArgs args)
//                {
//                    if (Player.IsDead && Player.IsZombie) return;

//                    if (TargetSelector.GetSelectedTarget() != null)
//                    {
//                        WindWall.SafeZone(TargetSelector.GetSelectedTarget()).Draw(System.Drawing.Color.AntiqueWhite, 5);
//                    }
//                }
//                */
//    }
//}
