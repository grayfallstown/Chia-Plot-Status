using System;

namespace ChiaPlotStatus
{
    /**
     * Stores informations about a plotting process.
     * This is the object shown in the ui table.
     */
    public class PlotLog
    {
        public string Tmp1Drive { get; set; } = "";
        public string Tmp2Drive { get; set; } = "";
        // public string DestDrive { get; set; }
        public int Errors { get; set; } = 0;
        public float Progress { get; set; } = 0;
        public int ETA { get; set; } = 0;
        public int CurrentBucket { get; set; } = 0;
        public int Phase1Table { get; set; } = 0;
        public int Phase2Table { get; set; } = 0;
        public int Phase3Table { get; set; } = 0;
        public int Phase1Seconds { get; set; } = 0;
        public int Phase2Seconds { get; set; } = 0;
        public int Phase3Seconds { get; set; } = 0;
        public int Phase4Seconds { get; set; } = 0;
        public int TotalSeconds { get; set; } = 0;
        public int PlotSize { get; set; } = 0;
        public int Threads { get; set; } = 0;
        public int Buffer { get; set; } = 0;
        public int Buckets { get; set; } = 0;
        public string StartDate { get; set; } = "";
        public string PlotName { get; set; } = "";
        public string LogFolder { get; set; } = "";
        public string LogFile { get; set; } = "";
        public string ApproximateWorkingSpace { get; set; } = "";
        public string FinalFileSize { get; set; } = "";

        public void UpdateProgress()
        {
            float part = 0;
            // 22 parts total:
            // 7 tables in phase 1
            // 7 tables in phase 2
            // 7 tables in phase 3
            // 1 phase 4
            float subpart = 0;
            if (Phase4Seconds > 0)
            {
                // we are Done
                part = 22;
            }
            else if (Phase3Seconds > 0)
            {
                // we are in 4
                part = 21;
            }
            else if (Phase3Table > 0)
            {
                // we are in 3
                int totalTablesIn3 = 7;
                part = 20 - (totalTablesIn3 - Phase3Table);
                subpart = (float)CurrentBucket / Buckets;
            }
            else if (Phase2Table > 0)
            {
                // we are in 2
                part = 14 - Phase2Table;
                subpart = (float)CurrentBucket / Buckets;
            }
            else
            {
                // we are in 1
                int totalTablesIn1 = 7;
                part = 7 - (totalTablesIn1 - Phase1Table);
                subpart = (float)CurrentBucket / Buckets;
            }
            Progress = (part + subpart) / 22 * 100;
        }

        public void UpdateEta(PlottingStatistics stats)
        {
            this.ETA = 0;
            if (this.Buckets == 0)
            {
                // log too short to know anything yet
                return;
            }
            int currentPhase = 1;
            if (Math.Abs(this.Progress - 100f) < 0.00001)
            {
                return;
            }
            else
            {
                if (this.Phase3Seconds > 0)
                {
                    currentPhase = 4;
                }
                else if (this.Phase2Seconds > 0)
                {
                    currentPhase = 3;
                }
                else if (this.Phase1Seconds > 0)
                {
                    currentPhase = 2;
                }
            }
            if (currentPhase <= 4)
            {
                this.ETA += stats.Phase4AvgTimeNeed;
            }
            if (currentPhase <= 3)
            {
                float factor = 1;
                if (currentPhase == 3)
                {
                    factor = (float)(((float)this.Phase3Table - 1) + ((float)this.CurrentBucket / this.Buckets)) / 7;
                }
                this.ETA += (int)(factor * stats.Phase3AvgTimeNeed);
            }
            if (currentPhase <= 2)
            {
                float factor = 1;
                if (currentPhase == 2)
                {
                    factor = (float)(((float)(7- this.Phase2Table) - 1) + ((float)this.CurrentBucket / this.Buckets)) / 7;
                }
                this.ETA += (int)(factor * stats.Phase2AvgTimeNeed);
            }
            if (currentPhase == 1)
            {
                var factor = (float)(((float)this.Phase3Table - 1) + ((float)this.CurrentBucket / this.Buckets)) / 7;
                this.ETA += (int)(factor * stats.Phase2AvgTimeNeed);
            }
        }

    }
}