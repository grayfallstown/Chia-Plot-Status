using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChiaPlotStatus
{
    /**
     * The entire UI code
     */
    public partial class MainWindow : Window
    {
        private readonly PlotManager PlotManager = new PlotManager();
        private readonly string DEFAULT_LOG_FOLDER = Environment.GetFolderPath(
                                        Environment.SpecialFolder.UserProfile)
                                            + @"\.chia\mainnet\plotter\";

        public MainWindow()
        {
            System.Windows.Application.Current.MainWindow.WindowState = WindowState.Maximized;
            InitializeComponent();
            DataContext = this;

            //PlotManager.AddLogFolder(DEFAULT_LOG_FOLDER);
            LoadConfig();
            folderListView.ItemsSource = PlotManager.LogDirectories;

            var dueTime = TimeSpan.FromSeconds(0);
            var interval = TimeSpan.FromSeconds(30);
            _ = RunPeriodicAsync(Load, dueTime, interval, CancellationToken.None);
        }

        public void LoadConfig()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
                foreach (var key in config.AppSettings.Settings.AllKeys)
                {
                    if (string.Equals(key, "folders"))
                    {
                        string foldersStr = config.AppSettings.Settings[key].Value;
                        var folders = foldersStr.Split("#");
                        foreach (var folder in folders)
                            AddFolder(folder);
                    }
                    if (string.Equals(key, "fontSize"))
                    {
                        this.FontSize = Double.Parse(config.AppSettings.Settings[key].Value);
                    }
                }
                if (PlotManager.LogDirectories.Count == 0)
                    AddFolder(DEFAULT_LOG_FOLDER);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void SaveConfig()
        {
            try {
                Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
                string folders = String.Join("#", PlotManager.LogDirectories);
                config.AppSettings.Settings.Clear();
                config.AppSettings.Settings.Add("folders", folders);
                config.AppSettings.Settings.Add("fontSize", this.FontSize.ToString());
                config.Save(ConfigurationSaveMode.Minimal);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void Load()
        {
            Task.Run(() =>
            {
                Dispatcher.Invoke(() => {
                    var plotLogs = PlotManager.PollPlotLogs();
                    var plotLogUis = new List<PlotLogUI>();
                    foreach (var plotLog in plotLogs)
                    {
                        plotLogUis.Add(new PlotLogUI(plotLog));
                    }
                    dataGrid.ItemsSource = plotLogUis;
                    dataGrid.Items.Refresh();
                });
            });
        }


        public void AddFolder(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderPicker = new VistaFolderBrowserDialog();
            folderPicker.Description = "Plotting processe log folder";
            if (folderPicker.ShowDialog() == true) // nullable bool
                AddFolder(folderPicker.SelectedPath);
        }

        public void AddFolder(string folder)
        {
            if (Directory.Exists(folder) && !PlotManager.LogDirectories.Contains(folder))
            {
                PlotManager.AddLogFolder(folder);
                folderListView.Items.Refresh();
                SaveConfig();
                Load();
            }
        }

        public void RemoveFolder(object sender, RoutedEventArgs e)
        {
            string folder = (string) ((System.Windows.Controls.Button)sender).Tag;
            if (Directory.Exists(folder))
            {
                PlotManager.RemoveLogFolder(folder);
                folderListView.Items.Refresh();
                SaveConfig();
                Load();
            }
        }

        public void IncreaseFontSize(object sender, RoutedEventArgs e)
        {
            this.FontSize++;
            SaveConfig();
        }

        public void DecreaseFontSize(object sender, RoutedEventArgs e)
        {
            this.FontSize--;
            SaveConfig();
        }


        private static async Task RunPeriodicAsync(Action onTick,
                                               TimeSpan dueTime,
                                               TimeSpan interval,
                                               CancellationToken token)
        {
            // Initial wait time before we begin the periodic loop.
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token);

            // Repeat this loop until cancelled.
            while (!token.IsCancellationRequested)
            {
                // Call our onTick function.
                onTick?.Invoke();

                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);
            }
        }

    }
}
