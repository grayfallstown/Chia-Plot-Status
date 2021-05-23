using ChiaPlotStatusGUI.GUI.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus
{

    /**
     * Collects Statistics on finished plotting processes and can give you
     * a statistic most relevant to your PlogLog
     */
    public class PlottingStatisticsHolder
    {
        private readonly ConcurrentBag<PlotLog> AllPlotLogs = new ConcurrentBag<PlotLog>();
        private readonly PlottingStatisticsIdRelevanceWeights weights;

        public PlottingStatisticsHolder(List<PlotLog> plotLogs, PlottingStatisticsIdRelevanceWeights weights)
        {
            foreach (var plotLog in plotLogs)
                if (plotLog.TotalSeconds > 0)
                    AllPlotLogs.Add(plotLog);
            this.weights = weights;
        }

        public PlottingStatistics GetMostRelevantStatistics(PlotLog plotLog)
        {
            PlottingStatisticsID id = new PlottingStatisticsID(plotLog);
            return GetMostRelevantStatistics(id);
        }

        public PlottingStatistics GetMostRelevantStatistics(PlottingStatisticsID id)
        {
            Dictionary<int, List<PlotLog>> byRelevance = new Dictionary<int, List<PlotLog>>();
            foreach (var fromAll in AllPlotLogs)
            {
                int relevance = id.CalcRelevance(new PlottingStatisticsID(fromAll), weights);
                // Debug.WriteLine(relevance);
                if (!byRelevance.ContainsKey(relevance))
                    byRelevance.Add(relevance, new List<PlotLog>());
                byRelevance[relevance].Add(fromAll);
            }

            int highestRelevance = -1;
            foreach (var relevance in byRelevance.Keys)
            {
                if (relevance > highestRelevance)
                {
                    highestRelevance = relevance;
                }
            }

            if (highestRelevance > -1)
            {
                List<PlotLog> plotLogs = byRelevance[highestRelevance];
                return new PlottingStatistics(plotLogs.ToList());
            }
            if (AllPlotLogs.Count > 0)
                return new PlottingStatistics(AllPlotLogs.ToList());
            else
                return MagicNumbers();
        }

        /**
         * An average over some plotlogs I had on my drive.
         * This way we can display ETA and Time Remaining even
         * when the user does not have finished plotlogs (will
         * be imprecise tho).
         * That way we get better warning and error thresholds
         * from the beginning too, as they were diced before.
         * See Issue #23
         */
        private PlottingStatistics MagicNumbers()
        {
            List<PlotLog> plotLogs = new List<PlotLog>();
            var magicPlotLog = new PlotLog();
            magicPlotLog.Phase1Seconds = 31573;
            magicPlotLog.Phase2Seconds = 12298;
            magicPlotLog.Phase3Seconds = 34925;
            magicPlotLog.Phase4Seconds = 3024;
            magicPlotLog.CopyTimeSeconds = 934;
            plotLogs.Add(magicPlotLog);
            return new PlottingStatistics(plotLogs);
        }

        public List<(PlottingStatisticsFull, PlottingStatisticsFullReadable)> AllStatistics()
        {
            Dictionary<PlottingStatisticsID, PlottingStatistics> statsDict = new();
            foreach (var plotLog in AllPlotLogs)
            {
                var id = new PlottingStatisticsID(plotLog);
                if (!statsDict.ContainsKey(id))
                    statsDict.Add(id, this.GetMostRelevantStatistics(plotLog));
            }
            List<(PlottingStatisticsFull, PlottingStatisticsFullReadable)> statsFull = new();
            foreach (var entry in statsDict) {
                var stat = new PlottingStatisticsFull(entry.Key, entry.Value);
                statsFull.Add((stat, new PlottingStatisticsFullReadable(stat)));
            }
            return statsFull;
        }

    }
}
