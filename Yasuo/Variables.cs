using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SDK = LeagueSharp.SDK;
using Yasuo.EvadeSkills;
using Yasuo.Modules;

namespace Yasuo
{
    using Yasuo.Common.Extensions;
    using Yasuo.Common.Utility;

    class Variables : Helper
    {
        public static MediaSuo Assembly = null;

        public static Orbwalking.Orbwalker Orbwalker { get; internal set; }

        public static Obj_AI_Hero Player => ObjectManager.Player;

        public static string ChampionName = "Yasuo";
        public static string GitHubPath = "Media/LeagueSharp/master/Yasuo";
        public static string Name => "MediaSuo";
        public static string Author => "Media";

        //TODO: Set collision
        public static void SetSpells()
        {
            var q = Spells[SpellSlot.Q];
            var w = Spells[SpellSlot.W];
            var e = Spells[SpellSlot.E];
            var r = Spells[SpellSlot.R];

            if (Player.HasQ3())
            {
                q.SetSkillshot(GetQDelay, GetSpellWidth("YasuoQ3W"), GetMissileSpeed("YasuoQ3W"), false, SkillshotType.SkillshotLine);
                q.Range = GetSpellRange("YasuoQ3W");
                q.MinHitChance = HitChance.VeryHigh;
            }
            else
            {
                q.SetSkillshot(GetQDelay, GetSpellWidth("YasuoQ"), GetMissileSpeed("YasuoQ"), false, SkillshotType.SkillshotLine);
                q.Range = GetSpellRange("YasuoQ3W");
                q.MinHitChance = HitChance.VeryHigh;
            }
            if (Player.IsDashing())
            {
                q.SetSkillshot(GetQDelay, GetSpellWidth("YasuoEQ"), GetMissileSpeed("YasuoQ"), false, SkillshotType.SkillshotCircle);
                q.MinHitChance = HitChance.High;
            }
            

            w = new Spell(SpellSlot.W, GetSpellRange("YasuoWMovingWall"));
            w.SetSkillshot(GetSpellDelay("YasuoMovingWall"), GetSpellWidth("YasuoWmovingWall"), 400, false, SkillshotType.SkillshotLine);

            e = new Spell(SpellSlot.E, GetSpellRange("YasuoDash"));
            e.SetTargetted(GetSpellDelay("YasuoDash"), GetMissileSpeed("YasuoDash"));
            
            r = new Spell(SpellSlot.R, GetSpellRange("YasuoR"));
            r.SetTargetted(GetSpellDelay("YasuoR"), GetMissileSpeed("YasuoR"));
        }
        
        public static Dictionary<SpellSlot, Spell> Spells = new Dictionary<SpellSlot, Spell>()
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q, 515)},
            { SpellSlot.W, new Spell(SpellSlot.W, 400)},
            { SpellSlot.E, new Spell(SpellSlot.E, 475, TargetSelector.DamageType.Magical) },
            { SpellSlot.R, new Spell(SpellSlot.R, 1200) }
        }; 

        public static string[] Predictions = new[] { "OKTW", "SPRED", "COMMON" };
    }
}
