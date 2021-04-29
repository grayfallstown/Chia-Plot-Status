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
    public class PlotManager
    {
        public Settings Settings { get; }
        private Dictionary<string, PlotLogFile> PlotLogFiles { get; } = new Dictionary<string, PlotLogFile>();
        public PlottingStatisticsIdRelevanceWeights Weights { get; } = new PlottingStatisticsIdRelevanceWeights();
        public PlottingStatisticsHolder Statistics { get; set; }

        public PlotManager(Settings settings) {
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

        public List<PlotLog> PollPlotLogs(string? searchString)
        {
            SearchForNewLogFiles();
            ConcurrentBag<PlotLog> plotLogs = ParseTheLogs();
            List<PlotLog> result = Filter(searchString, plotLogs.ToList());
            SortPlotLogs(result);
            HandleStatistics(result);
            return result;
        }

        public List<PlotLogReadable> PollPlotLogReadables(string? searchString)
        {
            SearchForNewLogFiles();
            List<PlotLog> plotLogs = ParseTheLogs().ToList();
            HandleStatistics(plotLogs);
            SortPlotLogs(plotLogs);
            List<PlotLogReadable> result = new List<PlotLogReadable>();
            foreach (var plotLog in plotLogs)
                result.Add(new PlotLogReadable(plotLog));
            result = Filter(searchString, result);
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

        private List<T> Filter<T>(string? searchString, List<T> plotLogs)
        {
            return SearchFilter.Search<T>(searchString, plotLogs);
        }

        private static void SortPlotLogs(List<PlotLog> plotLogs)
        {
            plotLogs.Sort((a, b) =>
            {
                if (a.PercentDone > b.PercentDone)
                {
                    return 1;
                }
                else if (a.PercentDone < b.PercentDone)
                {
                    return -1;
                }
                else
                {
                    return string.CompareOrdinal(a.Tmp1Drive, b.Tmp1Drive);
                }
            });
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
