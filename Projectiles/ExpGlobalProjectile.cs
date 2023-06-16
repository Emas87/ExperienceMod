using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Experience.Projectiles
{
    // The following classes showcases pattern matching of IEntitySource instances to make things happen only in specific contexts.

    internal class ExpGlobalProjectile : GlobalProjectile
	{
        public override bool InstancePerEntity => true;
        public int parentType = 0;
		public override void OnSpawn(Projectile projectile, IEntitySource source)
		{
			if(source is EntitySource_ItemUse)
			{
				EntitySource_ItemUse parent = (EntitySource_ItemUse)source;
				parentType = parent.Item.type;

            } else if (source is EntitySource_Parent { Entity: Projectile { minion: true} })
			{
                EntitySource_Parent parent = (EntitySource_Parent)source;
                Projectile projectileParent = parent.Entity as Projectile;
                projectileParent.TryGetGlobalProjectile(out ExpGlobalProjectile projectileInstance);
				parentType = projectileInstance.parentType;
            }
            //parentType = source['Item'].type;
        }
		public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
			int delete = 0;
        }
    }
	
	/*public sealed class ExampleSourceDependentItemTweaks : GlobalItem
	{
		public override void OnSpawn(Item item, IEntitySource source) {
			// Accompany all loot from trees with a slime.
			if (source is EntitySource_ShakeTree) {
				var newSource = item.GetSource_FromThis(); // Use a separate source for the newly created projectiles, to not cause a stack overflow.

				NPC.NewNPC(newSource, (int)item.position.X, (int)item.position.Y, NPCID.BlueSlime);
			}
		}
	}*/

    
}
