using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Xml;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace RBXTools
{
    public static class Roblox
    {
        private delegate void RobloxFolderDelegate(RobloxFolder folder);
        private delegate string RobloxFolderDelegatestr();
        [Serializable]
        public class RobloxException : System.Exception
        {
            public RobloxException(string message)
            : base(message)
            {

            }
        }
        public static RobloxFolder robloxFolder = null;
        private static List<char> invalidCharsDetected = new List<char>();
        public static List<RobloxFolder> robloxFolders = new List<RobloxFolder>();
        public static void Cleanup()
        {
            string robloxVersPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "Versions");
            DirectoryInfo[] infos = null;
            if(new DirectoryInfo(robloxVersPath).Exists)
            {
                infos = new DirectoryInfo(robloxVersPath).GetDirectories();
            }
            try
            {
                foreach (DirectoryInfo info in infos)
                {
                    if (info.GetFiles().Length == 0)
                    {
                        Console.WriteLine("Cleaning up directory: " + info.FullName);
                        info.Delete();
                        Console.WriteLine("Cleaned up directory!");
                        continue;
                    }
                    foreach (FileInfo info2 in info.GetFiles())
                    {
                        if (info2.Name == "RobloxPlayerLauncher.exe" && info.GetFiles().Length == 1)
                        {
                            Console.WriteLine("Cleaning up directory: " + info.FullName);
                            FileVersionInfo info3 = FileVersionInfo.GetVersionInfo(info2.FullName);
                            if (info3.FileDescription == "RobloxRepair Auto Update")
                            {
                                //Litteraly only our modded launcher. Safe to delete.
                                try
                                {
                                    info.Delete(true);
                                    Console.WriteLine("Cleaned up directory!");
                                }
                                catch (Exception)
                                {
                                    //Modded launcher is most likely still running.
                                    Console.WriteLine("Non-fatal error occured: Failed to delete custom launcher from old roblox folder: " + info.FullName + "\n You can delete this later.");
                                    continue;
                                }

                            }
                        }
                    }
                }
            } catch(Exception)
            {
                //ignore
            }
            
        }
        public static void CleanupAdmin()
        {
            string robloxVersPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "Versions");
            string robloxVers2Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Roblox", "Versions");
            DirectoryInfo[] infos = null;
            DirectoryInfo[] infos2 = null;
            if (Directory.Exists(robloxVersPath))
            {
                infos = new DirectoryInfo(robloxVersPath).GetDirectories();
            }

            if (Directory.Exists(robloxVers2Path))
            {
                infos2 = new DirectoryInfo(robloxVers2Path).GetDirectories();
            }
            try
            {
                foreach (DirectoryInfo info in infos)
                {
                    if (info.GetFiles().Length == 0)
                    {
                        Console.WriteLine("Cleaning up directory: " + info.FullName);
                        info.Delete();
                        Console.WriteLine("Cleaned up directory!");
                        continue;
                    }
                    foreach (FileInfo info2 in info.GetFiles())
                    {
                        if (info2.Name == "RobloxPlayerLauncher.exe" && info.GetFiles().Length == 1)
                        {
                            Console.WriteLine("Cleaning up directory: " + info.FullName);
                            FileVersionInfo info3 = FileVersionInfo.GetVersionInfo(info2.FullName);
                            if (info3.FileDescription == "RobloxRepair Auto Update")
                            {
                                //Litteraly only our modded launcher. Safe to delete.
                                try
                                {
                                    info.Delete(true);
                                    Console.WriteLine("Cleaned up directory!");
                                }
                                catch (Exception)
                                {
                                    //Modded launcher is most likely still running.
                                    Console.WriteLine("Non-fatal error occured: Failed to delete custom launcher from old roblox folder: " + info.FullName + "\n You can delete this later.");
                                    continue;
                                }

                            }
                        }
                    }
                }
            } catch(Exception)
            {
                //ignore
            }
            
            //Checking program files, (A new install location for roblox)
            try
            {
                foreach (DirectoryInfo info in infos2)
                {
                    if (info.GetFiles().Length == 0)
                    {
                        Console.WriteLine("Cleaning up directory: " + info.FullName);
                        info.Delete();
                        Console.WriteLine("Cleaned up directory!");
                        continue;
                    }
                    foreach (FileInfo info2 in info.GetFiles())
                    {
                        if (info2.Name == "RobloxPlayerLauncher.exe" && info.GetFiles().Length == 1)
                        {
                            Console.WriteLine("Cleaning up directory: " + info.FullName);
                            FileVersionInfo info3 = FileVersionInfo.GetVersionInfo(info2.FullName);
                            if (info3.FileDescription == "RobloxRepair Auto Update")
                            {
                                //Litteraly only our modded launcher. Safe to delete.
                                try
                                {
                                    info.Delete(true);
                                    Console.WriteLine("Cleaned up directory!");
                                }
                                catch (Exception)
                                {
                                    //Modded launcher is most likely still running.
                                    Console.WriteLine("Non-fatal error occured: Failed to delete custom launcher from old roblox folder: " + info.FullName + "\n You can delete this later.");
                                    continue;
                                }

                            }
                        }
                    }
                }
            } catch(Exception)
            {
                //ignore
            }
            
        }
        public static bool DoWeHaveAdmin()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return true;
            }
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }
        public static void AutoFindRobloxFolder()
        {
            string robloxVersPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "Versions");
            string robloxVers2Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Roblox", "Versions");
            string home = Environment.GetEnvironmentVariable("HOME");
            string user = Environment.GetEnvironmentVariable("USER");
            string robloxVersLinuxPath = home + "/.local/share/grapejuice/prefixes/player/drive_c/users/" + user + "/AppData/Local/Roblox/Versions/";
            string robloxVers2LinuxPath = home + "/.local/share/grapejuice/prefixes/player/drive_c/Program Files (x86)/Roblox/Versions/";
            DirectoryInfo[] infos = null;
            DirectoryInfo[] infos2 = null;
            if (Directory.Exists(robloxVersPath))
            {
                infos = new DirectoryInfo(robloxVersPath).GetDirectories();
            } else
            {
                if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && Directory.Exists(robloxVersLinuxPath))
                {
                    infos = new DirectoryInfo(robloxVersLinuxPath).GetDirectories();
                }
            }

            if(Directory.Exists(robloxVers2Path))
            {
                infos2 = new DirectoryInfo(robloxVers2Path).GetDirectories();
            } else
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && Directory.Exists(robloxVers2LinuxPath))
                {
                    infos = new DirectoryInfo(robloxVers2LinuxPath).GetDirectories();
                }
            }
            bool found = false;
            try
            {
                foreach (DirectoryInfo info in infos)
                {
                    foreach (FileInfo info2 in info.GetFiles())
                    {
                        if (info2.Name == "RobloxPlayerBeta.exe")
                        {
                            found = true;
                            robloxFolders.Add(new RobloxFolder(info));
                        }

                        if (info2.Name == "RobloxPlayerLauncher.exe" && info.GetFiles().Length == 1)
                        {
                            FileVersionInfo info3 = FileVersionInfo.GetVersionInfo(info2.FullName);
                            if (info3.FileDescription == "RobloxRepair Auto Update")
                            {
                                //Litteraly only our modded launcher. Safe to delete.
                                try
                                {
                                    info.Delete(true);
                                }
                                catch (Exception)
                                {
                                    //Modded launcher is most likely still running.
                                    Console.WriteLine("Non-fatal error occured: Failed to delete custom launcher from old roblox folder: " + info.FullName + "\n You can delete this later.");
                                    continue;
                                }

                            }
                        }
                    }


                }
            } catch(Exception)
            {
                //Well, an error occured, (Most likely trying to find the thing. Just ignore it.)
            } 
            
            //Checking program files, (A new install location for roblox)
            try
            {
                foreach (DirectoryInfo info in infos2)
                {
                    foreach (FileInfo info2 in info.GetFiles())
                    {
                        if (info2.Name == "RobloxPlayerBeta.exe")
                        {
                            found = true;
                            RobloxFolder folder = new RobloxFolder(info, true);
                            folder.Verifier();
                            robloxFolders.Add(folder);
                            break;
                            if ((!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || !RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) && !DoWeHaveAdmin())
                            {
                                RestartAsAdmin();
                            }
                        }

                        if (info2.Name == "RobloxPlayerLauncher.exe" && info.GetFiles().Length == 1)
                        {
                            FileVersionInfo info3 = FileVersionInfo.GetVersionInfo(info2.FullName);
                            if (info3.FileDescription == "RobloxRepair Auto Update")
                            {
                                //Litteraly only our modded launcher. Safe to delete.
                                try
                                {
                                    info.Delete(true);
                                }
                                catch (Exception)
                                {
                                    //Modded launcher is most likely still running.
                                    Console.WriteLine("Non-fatal error occured: Failed to delete custom launcher from old roblox folder: " + info.FullName + "\n You can delete this later.");
                                    continue;
                                }

                            }
                        }
                    }
                }
            } catch(Exception)
            {
                //Well, an error occured, (Most likely trying to find the thing. Just ignore it.)
            }

            if(Config.CheckIfModHasBeenAddedAlready("RobloxFolderDefaultSetting"))
            {
                string[] paths = Config.ReturnPathsOfMod("RobloxFolderDefaultSetting");
                string path = paths[0];
                if(paths.Length > 1)
                {
                    path = String.Join(" ", paths);
                }
                if(Directory.Exists(path))
                {
                    robloxFolder = new RobloxFolder(new DirectoryInfo(paths[0]));
                }
            }
            if(robloxFolders.Count == 1)
            {
                robloxFolder = robloxFolders[0];
            } else if(robloxFolders.Count >= 2)
            {
                RobloxFolderChoose();
            }

            if (robloxFolders.Count == 0)
            {
                throw new RobloxException("Failed to find Roblox Folder.");
            }
        }

        public static void FindRobloxFolder(bool force = false)
        {
            if (robloxFolder != null && !force)
            {
                return;
            }
            try
            {
                AutoFindRobloxFolder();
            }
            catch (RobloxException)
            {
                //We couldn't find the roblox folder.
                //Ask the user to input it.
                InputRobloxFolder();
            }
        }

        public static void AddInvalidPathCharactersIfContains(this string input, char c)
        {
            if (input.Contains(c))
            {
                invalidCharsDetected.Add(c);
            }
        }

        private static void InputRobloxFolder()
        {
            Console.WriteLine("We couldn't find the roblox folder, please input it here.");
            string path = Console.ReadLine();
            //Check if we have invalid characters in our path.
            RemoveInvalidCharactersRef(ref path);
            bool isRobloxFolder = false;
            if (String.IsNullOrWhiteSpace(path))
            {
                Console.Clear();
                Console.WriteLine("This is not a valid roblox folder (Directory did not exist), enter that again.");
                InputRobloxFolder();
                return;
            }
            //Verify that this is the roblox folder.
            if (!new DirectoryInfo(path).Exists)
            {
                //Well, it can't be valid if it doesn't exist.
                Console.Clear();
                Console.WriteLine("This is not a valid roblox folder (Directory did not exist), enter that again.");
                InputRobloxFolder();
                return;
            }
            foreach (FileInfo info in new DirectoryInfo(path).GetFiles())
            {
                if (info.Name == "RobloxPlayerBeta.exe")
                {
                    //Correct! Valid roblox folder.
                    isRobloxFolder = true;
                }
            }
            if (!isRobloxFolder)
            {
                //Not a valid roblox folder, tell the user about it.
                Console.Clear();
                Console.WriteLine("This is not a valid roblox folder (Couldn't find RobloxPlayerBeta), enter that again.");
                InputRobloxFolder();
                return;
            }
            robloxFolder = new RobloxFolder(new DirectoryInfo(path));
            try
            {
                File.Create(Path.Combine(robloxFolder.folderinfo.FullName, "test.test")).Close();
                File.Delete(Path.Combine(robloxFolder.folderinfo.FullName, "test.test"));
            }
            catch (UnauthorizedAccessException)
            {
                //Check if we have admin.
                if (DoWeHaveAdmin())
                {
                    //Weird bug.
                    Console.WriteLine("Error occured, and we couldn't access your roblox folder. This error is not supposed to happen, however is not a bug. If you need support, send it to the issues, under a support request. Press enter to close out.");
                    Console.ReadLine();
                    Environment.Exit(-1);
                }
                else
                {
                    //Reboot into admin.
                    RestartAsAdmin();
                }
            }
        }
        public static bool RemoveInvalidCharactersRef(ref string path, bool file = false, bool original = true)
        {
            string path2 = path;
            Path.GetInvalidPathChars().ToList().ForEach(action: delegate (char c)
            {
                path2.AddInvalidPathCharactersIfContains(c);
            });
            path.AddInvalidPathCharactersIfContains('"');
            if (invalidCharsDetected.Count > 0)
            {
                Console.Write("Invalid path characters detected: ");
                string writeToConsole = "";
                foreach (char c in invalidCharsDetected)
                {
                    writeToConsole += (c + ", ");
                }
                int lastIndexOfComma = writeToConsole.LastIndexOf(',');
                if (lastIndexOfComma != -1)
                {
                    writeToConsole = writeToConsole.Remove(lastIndexOfComma, 1);
                    Console.Write(writeToConsole);
                }
                Console.WriteLine("\nAttempting to remove...");
                int startIndex = 0;
                foreach (char c in invalidCharsDetected)
                {
                    int currentIndex = path.IndexOf(c, startIndex);
                    while (currentIndex != -1)
                    {
                        currentIndex = path.IndexOf(c, currentIndex);
                        if (currentIndex == -1)
                        {
                            break;
                        }
                        path = path.Remove(currentIndex, 1);
                    }
                }
                Console.WriteLine("Valid String: " + path);
                Console.WriteLine("Verifying the path is still valid...");
                invalidCharsDetected.Clear();
                if (file && !new FileInfo(path).Exists)
                {
                    Console.WriteLine("Path is invalid. \nEither it needed some of those characters, if so it's a hacked file name/path. \nThe path never existed in the first place, or the path is actually a directory, which doesn't work.");
                    Console.WriteLine("Please reenter it.");
                    return false;
                }
                if (!file && !new DirectoryInfo(path).Exists)
                {
                    //Path is invalid. :C
                    Console.WriteLine("Path is invalid. \nEither it needed some of those characters, if so it's a hacked file name/path. \nThe path never existed in the first place, or the path is actually a file, which doesn't work.");
                    Console.WriteLine("Please reenter it.");
                    if (!original) return false;
                    Console.Clear();
                    Console.WriteLine("This is not a valid roblox folder (Path invalid before and after conversion), enter that again.");
                    InputRobloxFolder();
                }
                Console.WriteLine("Path is valid! Continuing...");
                return true;
            }
            return true;
        }

        public static void UpdateRobloxIcon()
        {

        }

        public static DirectoryInfo SearchRegistry()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return null;
            }
            //Main key where protocols are stored.
            RegistryKey classesKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Default);
            //Roblox Player key where the application to run is stored.
            RegistryKey robloxPlayerKey = classesKey.OpenSubKey("roblox-player").OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command", false);
            //Some browsers use this, this specifies the icon of the program, (I think).
            RegistryKey robloxPlayerIconKey = classesKey.OpenSubKey("roblox-player").OpenSubKey("DefaultIcon", false);
            //Alternates of the other two keys I already described.
            RegistryKey robloxKey = classesKey.OpenSubKey("roblox").OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command", false);
            RegistryKey robloxIconKey = classesKey.OpenSubKey("roblox").OpenSubKey("DefaultIcon", false);
            string file = robloxPlayerKey.GetValue("").ToString();
            try
            {
                file = file.Remove(file.LastIndexOf(" "));
                file = file.Remove(file.IndexOf('"'), 1);
                file = file.Remove(file.LastIndexOf('"'));
            } catch(Exception)
            {
                //Ignore
            }
            
            FileInfo info = new FileInfo(file);
            return info.Directory;
            
        }
        
        public static void UpdateRobloxProtocolURL()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return;
            }
            Console.WriteLine("Updating protocol URL in registry...");
            if (!DoWeHaveAdmin())
            {
                //Reboot into admin.
                string exename = Process.GetCurrentProcess().MainModule.FileName;
                ProcessStartInfo restartasadmin = new ProcessStartInfo
                {
                    FileName = exename,
                    Verb = "runas",
                    Arguments = $" -protocolupdate",
                    UseShellExecute = true
                };
                Console.WriteLine("Restarting to get into administrator...");
                try
                {
                    Process.Start(restartasadmin);
                    Environment.Exit(0);
                }
                catch (Win32Exception ex)
                {
                    if (ex.NativeErrorCode == 1223)
                    {
                        Console.WriteLine("We need administrator to edit the protocol URL, Roblox will NOT work if you don't do this!");
                        Thread.Sleep(5000);
                        UpdateRobloxProtocolURL();
                    }
                }
            }
            //Main key where protocols are stored.
            RegistryKey classesKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Default);
            //Roblox Player key where the application to run is stored.
            RegistryKey robloxPlayerKey = classesKey.OpenSubKey("roblox-player").OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command", true);
            //Some browsers use this, this specifies the icon of the program, (I think).
            RegistryKey robloxPlayerIconKey = classesKey.OpenSubKey("roblox-player").OpenSubKey("DefaultIcon", true);
            //Alternates of the other two keys I already described.
            RegistryKey robloxKey = classesKey.OpenSubKey("roblox").OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command", true);
            RegistryKey robloxIconKey = classesKey.OpenSubKey("roblox").OpenSubKey("DefaultIcon", true);
            //The path of the application we want to run.
            string robloxPlayerLauncherPath = Path.Combine(robloxFolder.folderinfo.FullName, "RobloxPlayerLauncher.exe");
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
            Console.WriteLine("Completed update of protocol URL in registry!");
        }
        
        public static bool CheckRobloxRegistryInstall()
        {
            if(!SearchRegistry().Exists)
            {
                return false;
            }
            return true;
        }

        public static void RobloxFolderChoices(RobloxFolder folder)
        {
            //Console.Clear();
            if(folder.adminFolder && !DoWeHaveAdmin())
            {
                Console.WriteLine("Hey! This folder requires admin access. Are you sure you want to use this folder? (Y/N)");
                string inp = Console.ReadLine();
                if(inp.ToLower() == "n")
                {
                    RobloxFolderChoose();
                }
                RestartAsAdmin();
            }
            List<Choice> choices = new List<Choice>();
            Console.WriteLine("Current folder selected: " + folder.folderinfo.FullName);
            Console.WriteLine("Options: ");
            Choice.Add(choices,"Continue with this Folder", null);
            Choice.Add(choices, "Set as Default", new RobloxFolderDelegatestr(folder.SetAsDefault));
            Choice.Add(choices, "Delete Roblox Install", new RobloxFolderDelegatestr(folder.DeleteRobloxInstall));
            Choice.Add(choices, "Edit Registry to Set as Using Roblox Install",new RobloxFolderDelegatestr(folder.DefaultRegistryEdit));
            Choice.Add(choices, "Swap Roblox Folder Location ", new RobloxFolderDelegatestr(folder.SwapRobloxFolder));
            Choice.Add(choices, "Back", null);
            Console.WriteLine("Select an option: ");
            string choice = Console.ReadLine();
            try
            {
                int i = Convert.ToInt32(choice);
                if(i-1 == 0)
                {
                    //Continue with this folder
                    robloxFolder = folder;
                    Console.Clear();
                    return;
                }
                if(choices[i-1].choiceFunction == null)
                {
                    //Back
                    Console.Clear();
                    RobloxFolderChoose();
                } else
                {
                    string response = (string)choices[i - 1].choiceFunction.DynamicInvoke();
                    Console.Clear();
                    Console.WriteLine(response);
                    RobloxFolderChoices(folder);
                }
            } catch(Exception)
            {
                Console.Clear();
                Console.WriteLine("Invalid input, please try again.");
                RobloxFolderChoices(folder);
            }
            

        }

        public static void RobloxFolderChoose()
        {
            bool foundDefaultFolder = false;
            Console.Clear();
            List<Choice> choices = new List<Choice>();
            List<RobloxFolder> infos3 = new List<RobloxFolder>();
            RobloxFolder recRBXFolder = new RobloxFolder(new DirectoryInfo("thiswillneverexistgetfuckedlmfaoyesyesyes"));
            DirectoryInfo recommendedRobloxFolder = SearchRegistry();
            if(recommendedRobloxFolder == null)
            {
                recommendedRobloxFolder = new DirectoryInfo("thiswillneverexistgetfuckedlmfaoyesyesyes");
            }
            if (recommendedRobloxFolder.Exists)
            {
                    recRBXFolder = new RobloxFolder(recommendedRobloxFolder);
                    recRBXFolder.Verifier();
            }
            if (recommendedRobloxFolder.Exists)
            {
                foreach (RobloxFolder info in robloxFolders)
                {
                    if (info.folderinfo.FullName == recommendedRobloxFolder.FullName)
                    {
                    infos3.Add(info);
                    }
                }
            }
            

            Console.WriteLine("We detected multiple roblox folders, please select the one you want.");
            Console.WriteLine("Roblox Folder Selection");
            if (recommendedRobloxFolder.Exists && !recRBXFolder.defaultFolder)
            {
                Choice.Add(choices, recommendedRobloxFolder.FullName + " (Recommended, found in registry as using)", new RobloxFolderDelegate(RobloxFolderChoices));
            } else if(recommendedRobloxFolder.Exists && recRBXFolder.defaultFolder)
            {
                foundDefaultFolder = true;
                Choice.Add(choices, recommendedRobloxFolder.FullName + " (Recommended, found in registry as using) (RBXTools Default)", new RobloxFolderDelegate(RobloxFolderChoices));
            }
            foreach (RobloxFolder info in robloxFolders)
            {
                if (info.folderinfo.FullName != recommendedRobloxFolder.FullName)
                {
                    infos3.Add(info);
                }  
            }
            robloxFolders = new List<RobloxFolder>(infos3);
            infos3.Clear();
            foreach (RobloxFolder info in robloxFolders)
            {
                if (info.folderinfo.FullName != recommendedRobloxFolder.FullName)
                {
                    if(!info.defaultFolder)
                    {
                        Choice.Add(choices, info.folderinfo.FullName, new RobloxFolderDelegate(RobloxFolderChoices));
                    } else
                    {
                        foundDefaultFolder = true;
                        Choice.Add(choices, info.folderinfo.FullName + " (RBXTools Default)", new RobloxFolderDelegate(RobloxFolderChoices));
                    }
                }
            }
            if(!foundDefaultFolder && Config.CheckIfModHasBeenAddedAlready("RobloxFolderDefaultSetting"))
            {
                RobloxFolder def = new RobloxFolder(new DirectoryInfo(Config.ReturnPathsOfMod("RobloxFolderDefaultSetting")[0]));
                def.Verifier();
                Choice.Add(choices, def.folderinfo.FullName + " (RBXTools Default)", new RobloxFolderDelegate(RobloxFolderChoices));
            }
            Choice.Add(choices, "Enter in Folder Path", new RobloxFolderDelegate(RobloxFolderChoices));
            int folderpathi = choices.Count;
            Choice.Add(choices, "Rescan Folders", new Program.ChoiceDelegate(AutoFindRobloxFolder));
            int rescanfoldersi = choices.Count;
            int cleardefaulti = -1;
            if (foundDefaultFolder)
            {
                Choice.Add(choices, "Clear Default RBXTools Folder", null);
                cleardefaulti = choices.Count;
            }
            try
            {
                string choice = Console.ReadLine();
                int i = Convert.ToInt32(choice);
                if (i == folderpathi)
                {
                    InputRobloxFolder();
                    Console.Clear();
                    robloxFolder.Verifier();
                    choices[i - 1].choiceFunction.DynamicInvoke(robloxFolder);
                    Console.WriteLine("Done managing folders? Press enter to go back.");
                }
                else if (i == rescanfoldersi)
                {
                    robloxFolders.Clear();
                    choices[i - 1].choiceFunction.DynamicInvoke();
                    if (robloxFolders.Count == 1)
                    {
                        RobloxFolderChoose();
                    }
                }
                else if (i == cleardefaulti) 
                {
                    Console.WriteLine("Removing default...");
                    Config.RemoveMod("RobloxFolderDefaultSetting");
                    Console.WriteLine("Complete!");
                    AutoFindRobloxFolder();
                    if(robloxFolders.Count == 1)
                    {
                        RobloxFolderChoose();
                    }
                }
                else
                {
                    Console.Clear();
                    robloxFolders[i - 1].Verifier();
                    choices[i - 1].choiceFunction.DynamicInvoke(robloxFolders[i - 1]);

                }
            }
            catch (Exception)
            {
                Console.Clear();
                Console.WriteLine("Invalid input, lets try that again.");
                RobloxFolderChoose();
            }
        }

        public static void RestartAsAdmin()
        {
            string exename = Process.GetCurrentProcess().MainModule.FileName;
            ProcessStartInfo restartasadmin = new ProcessStartInfo
            {
                FileName = exename,
                Verb = "runas",
                UseShellExecute = true
            };
            Console.WriteLine("Restarting to get into administrator...");
            try
            {
                Process.Start(restartasadmin);
                Environment.Exit(0);
            }
            catch (Win32Exception e)
            {
                if (e.NativeErrorCode == 1223)
                {
                    Console.WriteLine("We need administrator to get into your Roblox Install Location.");
                    Thread.Sleep(5000);
                    Environment.Exit(-1);
                }
            }
        }

        public static string GetLatestVersion()
        {
            WebClient client = new WebClient();
            string versionstring = client.DownloadString("https://versioncompatibility.api.roblox.com/GetCurrentClientVersionUpload/?apiKey=76e5a40c-3ae1-4028-9f10-7c62520bd94f&binaryType=WindowsPlayer");
            int index = 0;
            while (true)
            {
                index = versionstring.IndexOf('"', index);
                if (index == -1)
                {
                    break;
                }
                versionstring = versionstring.Remove(index, 1);
            }
            return versionstring;
        }

        public static void DownloadRobloxVersion(DirectoryInfo robloxPath)
        {
            WebClient client = new WebClient();
            string jsonCDNs = client.DownloadString("https://roblox.com/install/GetInstallerCdns.ashx");
            string cdn = "";
            foreach (JToken token in JObject.Parse(jsonCDNs).Children())
            {
                if (token.First().ToString() == 100.ToString())
                {
                    cdn = token.ToObject<JProperty>().Name;
                }
            }
            Uri robloxPackageManifest = new Uri($"https://{cdn}/{GetLatestVersion()}-rbxPkgManifest.txt");
            string listofZips = client.DownloadString(robloxPackageManifest);
            string[] arrayofZips = listofZips.Split('\n');
            for (int i = 1; i < arrayofZips.Length; i += 4)
            {
                //The end, which doesnt include anything we need, so we break. Also if we don't, it throws an exception.
                if (i == 77)
                {
                    break;
                }
                //Remove \r from the file name. (Leaving \r will screw up our download URLs.
                arrayofZips[i] = arrayofZips[i].Remove(arrayofZips[i].IndexOf('\r'));
                //Create the URL that we download the zip/exe from.
                Uri downloadURL = new Uri($"https://{cdn}/{GetLatestVersion()}-{arrayofZips[i]}");
                //If its a root folder:
                if (arrayofZips[i].Split('-').Length == 1)
                {
                    //If its not the RobloxApp, (belongs in the root folder and NO other.)
                    if (i != 1)
                    {
                        //If it's a zip: (The only other option is RobloxPlayerLauncher.exe, and we don't wanna screw that up.)
                        if (Path.GetExtension(arrayofZips[i]) == ".zip")
                        {
                            //Split it and grab the directory name. If this directory exists, go ahead and download it, otherwise, create it.
                            if (!Directory.Exists(Path.Combine(robloxPath.FullName, arrayofZips[i].Split(".zip")[0])))
                            {
                                Directory.CreateDirectory(Path.Combine(robloxPath.FullName, arrayofZips[i].Split(".zip")[0]));
                            }
                            Console.WriteLine($"Downloading: {arrayofZips[i]}...");
                            client.DownloadFile(downloadURL, Path.Combine(robloxPath.FullName, arrayofZips[i]));
                            Console.WriteLine($"Download complete for: {arrayofZips[i]}!");
                            try
                            {
                                ZipFile.ExtractToDirectory(Path.Combine(Path.Combine(robloxPath.FullName, arrayofZips[i])), Path.Combine(robloxPath.FullName, Path.GetFileNameWithoutExtension(Path.Combine(robloxPath.FullName, arrayofZips[i]))));
                            }
                            catch (IOException)
                            {
                                using (var ziparchive = ZipFile.OpenRead(Path.Combine(robloxPath.FullName, arrayofZips[i])))
                                {
                                    foreach (var entry in ziparchive.Entries)
                                    {
                                        if (Path.IsPathRooted(entry.FullName))
                                        {
                                            continue;
                                        }
                                        string fileName = string.Format("{0}/{1}", Path.Combine(robloxPath.FullName, Path.GetFileNameWithoutExtension(arrayofZips[i])), entry.FullName).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                                        FileInfo info = new FileInfo(fileName);
                                        if (!info.Directory.Exists)
                                            Directory.CreateDirectory(info.Directory.FullName);

                                        try
                                        {
                                            entry.ExtractToFile(info.FullName);
                                        }
                                        catch (Exception e)
                                        {
                                            Debug.WriteLine("EXCEPTION: [" + entry.FullName + "]");
                                            Debug.Write(e);
                                        }
                                    }
                                }
                            }
                            Console.WriteLine("Deleting: " + arrayofZips[i] + "...");
                            File.Delete(Path.Combine(robloxPath.FullName, arrayofZips[i]));
                            Console.WriteLine("Deleted: " + arrayofZips[i] + "!");

                        }
                        else
                        {
                            //This is RobloxPlayerLauncher.exe, go ahead and download it directly to the root folder.
                            Console.WriteLine($"Downloading: {arrayofZips[i]}...");
                            client.DownloadFile(downloadURL, Path.Combine(robloxPath.FullName, arrayofZips[i]));
                            Console.WriteLine($"Download complete for: {arrayofZips[i]}!");
                        }
                    }
                    else
                    {
                        //If it is the RobloxApp, save it directly in the folder.
                        Console.WriteLine($"Downloading: {arrayofZips[i]}...");
                        client.DownloadFile(downloadURL, Path.Combine(robloxPath.FullName, arrayofZips[i]));
                        Console.WriteLine($"Download complete for: {arrayofZips[i]}!");
                        Console.WriteLine($"Extracting: {arrayofZips[i]}...");
                        ZipFile.ExtractToDirectory(Path.Combine(robloxPath.FullName, arrayofZips[i]), robloxPath.FullName);
                        Console.WriteLine($"Extraction complete for: {arrayofZips[i]}!");
                        Console.WriteLine("Deleting: " + arrayofZips[i] + "...");
                        File.Delete(Path.Combine(robloxPath.FullName, arrayofZips[i]));
                        Console.WriteLine("Deleted: " + arrayofZips[i] + "!");

                    }
                }
                else if (arrayofZips[i].Split('-').Length == 2)
                {
                    //If it's not the roblox app.
                    if (i != 1)
                    {
                        //If the directory does not exist, then run a check for if it it is something.
                        if (!Directory.Exists(Path.Combine(robloxPath.FullName, arrayofZips[i].Split(".zip")[0])))
                        {
                            if (arrayofZips[i].Contains("extracontent"))
                            {
                                //If it's extracontent, we need to capitalize it to ExtraContent.
                                char[] array = arrayofZips[i].Split('-')[0].ToCharArray();
                                array[0] = array[0].ToString().ToUpper().ToCharArray()[0];
                                array[5] = array[5].ToString().ToUpper().ToCharArray()[0];
                                Directory.CreateDirectory(Path.Combine(robloxPath.FullName, new string(array)));
                            }
                            else if (arrayofZips[i].Contains("terrain") || arrayofZips[i].Contains("textures3"))
                            {
                                //If it's either of these, they need to be in PlatformContent.
                                Directory.CreateDirectory(Path.Combine(robloxPath.FullName, "PlatformContent"));
                                Directory.CreateDirectory(Path.Combine(robloxPath.FullName, "PlatformContent", "pc"));
                            }
                            else
                            {
                                //Not a special folder, just make it.
                                Directory.CreateDirectory(Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0]));
                            }
                        }
                        Console.WriteLine($"Downloading: {arrayofZips[i]}...");
                        if (arrayofZips[i].Contains("terrain") || arrayofZips[i].Contains("textures3"))
                        {
                            //If it's either of these, we need to download them into PlatformContent, the folder that we created earlier.
                            client.DownloadFile(downloadURL, Path.Combine(robloxPath.FullName, "PlatformContent", "pc", arrayofZips[i].Split('-')[1].Remove(Path.GetFileNameWithoutExtension(arrayofZips[i].Split('-')[1]).Length - 1, 1)));

                        }
                        else
                        {
                            //If it's not a PlatformContent zip, then we need to check if its in other special folders.
                            if (arrayofZips[i].Contains("extracontent"))
                            {
                                //If it's a extracontent zip, capitalize it, and then download the zip to there.
                                char[] array = arrayofZips[i].Split('-')[0].ToCharArray();
                                array[0] = array[0].ToString().ToUpper().ToCharArray()[0];
                                array[5] = array[5].ToString().ToUpper().ToCharArray()[0];
                                client.DownloadFile(downloadURL, Path.Combine(robloxPath.FullName, new string(array), arrayofZips[i].Split('-')[1]));
                            }
                            else if (arrayofZips[i].Contains("textures2"))
                            {
                                //If this is textures2, this means it needs to go into content/textures. So, remove the number at the end, and save the zip.
                                client.DownloadFile(downloadURL, Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0], arrayofZips[i].Split('-')[1].Remove(Path.GetFileNameWithoutExtension(arrayofZips[i].Split('-')[1]).Length - 1, 1)));
                            }
                            else
                            {
                                //Not a special zip, just download it normally.
                                client.DownloadFile(downloadURL, Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0], arrayofZips[i].Split('-')[1]));
                            }

                        }
                        Console.WriteLine($"Download complete for: {arrayofZips[i]}!");
                        Console.WriteLine($"Extracting: {arrayofZips[i]}...");
                        if (arrayofZips[i].Contains("terrain") || arrayofZips[i].Contains("textures3"))
                        {
                            //If it's a PlatformContent zip, we need to extract it to the PlatformContent folder.
                            using (var ziparchive = ZipFile.OpenRead(Path.Combine(robloxPath.FullName, "PlatformContent", "pc", arrayofZips[i].Split('-')[1].Remove(Path.GetFileNameWithoutExtension(arrayofZips[i].Split('-')[1]).Length - 1, 1))))
                            {
                                foreach (var entry in ziparchive.Entries)
                                {
                                    //If it's rooted, we need to continue, because it will throw an exception if we don't.
                                    if (Path.IsPathRooted(entry.FullName))
                                    {
                                        continue;
                                    }
                                    string fileName = "";
                                    if (arrayofZips[i].Contains("textures3"))
                                    {
                                        //Textures3, need to remove the number at the end, we need this if statement, because otherwise our terrain folder might be called "terrai".
                                        fileName = string.Format("{0}/{1}", Path.Combine(robloxPath.FullName, "PlatformContent", "pc", Path.GetFileNameWithoutExtension(arrayofZips[i].Split('-')[1]).Remove(Path.GetFileNameWithoutExtension(arrayofZips[i].Split('-')[1]).Length - 1, 1)), entry.FullName).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                                    }
                                    else
                                    {
                                        //Terrain, basically just the other line, without the removing number code.
                                        fileName = string.Format("{0}/{1}", Path.Combine(robloxPath.FullName, "PlatformContent", "pc", Path.GetFileNameWithoutExtension(arrayofZips[i].Split('-')[1])), entry.FullName).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                                    }

                                    FileInfo info = new FileInfo(fileName);
                                    if (!info.Directory.Exists)
                                        Directory.CreateDirectory(info.Directory.FullName);

                                    try
                                    {
                                        entry.ExtractToFile(info.FullName);
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.WriteLine("EXCEPION: [" + entry.FullName + "]");
                                        Debug.Write(e);
                                    }
                                }
                            }
                            Console.WriteLine("Deleting: " + arrayofZips[i] + "...");
                            File.Delete(Path.Combine(robloxPath.FullName, Path.Combine(robloxPath.FullName, "PlatformContent", "pc", arrayofZips[i].Split('-')[1].Remove(Path.GetFileNameWithoutExtension(arrayofZips[i].Split('-')[1]).Length - 1, 1))));
                            Console.WriteLine("Deleted: " + arrayofZips[i] + "!");
                        }
                        else if (arrayofZips[i].Contains("textures2"))
                        {
                            //This special using statement, just removes the number from the zip filename, so we can access it without a System.IO.FileNotFound exception.
                            using (var ziparchive = ZipFile.OpenRead(Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0], arrayofZips[i].Split('-')[1].Remove(Path.GetFileNameWithoutExtension(arrayofZips[i].Split('-')[1]).Length - 1, 1))))
                            {
                                foreach (var entry in ziparchive.Entries)
                                {
                                    if (Path.IsPathRooted(entry.FullName))
                                    {
                                        continue;
                                    }
                                    string fileName = "";
                                    fileName = string.Format("{0}/{1}", Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0], Path.GetFileNameWithoutExtension(arrayofZips[i].Split('-')[1].Remove(Path.GetFileNameWithoutExtension(arrayofZips[i].Split('-')[1]).Length - 1, 1))), entry.FullName).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                                    FileInfo info = new FileInfo(fileName);
                                    if (!info.Directory.Exists)
                                        Directory.CreateDirectory(info.Directory.FullName);

                                    try
                                    {
                                        entry.ExtractToFile(info.FullName);
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.WriteLine("EXCEPION: [" + entry.FullName + "]");
                                        Debug.Write(e);
                                    }
                                }
                            }
                            Console.WriteLine("Deleting: " + arrayofZips[i] + "...");
                            File.Delete(Path.Combine(robloxPath.FullName, Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0], arrayofZips[i].Split('-')[1].Remove(Path.GetFileNameWithoutExtension(arrayofZips[i].Split('-')[1]).Length - 1, 1))));
                            Console.WriteLine("Deleted: " + arrayofZips[i] + "!");
                        }
                        else
                        {
                            char[] chararra = null;
                            if (arrayofZips[i].Contains("extracontent"))
                            {
                                //If this is a extracontent zip, we need to make it ExtraContent, and make the directory that. Otherwise, exception.
                                chararra = arrayofZips[i].Split('-')[0].ToCharArray();
                                chararra[0] = chararra[0].ToString().ToUpper().ToCharArray()[0];
                                chararra[5] = chararra[5].ToString().ToUpper().ToCharArray()[0];
                            }
                            else
                            {
                                chararra = arrayofZips[i].Split('-')[0].ToCharArray();
                            }
                            using (var ziparchive = ZipFile.OpenRead(Path.Combine(robloxPath.FullName, new string(chararra), arrayofZips[i].Split('-')[1])))
                            {
                                string fileName = "";
                                foreach (var entry in ziparchive.Entries)
                                {
                                    if (Path.IsPathRooted(entry.FullName))
                                    {
                                        continue;
                                    }
                                    if (arrayofZips[i].Contains("extracontent"))
                                    {
                                        //If extracontent, make the folder ExtraContent.
                                        char[] chararray = arrayofZips[i].Split('-')[0].ToCharArray();
                                        chararray[0] = chararray[0].ToString().ToUpper().ToCharArray()[0];
                                        chararray[5] = chararray[5].ToString().ToUpper().ToCharArray()[0];
                                        if (arrayofZips[i].Contains("luapackages"))
                                        {
                                            //If we are already in extracontent, and this is extracontent-luapackages.zip, make the folder LuaPackages.
                                            char[] entrys = Path.GetFileNameWithoutExtension(arrayofZips[i].Split('-')[1]).ToCharArray();
                                            entrys[0] = entrys[0].ToString().ToUpper().ToCharArray()[0];
                                            entrys[3] = entrys[3].ToString().ToUpper().ToCharArray()[0];
                                            fileName = string.Format("{0}/{1}", Path.Combine(robloxPath.FullName, new string(chararray), new string(entrys)), entry.FullName).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                                        }
                                        else
                                        {
                                            //Any other ExtraContent zips, go in here.
                                            fileName = string.Format("{0}/{1}", Path.Combine(robloxPath.FullName, new string(chararray), Path.GetFileNameWithoutExtension(arrayofZips[i].Split('-')[1])), entry.FullName).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                                        }
                                    }
                                    else
                                    {
                                        //Normal files that aren't in special folders.
                                        fileName = string.Format("{0}/{1}", Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0], Path.GetFileNameWithoutExtension(arrayofZips[i].Split('-')[1])), entry.FullName).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                                    }

                                    FileInfo info = new FileInfo(fileName);
                                    if (!info.Directory.Exists)
                                        Directory.CreateDirectory(info.Directory.FullName);

                                    try
                                    {
                                        entry.ExtractToFile(info.FullName);
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.WriteLine("EXCEPION: [" + entry.FullName + "]");
                                        Debug.Write(e);
                                    }
                                }
                            }
                            Console.WriteLine("Deleting: " + arrayofZips[i] + "...");
                            File.Delete(Path.Combine(robloxPath.FullName, Path.Combine(robloxPath.FullName, new string(chararra), arrayofZips[i].Split('-')[1])));
                            Console.WriteLine("Deleted: " + arrayofZips[i] + "!");
                        }

                        Console.WriteLine($"Extraction complete for: {arrayofZips[i]}!");
                    }
                }
                else
                {

                    if (!Directory.Exists(Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0])))
                    {
                        Directory.CreateDirectory(Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0]));
                    }
                    if (!Directory.Exists(Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0], arrayofZips[i].Split('-')[1])))
                    {
                        Directory.CreateDirectory(Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0], arrayofZips[i].Split('-')[1]));
                    }
                    Console.WriteLine($"Downloading: {arrayofZips[i]}...");
                    client.DownloadFile(downloadURL, Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0], arrayofZips[i].Split('-')[1], arrayofZips[i].Split('-')[2]));
                    Console.WriteLine($"Download complete for: {arrayofZips[i]}!");
                    Console.WriteLine($"Extracting: {arrayofZips[i]}...");
                    try
                    {
                        ZipFile.ExtractToDirectory(Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0], arrayofZips[i].Split('-')[1], arrayofZips[i].Split('-')[2]), Path.Combine(robloxPath.FullName, Path.GetFileNameWithoutExtension(Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0], arrayofZips[i].Split('-')[1], arrayofZips[i].Split('-')[2]))));
                    }
                    catch (IOException)
                    {
                        using (var ziparchive = ZipFile.OpenRead(Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0], arrayofZips[i].Split('-')[1], arrayofZips[i].Split('-')[2])))
                        {
                            foreach (var entry in ziparchive.Entries)
                            {
                                if (Path.IsPathRooted(entry.FullName))
                                {
                                    continue;
                                }
                                string fileName = string.Format("{0}/{1}", Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0], arrayofZips[i].Split('-')[1], Path.GetFileNameWithoutExtension(arrayofZips[i].Split('-')[2])), entry.FullName).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                                FileInfo info = new FileInfo(fileName);
                                if (!info.Directory.Exists)
                                    Directory.CreateDirectory(info.Directory.FullName);

                                try
                                {
                                    entry.ExtractToFile(info.FullName);
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine("EXCEPION: [" + entry.FullName + "]");
                                    Debug.Write(e);
                                }
                            }
                        }
                    }

                    Console.WriteLine($"Extraction complete for: {arrayofZips[i]}!");
                    Console.WriteLine("Deleting: " + arrayofZips[i] + "...");
                    File.Delete(Path.Combine(robloxPath.FullName, arrayofZips[i].Split('-')[0], arrayofZips[i].Split('-')[1], arrayofZips[i].Split('-')[2]));
                    Console.WriteLine("Deleted: " + arrayofZips[i] + "!");
                }

            }
            Console.WriteLine("Writing: AppSettings.xml...");
            using (XmlTextWriter writer = new XmlTextWriter(Path.Combine(robloxPath.FullName, "AppSettings.xml"), System.Text.Encoding.UTF8))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Settings");
                writer.WriteElementString("ContentFolder", "content");
                writer.WriteElementString("BaseUrl", "http://www.roblox.com");
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }
            Console.WriteLine("Wrote: AppSettings.xml!");
            Console.WriteLine("Completed Roblox installation.");
        }
    }
}
