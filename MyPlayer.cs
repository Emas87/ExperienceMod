using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.Linq;
using Experience.Items;
using Experience.Projectiles;
using System.Drawing;
using Terraria.ID;
using static System.Net.Mime.MediaTypeNames;
using System;

namespace Experience
{
    public class MyPlayer : ModPlayer
    {
        // Dictionary: Key = item.type, value = [item experience, item level, currentDamage, exp for next level]
        public Dictionary<int, int[]> expInfo = new();
        public bool updateWeaponExp = true;
        public bool updateArmorExp = true;
        public int[] expByLevel = { 100, 300, 1000, 3000, 5000, 6000, 7000, 8000, 9000, 10000, 20000, 30000, 40000, 50000, 60000, 70000, 80000, 90000, 100000, 200000, 300000, 400000, 500000, 600000, 700000, 800000, 1000000, 2000000, 3000000, 4000000, 5000000, 6000000, 7000000, 8000000, 9000000, 10000000};
        public int[] expDefByLevel = { 1000, 3000, 6000, 9000, 12000, 15000, 18000, 21000, 24000, 27000, 30000, 33000, 36000, 39000, 42000, 45000, 48000, 51000, 54000, 57000, 60000, 630000 };

        public void UpdateExpInfo(Item item, int amount, int origValue, bool isWeapon = true)
        {
            if (expInfo.ContainsKey(item.type))
            {
                expInfo[item.type][0] += amount;
                expInfo[item.type][3] -= amount;
            }
            else
            {
                if (isWeapon)
                {
                    int[] newExpInfo = { amount, 0, origValue, expByLevel[0] - amount };
                    expInfo.Add(item.type, newExpInfo);
                }
                else
                {
                    int[] newExpInfo = { amount, 0, origValue, expDefByLevel[0] - amount };
                    expInfo.Add(item.type, newExpInfo);
                }
            }
            if (isWeapon)
            {
                updateWeaponExp = true;
            }
            else
            {
                updateArmorExp = true;
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)   
        {
            if(target.life <= 0)
            {
                UpdateExpInfo(item, target.lifeMax, item.OriginalDamage);
            }
        }
        
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0)
            {
                proj.TryGetGlobalProjectile(out ExpGlobalProjectile projectileInstance);
                Item item;
                if(projectileInstance.parentType != 0)
                {
                    item = new(projectileInstance.parentType);
                } else
                {
                    item = this.Player.HeldItem;
                }
                // Get item.type from projectile
                UpdateExpInfo(item, target.lifeMax, item.OriginalDamage);
            }

        }
        public override void SaveData(TagCompound tag)
        {
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
        public override void OnEnterWorld(Player player)
        {
            // Update all inventory
            foreach (var item in player.inventory)
            {
                UpdateWeapon(item);
                UpdateArmor(item);
            }
        }

        /*public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            return new[] {
                new Item(ItemID.LifeCrystal,15),
                new Item(ItemID.ManaCrystal,9),
                new Item(ItemID.MagicMirror)
            };
        }*/


        public void UpdateWeapon(Item item)
        {
            //TODO item damage is not modified by armor bonuses or accesories modifier
            if (item.type == ItemID.None || item.ammo != 0 || item.damage <= 0 || item.accessory)
                return;

            int newLevel = 0;
            int[] newExpInfo = { 0, 0, item.OriginalDamage, expByLevel[0] };
            if (!expInfo.ContainsKey(item.type))
            {
                expInfo.Add(item.type, newExpInfo);
            }
            item.damage = item.OriginalDamage;
            for (int i = 0; i < expByLevel.Length; i++)
            {
                if (expInfo[item.type][0] >= expByLevel[i])
                {
                    newLevel = i + 1;
                }
                else
                {
                    break;
                }
            }
            // Upgrade damage by amount of levels updated newlevel
            for (int i = 1; i <= newLevel; i++)
            {
                if (i < 10)
                {
                    item.damage += 1;
                }
                else if (i < 20)
                {
                    if(item.DamageType.DisplayName == "summon damage")
                    {
                        item.damage += 1;
                    } else
                    {
                        item.damage += 2;
                    }
                }
                else
                {
                    if (item.DamageType.DisplayName == "summon damage")
                    {
                        item.damage += 2;
                    }
                    else
                    {
                        item.damage += i - 18;                        
                    }
                }
            }
            if(newLevel >= expByLevel.Length)
            {
                newLevel = expByLevel.Length-1;
                expInfo[item.type][3] = 0;
                expInfo[item.type][0] = expByLevel[expByLevel.Length - 1];
            } else
            {
                expInfo[item.type][3] = expByLevel[newLevel] - expInfo[item.type][0];
            }
            expInfo[item.type][1] = newLevel;
            
            //Update Tooltip
            item.TryGetGlobalItem(out ExpGlobalItem itemInstance);
            itemInstance.experience = expInfo[item.type];

            // Update Damage by modifier
            itemInstance.TryGetPrefixStatMultipliersForItem(item, out float dmg, out float kb, out float spd, out float size, out float shtspd, out float mcst, out int crt, out string changes);
            if (changes.Contains("dmg"))
            {
                item.damage = (int)Math.Round(dmg * item.damage);
            }

            expInfo[item.type][2] = item.damage;

            // Reset item
            //newExpInfo[2] = 10;
            //expInfo[item.type] = newExpInfo;
        }
        public void UpdateArmor(Item item)
        {
            if (item.type == ItemID.None || item.ammo != 0 || item.defense <= 0 || item.accessory)
                return;

            int newLevel = 0;
            int[] newExpInfo = { 0, 0, item.OriginalDefense, expDefByLevel[0] };
            if (!expInfo.ContainsKey(item.type))
            {
                expInfo.Add(item.type, newExpInfo);
            }
            item.defense = expInfo[item.type][2];
            for (int i = 0; i < expDefByLevel.Length; i++)
            {
                if (expInfo[item.type][0] >= expDefByLevel[i])
                {
                    newLevel = i + 1;
                }
                else
                {
                    break;
                }
            }
            // Upgrade defense by amount of levels updated, newlevel - oldlevel
            if (newLevel > expInfo[item.type][1])
            {
                for (int i = expInfo[item.type][1] + 1; i <= newLevel; i++)
                {
                    expInfo[item.type][2] += 1;
                    item.defense = expInfo[item.type][2];
                }
                if (newLevel >= expDefByLevel.Length)
                {
                    newLevel = expDefByLevel.Length - 1;
                    expInfo[item.type][3] = 0;
                    expInfo[item.type][0] = expByLevel[expByLevel.Length - 1];
                }
                else
                {
                    expInfo[item.type][3] = expDefByLevel[newLevel] - expInfo[item.type][0];
                }
                expInfo[item.type][1] = newLevel;

                //Update shop value of the item
                this.Player.GetItemExpectedPrice(item, out int calcForSelling, out int _);
                item.shopCustomPrice = calcForSelling + 50 * newLevel;

            }
            //Update Tooltip
            item.TryGetGlobalItem(out ExpGlobalItem itemInstance);
            itemInstance.experience = expInfo[item.type];

            // Reset item
            //newExpInfo[2] = item.OriginalDefense;
            //expInfo[item.type] = newExpInfo;
        }

        public override void PostUpdate() {

            if (updateWeaponExp) {
                updateWeaponExp = false;
                Item item = this.Player.HeldItem;
                UpdateWeapon(item);         
            }
            if (updateArmorExp)
            {
                updateArmorExp = false;
                Item head = this.Player.armor[0];
                Item breastPlate = this.Player.armor[1];
                Item legs = this.Player.armor[2];
                UpdateArmor(head);
                UpdateArmor(breastPlate);
                UpdateArmor(legs);
            }
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            //each piece of equipped armor get experience, amount is defined by damage
            _ = this.Player.armor;
            Item head = this.Player.armor[0];
            Item breastPlate = this.Player.armor[1];
            Item legs = this.Player.armor[2];
            UpdateExpInfo(head, damage, head.OriginalDefense, false);
            UpdateExpInfo(breastPlate, damage, breastPlate.OriginalDefense, false);
            UpdateExpInfo(legs, damage, legs.OriginalDefense, false);
        }
    }
}
