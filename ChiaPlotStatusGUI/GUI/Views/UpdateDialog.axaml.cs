using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Platform;
using ChiaPlotStatus.GUI.Models;
using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatusGUI.GUI.Utils;
using Octokit;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reactive;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace ChiaPlotStatus.Views
{
    public class UpdateDialog : Window
    {
        public GUI.Models.Language Language { get; set; }
        public Release Latest { get; set; }
        public string Current { get; set; }
        public string current { get; set; }

        public UpdateDialog() { }

        public UpdateDialog(GUI.Models.Language language)
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
            return "" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }

        public void DownloadWindows(object sender, RoutedEventArgs e)
        {
            Utils.OpenUrl("https://github.com/grayfallstown/Chia-Plot-Status/releases/latest/download/Setup.exe");
            Close();
        }

        public void DownloadDeb(object sender, RoutedEventArgs e)
        {
            Utils.OpenUrl("https://github.com/grayfallstown/Chia-Plot-Status/releases/latest/downloadChiaPlotStatus.linux-x64.deb");
            Close();
        }

        public void DownloadRpm(object sender, RoutedEventArgs e)
        {
            Utils.OpenUrl("https://github.com/grayfallstown/Chia-Plot-Status/releases/latest/downloadChiaPlotStatus.linux-x64.rpm");
            Close();
        }


    }

    public class VersionFile
    {
        public string? Version { get; set; }
    }
}
