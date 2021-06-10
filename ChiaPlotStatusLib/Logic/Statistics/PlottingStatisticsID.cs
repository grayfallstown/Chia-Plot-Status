using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus
{

    /**
     * Key used to determine best matches / relevance on finished plotting processes for a given PlotLog
     * Relevance is calculated by summing up weigths assigned to the Properties that are equal between
     * PlotLogs represented by this PlottingStatisticsID
     */
    public class PlottingStatisticsID
    {
        public string UsedPlotter { get; set; }
        public string LogFolder { get; set; }
        public string Tmp1Drive { get; set; }
        public string Tmp2Drive { get; set; }
        public int PlotSize { get; set; }
        public int Threads { get; set; }
        public int Buffer { get; set; }
        public int Buckets { get; set; }

        public PlottingStatisticsID(PlotLog plotLog)
        {
            UsedPlotter = plotLog.UsedPlotter;
            LogFolder = plotLog.LogFolder;
            Tmp1Drive = plotLog.Tmp1Drive;
            Tmp2Drive = plotLog.Tmp2Drive;
            PlotSize = plotLog.PlotSize;
            Threads = plotLog.Threads;
            Buffer = plotLog.Buffer;
            Tmp2Drive = plotLog.Tmp2Drive;
            Buckets = plotLog.Buckets;
        }

        public int CalcRelevance(PlottingStatisticsID other, PlottingStatisticsIdRelevanceWeights weights)
        {
            var relevance = 0;
            relevance += UsedPlotterRelevance(other) * (weights.TmpDir + weights.ComputeConfiguration) * 10;
            relevance += PlotSizeRelevance(other) * weights.PlotSize;
            relevance += LogFolderRelevance(other) * weights.LogFolder;
            relevance += TmpDirRelevance(other) * weights.TmpDir;
            relevance += ComputeConfiguration(other) * weights.ComputeConfiguration;
            return relevance;
        }

        private int UsedPlotterRelevance(PlottingStatisticsID other)
        {
            if (this.UsedPlotter == other.UsedPlotter)
                return 1;
            return 0;
        }

        private int PlotSizeRelevance(PlottingStatisticsID other)
        {
            if (this.PlotSize == other.PlotSize)
                return 1;
            return 0;
        }

        private int LogFolderRelevance(PlottingStatisticsID other)
        {
            if (string.Equals(this.LogFolder, other.LogFolder))
                return 1;
            return 0;
        }

        private int TmpDirRelevance(PlottingStatisticsID other)
        {
            bool tmp1Equal = string.Equals(this.Tmp1Drive, other.Tmp1Drive);
            bool tmp2Equal = string.Equals(this.Tmp2Drive, other.Tmp2Drive);
            if (tmp1Equal && tmp2Equal)
                return 2;
            if (!tmp1Equal && !tmp2Equal)
                return 0;
            return 1;
        }

        private int ComputeConfiguration(PlottingStatisticsID other)
        {
            int relevance = 0;
            if (this.Threads == other.Threads) relevance++;
            if (this.Buckets == other.Buckets) relevance++;
            if (this.Buffer == other.Buffer) relevance++;
            return relevance;
        }

        public override bool Equals(object obj)
        {
            return obj is PlottingStatisticsID iD &&
                   LogFolder == iD.LogFolder &&
                   Tmp1Drive == iD.Tmp1Drive &&
                   Tmp2Drive == iD.Tmp2Drive &&
                   PlotSize == iD.PlotSize &&
                   Threads == iD.Threads &&
                   Buffer == iD.Buffer &&
                   Buckets == iD.Buckets;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LogFolder, Tmp1Drive, Tmp2Drive, PlotSize, Threads, Buffer, Buckets);
        }
    }
}
