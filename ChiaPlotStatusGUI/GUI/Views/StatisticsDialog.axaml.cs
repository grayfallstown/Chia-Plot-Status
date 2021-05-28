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
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Reactive;
using System.Text.RegularExpressions;

namespace ChiaPlotStatus.Views
{
    public class StatisticsDialog : Window
    {
        public Language Language { get; set; }
        public List<(PlottingStatisticsFull, PlottingStatisticsFullReadable)> StatsTuples { get; set; }
        public ObservableCollection<PlottingStatisticsFullReadable> Stats { get; set; } = new();
        public List<PlottingStatisticsDayReadable> DailyStats { get; set; }
        public ChiaPlotStatus PlotManager { get; set; }
        public string SortProperty { get; set; } = "Tmp1Drive";
        public bool SortAsc { get; set; } = true;


        public StatisticsDialog()
        {
        }

        public StatisticsDialog(ChiaPlotStatus plotManager, Language language, string theme)
        {
            this.DataContext = this;
            this.Language = language;
            this.PlotManager = plotManager;
            LoadData();
            InitDailyStatsTable();
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            KeepGridScrollbarOnScreen();
            this.WindowState = WindowState.Maximized;
            Utils.SetTheme(this, theme);
            this.Focus();
        }

        private void LoadData()
        {
            this.StatsTuples = PlotManager.Statistics.AllStatistics();
            Sorter.Sort(SortProperty, SortAsc, StatsTuples);
            Stats.Clear();
            foreach (var tuple in this.StatsTuples)
                Stats.Add(tuple.Item2);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void DataGridHeaderClick(object sender, RoutedEventArgs e)
        {
            Button header = ((Button)sender);
            string headerText = (string)header.Content;
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
            Sorter.Sort(SortProperty, SortAsc, StatsTuples);
            Stats.Clear();
            foreach (var tuple in StatsTuples)
                Stats.Add(tuple.Item2);
            // Debug.WriteLine("SearchProperty: " + SortProperty + ", ASC: " + SortAsc);
        }

        private void KeepGridScrollbarOnScreen()
        {
            this.WhenAnyValue(x => x.Height)
                .Subscribe(x =>
                {
                    var statsDataGrid = this.Find<DataGrid>("StatsDataGrid");
                    var dailyStatsDataGrid = this.Find<DataGrid>("DailyStatsDataGrid");
                    statsDataGrid.Height = x / 2;
                    dailyStatsDataGrid.Height = x / 2;
                });
        }

        public void InitDailyStatsTable()
        {
            Dictionary<DateTime, PlottingStatisticsDay> dailyStats = PlotManager.Statistics.GetDailyStats();
            DailyStats = new();
            foreach (var stat in dailyStats.Values)
                DailyStats.Add(new(stat));
            DailyStats.Sort((a, b) => -1 * a.Day.CompareTo(b.Day));
        }

        /*
        private void BuildPlotModel()
        {
            Dictionary<DateTime, PlottingStatisticsDay> dailyStats = PlotManager.Statistics.GetDailyStats();

            Dictionary<string, LineSeries> lineSeries = new();

            LineSeries getLineSeries(string name)
            {
                if (!lineSeries.ContainsKey(name))
                    lineSeries.Add(name, new LineSeries());
                return lineSeries[name];
            }


            var plotModel = new PlotModel
            {
                Title = "test1",
                TitleToolTip = "test2"
            };

            DateTime twoWeeksAgo = DateTime.Now.AddDays(-14);
            plotModel.Axes.Add(new DateTimeAxis {
                Position = AxisPosition.Bottom,
                Minimum = DateTimeAxis.ToDouble(twoWeeksAgo),
                Maximum = DateTimeAxis.ToDouble(DateTime.Now),
                StringFormat = "MMM dd" });

            List<PlottingStatisticsDay> stats = new(dailyStats.Values);
            stats.Sort((a, b)=> a.Day.CompareTo(b.Day));
            foreach (var stat in stats)
            {
                getLineSeries("Phase1").Points.Add(new DataPoint(DateTimeAxis.ToDouble(stat.Day), stat.Phase1));
                getLineSeries("Phase2").Points.Add(new DataPoint(DateTimeAxis.ToDouble(stat.Day), stat.Phase2));
                getLineSeries("Phase3").Points.Add(new DataPoint(DateTimeAxis.ToDouble(stat.Day), stat.Phase3));
                getLineSeries("Phase4").Points.Add(new DataPoint(DateTimeAxis.ToDouble(stat.Day), stat.Phase4));
                getLineSeries("Phase5").Points.Add(new DataPoint(DateTimeAxis.ToDouble(stat.Day), stat.Phase5));
                getLineSeries("Finished").Points.Add(new DataPoint(DateTimeAxis.ToDouble(stat.Day), stat.Finished));
                getLineSeries("Died").Points.Add(new DataPoint(DateTimeAxis.ToDouble(stat.Day), stat.Died));
            }

            foreach (var series in lineSeries.Values)
                plotModel.Series.Add(series);

            this.PlotModel = plotModel;
        }
        */
    }
}
