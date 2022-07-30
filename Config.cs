using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RBXTools
{
    class Config
    {
        public static FileInfo configFilePath = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RBXTools", "mods.config"));
        public static string[] strings = new string[] { "DeathSFXMod", "MouseCursorMod" };
        public static List<string> validIDs = new List<string>(strings);
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
            configcurrent += id + " " + custompath + "\n";
            File.WriteAllText(configFilePath.FullName, configcurrent);
            Console.WriteLine("Wrote mod: " + id + " to config file!");
        }

        public static void WriteMod(string id, string[] custompaths)
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
                if (!line.Contains(id))
                {
                    removedlines.Add(line);
                } else
                {
                    Console.WriteLine("Mod found and removed from config file.");
                }
            }
            File.WriteAllLines(configFilePath.FullName,removedlines);
        }

        public static bool CheckIfModHasBeenAddedAlready(string id)
        {
            Console.WriteLine("Searching for mod...");
            string[] lines = File.ReadAllLines(configFilePath.FullName);
            foreach(string line in lines)
            {
                if(line.Contains(id))
                {
                    Console.WriteLine("Mod already exists!");
                    return true;
                }
            }
            return false;
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
    }
}
