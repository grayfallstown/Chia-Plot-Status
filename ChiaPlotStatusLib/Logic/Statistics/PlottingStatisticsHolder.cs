using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatusGUI.GUI.ViewModels;
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
     * Collects Statistics on plotting processes and can give you a statistic most
     * relevant to your PlogLog
     */
    public class PlottingStatisticsHolder
    {
        private readonly ConcurrentBag<PlotLog> FinishedPlotLogs = new ConcurrentBag<PlotLog>();
        private readonly PlottingStatisticsIdRelevanceWeights weights;
        private readonly Dictionary<DateTime, PlottingStatisticsDay> Days = new();

        public PlottingStatisticsHolder(List<PlotLog> plotLogs, PlottingStatisticsIdRelevanceWeights weights, List<MarkOfDeath> markOfDeaths)
        {
            this.weights = weights;

            PlottingStatisticsDay GetOrCreatePlottingStatisticsDay(DateTime? dateTime)
            {
                if (dateTime == null)
                    // silently give them an instance that gets thrown away
                    return new PlottingStatisticsDay(DateTime.Now.Date);
                DateTime day = ((DateTime)dateTime).Date;
                if (!Days.ContainsKey(day))
                    Days.Add(day, new PlottingStatisticsDay(day));
                return Days[day];
            }

            foreach (var plotLog in plotLogs)
            {
                GetOrCreatePlottingStatisticsDay(plotLog.StartDate).Phase1++;
                GetOrCreatePlottingStatisticsDay(plotLog.FinishDate).Finished++;
                // max reached moment the process was likely still running. Not very precise when --num is used.
                // could use currentTable + currentBucket and ETA-like Calculation to improve estimate in that case.
                // Should be a noticable improvement on HDDs
                DateTime? max = plotLog.StartDate;
                if (plotLog.Phase1Seconds != 0)
                {
                    var phase2 = plotLog.StartDate.Value.AddSeconds(plotLog.Phase1Seconds);
                    max = phase2;
                    GetOrCreatePlottingStatisticsDay(phase2).Phase2++;
                    if (plotLog.Phase2Seconds != 0)
                    {
                        var phase3 = phase2.AddSeconds(plotLog.Phase2Seconds);
                        max = phase3;
                        GetOrCreatePlottingStatisticsDay(phase3).Phase3++;
                        if (plotLog.Phase3Seconds != 0)
                        {
                            var phase4 = phase3.AddSeconds(plotLog.Phase3Seconds);
                            max = phase4;
                            GetOrCreatePlottingStatisticsDay(phase4).Phase4++;
                            if (plotLog.Phase4Seconds != 0)
                            {
                                var phase5 = phase4.AddSeconds(plotLog.Phase4Seconds);
                                max = phase5;
                                GetOrCreatePlottingStatisticsDay(phase5).Phase5++;
                            }
                        }
                    }
                }

                foreach (var mark in markOfDeaths)
                {
                    if (mark.IsMatch(plotLog) && mark.DiedAt != null)
                    {
                        max = mark.DiedAt.Value;
                        break;
                    }
                }
                if (plotLog.Health is ConfirmedDead)
                {
                    // if --num is not used then lastModifiedAt property on the log file should be very
                    // near our time of death.
                    // if --num is used we can only assume the above if this was the last plot in queue
                    // plotLog.PlaceInLogFile == plotLog.QueueSize catches both cases
                    if (plotLog.PlaceInLogFile == plotLog.QueueSize)
                    {
                        max = File.GetLastWriteTime(plotLog.LogFile);
                    }
                    GetOrCreatePlottingStatisticsDay(max).Died++;
                }

                if (plotLog.TotalSeconds > 0)
                    FinishedPlotLogs.Add(plotLog);
            }
        }

        public PlottingStatistics GetMostRelevantStatistics(PlotLog plotLog)
        {
            PlottingStatisticsID id = new PlottingStatisticsID(plotLog);
            return GetMostRelevantStatistics(id);
        }

        public PlottingStatistics GetMostRelevantStatistics(PlottingStatisticsID id)
        {
            Dictionary<int, List<PlotLog>> byRelevance = new Dictionary<int, List<PlotLog>>();
            foreach (var fromAll in FinishedPlotLogs)
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
            if (FinishedPlotLogs.Count > 0)
                return new PlottingStatistics(FinishedPlotLogs.ToList());
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
            foreach (var plotLog in FinishedPlotLogs)
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
