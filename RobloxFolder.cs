using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RBXTools
{
    public class RobloxFolder
    {
        public DirectoryInfo folderinfo;
        public bool adminFolder;
        public bool defaultFolder;
        private delegate void RobloxFolderSwitch(DirectoryInfo switchto);
        public RobloxFolder(DirectoryInfo info, bool adminFold=false)
        {
            folderinfo = info;
            adminFolder = adminFold;
        }

        public void Verifier()
        {
            //Check if we are readonly
            if(folderinfo.Attributes.HasFlag(FileAttributes.ReadOnly))
            {
                ClearReadOnly(folderinfo);
            }
            try
            {// Check if it still fails.
                File.Create(Path.Combine(folderinfo.FullName, "test.test")).Close();
                File.Delete(Path.Combine(folderinfo.FullName, "test.test"));
            } catch(UnauthorizedAccessException)
            {
                //Admin.
                adminFolder = true;
            }
            //Checking if this folder is a default folder.
            if(Config.CheckIfModHasBeenAddedAlready("RobloxFolderDefaultSetting"))
            {
                string[] paths = Config.ReturnPathsOfMod("RobloxFolderDefaultSetting");
                string path = paths[0];
                if(paths.Length > 1)
                {
                    path = String.Join(" ", paths);
                }
                if ( path == folderinfo.FullName)
                {
                    defaultFolder = true;
                }
            }
            
        }

        public string SetAsDefault()
        {
            //Edit config file with a new setting to set this as default.
            Console.WriteLine("Setting this Roblox Folder as default...");
            Config.WriteMod("RobloxFolderDefaultSetting", folderinfo.FullName);
            return "Set this Roblox Folder as default.";
        }

        public string DeleteRobloxInstall()
        {
            //Delete the directory info that was found.
            Console.WriteLine("Deleting Roblox install...");
            folderinfo.Delete(true);
            return "Deleted Roblox install.";
        }

        public string DefaultRegistryEdit()
        {
            //Edit the protocol URL to this roblox folder.
            if (!Roblox.DoWeHaveAdmin())
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Information: You need admin in order to use this. Are you ok with this? (Y/N)");
                Console.ResetColor();
                string input = Console.ReadLine();
                if (input.ToLower() == "y")
                {
                    string exename = Process.GetCurrentProcess().MainModule.FileName;
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = exename,
                        Verb = "runas",
                        Arguments = "-regedit",
                        UseShellExecute = true
                    }).WaitForExit();
                    Console.WriteLine("Editted registry!");
                }
                else if (input.ToLower() == "n")
                {
                    return "";
                }
                else
                {
                    Console.WriteLine("Invalid, try again.");
                    DefaultRegistryEdit();
                }
            } 
            else
            {
                RegistryKey classesKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Default);
                //Roblox Player key where the application to run is stored.
                RegistryKey robloxPlayerKey = classesKey.OpenSubKey("roblox-player").OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command", true);
                //Some browsers use this, this specifies the icon of the program, (I think).
                RegistryKey robloxPlayerIconKey = classesKey.OpenSubKey("roblox-player").OpenSubKey("DefaultIcon", true);
                //Alternates of the other two keys I already described.
                RegistryKey robloxKey = classesKey.OpenSubKey("roblox").OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command", true);
                RegistryKey robloxIconKey = classesKey.OpenSubKey("roblox").OpenSubKey("DefaultIcon", true);
                //The path of the application we want to run.
                string robloxPlayerLauncherPath = Path.Combine(folderinfo.FullName, "RobloxPlayerLauncher.exe");
                //Setting all of the application values to our application.
                robloxPlayerKey.SetValue("", robloxPlayerLauncherPath + " %1");
                robloxPlayerIconKey.SetValue("", robloxPlayerLauncherPath);
                robloxKey.SetValue("", robloxPlayerLauncherPath + " %1");
                robloxIconKey.SetValue("", robloxPlayerLauncherPath);
                //Flush all values to disk, and also unlock the registry keys.
                robloxPlayerKey.Close();
                robloxPlayerIconKey.Close();
                robloxKey.Close();
                robloxIconKey.Close();
                //Clear up memory.
                robloxPlayerKey.Dispose();
                robloxPlayerIconKey.Dispose();
                robloxKey.Dispose();
                robloxIconKey.Dispose();
                return "Completed update of protocol URL in registry!";
            }
            return "";
        }

        public string SwapRobloxFolder()
        {
            List<Choice> choices = new List<Choice>();
            //Swap to the other roblox install location.
            Console.WriteLine("Choices: ");
            if(folderinfo.FullName.Contains("Program Files"))
            {
                Choice.Add(choices, "AppData", new RobloxFolderSwitch(SwitchRobloxFolderAppData));
            } else if(folderinfo.FullName.Contains("AppData"))
            {
                Choice.Add(choices, "Program Files", new RobloxFolderSwitch(SwitchRobloxFolderProgramFiles));
            } else
            {
                Choice.Add(choices, "AppData", new RobloxFolderSwitch(SwitchRobloxFolderAppData));
                Choice.Add(choices, "Program Files", new RobloxFolderSwitch(SwitchRobloxFolderProgramFiles));
            }
            Choice.Add(choices, "Custom Path", new RobloxFolderSwitch(SwitchRobloxFolder));
            Choice.Add(choices, "Back", null);
            Console.Write("Please enter a choice number: ");
            string input = Console.ReadLine();
            try
            {
                int i = Convert.ToInt32(input);
                if(i == choices.Count)
                {
                    return "";
                }
                choices[i - 1].choiceFunction.DynamicInvoke(new DirectoryInfo("fuck"));
            } catch(IOException)
            {
                Console.Clear();
                Console.WriteLine("Roblox appears to be using this. Please close roblox and try again.");
                SwapRobloxFolder();
            } catch(Exception)
            {
                Console.Clear();
                Console.WriteLine("Invalid input. Try again.");
                SwapRobloxFolder();
            }
            return "";
        }

        private void SwitchRobloxFolderProgramFiles(DirectoryInfo switchto)
        {
            DirectoryInfo programFiles = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Roblox", "Versions", folderinfo.Name));
            if(!Roblox.DoWeHaveAdmin())
            {
                Console.WriteLine("You need admin to switch to this specific folder. Are you sure you want to continue? (Y/N)");
                string inp = Console.ReadLine();
                if(inp.ToLower() == "y")
                {
                    Roblox.RestartAsAdmin();
                } else
                {
                    return;
                }
            }
            if(programFiles.Exists)
            {
                Console.WriteLine("A roblox install already exists at this folder, do you want to delete it, or cancel? (D/C)");
                string inp = Console.ReadLine();
                if(inp.ToLower() == "d")
                {
                    try
                    {
                        programFiles.Delete(true);
                    } catch(Exception)
                    {
                        Console.WriteLine("Something went wrong while trying to delete the roblox install. Please try again later.");
                    }
                    
                }  else if(inp.ToLower() == "c")
                {
                    SwapRobloxFolder();
                } else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input, try again.");
                    SwitchRobloxFolderProgramFiles(switchto);
                }
            }
            folderinfo.MoveTo(programFiles.FullName);
            
        }

        private void SwitchRobloxFolderAppData(DirectoryInfo switchto)
        {
            DirectoryInfo appData = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "Versions", folderinfo.Name));
            if (appData.Exists)
            {
                Console.WriteLine("A roblox install already exists at this folder, do you want to delete it, or cancel? (D/C)");
                string inp = Console.ReadLine();
                if (inp.ToLower() == "d")
                {
                    try
                    {
                        appData.Delete(true);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Something went wrong while trying to delete the roblox install. Please try again later.");
                        return;
                    }

                }
                else if (inp.ToLower() == "c")
                {
                    SwapRobloxFolder();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input, try again.");
                    SwitchRobloxFolderAppData(switchto);
                }
            }
            folderinfo.MoveTo(appData.FullName);
        }

        private void SwitchRobloxFolder(DirectoryInfo switchto)
        {
            switchto = EnterPath();
            if (switchto.Exists)
            {
                Console.WriteLine("A roblox install already exists at this folder, do you want to delete it, or cancel?");
                string inp = Console.ReadLine();
                if (inp.ToLower() == "d")
                {
                    try
                    {
                        switchto.Delete(true);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Something went wrong while trying to delete the roblox install. Please try again later.");
                        return;
                    }

                }
                else if (inp.ToLower() == "c")
                {
                    SwapRobloxFolder();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input, try again.");
                    SwitchRobloxFolder(switchto);
                }
            }
            Console.WriteLine(Roblox.robloxFolders.Count);
            folderinfo.MoveTo(switchto.FullName);
            Console.WriteLine(Roblox.robloxFolders.Count);
        }

        public static DirectoryInfo EnterPath()
        {
            Console.WriteLine("Please enter in the path you want. (Warning, you will only be able to modify this if you set this roblox folder as default, set the roblox folder as using in the registry, or manually type it in.");
            string path = Console.ReadLine();
            Console.WriteLine("Is this correct? " + path + " (Y/N)");
            string input2 = Console.ReadLine();
            if (input2.ToLower() == "y")
            {
                DirectoryInfo info = new DirectoryInfo(path);
                return info;
            }
            else
            {
                EnterPath();
            }
            return null;
        }

        public static void ClearReadOnly(DirectoryInfo parentDirectory)
        {
            if (parentDirectory != null)
            {
                parentDirectory.Attributes = FileAttributes.Normal;
                foreach (FileInfo fi in parentDirectory.GetFiles())
                {
                    fi.Attributes = FileAttributes.Normal;
                }
                foreach (DirectoryInfo di in parentDirectory.GetDirectories())
                {
                    ClearReadOnly(di);
                }
            }
        }
    }
}
