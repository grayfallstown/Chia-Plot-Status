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
    class PlotManager
    {
        public List<string> LogDirectories { get; } = new List<string>();
        private Dictionary<string, PlotLogFile> PlotLogFiles { get; } = new Dictionary<string, PlotLogFile>();
        public PlottingStatisticsIdRelevanceWeights Weights { get; } = new PlottingStatisticsIdRelevanceWeights();
        public PlottingStatisticsHolder Statistics { get; set; }

        public PlotManager() { }

        public void AddLogFolder(string path)
        {
            LogDirectories.Add(path);
        }

        public void RemoveLogFolder(string path)
        {
            LogDirectories.Remove(path);
            // drop plotlogs from that folder
            foreach (var plotLogFile in PlotLogFiles.Values)
            {
                var folder = plotLogFile.LogFolder + "\\";
                if (string.Equals(folder, path))
                   PlotLogFiles.Remove(plotLogFile.LogFile);
            }
        }

        public List<PlotLog> PollPlotLogs()
        {
            SearchForNewLogFiles();
            ConcurrentBag<PlotLog> plotLogs = ParseTheLogs();
            List<PlotLog> result = SortPlotLogs(plotLogs);
            HandleStatistics(result);
            return result;
        }

        private void SearchForNewLogFiles()
        {
            foreach (var directory in LogDirectories)
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

        private static List<PlotLog> SortPlotLogs(ConcurrentBag<PlotLog> plotLogs)
        {
            var result = plotLogs.ToList();
            result.Sort((a, b) =>
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
            return result;
        }

        private void HandleStatistics(List<PlotLog> result)
        {
            Statistics = new PlottingStatisticsHolder(result, Weights);
            Parallel.ForEach(result, (plotLog) =>
            {
                PlottingStatistics stats = Statistics.GetMostRelevantStatistics(plotLog);
                plotLog.UpdateEta(stats);
            });
        }

    }

}
