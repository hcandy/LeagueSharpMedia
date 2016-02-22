//TODO: Set collision (Spells Q - YasuoWall)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SDK = LeagueSharp.SDK;

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
        public static string GitHubPath = "Media/LeagueSharpMedia/master/Yasuo";
        public static string Name => "MediaSuo";
        public static string Author => "Media";

        public static void SetSpells()
        {
            var q = Spells[SpellSlot.Q];
            var w = Spells[SpellSlot.W];
            var e = Spells[SpellSlot.E];
            var r = Spells[SpellSlot.R];

            if (Player.HasQ3())
            {
                q.SetSkillshot(GetQDelay, 90, 1200, false, SkillshotType.SkillshotLine);
                q.Range = 475 * 2;
                q.MinHitChance = HitChance.VeryHigh;
            }
            else
            {
                q.SetSkillshot(GetQDelay, 20, float.MaxValue, false, SkillshotType.SkillshotLine);
                q.Range = 475;
                q.MinHitChance = HitChance.VeryHigh;
            }
            if (Player.IsDashing())
            {
                q.SetSkillshot(GetQDelay, 375, float.MaxValue, false, SkillshotType.SkillshotCircle);
                q.MinHitChance = HitChance.High;
            }
            

            w = new Spell(SpellSlot.W, 400);
            w.SetSkillshot(0, 400, 400, false, SkillshotType.SkillshotLine);

            e = new Spell(SpellSlot.E, 475);
            e.SetTargetted(0, 1025);
            e.Speed = Player.MoveSpeed * 2;
            
            r = new Spell(SpellSlot.R, 900);
            r.SetTargetted(0, float.MaxValue);
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
