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
        private ConcurrentDictionary<string, PlotLogFile> PlotLogFiles { get; } = new ConcurrentDictionary<string, PlotLogFile>();
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
                   PlotLogFiles.TryRemove(plotLogFile.LogFile, out _);
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
            Parallel.ForEach(LogDirectories, (directory) =>
            {
                foreach (var filePath in Directory.GetFiles(directory))
                {
                    PlotLogFiles.GetOrAdd(filePath, (_) => new PlotLogFile(filePath));
                }
            });
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
            // foreach (var plotLog in result) {
                PlottingStatistics stats = Statistics.GetMostRelevantStatistics(plotLog);
                if (stats == null)
                {
                    plotLog.ETA = "N/A";
                }  else
                {
                    plotLog.ETA = CalcEta(plotLog, stats);
                }
            // }
            });
        }

        private string CalcEta(PlotLog plotLog, PlottingStatistics stats)
        {
            if (plotLog.Buckets == 0)
            {
                // log too short to know anything yet
                return "N/A";
            }
            int currentPhase = 1;
            int eta = 0;
            if (Math.Abs(plotLog.PercentDone - 100f) < 0.00001)
            {
                return "done";
            }
            else
            {
                if (plotLog.Phase3Seconds > 0)
                {
                    currentPhase = 4;
                }
                else if (plotLog.Phase2Seconds > 0)
                {
                    currentPhase = 3;
                }
                else if (plotLog.Phase1Seconds > 0)
                {
                    currentPhase = 2;
                }
            }
            if (currentPhase <= 4)
            {
                eta += stats.Phase4AvgTimeNeed;
            }
            if (currentPhase <= 3)
            {
                float factor = 1;
                if (currentPhase == 3)
                {
                    factor = (float) (plotLog.Phase3Table - 1) / 7;
                    factor += (plotLog.CurrentBucket / plotLog.Buckets);
                }
                eta += (int) (factor * stats.Phase3AvgTimeNeed);
            }
            if (currentPhase <= 2)
            {
                float factor = 1;
                if (currentPhase == 2)
                {
                    factor = (float)((7 - plotLog.Phase3Table) - 1) / 7;
                    factor += (plotLog.CurrentBucket / plotLog.Buckets);
                }
                eta += (int)(factor * stats.Phase2AvgTimeNeed);
            }
            if (currentPhase == 1)
            {
                var factor = (float)(plotLog.Phase3Table - 1) / 7;
                if (plotLog.Buckets != 0)
                {
                    factor += (plotLog.CurrentBucket / plotLog.Buckets);
                }
                eta += (int)(factor * stats.Phase1AvgTimeNeed);
            }

            TimeSpan time = TimeSpan.FromSeconds(eta);
            var result = time.ToString(@"hh\:mm\:ss");
            return result;
        }
    }

}


/**
 * Map<string, {
 *      List<Plot>
 *      avgPhase1
 *      avgPhase2
 *      avgPhase3
 *      avgPhase4
 * }
 *
 */