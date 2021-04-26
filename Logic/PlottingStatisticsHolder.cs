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
    class PlottingStatisticsHolder
    {
        private readonly ConcurrentBag<PlotLog> AllPlotLogs = new ConcurrentBag<PlotLog>();
        private readonly PlottingStatisticsIdRelevanceWeights weights;

        public PlottingStatisticsHolder(List<PlotLog> plotLogs, PlottingStatisticsIdRelevanceWeights weights)
        {
            foreach (var plotLog in plotLogs)
            {
                if (plotLog.TotalSeconds > 0)
                {
                    AllPlotLogs.Add(plotLog);
                }
            }
            this.weights = weights;
        }

        public PlottingStatistics GetMostRelevantStatistics(PlotLog plotLog)
        {
            PlottingStatisticsID id = new PlottingStatisticsID(plotLog);
            ConcurrentDictionary<int, ConcurrentBag<PlotLog>> byRelevance = new ConcurrentDictionary<int, ConcurrentBag<PlotLog>>();
            Parallel.ForEach(AllPlotLogs, (plotLog) =>
            {
                int relevance = id.CalcRelevance(new PlottingStatisticsID(plotLog), weights);
                // Debug.WriteLine(relevance);
                byRelevance.GetOrAdd(relevance, (_) => new ConcurrentBag<PlotLog>()).Add(plotLog);
            });

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
                ConcurrentBag<PlotLog> plotLogs;
                _ = byRelevance.TryGetValue(highestRelevance, out plotLogs);
                return new PlottingStatistics(plotLogs.ToList());
            }
            return new PlottingStatistics(AllPlotLogs.ToList());
        }

    }
}
