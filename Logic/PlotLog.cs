namespace ChiaPlotStatus
{
    /**
     * Stores informations about a plotting process.
     * This is the object shown in the ui table.
     */
    class PlotLog
    {
        public string Tmp1Drive { get; set; }
        public string Tmp2Drive { get; set; }
        // public string DestDrive { get; set; }
        public int Errors { get; set; }
        public float PercentDone { get; set; }
        public string ETA { get; set; }
        public int CurrentBucket { get; set; }
        public int Phase1Table { get; set; }
        public int Phase2Table { get; set; }
        public int Phase3Table { get; set; }
        public int Phase1Seconds { get; set; }
        public int Phase2Seconds { get; set; }
        public int Phase3Seconds { get; set; }
        public int Phase4Seconds { get; set; }
        public int TotalSeconds { get; set; }
        public int PlotSize { get; set; }
        public int Threads { get; set; }
        public int Buffer { get; set; }
        public int Buckets { get; set; }
        public string StartDate { get; set; }
        public string PlotName { get; set; }
        public string LogFolder { get; set; }
        public string LogFile { get; set; }

        public void UpdatePercentDone()
        {
            float part = 0;
            // 22 parts total
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
            PercentDone = (part + subpart) / 22 * 100;
        }
    }
}