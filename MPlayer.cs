using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System;
using System.IO;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader.IO;
using System.Globalization;

namespace LearnByDoing
{

    public class MPlayer : ModPlayer
    {
        public long[] experience = new long[22];
        public int[] level = new int[22];
        public float skillMult = 1f;//multiplier for all beneficial effects of skills
        public float skillFlat = 0f;//flat modifier for most parameters affected by skills. 0f = base, 1f would double base values
        public float expMult = 1f;//multiplier for all exp gained
        public int subLifeRegen = 0;
        public int oldMana = 0;
        public int displayTimer = 0;
        
        /*
        0,1,2,3: melee, ranged, magic summon prof. Raised by damaging and killing enemies with a damage type. Increases damage with that type. [1%]
        4: Running. Increases movespeed. [1%]
        5: Jumping. Increases jump speed and height. [3%]
        6: Swimming. Increases movespeed when swimming. [1.5%]
        7: Flying. Increases flight time and flight speed. [1%; 1%]
        8: Toughness, raised by taking damage and surviving. Increases defense and health. [1/2 defense, 1 HP] CAP 10
        9: Fall resistance, raised by taking fall damage and surviving. Negates fall damage.
        10: Mana manipulation, raised by using mana. Increases mana recovery speed. [1/2 bonus regen]
        11: Fishing, raised by fishing and completing angler quests. Increases fishing power. [1]
        12: Death resistance, raised by dying. Gives a chance to avoid death. [1%]
        13: Eating. Increases health and defense slightly. [1/2 HP; 1/4 defense]
        14: Potion tolerance. Increases health and damage slightly. [1/2 HP; 1/3% damage, potion CD -.5 sec] CAP 60
        15: Regeneraton, raised by naturally regenerating life. Increases natural life regen speed. [1HP/4sec]
        16: Ammo efficiency, raised by ammo consumption. Provides a chance to not consume ammo. [1%]
        17: Contact resistance, raised by taking contact damage. Unlocked when toughness caps. Decreases contact damage taken. CAP 10
        18: Projectile resistance, raised by taking projectile damage. Unlocked when toughness caps. Decreases projectile damage taken. CAP 10
        19: Thorns: raised by taking contact damage. Unlocked when contact resistance caps. Returns damage to enemies that contact with you.
        20: Evasion: raised by taking projectile damage. Unlocked when projectile resistance caps. Provides a chance to avoid projectile damage. CAP 10
        21: Throwing prof. [1%]
        */
        public override void ResetEffects()
        {
            displayTimer = Math.Max(displayTimer - 1, 0);
        }
        public override void UpdateLifeRegen()
        {
            subLifeRegen += (int)(level[15] * 5 * skillMult);
            while (subLifeRegen >= 60)
            {
                player.lifeRegen++;
                subLifeRegen -= 60;
            }
            int exp = player.lifeRegenTime / 480 + player.lifeRegen;
            if(player.statLife < player.statLifeMax2)
            {
                experience[15] += exp;
            }
        }
        public override void UpdateBadLifeRegen()
        {

        }
        public override void PreUpdateBuffs()
        {
            player.meleeDamage = player.meleeDamage * (1 + skillFlat) + (level[0] + level[14] / 3f) * skillMult * .01f;
            player.rangedDamage = player.rangedDamage * (1 + skillFlat) + (level[1] + level[14] / 3f) * skillMult * .01f;
            player.magicDamage = player.magicDamage * (1 + skillFlat) + (level[2] + level[14] / 3f) * skillMult * .01f;
            player.minionDamage = player.minionDamage * (1 + skillFlat) + (level[3] + level[14] / 3f) * skillMult * .01f;
            player.thrownDamage = player.thrownDamage * (1 + skillFlat) + (level[21] + level[14] / 3f) * skillMult * .01f;
            player.moveSpeed = player.moveSpeed * (1 + skillFlat) + level[4] * skillMult * .02f;
            player.jumpSpeedBoost = player.jumpSpeedBoost * (1 + skillFlat) + level[5] * skillMult * .15f;
            if (player.wet)
            {
                player.moveSpeed = player.moveSpeed * (1 + skillFlat) + level[6] * skillMult * .03f;
            }
            player.statLifeMax2 += (int)((level[8] + (level[13] + level[14]) / 2) * skillMult + 100 * skillFlat);
            player.statDefense += (int)(level[8] / 2.0 * skillMult + level[13] / 4.0 * skillMult);
            if (level[9] > 0)
            {
                player.noFallDmg = true;
            }
            player.manaRegenBonus += (int)(level[10] / 2.0 * skillMult);
            player.fishingSkill += (int)(level[11] * skillMult);
            player.thorns += level[19] * .1f * skillMult;

            int manaExp = oldMana - player.statMana;
            oldMana = player.statMana;
            if (manaExp > 0)
            {
                experience[10] += manaExp;
            }
        }
        public override void PostUpdateBuffs()
        {
            int id = player.FindBuffIndex(BuffID.WellFed);
            if(id != -1)
            {
                if(player.buffTime[id] % 200 == 0)
                {
                    experience[13]++;
                }
            }
            id = player.FindBuffIndex(BuffID.PotionSickness);
            if (id != -1)
            {
                if (player.buffTime[id] % 100 == 0)
                {
                    experience[14]++;
                }
                if (player.buffTime[id] == 3599)
                {
                    player.buffTime[id] -= level[14] * 30;
                }
            }
        }
        public override void PostUpdateRunSpeeds()
        {
            int exp;
            if (player.controlJump && player.velocity.Y == 0)
            {
                experience[5]++;
            }
            if (player.wet)
            {
                exp = (int)Math.Sqrt(Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y)) * 8;
                experience[6] += exp;
            }
            if (player.velocity.Y != 0)
            {
                player.accRunSpeed = player.accRunSpeed * (1 + skillFlat) + level[7] * skillMult * .01f;
                player.wingTimeMax = (int)(player.wingTimeMax * (1 + skillFlat) + level[8] * skillMult * .01f);
                if (player.controlJump && player.wings != 0)//might need to adjust, is 0 a wing or not?
                {
                    experience[7]++;
                }
            }
            else
            {
                exp = (int)(Math.Abs(player.velocity.X)) * 4;
                experience[4] += exp;
                if(experience[4] < 0)
                {
                    experience[4] = 0;
                }
            }
        }
        public override void PostUpdate()
        {
            //check for level ups
            for(int i=0; i < 4; i++)
            {
                if (level[i] * level[i] * 80 + 80 < experience[i])
                {
                    level[i]++;
                    experience[i] = 0;
                    if (i == 0)
                    {
                        Main.NewText(player.name + "'s [melee proficiency] has increased to level " + level[i] + "!");
                    }
                    else if (i == 1)
                    {
                        Main.NewText(player.name + "'s [ranged proficiency] has increased to level " + level[i] + "!");
                    }
                    else if (i == 2)
                    {
                        Main.NewText(player.name + "'s [magic proficiency] has increased to level " + level[i] + "!");
                    }
                    else
                    {
                        Main.NewText(player.name + "'s [summoning proficiency] has increased to level " + level[i] + "!");
                    }
                }
            }
            if(level[4] * level[4] * 4000 + 4000 < experience[4])
            {
                level[4]++;
                experience[4] = 0;
                Main.NewText(player.name + "'s [running] ability has increased to level " + level[4] + "!");
            }
            if (level[5] * level[5] * 14 + 12 < experience[5] * expMult)
            {
                level[5]++;
                experience[5] = 0;
                Main.NewText(player.name + "'s [jumping] ability has increased to level " + level[5] + "!");
            }
            if (level[6] * level[6] * 3000 + 3000 < experience[6])
            {
                level[6]++;
                experience[6] = 0;
                Main.NewText(player.name + "'s [swimming] ability has increased to level " + level[6] + "!");
            }
            if (level[7] * level[7] * 600 + 600 < experience[7])
            {
                level[7]++;
                experience[7] = 0;
                Main.NewText(player.name + "'s [flying] ability has increased to level " + level[7] + "!");
            }
            if (level[8] < 10 && level[8] * level[8] * 45 + 45 < experience[8])
            {
                level[8]++;
                experience[8] = 0;
                Main.NewText(player.name + "'s [toughness] has increased to level " + level[8] + "!");
                if(level[8] == 10)
                {
                    Main.NewText("[toughness] has been maxed out. [projectile resistance] and [contact resistance] have been unlocked.");
                }
            }
            if (level[9] == 0 && 2000 < experience[9])
            {
                level[9]++;
                experience[9] = 0;
                Main.NewText(player.name + "'s [fall immunity] has increased to level " + level[9] + "!");
            }
            if (level[10] * level[10] * 700 + 500 < experience[10] * expMult)
            {
                level[10]++;
                experience[10] = 0;
                Main.NewText(player.name + "'s [mana manipulation] ability has increased to level " + level[10] + "!");
            }
            if (level[11] * 10 + 10 < experience[11] * expMult)
            {
                level[11]++;
                experience[11] = 0;
                Main.NewText(player.name + "'s [fishing ability] has increased to level " + level[11] + "!");
            }
            if (level[12] * 3 + 1 < experience[12] * expMult)
            {
                level[12]++;
                experience[12] = 0;
                Main.NewText(player.name + "'s [death resistance] has increased to level " + level[12] + "!");
            }
            if (level[13] * level[13] * 20 + 30 < experience[13] * expMult)
            {
                level[13]++;
                experience[13] = 0;
                Main.NewText(player.name + "'s [eating] skill has increased to level " + level[13] + "!");
            }
            if (level[14] < 60 && level[14] * level[14] * 20 + 30 < experience[14] * expMult)
            {
                level[14]++;
                experience[14] = 0;
                Main.NewText(player.name + "'s [potion tolerance] has increased to level " + level[14] + "!");
            }
            if (level[15] * level[15] * 1800 + 1800 < experience[15] * expMult)
            {
                level[15]++;
                experience[15] = 0;
                Main.NewText(player.name + "'s [life regeneration] skill has increased to level " + level[15] + "!");
            }
            if (level[16] * level[16] * 30 + 30 < experience[16] * expMult)
            {
                level[16]++;
                experience[16] = 0;
                Main.NewText(player.name + "'s [ammo efficiency] skill has increased to level " + level[16] + "!");
            }
            if (level[17] < 10 && level[17] * level[17] * 100 + 300 < experience[17])
            {
                level[17]++;
                experience[17] = 0;
                Main.NewText(player.name + "'s [contact resistance] has increased to level " + level[17] + "!");
                if(level[17] == 10)
                {
                    Main.NewText("[contact resistance] has been maxed out. [thorns] has been unlocked.");
                }
            }
            else if (level[17] == 10 && level[19] * level[19] * 100 + 300 < experience[19])
            {
                level[19]++;
                experience[19] = 0;
                Main.NewText(player.name + "'s [thorns] has increased to level " + level[19] + "!");
            }
            if (level[18] < 10 && level[18] * level[18] * 100 + 300 < experience[18])
            {
                level[18]++;
                experience[18] = 0;
                Main.NewText(player.name + "'s [projectile resistance] has increased to level " + level[18] + "!");
                if (level[18] == 10)
                {
                    Main.NewText("[projectile resistance] has been maxed out. [evasion] has been unlocked.");
                }
            }
            else if(level[18] == 10 && level[20] != 10 && level[20] * level[20] * 100 + 300 < experience[20])
            {
                level[20]++;
                experience[20] = 0;
                Main.NewText(player.name + "'s [evasion] has increased to level " + level[20] + "!");
                if (level[20] == 10)
                {
                    Main.NewText("[evasion] has been maxed out.");
                }
            }
            if(level[21] * level[21] * 80 + 80 < experience[21])
            {
                level[21]++;
                experience[21] = 0;
                Main.NewText(player.name + "'s [throwing proficiency] has increased to level " + level[21] + "!");
            }
        }
        public override void Initialize()
        {
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            //read settings from file
            string[] settings = File.ReadAllLines(Main.SavePath + "\\LBDsettings.txt");
            skillMult = float.Parse(settings[0], ci);
            skillFlat = float.Parse(settings[1], ci);
            expMult = float.Parse(settings[2], ci);
        }
        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            int num21 = 25 + player.extraFall;
            int num22 = (int)(player.position.Y / 16f) - player.fallStart;
            if (player.velocity.Y == 0 && hitDirection == 0 && ((player.gravDir == 1f && num22 > num21) || (player.gravDir == -1f && num22 < -num21)))//if damage was taken from falling (almost foolproof)
            {
                experience[9] += (int)(damage * expMult);
            }
            experience[8] += (int)(damage* expMult);
        }
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            return base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);
        }
        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            damage = (int)(damage * (1f - level[17] * .01f));
            if(level[8] == 10)
            {
                if(level[17] == 10)
                {
                    experience[19] += (int)(expMult * damage);
                }
                else
                {
                    experience[17] += (int)(expMult * damage);
                }

            }

        }
        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            damage = (int)(damage * (1f - level[18] * .01f));
            if (level[20] > Main.rand.Next(100))
            {
                experience[20] += (int) (expMult * damage * 3);
                damage = 0;
            }
            if (level[8] == 10)
            {
                if (level[18] == 10)
                {
                    experience[20] += (int)(expMult * damage / 2);
                }
                else
                {
                    experience[18] += (int) (expMult * damage);
                }
            }
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            int exp = (int)((Math.Min(damage, target.lifeMax)) * expMult / 5);//cannot gain more EXP from a single hit past its max life
            if (item.melee)
            {
                experience[0] += exp;
            }
            else if (item.ranged)
            {
                experience[1] += exp;
            }
            else if (item.magic)
            {
                experience[2] += exp;
            }
            else if (item.summon)
            {
                experience[3] += exp;
            }

        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            int exp = (int)((Math.Min(damage, target.lifeMax)) * expMult / 5);//cannot gain more EXP from a single hit past its max life
            if (proj.melee)
            {
                experience[0] += exp;
            }
            else if (proj.ranged)
            {
                experience[1] += exp;
            }
            else if (proj.magic)
            {
                experience[2] += exp;
            }
            //minion definition includes vanilla summon projectiles NO DD2 YET
            else if ((proj.minion || proj.type == 374 || proj.type == 376 || proj.type == 378 || proj.type == 379 || proj.type == 389 || proj.type == 408 || proj.type == 433 || proj.type == 614 || proj.type == 624 || proj.type == 195 || proj.type == 642 || proj.type == 644))
            {
                experience[3] += exp;
            }
            else if (proj.thrown)
            {
                experience[21] += exp;
            }
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (Main.rand.Next(100) < Math.Min(level[12], 80))
            {
                player.statLife = 1;
                return false;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            experience[12]++;
        }
        public override bool ConsumeAmmo(Item weapon, Item ammo)
        {
            experience[16]++;
            if(Main.rand.Next(100) < level[16])
            {
                return false;
            }
            return base.ConsumeAmmo(weapon, ammo);
        }
        public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
        {
            experience[11]++;
        }
        public override void AnglerQuestReward(float rareMultiplier, List<Item> rewardItems)
        {
            experience[11] += 10;
        }

        public override TagCompound Save()
        {
            TagCompound data = new TagCompound();
            for(int i=0; i<22; i++)
            {
                data.Add("exp" + i, experience[i]);
            }
            data.Add("levels", level);
            return data;
        }
        public override void Load(TagCompound tag)
        {
            try
            {
                level = tag.GetIntArray("levels");
                if(level.Length == 21)
                {
                    int[] leveltemp = new int[22];
                    for(int i=0; i<21; i++)
                    {
                        leveltemp[i] = level[i];
                    }
                    level = leveltemp;
                    for (int i = 0; i < 21; i++)
                    {
                        experience[i] = tag.GetLong("exp" + i);
                    }
                }
                else
                {
                    for (int i = 0; i < 22; i++)
                    {
                        experience[i] = tag.GetLong("exp" + i);
                    }
                }
            }
            catch
            {

            }
        }
    }
}
