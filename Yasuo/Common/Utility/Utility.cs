namespace Yasuo.Common.Utility
{
    using System.Linq;
    using System.Runtime.InteropServices;

    using LeagueSharp;
    using LeagueSharp.Common;
    using SDK = LeagueSharp.SDK;

    using SharpDX;

    public static class Utility
    {
        // TODO: Add a method. Waiting for SDK/Core to get fixed. (Tracker)
        public static bool IsInSkillshot(this Obj_AI_Base unit)
        {
            //foreach (var skillshot in SDK.Tracker.DetectedSkillshots)
            //{
            //    if (skillshot.isInside(unit.ServerPosition)
                //{
                //    return true;
                //}
            //}
            return false;
        }
       
    }
}
