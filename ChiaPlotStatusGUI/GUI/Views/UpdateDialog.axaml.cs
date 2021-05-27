using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Platform;
using Avalonia.Threading;
using ChiaPlotStatus.GUI.Models;
using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatusGUI.GUI.Utils;
using Octokit;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reactive;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace ChiaPlotStatus.Views
{
    public enum InstallationPackageType
    {
        EXE,
        DEB,
        RPM,
        PKG
    }

    public class UpdateDialog : Window
    {
        public GUI.Models.Language Language { get; set; }
        public Release Latest { get; set; }
        public string Current { get; set; }
        public string current { get; set; }

        public UpdateDialog() { }

        public UpdateDialog(GUI.Models.Language language, string theme)
        {
            this.DataContext = this;
            this.Language = language;
            this.Current = LoadCurrentRelease();
            this.Latest = LoadLatestRelease();
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.Find<TextBlock>("UpToDate").IsVisible = string.Equals(this.Latest.TagName, this.Current);
            this.Find<StackPanel>("DownloadButtons").IsVisible = !string.Equals(this.Latest.TagName, this.Current);
            Utils.SetTheme(this, theme);
            this.Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        private Release LoadLatestRelease()
        {
            var client = new GitHubClient(new ProductHeaderValue("Chia-Plot-Status"));
            Release? latest = null;
            var task = Task.Run(async () =>
            {
                var result = await client.Repository.Release.GetAll("grayfallstown", "Chia-Plot-Status");
                latest = result[0];
            });
            task.Wait();
            return latest;
        }

        private string LoadCurrentRelease()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }

        public void DownloadWindows(object sender, RoutedEventArgs e)
        {
            Update(InstallationPackageType.EXE);
        }

        public void DownloadDeb(object sender, RoutedEventArgs e)
        {
            Update(InstallationPackageType.DEB);
        }

        public void DownloadRpm(object sender, RoutedEventArgs e)
        {
            Update(InstallationPackageType.RPM);
        }



        /**
         * It was requested to rename Setup.exe in something that contains the name of the application.
         * Problem is the download url and therefor the file name is hardcoded to Setup.exe.
         * Old applications would no longer be able to find the update if the name was changed.
         * Solving this by using a fallback name now and once a few updates passed I can change
         * the name without breaking too many installations.
         */
        public static (string, string?) GetInstallationPackageName(InstallationPackageType type)
        {
            switch (type)
            {
                case InstallationPackageType.EXE:
                    return ("ChiaPlotStatus.windows-Setup.exe", "Setup.exe");
                case InstallationPackageType.DEB:
                    return ("ChiaPlotStatus.linux-x64.deb", null);
                case InstallationPackageType.RPM:
                    return ("ChiaPlotStatus.linux-x64.rpm", null);
                case InstallationPackageType.PKG:
                    return ("ChiaPlotStatus.mac.pkg", null);
                default:
                    throw new NotImplementedException("unknown InstallationPackageType " + type);
            }
        }

        public void Update(InstallationPackageType type)
        {
            this.Find<StackPanel>("DownloadButtons").IsVisible = false;
            this.Find<StackPanel>("DownloadNotice").IsVisible = true;
            var fileName = GetInstallationPackageName(type);
            Task.Run(async () =>
            {
                if (DownloadInstallationPackage(fileName))
                {
                    void StartInstallationAndExit(string installerPath)
                    {
                        // starts a detached process that does not get terminated
                        // when we close Chia Plot Status, as we need to close
                        // it to update the files (on windows and maybe on mac too)
                        var startInfo = new ProcessStartInfo(installerPath);
                        startInfo.UseShellExecute = true;
                        Process.Start(startInfo);
                        Environment.Exit(0);
                    }

                    var paths = GetFullInstallationPackagePath(type);
                    if (File.Exists(paths.Item1))
                    {
                        StartInstallationAndExit(paths.Item1);
                        return;
                    }
                    if (paths.Item2 != null && File.Exists(paths.Item2))
                    {
                        StartInstallationAndExit(paths.Item2);
                        return;
                    }
                    else
                        Debug.WriteLine("Both installation package paths lead nowhere, falling back to manual installation");
                }
                else
                    Debug.WriteLine("Download of installation package failed, falling back to manual installation");

                // fallback to manual download via browser
                Utils.OpenUrl("https://github.com/grayfallstown/Chia-Plot-Status/releases/latest/");

            });
        }

        public bool DownloadInstallationPackage((string, string?) fileName)
        {
            using (var client = new WebClient())
            {
                try
                {
                    string baseUrl = "https://github.com/grayfallstown/Chia-Plot-Status/releases/latest/download/";
                    string updateDir = EnsureUpdateTempDirectoryExists(); ;
                    client.DownloadFile(baseUrl + fileName.Item1, updateDir + fileName.Item1);
                    return true;
                }
                catch (System.Net.WebException e)
                {
                    Debug.WriteLine("download of update file " + fileName + " failed " + e);
                    if (fileName.Item2 != null)
                        return DownloadInstallationPackage((fileName.Item2, null));
                }
            }
            return false;
        }

        public static string GetUpdateTempDirectory()
        {
            return Path.GetTempPath() + "ChiaPlotStatusUpdate" + Path.DirectorySeparatorChar;
        }

        public static (string, string?) GetFullInstallationPackagePath(InstallationPackageType type)
        {
            (string, string?) packageName = GetInstallationPackageName(type);
            string basePath = GetUpdateTempDirectory() + Path.DirectorySeparatorChar;
            if (packageName.Item2 == null)
                return (basePath + packageName.Item1, null);
            else
                return (basePath + packageName.Item1, basePath + packageName.Item2);
        }

        public static string EnsureUpdateTempDirectoryExists()
        {
            var dir = GetUpdateTempDirectory();
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        public static void DeleteUpdateTempDirectory()
        {
            try
            {
                var dir = GetUpdateTempDirectory();
                if (Directory.Exists(dir))
                    Directory.Delete(dir, true);
            } catch (Exception e)
            {
                Debug.WriteLine("DeleteUpdateTempDirectory failed: " + e);
            }
        }
    }
}
