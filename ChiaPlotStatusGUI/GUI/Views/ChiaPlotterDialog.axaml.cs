using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using ChiaPlotStatus.GUI.Models;
using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatus.Logic.Utils;
using ChiaPlotStatusGUI.GUI.Utils;
using ChiaPlotStatusGUI.GUI.ViewModels;
using ChiaPlotStatusLib.Logic.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reactive;
using System.Text.RegularExpressions;

namespace ChiaPlotStatus.Views
{
    public class ChiaPlotterDialog : Window
    {
        public Language Language { get; set; }
        public List<(CPPlotLog, CPPlotLogReadable)> PlotLogTuples { get; set; }
        public ObservableCollection<CPPlotLogReadable> PlotLogs { get; set; } = new();
        public ChiaPlotStatus PlotManager { get; set; }
        public string SortProperty { get; set; } = "Tmp1Drive";
        public string Search { get; set; } = "";
        public bool SortAsc { get; set; } = true;


        public ChiaPlotterDialog()
        {
        }

        public ChiaPlotterDialog(ChiaPlotStatus plotManager, Language language, string theme)
        {
            this.DataContext = this;
            this.Language = language;
            this.PlotManager = plotManager;
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            KeepGridScrollbarOnScreen();
            LoadData();
            this.WindowState = WindowState.Maximized;
            Utils.SetTheme(this, theme);
            this.Focus();
        }

        private void LoadData()
        {
            this.Find<DataGrid>("LogDataGrid").BeginBatchUpdate();
            PlotLogs.Clear();
            PlotLogTuples = new();
            foreach (var plotLog in PlotManager.PollCPPlotLogs(PlotManager.Settings.SortProperty, (bool)PlotManager.Settings.SortAsc, Search, PlotManager.Settings.Filter))
            {
                PlotLogs.Add(plotLog.Item2);
                PlotLogTuples.Add(plotLog);
            }
            // PlotCounts = new(PlotLogTuples);
            // HandleFinishDateVisibility();
            this.Find<DataGrid>("LogDataGrid").EndBatchUpdate();
            //this.RaisePropertyChanged("PlotCounts");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        public void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            // TODO: SelectionChangedAction();
        }

        public void OpenLogViewerWindow(object sender, RoutedEventArgs e)
        {
            var plotLogReadable = (PlotLogReadable)((Button)sender).Tag;
            var path = plotLogReadable.LogFolder + Path.DirectorySeparatorChar + plotLogReadable.LogFile;
            Utils.OpenLogFile(path);
        }

        public void LogDataGridHeaderClick(object sender, RoutedEventArgs e)
        {
            Button header = ((Button)sender);
            string headerText = (string)header.Content;
            var oldSortProperty = SortProperty;
            foreach (var property in typeof(GUI.Models.Columns).GetProperties())
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
            Sorter.Sort(SortProperty, SortAsc, PlotLogTuples);
            PlotLogs.Clear();
            foreach (var tuple in PlotLogTuples)
                PlotLogs.Add(tuple.Item2);
            // Debug.WriteLine("SearchProperty: " + SortProperty + ", ASC: " + SortAsc);
        }

        private void KeepGridScrollbarOnScreen()
        {
            this.WhenAnyValue(x => x.Height)
                .Subscribe(x =>
                {
                    var logDataGrid = this.Find<DataGrid>("LogDataGrid");
                    logDataGrid.Height = x - 145;
                });
        }

    }
}
