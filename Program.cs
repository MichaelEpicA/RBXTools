using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace RBXTools
{
    class Program
    {
		private static FileInfo info;
		private delegate void ChoiceDelegate();
		private static void Main(string[] args)
		{
			FileVersionInfo info2 = FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName);
			Updater.CheckForUpdates();
			if (Roblox.DoWeHaveAdmin())
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
			string custompath = "";
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
			if (custompath != "")
			{
				File.Copy(custompath, info.FullName);
			}
			else
			{
				ExtractResource(Path.Combine(Roblox.robloxFolder.FullName, "content", "sounds"), "ouch.ogg");
			}
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
			List<Choice> choices = new List<Choice>();
			choices.Clear();
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
			}
			catch (Exception)
			{
				Console.Clear();
				Welcome();
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002418 File Offset: 0x00000618
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
			Console.ReadLine();
			Console.Clear();
			Welcome();
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
			Console.ReadLine();
			Console.Clear();
			Welcome();
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
					Roblox.CleanupAdmin();
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
			Console.ReadLine();
			Console.Clear();
			Welcome();
		}
		public static void CleanupDelegateHandler()
		{
			CleanupRobloxFolders(false);
		}
	}
}
