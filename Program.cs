using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing.Imaging;
using SevenZip;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace RBXTools
{
    class Program
    {
		public static FileInfo info;
		public delegate void ChoiceDelegate();
		private delegate bool ChoiceDecisionDelegate();
		private static bool updateAvailable;
		public static List<Choice> choices = new List<Choice>();
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
					info = new FileInfo(Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "repairlauncher-roblox.backup"));
					DelegateStorage.SetupAutoReapply();
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
			Config.VerifyFormat();
			info = new FileInfo(Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "repairlauncher-roblox.backup"));
			if(Roblox.robloxFolder.adminFolder && !Roblox.DoWeHaveAdmin())
            {
				Roblox.RestartAsAdmin();
			}
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
			FileInfo info = new FileInfo(Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "sounds", "ouch.ogg"));
			info.Delete();
			Console.WriteLine("Replacing Death Sound Effect...");
			if (custompath != "default")
			{
				File.Copy(custompath, info.FullName);
			}
			else
			{
				ExtractResource(Path.Combine(Roblox.robloxFolder.folderinfo.FullName, "content", "sounds"), "ouch.ogg");
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
				Console.WriteLine("An update is available! Press 10 to download and install the new version!");
				Console.ResetColor();
            }
			Console.WriteLine("Current Roblox Folder: " + Roblox.robloxFolder.folderinfo.FullName);
			if(!Roblox.CheckRobloxRegistryInstall())
            {
				Console.WriteLine("Warning! Roblox Install has been detected as invalid. When you try to launch roblox, it will not work, and it will fail to launch.");
				Console.WriteLine("In order to repair this, we will probably need to reinstall roblox. Are you okay with this? (Y/N)");
				string input = Console.ReadLine();
				if(input.ToLower() == "y")
                {
					//Reinstall roblox
					Console.WriteLine("Do you want to just change the registry to a different roblox folder, or remake that roblox folder? (C/R)");
					input = Console.ReadLine();
					if(input.ToLower() == "c")
                    {
						Console.WriteLine("Is this the roblox folder you want to change to? (Roblox Folder: " + Roblox.robloxFolder.folderinfo.FullName + ") (Y/N)");
						input = Console.ReadLine();
						if(input.ToLower() == "y")
                        {
							//Edit registry
							Roblox.UpdateRobloxProtocolURL();

                        } else if(input.ToLower() == "n")
                        {
							Console.WriteLine("Please choose your roblox folder.");
							Roblox.RobloxFolderChoose();
							//Edit registry
							Roblox.UpdateRobloxProtocolURL();
						}
                    } else if(input.ToLower() == "r")
                    {
						Console.WriteLine("Getting current registry folder...");
						DirectoryInfo reg = Roblox.SearchRegistry();
						Console.WriteLine("Checking if this is the latest version of roblox...");
						if(Roblox.GetLatestVersion() == reg.Name)
                        {
							Console.WriteLine("This is the latest version of Roblox! Redownloading...");
                        } else
                        {
							Console.WriteLine("Not the latest version, Redownloading to a different folder...");
                        }
						DirectoryInfo robloxRedownload = new DirectoryInfo(Roblox.GetLatestVersion());
						if(robloxRedownload.Exists)
                        {
							Roblox.robloxFolder = new RobloxFolder(robloxRedownload);
							Roblox.UpdateRobloxProtocolURL();
                        } else
                        {
							robloxRedownload.Create();
							RobloxFolder.ClearReadOnly(robloxRedownload);
							Roblox.DownloadRobloxVersion(robloxRedownload);
							Roblox.robloxFolder = new RobloxFolder(robloxRedownload);
							Roblox.robloxFolder.Verifier();
							Roblox.UpdateRobloxProtocolURL();
						}
                    }
                } else if(input.ToLower() == "n")
                {
					//Welp, they aren't ok with it.
					Console.WriteLine("Ok, guess we aren't reinstalling roblox, continuing...");
                }
            }
			if (!Roblox.CheckRobloxRegistryInstall())
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("WARNING: ROBLOX FOLDER MARKED AS START IS INVALID, ROBLOX WILL NOT WORK");
				Console.ResetColor();
			}
			Console.WriteLine("Welcome! What would you like to do?");
			Choice.Add(choices, "Restore Old Noob Sound Effect", new ChoiceDelegate(DelegateStorage.RestoreOldNoobSoundEffect));
			if (new FileInfo(info.FullName).Exists)
			{
				Choice.Add(choices, "Restore Original Launcher", new ChoiceDelegate(DelegateStorage.RestoreOriginalLauncher));
			}
			else
			{
				Choice.Add(choices, "Auto Reapply Changes After Roblox Update", new ChoiceDelegate(DelegateStorage.SetupAutoReapply));
			}
			Choice.Add(choices, "Cleanup Roblox Folders", new ChoiceDelegate(DelegateStorage.CleanupDelegateHandler));
			Choice.Add(choices, "Restore Old (2014) Cursors", new ChoiceDelegate(DelegateStorage.RestoreOldMouseCursors), new ChoiceDecisionDelegate(DelegateStorage.CheckIfFilesExist));
			Choice.Add(choices, "Backup Roblox Folder", new ChoiceDelegate(DelegateStorage.BackupRobloxFolder));
			Choice.Add(choices, "Replace Moon And Sun Textures", new ChoiceDelegate(DelegateStorage.ReplaceMoonAndSun), new ChoiceDecisionDelegate(DelegateStorage.CheckConfigFileForMod));
			Choice.Add(choices, "Replace Roblox Icon", new ChoiceDelegate(DelegateStorage.ReplaceRobloxIcon), new ChoiceDecisionDelegate(DelegateStorage.CheckIfRobloxConfigExists));
			Choice.Add(choices, "Set Up Old Roblox Player Start (Requires Auto Reapply Changes After Roblox Update/Custom Launcher to work)", new ChoiceDelegate(DelegateStorage.RobloxForceStart));
			Choice.Add(choices, "Roblox Folder Management", new ChoiceDelegate(Roblox.RobloxFolderChoose));
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
				if(number == 9)
                {
					choices[number - 1].choiceFunction.DynamicInvoke();
					Welcome();
                } else
                {
					if (choices[number - 1] != null)
					{
						choices[number - 1].Run();
					}
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

		static bool update = false;
        public static void Compressing(object sender, ProgressEventArgs e)
        {
			ConsoleUtility.WriteProgressBar(e.PercentDone,update);
			update = true;
        }

		public static Bitmap ResizeImage(Image image, int width, int height)
        {
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}
    }
}
