using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatus.Logic;
using ChiaPlotStatus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChiaPlotStatusLib.Logic.Statistics.Harvest;
using System.Collections.ObjectModel;

namespace ChiaPlotStatus
{
    public class ChiaPlotStatusService
    {
        public ChiaPlotStatus PlotManager { get; internal set; }
        public Settings Settings { get; set; }
        public List<string> SortProperties = new();
        public string SortBy { get; set; }
        public string SearchStringInner { get; set; }
        public PlotCounts PlotCounts { get; set; }

        public event EventHandler OnUpdate;


        public ChiaPlotStatusService()
        {
            // TODO:
            // DeleteUpdateTempDirectory
            // Language

            foreach (var property in typeof(PlotLogReadable).GetProperties())
                SortProperties.Add(property.Name);

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar;
            Settings = new Settings(folder + "ChiaPlotStatu.config.json");
            Settings.Load();
            PlotManager = new(Settings);

            if (PlotManager.Settings.LogDirectories.Count == 0)
                PlotManager.AddDefaultLogFolders();
            PollPlotLogs();

            // make sure OnUpdate is set to something
            OnUpdate += (_, _) => { };

            InitPlotLogsData();
            _ = Task.Run(() => LoadHarvestStatsData());
        }


        public List<PlotLogReadable> PollPlotLogReadables()
        {
            List<(PlotLog, PlotLogReadable)> tuples = PollPlotLogs();
            PlotCounts = new(tuples);
            List<PlotLogReadable> readables = new(tuples.Count);
            foreach (var tuple in tuples)
                readables.Add(tuple.Item2);
            return readables;
        }

        public List<(PlotLog, PlotLogReadable)> PollPlotLogs()
        {
            return PlotManager.PollPlotLogs(PlotManager.Settings.SortProperty,
                (bool)PlotManager.Settings.SortAsc, SearchStringInner, PlotManager.Settings.Filter);
        }



        /*
         * For PlotLogs Page
         */
        public bool HideFinished
        {
            get { return PlotManager.Settings.Filter.HideFinished; }
            set
            {
                PlotManager.Settings.Filter.HideFinished = value;
                LoadPlotData();
            }
        }
        public bool HideConfirmedDead
        {
            get { return PlotManager.Settings.Filter.HideConfirmedDead; }
            set
            {
                PlotManager.Settings.Filter.HideConfirmedDead = value;
                LoadPlotData();
            }
        }
        public bool HidePossiblyDead
        {
            get { return PlotManager.Settings.Filter.HidePossiblyDead; }
            set
            {
                PlotManager.Settings.Filter.HidePossiblyDead = value;
                LoadPlotData();
            }
        }
        public bool HideHealthy
        {
            get { return PlotManager.Settings.Filter.HideHealthy; }
            set
            {
                PlotManager.Settings.Filter.HideHealthy = value;
                LoadPlotData();
            }
        }
        public string SearchString
        {
            get { return SearchStringInner; }
            set
            {
                SearchStringInner = value;
                LoadPlotData();
            }
        }



        protected void InitPlotLogsData()
        {
            ColumnShow = PlotManager.Settings.Columns.ShowColumn;
            ColumnOrder = PlotManager.Settings.Columns.Order;
            ColumnWidths = PlotManager.Settings.Columns.Widths;
            LoadPlotData();
        }


        public IEnumerable<PlotLogReadable> readables;
        public Dictionary<string, bool> ColumnShow;
        public Dictionary<string, int> ColumnWidths;
        public List<string> ColumnOrder;


        public string Content(PlotLogReadable plotLog, string col)
        {
            return (string)plotLog.GetType().GetProperty(col).GetValue(plotLog, null);
        }

        public void SortFn(string col)
        {
            if (PlotManager.Settings.SortProperty == col)
                PlotManager.Settings.SortAsc = !PlotManager.Settings.SortAsc;
            PlotManager.Settings.SortProperty = col;
            LoadPlotData();
        }


        public void LoadPlotData()
        {
            Console.WriteLine("Loading Data " + SearchStringInner);
            readables = PollPlotLogReadables();
            Settings.Persist();
            OnUpdate.Invoke(this, new EventArgs());
        }


        /*
         * HarvestStats
         */
        public List<Tuple<string, HarvestSummary, List<Harvest>>?> Results { get; set; } = new();
        public ObservableCollection<HarvestSummeryReadable> Summaries { get; set; } = new();
        public ObservableCollection<string> PathsWithoutResults { get; set; } = new();


        /*
        public void AddHarvestFolder(string folder)
        {
            if (!Settings.HarvesterLogDirectories.Contains(folder))
            {
                Settings.HarvesterLogDirectories.Add(folder);
                Settings.Persist();
                LoadData();
            }
        }
        */
/*
        public void AddFolder(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                OpenFolderDialog picker = new OpenFolderDialog();
                var result = await picker.ShowAsync(this);
                Dispatcher.UIThread.InvokeAsync(() => AddFolder(result));
            });
        }
*/
        public void RemoveHarvestFolder(string folder)
        {
            PlotManager.Settings.HarvesterLogDirectories.Remove(folder);
            PlotManager.Settings.Persist();
            LoadHarvestStatsData();
        }

        public void LoadHarvestStatsData()
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
            OnUpdate.Invoke(this, new EventArgs());
        }


    }
}
