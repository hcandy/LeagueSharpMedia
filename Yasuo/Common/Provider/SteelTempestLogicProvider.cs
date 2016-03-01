namespace Yasuo.Common.Provider
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.SDK.Core.Wrappers.Damages;

    internal class SteelTempestLogicProvider
    {
        internal float GetDamage(Obj_AI_Base unit)
        {
            var physicalDmg = 0f;
            var magicDmg = 0f;

            #region Sheen
            if (Items.HasItem((int)ItemId.Sheen)
                && (Items.CanUseItem((int)ItemId.Sheen) || Variables.Player.HasBuff("Sheen")))
            {
                physicalDmg = Variables.Player.BaseAttackDamage;
            }
            #endregion Sheen

            #region Trinity Force
            if (Items.HasItem((int)ItemId.Trinity_Force)
                && (Items.CanUseItem((int)ItemId.Trinity_Force) || Variables.Player.HasBuff("Sheen")))
            {
                physicalDmg = Variables.Player.BaseAttackDamage * 2;
            }
            #endregion

            #region Statikk Shiv
            if (Items.HasItem((int)ItemId.Statikk_Shiv) && (Items.CanUseItem((int)ItemId.Statikk_Shiv))
                || Variables.Player.GetBuffCount("StattikShiv") == 100)
            {
                #region unit is Minion

                if (unit is Obj_AI_Minion)
                {
                    magicDmg += 66;
                    switch (Variables.Player.Level)
                    {
                        case 6:
                            magicDmg += 13.2f;
                            break;
                        case 7:
                            magicDmg += 24.2f;
                            break;
                        case 8:
                            magicDmg += 37.4f;
                            break;
                        case 9:
                            magicDmg += 48.4f;
                            break;
                        case 10:
                            magicDmg += 59.4f;
                            break;
                        case 11:
                            magicDmg += 72.6f;
                            break;
                        case 12:
                            magicDmg += 83.6f;
                            break;
                        case 13:
                            magicDmg += 96.8f;
                            break;
                        case 14:
                            magicDmg += 107.8f;
                            break;
                        case 15:
                            magicDmg += 118.8f;
                            break;
                        case 16:
                            magicDmg += 132f;
                            break;
                        case 17:
                            magicDmg += 143f;
                            break;
                        case 18:
                            magicDmg += 154f;
                            break;
                    }

                }
                    #endregion
                #region unit is Hero

                else if (unit is Obj_AI_Hero)
                {
                    magicDmg += 30;
                    switch (Variables.Player.Level)
                    {
                        case 6:
                            magicDmg += 6;
                            break;
                        case 7:
                            magicDmg += 11;
                            break;
                        case 8:
                            magicDmg += 17;
                            break;
                        case 9:
                            magicDmg += 22;
                            break;
                        case 10:
                            magicDmg += 27;
                            break;
                        case 11:
                            magicDmg += 33;
                            break;
                        case 12:
                            magicDmg += 38;
                            break;
                        case 13:
                            magicDmg += 44;
                            break;
                        case 14:
                            magicDmg += 49;
                            break;
                        case 15:
                            magicDmg += 54;
                            break;
                        case 16:
                            magicDmg += 60;
                            break;
                        case 17:
                            magicDmg += 65;
                            break;
                        case 18:
                            magicDmg += 70;
                            break;
                    }




                }
                #endregion
            }
            #endregion

            #region rageblade

            if (Items.HasItem((int)ItemId.Guinsoos_Rageblade) && (Items.CanUseItem((int)ItemId.Guinsoos_Rageblade))
                || Variables.Player.GetBuffCount("Rageblade") == 8)
            {
                magicDmg += 20 + (Variables.Player.TotalAttackDamage - Variables.Player.BaseAttackDamage) * 0.15f;
            }

            #endregion

            if (physicalDmg > 0 || magicDmg > 0)
            {
                physicalDmg = (float)Variables.Player.CalculateDamage(unit, DamageType.Physical, physicalDmg);
                magicDmg = (float)Variables.Player.CalculateDamage(unit, DamageType.Magical, magicDmg);
            }
            return Variables.Spells[SpellSlot.Q].GetDamage(unit) + physicalDmg + magicDmg;            
        }
    }
}
    

