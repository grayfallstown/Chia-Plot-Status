using Avalonia.Controls;
using Avalonia.Interactivity;
using ChiaPlotStatus;
using ChiaPlottStatus.GUI.Models;
using ChiaPlottStatusAvalonia.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlottStatusAvalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ChiaPlotStatus.ChiaPlotStatus PlotManager { get; internal set; }

        public ObservableCollection<PlotLogReadable> PlotLogs { get; } = new();
        public string? Search { get; set; } = null;
        public string SortProperty { get; set; } = "Progress";
        public bool SortAsc { get; set; } = true;
        public ObservableCollection<string> SortProperties = new();

        public Language Language { get; set; }
        public Dictionary<string, Language> Languages { get; set; }

        public ReactiveCommand<Unit, Unit> AddFolderCommand { get; set; }
        public ReactiveCommand<string, Unit> RemoveFolderCommand { get; set; }
        public ReactiveCommand<Unit, Unit> IncreaseFontSizeCommand { get; set; }
        public ReactiveCommand<Unit, Unit> DecreaseFontSizeCommand { get; set; }

        public MainWindowViewModel()
        {
            foreach (var property in typeof(PlotLogReadable).GetProperties())
                SortProperties.Add(property.Name);
            Languages = Translation.LoadLanguages();
            Language = Languages["English"];
            InitializeChiaPlotStatus();
            InitializeButtons();
            InitializeSearchBox();
        }

        public void InitializeChiaPlotStatus()
        {
            Settings Settings = new Settings(System.Reflection.Assembly.GetExecutingAssembly().Location + ".config.json");
            Settings.Load();
            PlotManager = new(Settings);
            if (PlotManager.Settings.LogDirectories.Count == 0)
                PlotManager.AddDefaultLogFolder();
            LoadPlotLogs();
        }

        public void LoadPlotLogs()
        {
            PlotLogs.Clear();
            foreach (var plotLog in PlotManager.PollPlotLogs(Search, SortProperty, SortAsc))
                PlotLogs.Add(plotLog.Item2);
        }

        public void InitializeButtons()
        {
            AddFolderCommand = ReactiveCommand.Create(AddFolder);
            RemoveFolderCommand = ReactiveCommand.Create<string>(RemoveFolder);
            MainWindow.Instance.BtnClickWorkaround = (folder) =>
            {
                RemoveFolder(folder);
                return true;
            };

            MainWindow.Instance.SortChangeWorkaround = (headerText) =>
            {
                var oldSortProperty = SortProperty;
                foreach (var property in typeof(Columns).GetProperties())
                {
                    string translation = (string)property.GetValue(Language.Columns);
                    if (string.Equals(headerText, translation))
                    {
                        SortProperty = property.Name;
                        break;
                    }
                }
                if (string.Equals(SortProperty, oldSortProperty))
                    SortAsc = !SortAsc;
                Debug.WriteLine("SearchProperty: " + SortProperty + ", ASC: " + SortAsc);
                LoadPlotLogs();
                return true;
            };

            IncreaseFontSizeCommand = ReactiveCommand.Create(() =>
            {
                this.PlotManager.Settings.FontSize += 0.3;
                this.PlotManager.Settings.Persist();
            });

            DecreaseFontSizeCommand = ReactiveCommand.Create(() =>
            {
                this.PlotManager.Settings.FontSize -= 0.3;
                this.PlotManager.Settings.Persist();
            });
        }

        public void InitializeSearchBox()
        {
            MainWindow.Instance.TextChangeWorkaround = (text) =>
            {
                Search = text;
                LoadPlotLogs();
                return true;
            };
        }

        public async void AddFolder()
        {
            OpenFolderDialog picker = new OpenFolderDialog();
            var result = await picker.ShowAsync(MainWindow.Instance);
            AddFolder(result);
        }

        public void AddFolder(string folder)
        {
            if (Directory.Exists(folder) && !PlotManager.Settings.LogDirectories.Contains(folder))
            {
                PlotManager.AddLogFolder(folder);
                PlotManager.Settings.Persist();
                LoadPlotLogs();
            }
        }

        public void RemoveFolder(string folder)
        {
            PlotManager.RemoveLogFolder(folder);
            PlotManager.Settings.Persist();
            LoadPlotLogs();
        }

        public void SetGridHeight()
        {
            double height = MainWindow.Instance.Height - 600;
            MainWindow.Instance.Find<DataGrid>("PlotLogGrid").Height = height;
        }

    }
}
