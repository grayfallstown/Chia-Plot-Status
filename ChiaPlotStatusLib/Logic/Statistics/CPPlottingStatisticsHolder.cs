using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatusGUI.GUI.ViewModels;
using ChiaPlotStatusLib.Logic.Models;
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
    public class CPPlottingStatisticsHolder
    {
        private readonly ConcurrentBag<CPPlotLog> FinishedPlotLogs = new();
        private readonly PlottingStatisticsIdRelevanceWeights weights;
        private readonly Dictionary<DateTime, CPPlottingStatisticsDay> Days = new();

        public CPPlottingStatisticsHolder(List<CPPlotLog> plotLogs, PlottingStatisticsIdRelevanceWeights weights, List<MarkOfDeath> markOfDeaths)
        {
            this.weights = weights;

            /*
            CPPlottingStatisticsDay GetOrCreatePlottingStatisticsDay(DateTime? dateTime)
            {
                if (dateTime == null)
                    // silently give them an instance that gets thrown away
                    return new CPPlottingStatisticsDay(DateTime.Now.Date);
                DateTime day = ((DateTime)dateTime).Date;
                if (!Days.ContainsKey(day))
                    Days.Add(day, new CPPlottingStatisticsDay(day));
                return Days[day];
            }

            foreach (var plotLog in plotLogs)
            {
                GetOrCreatePlottingStatisticsDay(plotLog.StartDate).Phase1++;
                //GetOrCreatePlottingStatisticsDay(plotLog.FinishDate).Finished++;
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
            */
        }

        public CPPlottingStatistics GetMostRelevantStatistics(CPPlotLog plotLog)
        {
            CPPlottingStatisticsID id = new(plotLog);
            return GetMostRelevantStatistics(id);
        }

        public CPPlottingStatistics GetMostRelevantStatistics(CPPlottingStatisticsID id)
        {
            Dictionary<int, List<CPPlotLog>> byRelevance = new Dictionary<int, List<CPPlotLog>>();
            foreach (var fromAll in FinishedPlotLogs)
            {
                int relevance = id.CalcRelevance(new CPPlottingStatisticsID(fromAll), weights);
                // Debug.WriteLine(relevance);
                if (!byRelevance.ContainsKey(relevance))
                    byRelevance.Add(relevance, new List<CPPlotLog>());
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
                List<CPPlotLog> plotLogs = byRelevance[highestRelevance];
                return new CPPlottingStatistics(plotLogs.ToList());
            }
            if (FinishedPlotLogs.Count > 0)
                return new CPPlottingStatistics(FinishedPlotLogs.ToList());
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
        private CPPlottingStatistics MagicNumbers()
        {
            // TODO:
            List<CPPlotLog> plotLogs = new List<CPPlotLog>();
            var magicPlotLog = new CPPlotLog();
            magicPlotLog.P1 = 42;
            magicPlotLog.P2 = 42;
            magicPlotLog.P3 = 42;
            magicPlotLog.P4 = 42;
            //magicPlotLog.P5 = 42;
            magicPlotLog.Total = 42;
            plotLogs.Add(magicPlotLog);
            return new CPPlottingStatistics(plotLogs);
        }

        /*
        public List<(CPPlottingStatisticsFull, CPPlottingStatisticsFullReadable)> AllStatistics()
        {
            Dictionary<CPPlottingStatisticsID, CPPlottingStatistics> statsDict = new();
            foreach (var plotLog in FinishedPlotLogs)
            {
                var id = new CPPlottingStatisticsID(plotLog);
                if (!statsDict.ContainsKey(id))
                    statsDict.Add(id, this.GetMostRelevantStatistics(plotLog));
            }
            List<(CPPlottingStatisticsFull, CPPlottingStatisticsFullReadable)> statsFull = new();
            foreach (var entry in statsDict) {
                var stat = new CPPlottingStatisticsFull(entry.Key, entry.Value);
                statsFull.Add((stat, new CPPlottingStatisticsFullReadable(stat)));
            }
            return statsFull;
        }
        */

        public Dictionary<DateTime, CPPlottingStatisticsDay> GetDailyStats()
        {
            return new(Days);
        }

    }
}
