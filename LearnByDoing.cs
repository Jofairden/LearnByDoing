using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace LearnByDoing
{
    public class LearnByDoing : Mod
    {
        public LearnByDoing()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
            };
        }
        public override void Load()
        {
            RegisterHotKey("Display Skills", "/");
            string filename = Main.SavePath + "\\LBDsettings.txt";
            if (!System.IO.File.Exists(filename))
            {
                using (StreamWriter tw = new StreamWriter(filename, false))
                {
                    tw.WriteLine("1");
                    tw.WriteLine("0");
                    tw.WriteLine("1");
                    tw.WriteLine("First line is a multiplier for the beneficial effects of skills. Setting this to a negative value will probably have some really weird consequences.");
                    tw.WriteLine("Second line is a flat adjustment to the values of some skills (damage, health, movespeed, jump speed)");
                    tw.WriteLine("It is not recommended to go below -0.5 on this value. -0.05 will normalize to vanilla values pretty early on with skill boosts.");
                    tw.WriteLine("Third line is a multiplier to all experience gained. Setting this to a negative value will probably have some really weird consequences.");
                }
            }
        }
        public override void HotKeyPressed(string name)
        {
            if(name == "Display Skills")
            {
                Player player = Main.player[Main.myPlayer];
                MPlayer mplayer = (MPlayer)(player.GetModPlayer(this, "MPlayer"));
                if(mplayer.displayTimer > 0)
                {
                    return;
                }
                mplayer.displayTimer = 60;
                string output = "";
                string line = Environment.NewLine;
                string text1 = " is level ";
                string text2 = ". Current bonus: ";
                output += "[melee proficiency]" + text1 + mplayer.level[0] + text2 + "+ " + mplayer.level[0] * mplayer.skillMult + "% melee damage. Progress to next level: " + (int)(100 * mplayer.experience[0]/(mplayer.level[0] * mplayer.level[0] * 80 + 80)) + "%" + line;
                output += "[ranged proficiency]" + text1 + mplayer.level[1] + text2 + "+ " + mplayer.level[1] * mplayer.skillMult + "% ranged damage. Progress to next level: " + (int)(100 * mplayer.experience[1] / (mplayer.level[1] * mplayer.level[1] * 80 + 80)) + "%" + line;
                output += "[magic proficiency]" + text1 + mplayer.level[2] + text2 + "+ " + mplayer.level[2] * mplayer.skillMult + "% magic damage. Progress to next level: " + (int)(100 * mplayer.experience[2] / (mplayer.level[2] * mplayer.level[2] * 80 + 80)) + "%" + line;
                output += "[summon proficiency]" + text1 + mplayer.level[3] + text2 + "+ " + mplayer.level[3] * mplayer.skillMult + "% minion damage. Progress to next level: " + (int)(100 * mplayer.experience[3] / (mplayer.level[3] * mplayer.level[3] * 80 + 80)) + "%" + line;
                output += "[throwing proficiency]" + text1 + mplayer.level[21] + text2 + "+ " + mplayer.level[21] * mplayer.skillMult + "% thrown damage. Progress to next level: " + (int)(100 * mplayer.experience[21] / (mplayer.level[21] * mplayer.level[21] * 80 + 80)) + "%" + line;
                output += "[running]" + text1 + mplayer.level[4] + text2 + "+ " + mplayer.level[4] * mplayer.skillMult * 2+ "% movespeed. Progress to next level: " + (int)(100 * mplayer.experience[4] / (mplayer.level[4] * mplayer.level[4] * 5000 + 5000)) + "%" + line;
                output += "[jumping]" + text1 + mplayer.level[5] + text2 + "+ " + mplayer.level[5] * mplayer.skillMult * 3 + "% jump speed. Progress to next level: " + (int)(100 * mplayer.expMult * mplayer.experience[5] / (mplayer.level[5] * mplayer.level[5] * 12 + 12)) + "%" + line;
                output += "[swimming]" + text1 + mplayer.level[6] + text2 + "+ " + mplayer.level[6] * mplayer.skillMult * 3 + "% movespeed underwater. Progress to next level: " + (int)(100 * mplayer.experience[6] / (mplayer.level[6] * mplayer.level[6] * 3000 + 3000)) + "%" + line;
                output += "[flying]" + text1 + mplayer.level[7] + text2 + "+ " + mplayer.level[7] * mplayer.skillMult + "% flight time and speed. Progress to next level: " + (int)(100 * mplayer.experience[7] / (mplayer.level[7] * mplayer.level[7] * 400 + 600)) + "%";
                Main.NewTextMultiline(output);
                output = "";
                output += "[toughness]" + text1 + mplayer.level[8] + text2 + "+ " + (int)(mplayer.level[8] * mplayer.skillMult) + " max HP, " + (int)(mplayer.level[8] * mplayer.skillMult) / 2 + "defense. Progress to next level: " + (mplayer.level[8] != 10 ? (int)(100 * mplayer.experience[8] / (mplayer.level[8] * mplayer.level[8] * 45 + 45)) + "%" : "N/A") + line;
                output += "[fall immunity]" + text1 + mplayer.level[9] + ". Progress to next level: " + (mplayer.level[9] != 1 ? (int)(100 * mplayer.experience[9] / (2000)) + "%" : "N/A") + line;
                output += "[mana manipulation]" + text1 + mplayer.level[10] + text2 + "+ " + (int)(mplayer.level[10] * mplayer.skillMult / 2) + " bonus mana regen. Progress to next level: " + (int)(100 * mplayer.expMult * mplayer.experience[10] / (mplayer.level[10] * mplayer.level[10] * 700 + 500)) + "%" + line;
                output += "[fishing]" + text1 + mplayer.level[11] + text2 + "+ " + mplayer.level[11] * mplayer.skillMult + " fishing power. Progress to next level: " + (int)(100 * mplayer.expMult * mplayer.experience[11] / (mplayer.level[11] * 10 + 10)) + "%" + line;
                output += "[death resistance]" + text1 + mplayer.level[12] + text2 + "+ " + Math.Min(mplayer.level[12] * mplayer.skillMult, 80) + "% chance to survive a fatal blow. Progress to next level: " + (int)(100 * mplayer.expMult * mplayer.experience[12] / (mplayer.level[12] * 3 + 1)) + "%" + line;
                output += "[eating]" + text1 + mplayer.level[13] + text2 + "+ " + mplayer.level[13] * mplayer.skillMult/2 + " max HP, " + mplayer.level[13] * mplayer.skillMult / 4 + " defense. Progress to next level: " + (int)(100 * mplayer.expMult * mplayer.experience[13] / (mplayer.level[13]*mplayer.level[13] * 20 + 30)) + "%" + line;
                output += "[potion tolerance]" + text1 + mplayer.level[14] + text2 + "+ " + mplayer.level[14] * mplayer.skillMult / 2 + " max HP, " + mplayer.level[14] * mplayer.skillMult / 3 + "% damage, -" + mplayer.level[14] * mplayer.expMult * mplayer.skillMult / 2 +  " sec potion CD. Progress to next level: " + (int)(100 * mplayer.experience[14] / (mplayer.level[14] * mplayer.level[14] * 20 + 30)) + "%" + line;
                output += "[life regeneration]" + text1 + mplayer.level[15] + text2 + "+ " + (mplayer.level[15] * mplayer.skillMult / 12f) + " life regen/sec. Progress to next level: " + (int)(100 * mplayer.expMult * mplayer.experience[15] / (mplayer.level[15] * mplayer.level[15] * 1800 + 1800)) + "%" + line;
                output += "[ammo efficiency]" + text1 + mplayer.level[16] + text2 + "+ " + mplayer.level[16] * mplayer.skillMult + "% chance to not consume ammo. Progress to next level: " + (int)(100 * mplayer.expMult * mplayer.experience[16] / (mplayer.level[16] * mplayer.level[16] * 30 + 30)) + "%";
                Main.NewTextMultiline(output);
                output = "";
                output += "[contact resistance]" + text1 + mplayer.level[17] + text2 + "+ " + (int)(mplayer.level[17] * mplayer.skillMult) + "% contact damage reduction. Progress to next level: " + (mplayer.level[17] != 10 ? (int)(100 * mplayer.experience[17] / (mplayer.level[17] * mplayer.level[17] * 100 + 300)) + "%" : "N/A") + line;
                output += "[projectile resistance]" + text1 + mplayer.level[18] + text2 + "+ " + (int)(mplayer.level[18] * mplayer.skillMult) + "% projectile damage reduction. Progress to next level: " + (mplayer.level[18] != 10 ? (int)(100 * mplayer.experience[18] / (mplayer.level[18] * mplayer.level[18] * 100 + 300)) + "%" : "N/A") + line;
                output += "[thorns]" + text1 + mplayer.level[19] + text2 + "+ " + mplayer.level[19] * mplayer.skillMult * 10 + "% damage returned to melee attackers. Progress to next level: " + (int)(100 * mplayer.experience[19] / (mplayer.level[19] * mplayer.level[19] * 100 + 300)) + "%" + line;
                output += "[evasion]" + text1 + mplayer.level[20] + text2 + "+ " + (int)(mplayer.level[20] * mplayer.skillMult) + "% chance to avoid projectiles. Progress to next level: " + (mplayer.level[20] != 10 ? (int)(100 * mplayer.experience[20] / (mplayer.level[20] * mplayer.level[20] * 100 + 300)) + "%" : "N/A");
                Main.NewTextMultiline(output);
            }
        }
    }
}
