﻿using ChiaPlotStatus;
using ChiaPlotStatusLib.Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus
{
    public class PlottingStatisticsFullReadable
    {
        public string LogFolder { get; set; }
        public string Tmp1Drive { get; set; }
        public string Tmp2Drive { get; set; }
        public int PlotSize { get; set; }
        public int Threads { get; set; }
        public string Buffer { get; set; }
        public int Buckets { get; set; }

        public string Phase1AvgTimeNeed { get; set; }
        public int Phase1Completed { get; set; }
        public string Phase2AvgTimeNeed { get; set; }
        public int Phase2Completed { get; set; }
        public string Phase3AvgTimeNeed { get; set; }
        public int Phase3Completed { get; set; }
        public string Phase4AvgTimeNeed { get; set; }
        public int Phase4Completed { get; set; }
        public string CopyTimeAvgTimeNeed { get; set; }
        public int CopyTimeCompleted { get; set; }
        public string TotalAvgTimeNeed { get; set; }

        public PlottingStatisticsFullReadable(PlottingStatisticsFull stats)
        {
            this.LogFolder = stats.LogFolder;
            this.Tmp1Drive = stats.Tmp1Drive;
            this.Tmp2Drive = stats.Tmp2Drive;
            this.PlotSize = stats.PlotSize;
            this.Threads = stats.Threads;
            this.Buffer = stats.Buffer + "MB";
            this.Buckets = stats.Buckets;
            this.Phase1AvgTimeNeed = Formatter.formatSeconds(stats.Phase1AvgTimeNeed, true);
            this.Phase1Completed = stats.Phase1Completed;
            this.Phase2AvgTimeNeed = Formatter.formatSeconds(stats.Phase2AvgTimeNeed, true);
            this.Phase2Completed = stats.Phase2Completed;
            this.Phase3AvgTimeNeed = Formatter.formatSeconds(stats.Phase3AvgTimeNeed, true);
            this.Phase3Completed = stats.Phase3Completed;
            this.Phase4AvgTimeNeed = Formatter.formatSeconds(stats.Phase4AvgTimeNeed, false);
            this.Phase4Completed = stats.Phase4Completed;
            this.CopyTimeAvgTimeNeed = Formatter.formatSeconds(stats.CopyTimeAvgTimeNeed, false);
            this.CopyTimeCompleted = stats.CopyTimeCompleted;
            this.TotalAvgTimeNeed = Formatter.formatSeconds(stats.TotalAvgTimeNeed, true);
        }
    }
}
