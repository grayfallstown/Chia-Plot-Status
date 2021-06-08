using ChiaPlotStatusLib.Logic.Models;
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
    public class CPPlottingStatistics
    {
        // in seconds
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

        public CPPlottingStatistics(List<CPPlotLog> plotLogs)
        {
            /*
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
            */
        }

        public override bool Equals(object obj)
        {
            return obj is PlottingStatistics statistics &&
                   Phase1AvgTimeNeed == statistics.Phase1AvgTimeNeed &&
                   Phase1Completed == statistics.Phase1Completed &&
                   Phase2AvgTimeNeed == statistics.Phase2AvgTimeNeed &&
                   Phase2Completed == statistics.Phase2Completed &&
                   Phase3AvgTimeNeed == statistics.Phase3AvgTimeNeed &&
                   Phase3Completed == statistics.Phase3Completed &&
                   Phase4AvgTimeNeed == statistics.Phase4AvgTimeNeed &&
                   Phase4Completed == statistics.Phase4Completed &&
                   CopyTimeAvgTimeNeed == statistics.CopyTimeAvgTimeNeed &&
                   CopyTimeCompleted == statistics.CopyTimeCompleted;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Phase1AvgTimeNeed);
            hash.Add(Phase1Completed);
            hash.Add(Phase2AvgTimeNeed);
            hash.Add(Phase2Completed);
            hash.Add(Phase3AvgTimeNeed);
            hash.Add(Phase3Completed);
            hash.Add(Phase4AvgTimeNeed);
            hash.Add(Phase4Completed);
            hash.Add(CopyTimeAvgTimeNeed);
            hash.Add(CopyTimeCompleted);
            return hash.ToHashCode();
        }
    }
}
