using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using System;
using System.Collections.Generic;
using Terraria.Utilities;
using rail;
using System.Collections.ObjectModel;
using log4net.Core;

namespace Experience.Items
{
    internal class ExpGlobalItem : GlobalItem
    {
        public int[] experience;
        public override bool InstancePerEntity => true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            Main.player[Main.myPlayer].TryGetModPlayer(out MyPlayer playerInstance);
            playerInstance.UpdateWeapon(item);
            if (experience != null)
            {//experience may be null if spawned from other mods which don't call OnCreate

                TooltipLine tooltipLine = new TooltipLine(Mod, "Experience","Level: " + experience[1] + "\nExperience : " + experience[0].ToString() + "\nExperience for next level: " + experience[3].ToString()) { OverrideColor = Color.LightGreen };
                tooltips.Add(tooltipLine);

                // Fix damage percentage by Prefix
                foreach (var tooltip in tooltips)
                {            
                    if (tooltip.Name == "PrefixDamage")
                    {
                        TryGetPrefixStatMultipliersForItem(item, out float dmg, out float kb, out float spd, out float size, out float shtspd, out float mcst, out int crt, out string changes);
                        if (changes.Contains("dmg"))
                        {
                            int damagePercentage = (int)Math.Round((dmg - 1f) * 100);
                            tooltip.Text = damagePercentage < 0? damagePercentage.ToString() + "% damage": "+" + damagePercentage.ToString() + "% damage";
                        } else {
                            tooltips.Remove(tooltip);
                        }
                        break;
                    }
                }
            }
        }


        public override bool OnPickup(Item item, Player player)
        {
            player.TryGetModPlayer(out MyPlayer playerInstance);
            playerInstance.UpdateWeapon(item);

            return true;
        }

