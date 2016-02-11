//// TODO: Maybe Block spells that aiming an enemy and are blockable i.e: Lux W
//// TODO: E when W not needed
//// TODO: E behind W when skillshot is targeted (ie. cait ult) will hit you or next AA will kill you or do much dmg
//// TODO: Anti Gragas Insec
//// TODO: Add delay to Special Interactions
//// TODO: Crit in AA
//// TODO: 1v1 SafeZone logic
//// TODO: Clean up code
//// TODO: Annie Stun, Katarina Ult

//namespace Yasuo
//{
//    using System.Collections.Generic;
//    using System.Linq;

//    using LeagueSharp;
//    using LeagueSharp.Common;
//    using Geometry = LeagueSharp.Common.Geometry;

//    using SDK = LeagueSharp.SDK;

//    using SharpDX;

//    using Yasuo.Common;
//    using Yasuo.Evade;

//    public class WindWall : Child<Spells>
//    {

//        public static Obj_AI_Hero Player => ObjectManager.Player;

//        public static Geometry.Polygon SafeZone(Skillshot Skillshot)
//        {
//            var direction = (Skillshot.Start - Manager.Player.ServerPosition.To2D()).Normalized().Perpendicular();

//            var wwrPos = predictedWwPos + 500 / 2 * direction;
//            var wwlPos = predictedWwPos - 500 / 2 * direction;

//            float radius;

//            radius = args != null ? args.SData.CastRadius : 1500f;

//            var wwrPosExtend = sender.ServerPosition.To2D().Extend(wwrPos, radius);
//            var wwlPosExtend = sender.ServerPosition.To2D().Extend(wwlPos, radius);

//            var safeZone = new Geometry.Polygon();
//            {
//                safeZone.Add(wwlPos);
//                safeZone.Add(wwrPos);
//                safeZone.Add(wwrPosExtend);
//                safeZone.Add(wwlPosExtend);
//                //SafeZone.Add(new Geometry.Polygon.Arc(WWRPosExtend.To3D(), WWLPosExtend.To3D(), 1, 100, 20));
//            }
//            //SafeZone.Draw(Color.Blue, 1);
//            return safeZone;
//        }

//        public static List<Obj_AI_Base> AlliesInSafeZone(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
//        {
//            var count = 0;

//            foreach (
//                var ally in
//                    HeroManager.Allies.Where(
//                        x =>
//                        !x.IsMe && !x.IsInvulnerable && !x.IsDead && !x.IsZombie
//                        && SafeZone(sender, args).IsInside(x.ServerPosition)))
//            {
//            }

//            var allys = new List<Obj_AI_Base>();
//            allys.AddRange(HeroManager.Allies);

//            return (List<Obj_AI_Base>)allys.Where(x => SafeZone(sender, args).IsInside(x.ServerPosition));
//        }

//        public static Obj_AI_Base DodgeUnit(Obj_AI_Base hero = null)
//        {
//            var startPos = hero.ServerPosition;
//            var endPos = TargetSelector.GetTarget(2500f, TargetSelector.DamageType.Physical).ServerPosition;
//            if (startPos == Vector3.Zero)
//            {
//                startPos = Player.ServerPosition;
//            }

//            var obj = new List<Obj_AI_Base>();
//            obj.AddRange(
//                MinionManager.GetMinions(Player.ServerPosition, Program.E.Range, MinionTypes.All, MinionTeam.Enemy));
//            obj.AddRange(
//                MinionManager.GetMinions(Player.ServerPosition, Program.E.Range, MinionTypes.All, MinionTeam.Neutral));
//            obj.AddRange(HeroManager.Enemies.Where(i => i.IsValidTarget(Program.E.Range)));

//            return obj.Where(
//                x =>
//                Manager.GetEndPositionDash(x).Distance(endPos) < Player.ServerPosition.Distance(endPos)
//                && !Manager.HasDashWrapper(x) && Manager.UnderTowerSafe(x) && Program.E.IsInRange(x)
//                && x.IsValidTarget() && Manager.GetEndPositionDash(x).To2D().CheckDangerousPos(50))
//                //EzEvade Buffer (need to recheck)
//                .MinOrDefault(x => endPos.Distance(Manager.GetEndPositionDash(x)));
//        }

//        public static void Dodge()
//        {
//            foreach (KeyValuePair<int, Spell> entry in SpellDetector.detectedSpells)
//            {
//                Game.PrintChat("Dodge(); " + entry.Value.info.spellName);
//                Spell spell = entry.Value;

//                var danger = spell.GetSpellDangerLevel();
//                var distBuffer = 200;

//                if (!ObjectCache.myHeroCache.serverPos2D.InSkillShot(spell, ObjectCache.myHeroCache.boundingRadius))
//                {
//                    continue;
//                }

//                if (spell.hasProjectile()) // Not sure but should filter out everything that is not a projectile
//                {
//                    if (spell.spellType == SpellType.Line)
//                    {
//                        Vector2 spellPos = spell.currentSpellPosition;
//                        Vector2 spellEndPos = spell.GetSpellEndPosition();

//                        if (Player.Distance(spellPos) <= Program.W.Range + distBuffer)
//                        {
//                            Game.PrintChat("Dodge(); Cast W");
//                            Program.W.Cast(spellPos);
//                        }
//                    }
//                    //else if (spell.spellType == SpellType.Circular)
//                    //{
//                    //    Render.Circle.DrawCircle(new Vector3(spell.endPos.X, spell.endPos.Y, spell.height), (int)spell.radius, spellDrawingConfig.Color, spellDrawingWidth);

//                    //    if (spell.info.spellName == "VeigarEventHorizon")
//                    //    {
//                    //        Render.Circle.DrawCircle(new Vector3(spell.endPos.X, spell.endPos.Y, spell.height), (int)spell.radius - 125, spellDrawingConfig.Color, spellDrawingWidth);
//                    //    }
//                    //}
//                    //else if (spell.spellType == SpellType.Arc)
//                    //{
//                    //    var spellRange = spell.startPos.Distance(spell.endPos);
//                    //    var midPoint = spell.startPos + spell.direction * (spellRange / 2);

//                    //    Render.Circle.DrawCircle(new Vector3(midPoint.X, midPoint.Y, Player.Position.Z), (int)spell.radius, spellDrawingConfig.Color, spellDrawingWidth);

//                    //    Drawing.DrawLine(Drawing.WorldToScreen(spell.startPos.To3D()),
//                    //                     Drawing.WorldToScreen(spell.endPos.To3D()), 
//                    //                     spellDrawingWidth, spellDrawingConfig.Color);
//                    //}
//                    //    else if (spell.spellType == SpellType.Cone)
//                    //{

//                    //}
//                }
//            }
//        }
//    }
//}



