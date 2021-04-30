using ChiaPlottStatus.GUI.Models;
using ChiaPlottStatus.Logic.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus
{
    /**
     * Core Object of this tool.
     * Knows everything, does everything.
     */
    public class ChiaPlotStatus
    {
        public Settings Settings { get; }
        private Dictionary<string, PlotLogFile> PlotLogFiles { get; } = new Dictionary<string, PlotLogFile>();
        public PlottingStatisticsIdRelevanceWeights Weights { get; } = new PlottingStatisticsIdRelevanceWeights();
        public PlottingStatisticsHolder Statistics { get; set; }

        public ChiaPlotStatus(Settings settings) {
            this.Settings = settings;
            Statistics = new PlottingStatisticsHolder(new List<PlotLog>(), Weights);
        }

        public void AddDefaultLogFolder()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                                            + @"\.chia\mainnet\plotter\";
            AddLogFolder(path);
        }

        public void AddLogFolder(string path)
        {
            Settings.LogDirectories.Add(path);
        }

        public void RemoveLogFolder(string path)
        {
            Settings.LogDirectories.Remove(path);
            // drop plotlogs from that folder
            foreach (var plotLogFile in PlotLogFiles.Values)
            {
                var folder = plotLogFile.LogFolder + "\\";
                if (string.Equals(folder, path))
                   PlotLogFiles.Remove(plotLogFile.LogFile);
            }
        }

        public List<(PlotLog, PlotLogReadable)> PollPlotLogs(string? searchString, string sortPropertyName, bool sortAsc)
        {
            SearchForNewLogFiles();
            ConcurrentBag<PlotLog> plotLogs = ParseTheLogs();
            HandleStatistics(plotLogs.ToList());
            List<(PlotLog, PlotLogReadable)> plusReadable = new();
            foreach (var plotLog in plotLogs)
                plusReadable.Add((plotLog, new PlotLogReadable(plotLog)));
            List<(PlotLog, PlotLogReadable)> result = Filter(searchString, plusReadable);
            SortPlotLogs(sortPropertyName, sortAsc, result);
            return result;
        }

        private void SearchForNewLogFiles()
        {
            foreach (var directory in Settings.LogDirectories)
            {
                foreach (var filePath in Directory.GetFiles(directory))
                {
                    if (!PlotLogFiles.ContainsKey(filePath))
                    {
                        PlotLogFiles[filePath] = new PlotLogFile(filePath);
                    }
                }
            }
        }

        private ConcurrentBag<PlotLog> ParseTheLogs()
        {
            ConcurrentBag<PlotLog> plotLogs = new ConcurrentBag<PlotLog>();
            Parallel.ForEach(PlotLogFiles.Values, (plotLogFile) =>
            {
                foreach (var plotLog in plotLogFile.ParsePlotLog())
                {
                    plotLogs.Add(plotLog);
                }
            });
            return plotLogs;
        }

        private List<(PlotLog, PlotLogReadable)> Filter(string? searchString, List<(PlotLog, PlotLogReadable)> plotLogs)
        {
            return SearchFilter.Search<PlotLog, PlotLogReadable>(searchString, plotLogs);
        }

        private static void SortPlotLogs(string propertyName, bool sortAsc, List<(PlotLog, PlotLogReadable)> plotLogs)
        {
            Sorter.Sort(propertyName, sortAsc, plotLogs);
        }

        private void HandleStatistics(List<PlotLog> plotLogs)
        {
            Statistics = new PlottingStatisticsHolder(plotLogs, Weights);
            //Parallel.ForEach(result, (plotLog) =>
            foreach (var plotLog in plotLogs)
            {
                PlottingStatistics stats = Statistics.GetMostRelevantStatistics(plotLog);
                plotLog.UpdateEta(stats);
            }
        }

    }

}
