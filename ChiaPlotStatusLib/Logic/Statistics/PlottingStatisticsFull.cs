using ChiaPlotStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatusGUI.GUI.ViewModels
{
    public class PlottingStatisticsFull
    {
        public string LogFolder { get; set; }
        public string Tmp1Drive { get; set; }
        public string Tmp2Drive { get; set; }
        public int PlotSize { get; set; }
        public int Threads { get; set; }
        public int BufferSize { get; set; }
        public int Buckets { get; set; }

        public int Phase1AvgTimeNeed { get; set; }
        public int Phase1Completed { get; set; }
        public int Phase2AvgTimeNeed { get; set; }
        public int Phase2Completed { get; set; }
        public int Phase3AvgTimeNeed { get; set; }
        public int Phase3Completed { get; set; }
        public int Phase4AvgTimeNeed { get; set; }
        public int Phase4Completed { get; set; }
        public int CopyTimeAvgTimeNeed { get; set; }
        public int CopyTimeCompleted { get; set; }
        public int TotalAvgTimeNeed { get; set; }

        public PlottingStatisticsFull(PlottingStatisticsID id, PlottingStatistics stats)
        {
            this.LogFolder = id.LogFolder;
            this.Tmp1Drive = id.Tmp1Drive;
            this.Tmp2Drive = id.Tmp2Drive;
            this.PlotSize = id.PlotSize;
            this.Threads = id.Threads;
            this.BufferSize = id.BufferSize;
            this.Buckets = id.Buckets;
            this.Phase1AvgTimeNeed = stats.Phase1AvgTimeNeed;
            this.Phase1Completed = stats.Phase1Completed;
            this.Phase2AvgTimeNeed = stats.Phase2AvgTimeNeed;
            this.Phase2Completed = stats.Phase2Completed;
            this.Phase3AvgTimeNeed = stats.Phase3AvgTimeNeed;
            this.Phase3Completed = stats.Phase3Completed;
            this.Phase4AvgTimeNeed = stats.Phase4AvgTimeNeed;
            this.Phase4Completed = stats.Phase4Completed;
            this.CopyTimeAvgTimeNeed = stats.CopyTimeAvgTimeNeed;
            this.CopyTimeCompleted = stats.CopyTimeCompleted;
            this.TotalAvgTimeNeed = stats.Phase1AvgTimeNeed + stats.Phase2AvgTimeNeed +
                stats.Phase3AvgTimeNeed + stats.Phase4AvgTimeNeed + stats.CopyTimeAvgTimeNeed;
        }
    }
}
