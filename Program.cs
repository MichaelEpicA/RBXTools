using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using SevenZip;

namespace RBXTools
{
    class Program
    {
		public static FileInfo info;
		private delegate void ChoiceDelegate();
		private delegate bool ChoiceDecisionDelegate();
		private static bool updateAvailable;
		static List<Choice> choices = new List<Choice>();
		private static void Main(string[] args)
		{
			FileVersionInfo info2 = FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName);
			
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
				Console.Title = $"RBXTools v{string.Join(",", Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.').Where((source, index) => index != 3).ToArray())} [Linux]";
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
				Console.Title = $"RBXTools v{string.Join(",", Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.').Where((source, index) => index != 3).ToArray())} [MacOS]";
			}
			else if (Roblox.DoWeHaveAdmin())
			{
				Console.Title = $"RBXTools v{info2.FileVersion} [Administrator]";
			}
			else
			{
				Console.Title = $"RBXTools v{info2.FileVersion}";
			}
			if (args.Length != 0 && args[0] == "-cleanupreboot")
			{
				CleanupRobloxFolders(true);
				Environment.Exit(0);
			}
			if(args.Length != 0 && args[0] == "-updatereboot")
            {
				//Update of this program.
				File.Delete("RBXTools.exe");
				if(Config.CheckIfModHasBeenAddedAlready("ReinstallLauncherMod"))
                {
					//Launcher update, reinject.
					Console.WriteLine("Reinjecting launcher...");
					Roblox.FindRobloxFolder();
					info = new FileInfo(Path.Combine(Roblox.robloxFolder.FullName, "repairlauncher-roblox.backup"));
					SetupAutoReapply();
					Config.RemoveMod("ReinstallLauncherMod");
					Console.WriteLine("Completed reinject of launcher.");
				}
				File.Move("RBXTools_new.exe", "RBXTools.exe");
				//Reboot the program.
				Process.Start("RBXTools.exe");
				Environment.Exit(0);
            }
			updateAvailable = Updater.CheckForUpdates();
			if(!Config.configFilePath.Exists)
            {
				//Directory needs to be created, otherwise we will have a System.IO.DirectoryNotFound issue.
				try
                {
					Config.configFilePath.Directory.Create();
					Config.configFilePath.Create().Close();
				} catch(Exception e)
                {
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("An exception occured: " + e + "If this keeps happening, please leave an issue on https://github.com/MichaelEpicA/RBXTools/issues, so that we can help you. Press enter to continue.");
					Console.ReadLine();
					Console.ResetColor();
                }
				
            }
			Console.Clear();
			Roblox.FindRobloxFolder();
			info = new FileInfo(Path.Combine(Roblox.robloxFolder.FullName, "repairlauncher-roblox.backup"));
			Console.WriteLine($"RBXTools v{info2.FileVersion}");
			Thread.Sleep(2000);
			Console.WriteLine("Made by MichaelEpicA");
			Thread.Sleep(1000);
			Welcome();
			Console.ReadLine();
		}

		public static void RestoreOldNoobSoundEffect()
		{
			Console.WriteLine("Do you want to specify what death effect you would like to use? (Y/N)");
			string text = Console.ReadLine();
			string custompath = "default";
			if (text.ToLower().Contains("y"))
			{
				Console.WriteLine("Please input OGG file path to your custom Death Effect.");
				string path = Console.ReadLine();
				if (!Roblox.RemoveInvalidCharactersRef(ref path, true, false))
				{
					RestoreOldNoobSoundEffect();
				}
				if (!new FileInfo(path).Exists || Path.GetExtension(path) != ".ogg")
				{
					Console.Clear();
					Console.WriteLine("Invalid path, try again.");
					RestoreOldNoobSoundEffect();
					return;
				}
				custompath = path;
			}
			Console.Clear();
			Console.WriteLine("Restoring Old Death Sound Effect...");
			Console.WriteLine("Searching for roblox directory...");
			Roblox.FindRobloxFolder();
			Console.WriteLine("Deleting Death Sound Effect...");
			FileInfo info = new FileInfo(Path.Combine(Roblox.robloxFolder.FullName, "content", "sounds", "ouch.ogg"));
			info.Delete();
			Console.WriteLine("Replacing Death Sound Effect...");
			if (custompath != "default")
			{
				File.Copy(custompath, info.FullName);
			}
			else
			{
				ExtractResource(Path.Combine(Roblox.robloxFolder.FullName, "content", "sounds"), "ouch.ogg");
			}
			Config.WriteMod("DeathSFXMod", custompath);
			Console.WriteLine("Completed! Test it out in Roblox. Press enter to go back.");
			Console.ReadLine();
			Console.Clear();
			Welcome();
		}

		public static void ExtractResource(string directoryPath, string resourceName)
		{
			string resourcePath = "RBXTools.Resources.";
			resourcePath += resourceName;
			Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
			byte[] buff = new byte[resourceStream.Length];
			resourceStream.Read(buff, 0, (int)resourceStream.Length);
			resourceStream.Close();
			File.WriteAllBytes(Path.Combine(directoryPath, resourceName), buff);
		}

		public static void Welcome()
		{
			choices.Clear();
			if(updateAvailable)
            {
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("An update is available! Press 7 to download and install the new version!");
				Console.ResetColor();
            }
			Console.WriteLine("Welcome! What would you like to do?");
			Choice.Add(choices, "Restore Old Noob Sound Effect", new ChoiceDelegate(RestoreOldNoobSoundEffect));
			if (new FileInfo(info.FullName).Exists)
			{
				Choice.Add(choices, "Restore Original Launcher", new ChoiceDelegate(RestoreOriginalLauncher));
			}
			else
			{
				Choice.Add(choices, "Auto Reapply Changes After Roblox Update", new ChoiceDelegate(SetupAutoReapply));
			}
			Choice.Add(choices, "Cleanup Roblox Folders", new ChoiceDelegate(CleanupDelegateHandler));
			Choice.Add(choices, "Restore Old (2014) Cursors", new ChoiceDelegate(RestoreOldMouseCursors), new ChoiceDecisionDelegate(CheckIfFilesExist));
			Choice.Add(choices, "Backup Roblox Folder", new ChoiceDelegate(BackupRobloxFolder));
			Choice.Add(choices, "Replace Moon And Sun Textures", new ChoiceDelegate(ReplaceMoonAndSun), new ChoiceDecisionDelegate(CheckConfigFileForMod));
			if(updateAvailable)
            {
				Choice.Add(choices, "Update to New Version", new ChoiceDelegate(Updater.UpdateAndRestart));
            }
			Console.WriteLine("(Input nothing if you want to exit.)");
			Console.Write("Input a choice number: ");
			int number = 0;
			try
			{
				string value = Console.ReadLine();
				if (string.IsNullOrWhiteSpace(value))
				{
					Console.WriteLine("Bye!");
					Environment.Exit(0);
				}
				number = Convert.ToInt32(value);
			}
			catch (Exception)
			{
				Console.Clear();
				Console.WriteLine("That was an incorrect entry, try again!");
				Welcome();
			}
			try
			{
				if (choices[number - 1] != null)
				{
					choices[number - 1].Run();
				}
			} catch(ArgumentOutOfRangeException)
            {
				//Put in a number higher than our actual choice count.
				Console.Clear();
				Console.WriteLine("That was an incorrect entry, try again!");
				Welcome();
			}
			catch (Exception e)
			{
				Console.Clear();
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("PLEASE REPORT THIS TO https://github.com/MichaelEpicA/RBXTools/issues! An exception occured: " + e);
				Console.ResetColor();
				Welcome();
			}
		}

        private static bool CheckConfigFileForMod()
        {
           if(Config.CheckIfModHasBeenAddedAlready("ReplaceMoonAndSunMod"))
            {
				Choice.Add(choices, "Restore Moon And Sun to Originals", new ChoiceDelegate(RestoreMoonAndSun));
				return true;
            } else
            {
				return false;
            }
        }

        private static void RestoreMoonAndSun()
        {
			Console.WriteLine("Restoring moon and sun...");
			DirectoryInfo MoonAndSunDirectory = new DirectoryInfo(Path.Combine(Roblox.robloxFolder.FullName, "content", "sky"));
			FileInfo moon = new FileInfo(Path.Combine(MoonAndSunDirectory.FullName, "moon.jpg"));
			FileInfo sun = new FileInfo(Path.Combine(MoonAndSunDirectory.FullName, "sun.jpg"));
			FileInfo moonb = new FileInfo(Path.Combine(MoonAndSunDirectory.FullName, "moonb.jpg"));
			FileInfo sunb = new FileInfo(Path.Combine(MoonAndSunDirectory.FullName, "sunb.jpg"));
			Console.WriteLine("Deleting modded moon and sun...");
			moon.Delete();
			sun.Delete();
			Console.WriteLine("Restoring backup of moon and sun...");
			moonb.CopyTo(moon.FullName);
			moonb.Delete();
			sunb.CopyTo(sun.FullName);
			sunb.Delete();
			Config.RemoveMod("ReplaceMoonAndSunMod");
			Console.WriteLine("Restored backup of moon and sun! Press enter to go back.");

		}

		private static void ReplaceMoonAndSun()
		{
			Console.WriteLine("Replacing moon and sun...");
			DirectoryInfo MoonAndSunDirectory = new DirectoryInfo(Path.Combine(Roblox.robloxFolder.FullName, "content", "sky"));
			FileInfo moon = new FileInfo(Path.Combine(MoonAndSunDirectory.FullName, "moon.jpg"));
			FileInfo sun = new FileInfo(Path.Combine(MoonAndSunDirectory.FullName, "sun.jpg"));
			FileInfo moonb = new FileInfo(Path.Combine(MoonAndSunDirectory.FullName, "moonb.jpg"));
			FileInfo sunb = new FileInfo(Path.Combine(MoonAndSunDirectory.FullName, "sunb.jpg"));
			Console.WriteLine("Please enter path you want to use to replace the moon.");
			string moonpath = Console.ReadLine();
			if (!Roblox.RemoveInvalidCharactersRef(ref moonpath, true, false))
			{
				//Reenter the path.
				if(Path.GetExtension(moonpath) != ".jpg")
                {
					Console.WriteLine("In order to replace the texture correctly, we need to have it be a jpg. Try again.");
					ReplaceMoonAndSun();
                }
				Console.WriteLine("Incorrect path. Please reenter it.");
				ReplaceMoonAndSun();
			}
			Console.WriteLine("Please enter path you want to use to replace the sun.");
			string sunpath = Console.ReadLine();
			if(!Roblox.RemoveInvalidCharactersRef(ref sunpath, true, false))
            {
				//Reenter the path.
				if (Path.GetExtension(sunpath) != ".jpg")
				{
					Console.WriteLine("In order to replace the texture correctly, we need to have it be a jpg. Try again.");
					ReplaceMoonAndSun();
				}
				Console.WriteLine("Incorrect path. Please reenter it.");
				ReplaceMoonAndSun();
            }
			Console.WriteLine("Backing up moon and sun...");
			if(moonb.Exists)
            {
				moonb.Delete();
            }
			if(sunb.Exists)
            {
				sunb.Delete();
            }
			moon.CopyTo(moonb.FullName);
			sun.CopyTo(sunb.FullName);
			Console.WriteLine("Backed up moon and sun.");
			Console.WriteLine("Replacing moon and sun...");
			moon.Delete();
			File.Copy(moonpath, moon.FullName);
			sun.Delete();
			File.Copy(sunpath, sun.FullName);
			Config.WriteMod("ReplaceMoonAndSunMod", moonpath, sunpath);
			Console.WriteLine("Replaced Moon and Sun, press enter to go back.");
		}

        public static void SetupAutoReapply()
		{
			Console.WriteLine("Backing up launcher...");
			Roblox.FindRobloxFolder();
			FileInfo launcherPath = new FileInfo(Path.Combine(Roblox.robloxFolder.FullName, "RobloxPlayerLauncher.exe"));
			launcherPath.CopyTo(info.FullName, true);
			launcherPath.Delete();
			Console.WriteLine("Back up complete!");
			Console.WriteLine("Replacing launcher...");
			ExtractResource(launcherPath.Directory.FullName, launcherPath.Name);
			Roblox.Cleanup();
			Console.WriteLine("Replaced launcher! Press enter to go back.");
		}
		public static void RestoreOriginalLauncher()
		{
			Console.WriteLine("Deleting modded launcher...");
			Roblox.FindRobloxFolder();
			FileInfo launcherPath = new FileInfo(Path.Combine(Roblox.robloxFolder.FullName, "RobloxPlayerLauncher.exe"));
			if (launcherPath.Exists)
			{
				launcherPath.Delete();
			}
			Console.WriteLine("Modded launcher deleted.");
			Console.WriteLine("Replacing with original launcher...");
			info.CopyTo(launcherPath.FullName);
			info.Delete();
			Console.WriteLine("Replaced with original launcher.");
			Console.WriteLine("Completed! (Auto reapply has been disabled.) Press enter to go back.");
		}

		public static void CleanupRobloxFolders(bool ranwithcleanupreboot = false)
		{
			if (!Roblox.DoWeHaveAdmin())
			{
				Console.WriteLine("In order to clean up all folders, we will need admin. Are you ok with this? (Y/N)");
				Console.WriteLine("Saying no will still start cleanup, just cleaning up non-admin folders.");
				string input = Console.ReadLine();
				if (input.ToLower().Contains("y"))
				{
					string exename = Process.GetCurrentProcess().MainModule.FileName;
					Process.Start(new ProcessStartInfo
					{
						FileName = exename,
						Verb = "runas",
						Arguments = "-cleanupreboot",
						UseShellExecute = true
					}).WaitForExit();
				}
				else if (!input.ToLower().Contains("n"))
				{
					Console.Clear();
					Console.WriteLine("Invalid input. Please try again.");
					CleanupRobloxFolders();
				}
			}
			Console.WriteLine("Cleaning up roblox folders...");
			if (Roblox.DoWeHaveAdmin())
			{
				Roblox.CleanupAdmin();
			}
			else
			{
				Roblox.Cleanup();
			}
			Console.WriteLine("Completed cleaning up roblox folders! Press enter to go back.");
			if (ranwithcleanupreboot)
			{
				Environment.Exit(0);
			}
		}
		public static void CleanupDelegateHandler()
		{
			CleanupRobloxFolders(false);
		}
		
		public static void RestoreOldMouseCursors()
        {
			Console.WriteLine("Backing up original mouse cursors...");
			DirectoryInfo mouseCursors = new DirectoryInfo(Path.Combine(Roblox.robloxFolder.FullName, "content", "textures", "Cursors", "KeyboardMouse"));
			FileInfo ArrowCursor = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowCursor.png"));
			FileInfo ArrowFarCursor = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowFarCursor.png"));
			ArrowCursor.CopyTo(Path.Combine(ArrowCursor.Directory.FullName, "ArrowCursor.backup"));
			ArrowFarCursor.CopyTo(Path.Combine(ArrowFarCursor.Directory.FullName, "ArrowFarCursor.backup"));
			try
            {
				ArrowCursor.Delete();
				ArrowFarCursor.Delete();
			}
			catch(Exception)
            {
				//Probably doesn't exist, oh well.
            }
			Console.WriteLine("Backed up original mouse cursors.");
			Console.WriteLine("Replacing with old mouse cursors...");
			File.Copy(Path.Combine(Roblox.robloxFolder.FullName, "content", "textures", ArrowCursor.Name), ArrowCursor.FullName);
			File.Copy(Path.Combine(Roblox.robloxFolder.FullName, "content", "textures", ArrowFarCursor.Name), ArrowFarCursor.FullName);
			Config.WriteMod("MouseCursorMod");
			Console.WriteLine("Replaced with old mouse cursors! Press enter to go back.");
		}

		public static bool CheckIfFilesExist()
        {
			DirectoryInfo mouseCursors = new DirectoryInfo(Path.Combine(Roblox.robloxFolder.FullName, "content", "textures", "Cursors", "KeyboardMouse"));
			FileInfo ArrowCursor = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowCursor.backup"));
			FileInfo ArrowFarCursor = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowFarCursor.backup"));
			if(ArrowCursor.Exists && ArrowFarCursor.Exists)
            {
				Choice.Add(choices, "Restore Original Mouse Cursors", new ChoiceDelegate(RestoreOriginalMouseCursors));
				return true;
            } else
            {
				return false;
            }
		}

		public static void RestoreOriginalMouseCursors()
        {
			Console.WriteLine("Restoring original mouse cursors...");
			DirectoryInfo mouseCursors = new DirectoryInfo(Path.Combine(Roblox.robloxFolder.FullName, "content", "textures", "Cursors", "KeyboardMouse"));
			FileInfo ArrowCursorb = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowCursor.backup"));
			FileInfo ArrowFarCursorb = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowFarCursor.backup"));
			FileInfo ArrowCursor = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowCursor.png"));
			FileInfo ArrowFarCursor = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowFarCursor.png"));
			try
            {
				ArrowCursor.Delete();
				ArrowFarCursor.Delete();
			} catch(Exception)
            {
				//Probably doesn't exist, oh well.
			}
			ArrowCursorb.MoveTo(ArrowCursor.FullName);
			ArrowFarCursorb.MoveTo(ArrowFarCursor.FullName);
			Config.RemoveMod("MouseCursorMod");
			Console.WriteLine("Restored original mouse cursors. Press enter to go back.");	
		}

		public static void BackupRobloxFolder()
        {
			Console.WriteLine("Please enter a path to save the back up to.");
			string path = Console.ReadLine();
			Roblox.RemoveInvalidCharactersRef(ref path, true, false);
			if(String.IsNullOrWhiteSpace(path))
            {
				Console.WriteLine("Path cannot be empty.");
				Console.Clear();
				BackupRobloxFolder();
				return;
            }
			FileInfo info = new FileInfo(path);
			if (info.Exists)
            {
				Console.WriteLine("We can't save to this path because a file already exists there. Choose a different path.");
				BackupRobloxFolder();
				return;
            }
			Console.WriteLine("Backing up...");
			string ext = Path.GetExtension(info.FullName);
			if(ext != ".zip")
            {
				info.Create().Close();
			}
			var path2 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Environment.Is64BitProcess ? "x64" : "x86", "7z.dll");
			SevenZipBase.SetLibraryPath(path2);
			SevenZipCompressor compressor = new SevenZipCompressor();
			//Decide which type we want to compress to.
			switch(ext)
            {
				case ".zip":
					ZipFile.CreateFromDirectory(Roblox.robloxFolder.FullName, info.FullName);
					Console.WriteLine("\nCompleted backup to: " + info.FullName);
					Console.WriteLine("Press enter to go back.");
					return;
					break;

				 default:
					compressor.ArchiveFormat = OutArchiveFormat.SevenZip;
					break;
            }
			compressor.Compressing += Compressing;
			if (ext == ".zip") return;
			compressor.CompressDirectoryAsync(Roblox.robloxFolder.FullName, info.FullName).Wait();
			Console.WriteLine("\nCompleted backup to: " + info.FullName);
			Console.WriteLine("Press enter to go back.");
		}
		static bool update = false;
        private static void Compressing(object sender, ProgressEventArgs e)
        {
			ConsoleUtility.WriteProgressBar(e.PercentDone,update);
			update = true;
        }
    }
}
