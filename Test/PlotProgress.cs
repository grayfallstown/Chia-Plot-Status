/*
using ChiaPlotStatus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace BankTests
{
    [TestClass]
    public class PlotProgressTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            PlotLog plotLog = new PlotLog();
            plotLog.Buckets = 128;
            DateTime? lastEta = DateTime.Now.AddDays(100);
            int lastTimeRemaining = 9999999;
            float lastProgress = 0;

            var plotLogs = new List<PlotLog>();
            plotLogs.Add(plotLog);
            var stats = new PlottingStatistics(plotLogs);
            stats.Phase1Completed = 1;
            stats.Phase2Completed = 1;
            stats.Phase3Completed = 1;
            stats.Phase4Completed = 1;
            stats.CopyTimeCompleted = 1;
            stats.Phase1AvgTimeNeed = 1000;
            stats.Phase2AvgTimeNeed = 2000;
            stats.Phase3AvgTimeNeed = 3000;
            stats.Phase4AvgTimeNeed = 4000;
            stats.CopyTimeAvgTimeNeed = 5000;

            void Check()
            {
                plotLog.UpdateProgress();
                plotLog.UpdateEta(stats);
                if (plotLog.ETA >= lastEta)
                    throw new Exception("ETA " + plotLog.CurrentPhase + "-" + plotLog.CurrentTable + "-" + plotLog.CurrentBucket);
                else
                    lastEta = plotLog.ETA;
                if (plotLog.TimeRemaining >= lastTimeRemaining)
                    throw new Exception("TimeRemaining " + plotLog.CurrentPhase + "-" + plotLog.CurrentTable + "-" + plotLog.CurrentBucket);
                else
                    lastTimeRemaining = plotLog.TimeRemaining;
                if (plotLog.Progress <= lastProgress)
                    throw new Exception("Progress " + plotLog.CurrentPhase + "-" + plotLog.CurrentTable + "-" + plotLog.CurrentBucket);
                else
                    lastProgress = plotLog.Progress;
            }

            for (int phase = 1; phase < 6; phase++)
            {
                plotLog.CurrentPhase = phase;
                plotLog.CurrentBucket = 0;
                plotLog.CurrentTable = 0;
                switch(phase)
                {
                    case 1:
                    case 3:
                        for (int table = 0; table < 8; table++)
                        {
                            plotLog.CurrentTable = table;
                            for (int bucket = 0; bucket <= 128; bucket++)
                            {
                                plotLog.CurrentBucket = bucket;
                                Check();
                            }
                        }
                        break;
                    case 2:
                        for (int table = 7; table > 0; table++)
                        {
                            plotLog.CurrentTable = table;
                            Check();
                        }
                        break;
                    case 4:
                        for (int bucket = 0; bucket <= 128; bucket++)
                        {
                            plotLog.CurrentBucket = bucket;
                            Check();
                        }
                        break;
                    case 5:
                    case 6:
                        break;
                }
            }
        }
    }
}
*/
