using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;

namespace RBXTools
{
    public static class Roblox
    {
        [Serializable]
        public class RobloxException : System.Exception
        {
            public RobloxException(string message)
            : base(message)
            {

            }
        }
        public static DirectoryInfo robloxFolder = null;
        private static List<char> invalidCharsDetected = new List<char>();
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
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }
        public static void AutoFindRobloxFolder()
        {
            string robloxVersPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "Versions");
            string robloxVers2Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Roblox", "Versions");
            DirectoryInfo[] infos = null;
            DirectoryInfo[] infos2 = null;
            if (Directory.Exists(robloxVersPath))
            {
                infos = new DirectoryInfo(robloxVersPath).GetDirectories();
            }

            if(Directory.Exists(robloxVers2Path))
            {
                infos2 = new DirectoryInfo(robloxVers2Path).GetDirectories();
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
                            robloxFolder = info;
                            break;
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


                    if (found)
                    {
                        break;
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
                    if (found)
                    {
                        break;
                    }
                    foreach (FileInfo info2 in info.GetFiles())
                    {
                        if (info2.Name == "RobloxPlayerBeta.exe")
                        {
                            if (!DoWeHaveAdmin())
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
                                    continue;
                                }
                                catch (Win32Exception e)
                                {
                                    if (e.NativeErrorCode == 1223)
                                    {
                                        Console.WriteLine("We need administrator to get into your Roblox Install Location.");
                                        Thread.Sleep(5000);
                                        Environment.Exit(-1);
                                    }
                                    continue;
                                }
                            }
                            found = true;
                            robloxFolder = info;
                            break;
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

            if (robloxFolder == null)
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
            if(String.IsNullOrWhiteSpace(path))
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
            robloxFolder = new DirectoryInfo(path);
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
    }
}
