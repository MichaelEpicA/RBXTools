using SevenZip;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace RBXTools
{
    class DelegateStorage
    {
		public static FileInfo info;
		#region Mod Delegates
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
			FileInfo info = new FileInfo(Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "sounds", "ouch.ogg"));
			info.Delete();
			Console.WriteLine("Replacing Death Sound Effect...");
			if (custompath != "default")
			{
				File.Copy(custompath, info.FullName);
			}
			else
			{
				Program.ExtractResource(Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "sounds"), "ouch.ogg");
			}
			Config.WriteMod("DeathSFXMod", custompath);
			Console.WriteLine("Completed! Test it out in Roblox. Press enter to go back.");
		}
		public static void SetupAutoReapply()
		{
			Console.WriteLine("Backing up launcher...");
			Roblox.FindRobloxFolder();
			FileInfo launcherPath = new FileInfo(Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "RobloxPlayerLauncher.exe"));
			launcherPath.CopyTo(Program.info.FullName, true);
			launcherPath.Delete();
			Console.WriteLine("Back up complete!");
			Console.WriteLine("Replacing launcher...");
			Program.ExtractResource(launcherPath.Directory.FullName, launcherPath.Name);
			Roblox.Cleanup();
			Console.WriteLine("Replaced launcher! Press enter to go back.");
		}
		public static void CleanupDelegateHandler()
		{
			Program.CleanupRobloxFolders(false);
		}
		public static void RestoreOldMouseCursors()
		{
			Console.WriteLine("Backing up original mouse cursors...");
			DirectoryInfo mouseCursors = new DirectoryInfo(Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "textures", "Cursors", "KeyboardMouse"));
			FileInfo ArrowCursor = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowCursor.png"));
			FileInfo ArrowFarCursor = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowFarCursor.png"));
			ArrowCursor.CopyTo(Path.Combine(ArrowCursor.Directory.FullName, "ArrowCursor.backup"));
			ArrowFarCursor.CopyTo(Path.Combine(ArrowFarCursor.Directory.FullName, "ArrowFarCursor.backup"));
			try
			{
				ArrowCursor.Delete();
				ArrowFarCursor.Delete();
			}
			catch (Exception)
			{
				//Probably doesn't exist, oh well.
			}
			Console.WriteLine("Backed up original mouse cursors.");
			Console.WriteLine("Replacing with old mouse cursors...");
			File.Copy(Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "textures", ArrowCursor.Name), ArrowCursor.FullName);
			File.Copy(Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "textures", ArrowFarCursor.Name), ArrowFarCursor.FullName);
			Config.WriteMod("MouseCursorMod");
			Console.WriteLine("Replaced with old mouse cursors! Press enter to go back.");
		}
		public static void BackupRobloxFolder()
		{
			Console.WriteLine("Please enter a path to save the back up to.");
			string path = Console.ReadLine();
			Roblox.RemoveInvalidCharactersRef(ref path, true, false);
			if (String.IsNullOrWhiteSpace(path))
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
			if (ext != ".zip")
			{
				info.Create().Close();
			}
			var path2 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Environment.Is64BitProcess ? "x64" : "x86", "7z.dll");
			SevenZipBase.SetLibraryPath(path2);
			SevenZipCompressor compressor = new SevenZipCompressor();
			//Decide which type we want to compress to.
			switch (ext)
			{
				case ".zip":
					ZipFile.CreateFromDirectory(Roblox.robloxFolder.folderinfo.FullName, info.FullName);
					Console.WriteLine("\nCompleted backup to: " + info.FullName);
					Console.WriteLine("Press enter to go back.");
					return;
					break;

				default:
					compressor.ArchiveFormat = OutArchiveFormat.SevenZip;
					break;
			}
			compressor.Compressing += Program.Compressing;
			if (ext == ".zip") return;
			compressor.CompressDirectoryAsync(Roblox.robloxFolder.folderinfo.FullName, info.FullName).Wait();
			Console.WriteLine("\nCompleted backup to: " + info.FullName);
			Console.WriteLine("Press enter to go back.");
		}
		public static void ReplaceMoonAndSun()
		{
			Console.WriteLine("Replacing moon and sun...");
			DirectoryInfo MoonAndSunDirectory = new DirectoryInfo(Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "sky"));
			FileInfo moon = new FileInfo(Path.Combine(MoonAndSunDirectory.FullName, "moon.jpg"));
			FileInfo sun = new FileInfo(Path.Combine(MoonAndSunDirectory.FullName, "sun.jpg"));
			FileInfo moonb = new FileInfo(Path.Combine(MoonAndSunDirectory.FullName, "moonb.jpg"));
			FileInfo sunb = new FileInfo(Path.Combine(MoonAndSunDirectory.FullName, "sunb.jpg"));
			Console.WriteLine("Please enter path you want to use to replace the moon.");
			string moonpath = Console.ReadLine();
			if (!Roblox.RemoveInvalidCharactersRef(ref moonpath, true, false))
			{
				//Reenter the path.
				if (Path.GetExtension(moonpath) != ".jpg")
				{
					Console.WriteLine("In order to replace the texture correctly, we need to have it be a jpg. Try again.");
					ReplaceMoonAndSun();
				}
				Console.WriteLine("Incorrect path. Please reenter it.");
				ReplaceMoonAndSun();
			}
			Console.WriteLine("Please enter path you want to use to replace the sun.");
			string sunpath = Console.ReadLine();
			if (!Roblox.RemoveInvalidCharactersRef(ref sunpath, true, false))
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
			if (moonb.Exists)
			{
				moonb.Delete();
			}
			if (sunb.Exists)
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
		public static void ReplaceRobloxIcon()
		{
			Console.WriteLine("Please enter an icon you would like to replace the Roblox Icon with (24x24px for best results) or leave it blank for default old roblox icon.");
			string input = Console.ReadLine();
			FileInfo info = null;
			FileInfo infof = null;
			FileInfo infos = null;
			FileInfo spin2 = null;
			if (!string.IsNullOrWhiteSpace(input))
			{
				if (!File.Exists(input))
				{
					if (Roblox.RemoveInvalidCharactersRef(ref input, true, false))
					{
						info = new FileInfo(input);
					}
					else
					{
						ReplaceRobloxIcon();
					}

				}
				Console.WriteLine("Would you like to supply the other 2 images or have the program resize this original image for you. (S/R)");
				string answer = Console.ReadLine();
				if (answer.ToLower() == "s")
				{
					Console.WriteLine("Supply the 48x48 image.");
					string f = Console.ReadLine();
					//Checking file shit
					if (!File.Exists(f))
					{
						while (!File.Exists(f))
						{
							if (Roblox.RemoveInvalidCharactersRef(ref f, true, false))
							{
								infof = new FileInfo(f);
							}
							else
							{

								Console.WriteLine("Supply the 48x48 image.");
								f = Console.ReadLine();
							}
						}
					}
					Console.WriteLine("Supply the 72x72 image.");
					string f2 = Console.ReadLine();
					//Checking file pt2
					if (!File.Exists(f2))
					{
						while (!File.Exists(f2))
						{
							if (Roblox.RemoveInvalidCharactersRef(ref f2, true, false))
							{
								infos = new FileInfo(f2);
							}
							else
							{

								Console.WriteLine("Supply the 48x48 image.");
								f = Console.ReadLine();
							}
						}
					}
				}
				else if (answer.ToLower() == "r")
				{
					//Resize the image to the correct resolution.
					Console.WriteLine("Resizing images...");
					Image image = Image.FromFile(input);
					if (image.Width != 24 || image.Height != 24)
					{
						Bitmap bmp = Program.ResizeImage(image, 24, 24);
						bmp.Save("24.png");
						info = new FileInfo("24.png");
						bmp.Dispose();
					}
					Bitmap bmp2 = Program.ResizeImage(image, 48, 48);
					bmp2.Save("48.png");
					bmp2.Dispose();
					infof = new FileInfo("48.png");
					Bitmap bmp3 = Program.ResizeImage(image, 72, 72);
					bmp3.Save("72.png");
					bmp3.Dispose();
					infos = new FileInfo("72.png");
					Console.WriteLine("Resized images.");
				}
			}
			else
			{
				//Use the default image.
				Program.ExtractResource("", "coloredlogo.png");
				Program.ExtractResource("", "coloredlogo@2x.png");
				Program.ExtractResource("", "coloredlogo@3x.png");
				info = new FileInfo("coloredlogo.png");
				infof = new FileInfo("coloredlogo@2x.png");
				infos = new FileInfo("coloredlogo@3x.png");
			}
			Console.WriteLine("Would you like to replace the spinning loading icon too? (Y/N)");
			string input2 = Console.ReadLine();
			bool spinreplace = input2.ToLower() == "y";
			if (spinreplace)
			{
				Console.WriteLine("Would you like to use the default spinning icon, use a custom one, or use the previous supplied image? (Resolution 256x256)");
				string input3 = Console.ReadLine();
				if (input3.ToLower() == "d")
				{
					string s = Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "textures", "loading");
					spin2 = new FileInfo(Path.Combine(s, "loadingCircle.png"));
				}
				else if (input3.ToLower() == "c")
				{
					Console.WriteLine("Please enter the path for the spinning icon.");
					string f = Console.ReadLine();
					while (!File.Exists(f))
					{
						if (Roblox.RemoveInvalidCharactersRef(ref f, true, false))
						{
							infof = new FileInfo(f);
						}
						else
						{

							Console.WriteLine("Supply the 48x48 image.");
							f = Console.ReadLine();
						}
					}
				}
				else if (input3.ToLower() == "p")
				{
					Image i = Image.FromFile(input);
					Bitmap bm = Program.ResizeImage(i, 256, 256);
					bm.Save("spin_resize.png");
					spin2 = new FileInfo("spin_resize.png");
				}
			}
			Console.WriteLine("Would you like to replace the application icon?");
			string spin = Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "textures", "loading");
			string topBarFolder = Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "textures", "ui", "TopBar");
			FileInfo coloredLogo = new FileInfo(Path.Combine(topBarFolder, "coloredlogo.png"));
			FileInfo coloredLogo2x = new FileInfo(Path.Combine(topBarFolder, "coloredlogo@2x.png"));
			FileInfo coloredLogo3x = new FileInfo(Path.Combine(topBarFolder, "coloredlogo@3x.png"));
			FileInfo spininfo = new FileInfo(Path.Combine(spin, "robloxTilt.png"));
			Console.WriteLine("Overwriting images...");
			info.MoveTo(coloredLogo.FullName, true);
			infof.MoveTo(coloredLogo2x.FullName, true);
			infos.MoveTo(coloredLogo3x.FullName, true);
			spininfo.CopyTo("loader.backup");
			spin2.MoveTo(spininfo.FullName, true);
			Console.WriteLine("Overwrote images!");
			if (string.IsNullOrWhiteSpace(input))
			{
				Config.WriteMod("RobloxIconReplaceMod");
			}
			else
			{
				Config.WriteMod("RobloxIconReplaceMod", info.FullName, infof.FullName, infos.FullName);
			}
			Console.WriteLine("Complete! Press enter to go back.");
		}
		public static void RobloxForceStart()
		{
			if (!new FileInfo(Program.info.FullName).Exists)
			{
				Console.WriteLine("The custom launcher is not injected. Please press 2 and wait until the launcher is injected, then go back and do this.");
			}
			Console.WriteLine("[1] Old (The old roblox launcher. This makes it that it will always launch this launcher.)");
			Console.WriteLine("[2] New/App (The roblox app thats being forced on people nowadays. No matter what, this option will launch the app.");
			Console.WriteLine("[3] Default (Depends on what you have your roblox settings set as, if its the app beta, it will launch that.");
			Console.WriteLine("Please make your choice.");
			string choice = Console.ReadLine();
			switch (choice)
			{
				case "1":
					Console.WriteLine("Setting config file to Old Roblox Launcher...");
					Config.WriteMod("AppLaunchSetting", "play");
					break;
				case "2":
					Console.WriteLine("Setting config file to Roblox App...");
					Config.WriteMod("AppLaunchSetting", "app");
					break;
				case "3":
					Console.WriteLine("Removing config...");
					Config.RemoveMod("AppLaunchSetting");
					break;
			}
			Console.WriteLine("Complete! Press enter to go back.");
		}
		#endregion Mod Delegates

		#region Uninstall Mod Delegates
		public static void RestoreOriginalLauncher()
		{
			Console.WriteLine("Deleting modded launcher...");
			Roblox.FindRobloxFolder();
			FileInfo launcherPath = new FileInfo(Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "RobloxPlayerLauncher.exe"));
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
		public static void RestoreOriginalMouseCursors()
		{
			Console.WriteLine("Restoring original mouse cursors...");
			DirectoryInfo mouseCursors = new DirectoryInfo(Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "textures", "Cursors", "KeyboardMouse"));
			FileInfo ArrowCursorb = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowCursor.backup"));
			FileInfo ArrowFarCursorb = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowFarCursor.backup"));
			FileInfo ArrowCursor = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowCursor.png"));
			FileInfo ArrowFarCursor = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowFarCursor.png"));
			try
			{
				ArrowCursor.Delete();
				ArrowFarCursor.Delete();
			}
			catch (Exception)
			{
				//Probably doesn't exist, oh well.
			}
			ArrowCursorb.MoveTo(ArrowCursor.FullName);
			ArrowFarCursorb.MoveTo(ArrowFarCursor.FullName);
			Config.RemoveMod("MouseCursorMod");
			Console.WriteLine("Restored original mouse cursors. Press enter to go back.");
		}
		private static void RestoreMoonAndSun()
		{
			Console.WriteLine("Restoring moon and sun...");
			DirectoryInfo MoonAndSunDirectory = new DirectoryInfo(Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "sky"));
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
		public static void RestoreRobloxIcon()
		{
			Console.WriteLine("Restoring roblox icon...");
			//Extract and write here.
			string topBarFolder = Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "textures", "ui", "TopBar");
			string s = Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "textures", "loading");
			FileInfo coloredLogo = new FileInfo(Path.Combine(topBarFolder, "coloredlogo.png"));
			FileInfo coloredLogo2x = new FileInfo(Path.Combine(topBarFolder, "coloredlogo@2x.png"));
			FileInfo coloredLogo3x = new FileInfo(Path.Combine(topBarFolder, "coloredlogo@3x.png"));
			FileInfo spininfo = new FileInfo(Path.Combine(s, "robloxTilt.png"));
			Console.WriteLine("Overwriting images...");
			Program.ExtractResource("", "coloredlogor (1).png");
			Program.ExtractResource("", "coloredlogor (2).png");
			Program.ExtractResource("", "coloredlogor (3).png");
			FileInfo info = new FileInfo("coloredlogor (1).png");
			FileInfo infof = new FileInfo("coloredlogor (2).png");
			FileInfo infos = new FileInfo("coloredlogor (3).png");
			FileInfo spinbackup = new FileInfo(Path.Combine(s, "loader.backup"));
			info.MoveTo(coloredLogo.FullName, true);
			infof.MoveTo(coloredLogo2x.FullName, true);
			infos.MoveTo(coloredLogo3x.FullName, true);
			spinbackup.MoveTo(spininfo.FullName, true);
			Console.WriteLine("Overwrote images!");
			Config.RemoveMod("RobloxIconReplaceMod");
			Console.WriteLine("Complete! Press enter to go back.");
		}

		#endregion Uninstall Mod Delegates

		#region Check Delegates
		public static bool CheckIfFilesExist()
		{
			DirectoryInfo mouseCursors = new DirectoryInfo(Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "textures", "Cursors", "KeyboardMouse"));
			FileInfo ArrowCursor = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowCursor.backup"));
			FileInfo ArrowFarCursor = new FileInfo(Path.Combine(mouseCursors.FullName, "ArrowFarCursor.backup"));
			if (ArrowCursor.Exists && ArrowFarCursor.Exists)
			{
				Choice.Add(Program.choices, "Restore Original Mouse Cursors", new Program.ChoiceDelegate(RestoreOriginalMouseCursors));
				return true;
			}
			else
			{
				return false;
			}
		}
		public static bool CheckConfigFileForMod()
		{
			if (Config.CheckIfModHasBeenAddedAlready("ReplaceMoonAndSunMod"))
			{
				Choice.Add(Program.choices, "Restore Moon And Sun to Originals", new Program.ChoiceDelegate(RestoreMoonAndSun));
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool CheckIfRobloxConfigExists()
		{
			if (Config.CheckIfModHasBeenAddedAlready("RobloxIconReplaceMod"))
			{
				Choice.Add(Program.choices, "Restore Roblox Icon", new Program.ChoiceDelegate(RestoreRobloxIcon));
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion Check Delegates

	}
}
