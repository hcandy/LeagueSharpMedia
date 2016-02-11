//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using LeagueSharp;
//using LeagueSharp.Common;

//using SharpDX;

//using Yasuo.EvadeSkills;

//namespace Yasuo.EvadeSkills
//{
//    using Yasuo.Common.Utility.Enums;
//    using Yasuo.Evade;

//    class SweepingBlade : ISkillEvade
//    {
//        public SweepingBladeLogicProvider Provider;

//        public SweepingBlade()
//        {
//            Provider = new SweepingBladeLogicProvider();
//        }

//        public SkillMode GetSkillMode()
//        {
//            return SkillMode.OnUpdate;
//        }

//        public void Execute(Skillshot skills)
//        {
//            if (MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Variables.Spells[SpellSlot.E].Range, MinionTypes.All, MinionTeam.NotAlly) != null)
//            {
                
//            }
//        }

//        private void CastSweepingBlade(Obj_AI_Base target)
//        {

//        }
//    }
//}
