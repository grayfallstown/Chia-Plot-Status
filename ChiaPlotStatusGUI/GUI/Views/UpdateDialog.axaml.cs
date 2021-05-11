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
using Octokit;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reactive;
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
            this.Find<Button>("DownloadUpdate").IsVisible = !string.Equals(this.Latest.TagName, this.Current);
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
            var deserializer = new DeserializerBuilder().Build();
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            System.IO.Stream filestream = assets.Open(new Uri("avares://ChiaPlotStatus/GUI/Assets/version.yaml"));
            StreamReader reader = new StreamReader(filestream);
            string yaml = reader.ReadToEnd();
            var versionFile = deserializer.Deserialize<VersionFile>(yaml);
            return "" + versionFile.Version;
        }

        public void DownloadUpdate(object sender, RoutedEventArgs e)
        {
            OpenUrl(Latest.HtmlUrl.Replace("/tag/", "/download/") + "/Setup.exe");
            Close();
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }

    public class VersionFile
    {
        public string? Version { get; set; }
    }
}
