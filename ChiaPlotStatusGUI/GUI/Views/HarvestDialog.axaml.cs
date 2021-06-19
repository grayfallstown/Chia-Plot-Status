using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Threading;
using ChiaPlotStatus.GUI.Models;
using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatusGUI.GUI.Utils;
using ChiaPlotStatusLib.Logic.Models;
using ChiaPlotStatusLib.Logic.Statistics.Harvest;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChiaPlotStatus.Views
{

    /**
     * TODO:
     * - FolderList
     * - AddFolder
     * - RemoveFolder
     * - Summary List
     * - Heatmaps in Details (either tabs or sub-dialogs)
     */
    public class HarvestDialog : Window
    {
        public Settings Settings { get; set; }
        public Language Language { get; set; }
        public List<Tuple<string, HarvestSummary, List<Harvest>>?> Results { get; set; }
        public ObservableCollection<HarvestSummeryReadable> Summaries { get; set; } = new();
        public ObservableCollection<string> PathsWithoutResults { get; set; } = new();

        public HarvestDialog()
        {
        }

        public HarvestDialog(Language language, Settings settings, string theme)
        {
            this.DataContext = this;
            this.Language = language;
            this.Settings = settings;
            foreach (var path in HarvestParser.DefaultPaths())
                if (Directory.Exists(path))
                    this.Settings.HarvesterLogDirectories.Add(path);
            InitializeComponent();
            KeepGridScrollbarOnScreen();
#if DEBUG
            this.AttachDevTools();
#endif
            Utils.SetTheme(this, theme);


            //Task.Run(async () =>
            //{
                LoadData();
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    this.Find<StackPanel>("Loading").IsVisible = false;
                    this.Find<StackPanel>("Loaded").IsVisible = true;
                });
            //});

            this.WindowState = WindowState.Maximized;
            this.Focus();
        }


        public void AddFolder(string folder)
        {
            if (Directory.Exists(folder) && !Settings.HarvesterLogDirectories.Contains(folder))
            {
                Settings.HarvesterLogDirectories.Add(folder);
                Settings.Persist();
                LoadData();
            }
        }

        public void AddFolder(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                OpenFolderDialog picker = new OpenFolderDialog();
                var result = await picker.ShowAsync(this);
                Dispatcher.UIThread.InvokeAsync(() => AddFolder(result));
            });
        }

        public void RemoveFolder (object sender, RoutedEventArgs e) {
            string folder = (string)((Button)sender).Tag;
            Settings.HarvesterLogDirectories.Remove(folder);
            Settings.Persist();
            LoadData();
        }

        private void LoadData()
        {
            Summaries.Clear();
            PathsWithoutResults.Clear();

            Results = new HarvestParser().ParseLogs(new List<string>(this.Settings.HarvesterLogDirectories),
                (double)this.Settings.MaxHarvestLookupSeconds, 500);

            // assume all are missing until a result is found
            foreach (var path in this.Settings.HarvesterLogDirectories)
                PathsWithoutResults.Add(path);

            foreach (var triplet in Results)
            {
                if (triplet != null)
                {
                    Summaries.Add(new HarvestSummeryReadable(triplet.Item2));
                    PathsWithoutResults.Remove(triplet.Item1);
                }
            }
        }

        private void KeepGridScrollbarOnScreen()
        {
            this.WhenAnyValue(x => x.Height)
                .Subscribe(x =>
                {
                    var summeriesDataGrid = this.Find<DataGrid>("SummeriesDataGrid");
                    summeriesDataGrid.Height = x - 100;
                });
        }

        public void DataGridHeaderClick(object sender, RoutedEventArgs e)
        {

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

    }
}
