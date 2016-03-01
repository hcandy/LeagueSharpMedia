//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using LeagueSharp;
//using LeagueSharp.Common;

//using SharpDX;

//using MinionTypes = LeagueSharp.Common.MinionTypes;
//using Spell = LeagueSharp.Common.Spell;
//using TargetSelector = LeagueSharp.Common.TargetSelector;
//using Utility = LeagueSharp.Common.Utility;

//namespace Yasuo.EvadeSkills
//{
//    using Yasuo.Evade;

//    using Geometry = LeagueSharp.Common.Geometry;

//    class WindWallLogicProvider
//    {
//        // TODO: Maybe Block spells that aiming an enemy and are blockable i.e: Lux W
//        // TODO: E when W not needed
//        // TODO: E behind W when skillshot is targeted (ie. cait ult) will hit you or next AA will kill you or do much dmg
//        // TODO: Anti Gragas Insec
//        // TODO: Add delay to Special Interactions
//        // TODO: Crit in AA
//        // TODO: 1v1 SafeZone logic
//        // TODO: Clean up code
//        // TODO: Annie Stun, Katarina Ult
//        public static Obj_AI_Hero Player => ObjectManager.Player;

//        public static Geometry.Polygon SafeZone(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args = null)
//        {
//            var predictedWwPos = Geometry.Extend(Manager.Player.ServerPosition, sender.ServerPosition, 300).To2D();
//            var direction = Geometry.Perpendicular((predictedWwPos - Manager.Player.ServerPosition.To2D()).Normalized());

//            var wwrPos = predictedWwPos + 500 / 2 * direction;
//            var wwlPos = predictedWwPos - 500 / 2 * direction;

//            var radius = args?.SData.CastRadius ?? 1500f;

//            var wwrPosExtend = Geometry.Extend(sender.ServerPosition.To2D(), wwrPos, radius);
//            var wwlPosExtend = Geometry.Extend(sender.ServerPosition.To2D(), wwlPos, radius);

//            var safeZone = new Geometry.Polygon();
//            {
//                safeZone.Add(wwlPos);
//                safeZone.Add(wwrPos);
//                safeZone.Add(wwrPosExtend);
//                safeZone.Add(wwlPosExtend);
//                safeZone.Add(new Geometry.Polygon.Arc(wwrPosExtend.To3D(), wwlPosExtend.To3D(), 90, sender.Distance(Player), 30));
//            }
//            //SafeZone.Draw(Color.Blue, 1);
//            return safeZone;
//        }

//        public static List<Obj_AI_Base> AlliesInSafeZone(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
//        {
//            foreach (var ally in HeroManager.Allies.Where(x => !x.IsMe && !x.IsInvulnerable && !x.IsDead && !x.IsZombie && SafeZone(sender, args).IsInside(x.ServerPosition)))
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
//            obj.AddRange(MinionManager.GetMinions(Player.ServerPosition, Variables.Spells[SpellSlot.E].Range, MinionTypes.All, MinionTeam.NotAlly));
//            obj.AddRange(HeroManager.Enemies.Where(x => x.Distance(Player) <= Variables.Spells[SpellSlot.E].Range));

//            return
//                EnumerableExtensions.MinOrDefault(obj.Where(
//                        x =>
//                        Manager.GetEndPositionDash(x).Distance(endPos) < Player.ServerPosition.Distance(endPos) && !Manager.HasDashWrapper(x)
//                        && Manager.UnderTowerSafe(x) && Program.E.IsInRange(x) && x.IsValidTarget() && Manager.GetEndPositionDash(x).To2D().CheckDangerousPos(50)), x => Geometry.Distance(endPos, Manager.GetEndPositionDash(x)));

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
//                    continue;

//                if (spell.hasProjectile()) // Not sure but should filter out everything that is not a projectile
//                {
//                    if (spell.spellType == SpellType.Line)
//                    {

//                        Vector2 spellPos = spell.currentSpellPosition;
//                        Vector2 spellEndPos = spell.GetSpellEndPosition();

//                        if (Geometry.Distance(Player, spellPos) <= Program.W.Range + distBuffer)
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

//        public static Vector2 GetWindwallPosition(Skillshot skillshot, Vector2 to)
//        {

//            return Vector2.Zero;
//        }

//        private List<Vector2> GetPossiblePoints(Vector2 finalPos, int precision = 50)
//        {
//            var position = Player.ServerPosition;
//            var radius = Variables.Spells[SpellSlot.W].Range;

//            List<Vector2> points = new List<Vector2>();
//            for (var i = 1; i <= precision; i++)
//            {
//                var angle = i * 2 * Math.PI / precision;
//                var point = new Vector3(position.X + radius * (float)Math.Cos(angle), position.Y + radius * (float)Math.Sin(angle), position.Z);

//                if (point.Distance(Player.Position.Extend(finalPos.To3D(), radius)) < 430)
//                {
//                    points.Add(point);
//                    //Utility.DrawCircle(point, 20, System.Drawing.Color.Aqua, 1, 1);
//                }
//            }

//            var point2 = points.OrderBy(x => x.Distance(finalPos));
//            points = point2.ToList();
//            points.RemoveAt(0);
//            points.RemoveAt(1);
//            return points;
//        }
//    }
//}

namespace Yasuo.Common.Provider
{
}