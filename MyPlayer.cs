using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.Linq;
using Experience.Items;
using System.Drawing;

namespace Experience
{
    public class MyPlayer : ModPlayer
    {
        // Dictionary: Key = item.type, value = [item experience, item level, currentDamage, exp for next level]
        public Dictionary<int, int[]> expInfo = new();
        public bool updateExp = true;
        //public int[] expByLevel = { 50, 130, 210, 340, 550, 890, 1440, 2330, 3770, 6100, 9780, 15970, 25840, 41810, 67650, 109460, 177110, 286570, 463680, 750250, 1213930, 1964180, 3178110, 5142290, 8320400, 13462690 };
        public int[] expByLevel = { 100, 300, 600, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000, 20000, 30000, 40000, 50000, 60000, 70000, 80000, 90000, 100000, 200000, 300000, 1000000 };

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)   
        {
            if(target.life <= 0)
            {
                if (expInfo.ContainsKey(item.type))
                {
                    expInfo[item.type][0] += target.lifeMax;
                    expInfo[item.type][3] -= target.lifeMax;
                }
                else
                {
                    int[] newExpInfo = { target.lifeMax, 0, item.damage, expByLevel[0]};
                    expInfo.Add(item.type, newExpInfo);
                }
                updateExp = true;
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0)
            {
                // Get item.type from projectile
                Item item = this.Player.HeldItem;
                if (expInfo.ContainsKey(item.type))
                {
                    expInfo[item.type][0] += target.lifeMax;
                    expInfo[item.type][3] -= target.lifeMax;
                }
                else
                {
                    int[] newExpInfo = { target.lifeMax, 0, item.damage, expByLevel[0] };
                    expInfo.Add(item.type, newExpInfo);
                }
                updateExp = true;                
            }
        }
        public override void SaveData(TagCompound tag)
        {
            //base.SaveData(tag);
            // Saving experience
            List<int> listkey = expInfo.Keys.ToList();
            List<int[]> listvalues = new();
            for(int i = 0; i < listkey.Count; i++) {
                listvalues.Add(expInfo[listkey[i]]);
            }
            tag["expKeys"] = listkey;
            tag["expValues"] = listvalues;
        }
        public override void LoadData(TagCompound tag)
        {
            List<int> listkey = tag.Get<List<int>>("expKeys");
            List<int[]> listvalues = tag.Get<List<int[]>>("expValues");
            for (int i = 0; i < listkey.Count; i++)
            {
                expInfo.Add(listkey[i], listvalues[i]);
            }
        }

        public override void PostUpdate() {
            //base.ModifyStartingInventory(itemsByMod, mediumCoreDeath);
            if(updateExp) {
                updateExp = false;
                Item item = this.Player.HeldItem;
                int newLevel = 0;
                int[] newExpInfo = { 0, 0, item.damage, expByLevel[0] };
                if (!expInfo.ContainsKey(item.type)) {
                    expInfo.Add(item.type, newExpInfo);
                }
                item.damage = expInfo[item.type][2];
                for (int i = 0; i < expByLevel.Length; i++) {
                    if(expInfo[item.type][0] >= expByLevel[i]) {
                        newLevel = i + 1;
                    } else {
                        break;
                    }
                }
                // Upgrade damage by amount of level updated newlevel - oldlevel
                if(newLevel > expInfo[item.type][1]) {
                    for (int i = expInfo[item.type][1] + 1; i <= newLevel; i++)
                    {
                        expInfo[item.type][2] += i;
                        item.damage = expInfo[item.type][2];

                    }
                    expInfo[item.type][1] = newLevel;
                    expInfo[item.type][3] = expByLevel[newLevel] - expInfo[item.type][0];

                    //Update shop value of the item
                    this.Player.GetItemExpectedPrice(item, out int calcForSelling, out int calcForBuying);
                    item.shopCustomPrice = calcForSelling + 50*newLevel;

                }
                //Update Tooltip
                item.TryGetGlobalItem(out ExpGlobalItem itemInstance);
                itemInstance.experience = expInfo[item.type];

                
                // Reset item
                //newExpInfo[2] = 10;
                //expInfo[item.type] = newExpInfo;
            }
        }
    }
}