        public override void PostReforge(Item item)
        {
            Main.player[Main.myPlayer].TryGetModPlayer(out MyPlayer playerInstance);
            playerInstance.UpdateWeapon(item);
        }
        public bool TryGetPrefixStatMultipliersForItem(Item item, out float dmg, out float kb, out float spd, out float size, out float shtspd, out float mcst, out int crt, out string changes)
        {
            int rolledPrefix = item.prefix;
            dmg = 1f;
            kb = 1f;
            spd = 1f;
            size = 1f;
            shtspd = 1f;
            mcst = 1f;
            crt = 0;
            changes = "";
            switch (rolledPrefix)
            {
                case 1:
                    size = 1.12f;
                    break;
                case 2:
                    size = 1.18f;
                    break;
                case 3:
                    dmg = 1.05f;
                    crt = 2;
                    size = 1.05f;
                    break;
                case 4:
                    dmg = 1.1f;
                    size = 1.1f;
                    kb = 1.1f;
                    break;
                case 5:
                    dmg = 1.15f;
                    break;
                case 6:
                    dmg = 1.1f;
                    break;
                case 81:
                    kb = 1.15f;
                    dmg = 1.15f;
                    crt = 5;
                    spd = 0.9f;
                    size = 1.1f;
                    break;
                case 7:
                    size = 0.82f;
                    break;
                case 8:
                    kb = 0.85f;
                    dmg = 0.85f;
                    size = 0.87f;
                    break;
                case 9:
                    size = 0.9f;
                    break;
                case 10:
                    dmg = 0.85f;
                    break;
                case 11:
                    spd = 1.1f;
                    kb = 0.9f;
                    size = 0.9f;
                    break;
                case 12:
                    kb = 1.1f;
                    dmg = 1.05f;
                    size = 1.1f;
                    spd = 1.15f;
                    break;
                case 13:
                    kb = 0.8f;
                    dmg = 0.9f;
                    size = 1.1f;
                    break;
                case 14:
                    kb = 1.15f;
                    spd = 1.1f;
                    break;
                case 15:
                    kb = 0.9f;
                    spd = 0.85f;
                    break;
                case 16:
                    dmg = 1.1f;
                    crt = 3;
                    break;
                case 17:
                    spd = 0.85f;
                    shtspd = 1.1f;
                    break;
                case 18:
                    spd = 0.9f;
                    shtspd = 1.15f;
                    break;
                case 19:
                    kb = 1.15f;
                    shtspd = 1.05f;
                    break;
                case 20:
                    kb = 1.05f;
                    shtspd = 1.05f;
                    dmg = 1.1f;
                    spd = 0.95f;
                    crt = 2;
                    break;
                case 21:
                    kb = 1.15f;
                    dmg = 1.1f;
                    break;
                case 82:
                    kb = 1.15f;
                    dmg = 1.15f;
                    crt = 5;
                    spd = 0.9f;
                    shtspd = 1.1f;
                    break;
                case 22:
                    kb = 0.9f;
                    shtspd = 0.9f;
                    dmg = 0.85f;
                    break;
                case 23:
                    spd = 1.15f;
                    shtspd = 0.9f;
                    break;
                case 24:
                    spd = 1.1f;
                    kb = 0.8f;
                    break;
                case 25:
                    spd = 1.1f;
                    dmg = 1.15f;
                    crt = 1;
                    break;
                case 58:
                    spd = 0.85f;
                    dmg = 0.85f;
                    break;
                case 26:
                    mcst = 0.85f;
                    dmg = 1.1f;
                    break;
                case 27:
                    mcst = 0.85f;
                    break;
                case 28:
                    mcst = 0.85f;
                    dmg = 1.15f;
                    kb = 1.05f;
                    break;
                case 83:
                    kb = 1.15f;
                    dmg = 1.15f;
                    crt = 5;
                    spd = 0.9f;
                    mcst = 0.9f;
                    break;
                case 29:
                    mcst = 1.1f;
                    break;
                case 30:
                    mcst = 1.2f;
                    dmg = 0.9f;
                    break;
                case 31:
                    kb = 0.9f;
                    dmg = 0.9f;
                    break;
                case 32:
                    mcst = 1.15f;
                    dmg = 1.1f;
                    break;
                case 33:
                    mcst = 1.1f;
                    kb = 1.1f;
                    spd = 0.9f;
                    break;
                case 34:
                    mcst = 0.9f;
                    kb = 1.1f;
                    spd = 1.1f;
                    dmg = 1.1f;
                    break;
                case 35:
                    mcst = 1.2f;
                    dmg = 1.15f;
                    kb = 1.15f;
                    break;
                case 52:
                    mcst = 0.9f;
                    dmg = 0.9f;
                    spd = 0.9f;
                    break;
                case 84:
                    kb = 1.17f;
                    dmg = 1.17f;
                    crt = 8;
                    break;
                case 36:
                    crt = 3;
                    break;
                case 37:
                    dmg = 1.1f;
                    crt = 3;
                    kb = 1.1f;
                    break;
                case 38:
                    kb = 1.15f;
                    break;
                case 53:
                    dmg = 1.1f;
                    break;
                case 54:
                    kb = 1.15f;
                    break;
                case 55:
                    kb = 1.15f;
                    dmg = 1.05f;
                    break;
                case 59:
                    kb = 1.15f;
                    dmg = 1.15f;
                    crt = 5;
                    break;
                case 60:
                    dmg = 1.15f;
                    crt = 5;
                    break;
                case 61:
                    crt = 5;
                    break;
                case 39:
                    dmg = 0.7f;
                    kb = 0.8f;
                    break;
                case 40:
                    dmg = 0.85f;
                    break;
                case 56:
                    kb = 0.8f;
                    break;
                case 41:
                    kb = 0.85f;
                    dmg = 0.9f;
                    break;
                case 57:
                    kb = 0.9f;
                    dmg = 1.18f;
                    break;
                case 42:
                    spd = 0.9f;
                    break;
                case 43:
                    dmg = 1.1f;
                    spd = 0.9f;
                    break;
                case 44:
                    spd = 0.9f;
                    crt = 3;
                    break;
                case 45:
                    spd = 0.95f;
                    break;
                case 46:
                    crt = 3;
                    spd = 0.94f;
                    dmg = 1.07f;
                    break;
                case 47:
                    spd = 1.15f;
                    break;
                case 48:
                    spd = 1.2f;
                    break;
                case 49:
                    spd = 1.08f;
                    break;
                case 50:
                    dmg = 0.8f;
                    spd = 1.15f;
                    break;
                case 51:
                    kb = 0.9f;
                    spd = 0.9f;
                    dmg = 1.05f;
                    crt = 2;
                    break;
            }

            if (dmg != 1f)
                changes += "dmg";
            if (kb != 1f)
                changes += "kb,";
            if (spd != 1f)
                changes += "spd,";
            if (size != 1f)
                changes += "size,";
            if (shtspd != 1f)
                changes += "shtspd,";
            if (mcst != 1f)
                changes += "mcst,";
            if (crt != 0)
                changes += "crt";

            if (dmg != 1f && Math.Round((float)item.damage * dmg) == (double)item.damage)
            {
                return false;
            }
            if (spd != 1f && Math.Round((float)item.useAnimation * spd) == (double)item.useAnimation)
            {
                return false;
            }
            if (mcst != 1f && Math.Round((float)item.mana * mcst) == (double)item.mana)
            {
                return false;
            }
            if (kb != 1f && item.knockBack == 0f)
            {
                return false;
            }
            return true;
        }

    }

}
