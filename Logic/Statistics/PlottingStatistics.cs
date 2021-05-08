using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus
{

    /**
     * Collects Statistics on plotting processes such as avarage time spend on phases.
     * Used for ETA.
     */
    public class PlottingStatistics
    {
        // in seconds
        public int Phase1AvgTimeNeed { get; }
        public int Phase1Completed { get; }
        public int Phase2AvgTimeNeed { get; }
        public int Phase2Completed { get; }
        public int Phase3AvgTimeNeed { get; }
        public int Phase3Completed { get; }
        public int Phase4AvgTimeNeed { get; }
        public int Phase4Completed { get; }
        public int CopyTimeAvgTimeNeed { get; }
        public int CopyTimeCompleted { get; }

        public PlottingStatistics(List<PlotLog> plotLogs)
        {
            foreach (var plotLog in plotLogs)
            {
                if (plotLog.Phase1Seconds != 0)
                {
                    Phase1Completed++;
                    Phase1AvgTimeNeed += plotLog.Phase1Seconds;
                }
                if (plotLog.Phase2Seconds != 0)
                {
                    Phase2Completed++;
                    Phase2AvgTimeNeed += plotLog.Phase2Seconds;
                }
                if (plotLog.Phase3Seconds != 0)
                {
                    Phase3Completed++;
                    Phase3AvgTimeNeed += plotLog.Phase3Seconds;
                }
                if (plotLog.Phase4Seconds != 0)
                {
                    Phase4Completed++;
                    Phase4AvgTimeNeed += plotLog.Phase4Seconds;
                }
                if (plotLog.CopyTimeSeconds != 0)
                {
                    CopyTimeCompleted++;
                    CopyTimeAvgTimeNeed += plotLog.CopyTimeSeconds;
                }
            }
            if (Phase1Completed > 0) Phase1AvgTimeNeed /= Phase1Completed;
            if (Phase2Completed > 0) Phase2AvgTimeNeed /= Phase2Completed;
            if (Phase3Completed > 0) Phase3AvgTimeNeed /= Phase3Completed;
            if (Phase4Completed > 0) Phase4AvgTimeNeed /= Phase4Completed;
            if (CopyTimeCompleted > 0) CopyTimeAvgTimeNeed /= CopyTimeCompleted;
        }
    }
}
