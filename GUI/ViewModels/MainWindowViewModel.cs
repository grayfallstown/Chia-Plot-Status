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
        public PlotManager PlotManager { get; internal set; }
        public ObservableCollection<PlotLogReadable> PlotLogs { get; } = new();
        public ReactiveCommand<Unit, Unit> AddFolderCommand { get; set; }
        public ReactiveCommand<string, Unit> RemoveFolderCommand { get; set; }

        public MainWindowViewModel()
        {
            InitializeChiaPlotStatus();
            InitializeButtons();
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
            foreach (var plotLog in PlotManager.PollPlotLogs())
                PlotLogs.Add(new PlotLogReadable(plotLog));
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

    }
}
