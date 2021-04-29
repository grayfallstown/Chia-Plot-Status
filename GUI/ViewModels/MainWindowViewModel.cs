using Avalonia.Controls;
using Avalonia.Interactivity;
using ChiaPlotStatus;
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
        public PlotManager PlotManager { get; } = new PlotManager();
        public ObservableCollection<PlotLogUI> PlotLogs { get; } = new ObservableCollection<PlotLogUI>();
        public ObservableCollection<string> LogDirectories { get; } = new ObservableCollection<string>();
        public ReactiveCommand<Unit, Unit> AddFolderCommand { get; set; }
        public ReactiveCommand<string, Unit> RemoveFolderCommand { get; set; }

        public MainWindowViewModel()
        {
            InitializeChiaPlotStatus();
            InitializeButtons();
        }

        public void InitializeChiaPlotStatus()
        {
            PlotManager.AddDefaultLogFolder();
            UpdateLogDirectories();
            LoadPlotLogs();
        }

        public void LoadPlotLogs()
        {
            PlotLogs.Clear();
            foreach (var plotLog in PlotManager.PollPlotLogs())
                PlotLogs.Add(new PlotLogUI(plotLog));
        }

        public void InitializeButtons()
        {
            AddFolderCommand = ReactiveCommand.Create(AddFolder);
            RemoveFolderCommand = ReactiveCommand.Create<string>(RemoveFolder);
            MainWindow.Instance.BtnClickWOrkaround = (folder) =>
            {
                RemoveFolder(folder);
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
            if (Directory.Exists(folder) && !PlotManager.LogDirectories.Contains(folder))
            {
                PlotManager.AddLogFolder(folder);
                // SaveConfig(); // TODO
                LoadPlotLogs();
                UpdateLogDirectories();
            }
        }

        public void RemoveFolder(string folder)
        {
            PlotManager.RemoveLogFolder(folder);
            // TODO: SaveConfig();
            LoadPlotLogs();
            UpdateLogDirectories();
        }

        public void UpdateLogDirectories()
        {
            LogDirectories.Clear();
            foreach(var folder in PlotManager.LogDirectories)
            {
                LogDirectories.Add(folder);
            }
        }

    }
}
