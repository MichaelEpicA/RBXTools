using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RBXTools
{
    class Config
    {
        public static FileInfo configFilePath = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RBXTools", "mods.config"));
        public static string[] strings = new string[] { "DeathSFXMod", "MouseCursorMod", "ReinstallLauncherMod", "ReplaceMoonAndSunMod", "RobloxIconReplaceMod", "AppLaunchSetting", "RobloxFolderDefaultSetting" };
        public static List<string> validIDs = new List<string>(strings);
        public static void VerifyFormat()
        {
            string[] ids = ReturnIDsOfModsInstalled();
            string[] lines = File.ReadAllLines(configFilePath.FullName);
            foreach(string line in lines)
            {
                int i = 0;
                if (line.Contains(ids[0]))
                {
                    if(lines[i+1].Split(" ")[0] == "RobloxFolder")
                    {
                        //New format.
                        
                    } else
                    {
                        //Old format, need to update the format.
                        Console.WriteLine("Your config file has been identified as outdated. It needs to be updated.");
                        Console.WriteLine("So, heres my one question. Do you use this current roblox folder for modding, or do you use some other folder? (C/O) (Current/Other)");
                        string input = Console.ReadLine();
                        if (input.ToLower().Contains("c"))
                        {
                            UpdateFormat();
                        } else if(input.ToLower().Contains("o"))
                        {
                            Roblox.RobloxFolderChoose();
                            VerifyFormat();
                            return;
                        }
                    }
                }
            }
            
        }

        public static void UpdateFormat()
        {
            Console.WriteLine("Updating...");
            string roblox = "RobloxFolder " + Roblox.robloxFolder.folderinfo.FullName;
            string[] lines = File.ReadAllLines(configFilePath.FullName);
            List<string> linestowrite = new List<string>();
            foreach(string s in lines)
            {
                linestowrite.Add(s);
                linestowrite.Add(roblox);
            }
            Console.WriteLine("Writing to file...");
            File.WriteAllLines(configFilePath.FullName,linestowrite);
            Console.WriteLine("Wrote to file. Complete!");
        }
        public static void WriteMod(string id, string custompath = "default")
        {
            if(!validIDs.Contains(id))
            {
                Console.WriteLine("This is not a valid ID! Writing cannot continue.");
                return;
            }
            if(CheckIfModHasBeenAddedAlready(id))
            {
                RemoveMod(id);
                Console.Clear();
            }
            Console.WriteLine("Writing mod to config file...");
            string configcurrent = File.ReadAllText(configFilePath.FullName);
            configcurrent += id + " " + custompath + "\n" + "RobloxFolder " + Roblox.robloxFolder.folderinfo.FullName + "\n";
            File.WriteAllText(configFilePath.FullName, configcurrent);
            Console.WriteLine("Wrote mod: " + id + " to config file!");
        }

        public static void WriteMod(string id, params string[] custompaths)
        {
            if (!validIDs.Contains(id))
            {
                Console.WriteLine("This is not a valid ID! Writing cannot continue.");
                return;
            }
            if (CheckIfModHasBeenAddedAlready(id))
            {
                RemoveMod(id);
                Console.Clear();
            }
            Console.WriteLine("Writing mod to config file...");
            string configcurrent = File.ReadAllText(configFilePath.FullName);
            configcurrent += id + " ";
            foreach(string custompath in custompaths)
            {
                configcurrent += custompath + ",";
            }
            int lastIndexOfComma = configcurrent.LastIndexOf(',');
            if (lastIndexOfComma != -1)
            {
                configcurrent.Remove(lastIndexOfComma, 1);
            }
            configcurrent += "\n" + Roblox.robloxFolder.folderinfo.FullName + "\n";
            File.WriteAllText(configFilePath.FullName, configcurrent);
            Console.WriteLine("Wrote mod: " + id + "to config file!");
        }

        public static void RemoveMod(string id)
        {
            if (!validIDs.Contains(id))
            {
                Console.WriteLine("This is not a valid ID! Writing cannot continue.");
                return;
            }
            Console.WriteLine("Searching for mod...");
            string[] lines = File.ReadAllLines(configFilePath.FullName);
            List<string> removedlines = new List<string>();
            foreach(string line in lines)
            {
                int i = 0;
                if (!line.Contains(id))
                {
                    removedlines.Add(line);
                } else
                {
                    Console.WriteLine("Mod found and removed from config file.");
                    lines[i + 1] = id + "";
                }
            }
            File.WriteAllLines(configFilePath.FullName,removedlines);
        }

        public static bool CheckIfModHasBeenAddedAlready(string id)
        {
            //Console.WriteLine("Searching for mod...");
            string[] lines = File.ReadAllLines(configFilePath.FullName);
            foreach(string line in lines)
            {
                if(line.Contains(id))
                {
                    RobloxFolder folder = GetRobloxFolderUsed(id);
                    if (folder != null)
                    {
                        if(Roblox.robloxFolder == null)
                        {
                            if(id == "RobloxFolderDefaultSetting")
                            {
                                return true;
                            }
                        }
                        if(folder.folderinfo.FullName == Roblox.robloxFolder.folderinfo.FullName || id == "RobloxFolderDefaultSetting")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static RobloxFolder GetRobloxFolderUsed(string id)
        {
            string[] lines = File.ReadAllLines(configFilePath.FullName);
            foreach (string line in lines)
            {
                int i = 0;
                if(line.Contains(id))
                {
                    string robloxmod = lines[i + 1];
                    return new RobloxFolder(new DirectoryInfo(robloxmod.Split(" ")[1]));
                }
                i++;
            }
            return null;
        }

        public static string[] ReturnIDsOfModsInstalled()
        {
            Console.WriteLine("Getting ID's of mods installed...");
            string[] lines = File.ReadAllLines(configFilePath.FullName);
            List<string> ids = new List<string>();
            foreach(string line in lines)
            {
                string[] idandpath = line.Split(' ');
                if(validIDs.Contains(idandpath[0]))
                {
                    //Valid ID, add to ids.
                    ids.Add(idandpath[0]);
                }
            }
            return ids.ToArray();
        }

        public static string[] ReturnPathsOfMod(string id)
        {
            //bool found = false;
            if (!validIDs.Contains(id))
            {
                Console.WriteLine("This is not a valid ID! Writing cannot continue.");
                return new string[] { "invalid" };
            }
            if (!CheckIfModHasBeenAddedAlready(id))
            {
                //Oops, we couldn't find this mod.
                return new string[] { "invalid" };
            }
            string[] configlines = File.ReadAllLines(configFilePath.FullName);
            List<string> paths = new List<string>();
            foreach (string currentline in configlines)
            {
                if (currentline.Contains(id))
                {
                    string[] lines = currentline.Split(' ');
                    //lines[0] == id, lines[1] == first path.
                    if (lines.Length > 2)
                    {
                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (i == 0)
                            {
                                continue;
                            }
                            paths.Add(lines[i]);
                        }
                    }
                    else
                    {
                        paths.Add(lines[1]);
                    }
                }
            }
            return paths.ToArray();
        }
    }
}
