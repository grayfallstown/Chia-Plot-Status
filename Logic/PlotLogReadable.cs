using System;

namespace ChiaPlotStatus
{
    /**
     * Stores readable informations about a plotting process.
     * This is the object shown in the ui table.
     */
    public class PlotLogReadable
    {
        public string Tmp1Drive { get; set; } = "";
        public string Tmp2Drive { get; set; } = "";
        public string Errors { get; set; } = "";
        public string Progress { get; set; } = "";
        public string TimeRemaining { get; set; } = "";
        public string ETA { get; set; } = "";
        public string CurrentTable { get; set; } = "";
        public string CurrentBucket { get; set; } = "";
        public string CurrentPhase { get; set; } = "";
        public string Phase1Seconds { get; set; } = "";
        public string Phase2Seconds { get; set; } = "";
        public string Phase3Seconds { get; set; } = "";
        public string Phase4Seconds { get; set; } = "";
        public string TotalSeconds { get; set; } = "";
        public string PlotSize { get; set; } = "";
        public string Threads { get; set; } = "";
        public string Buffer { get; set; } = "";
        public string Buckets { get; set; } = "";
        public string StartDate { get; set; } = "";
        public string PlotName { get; set; } = "";
        public string LogFolder { get; set; } = "";
        public string LogFile { get; set; } = "";
        public string ApproximateWorkingSpace { get; set; } = "";
        public string FinalFileSize { get; set; } = "";

        public PlotLogReadable(PlotLog plotLog)
        {
            this.Tmp1Drive = plotLog.Tmp1Drive;
            this.Tmp2Drive = plotLog.Tmp2Drive;
            if (plotLog.Errors > 0)
            {
                this.Errors = plotLog.Errors.ToString();
            }
            this.Progress = string.Format("{0:0.00}", plotLog.Progress) + "%";
            if (string.Equals(this.Progress, "NaN%")) this.Progress = "";
            this.CurrentPhase = "1";
            if (plotLog.Phase1Seconds > 0)
            {
                this.CurrentPhase = "2";
            }
            if (plotLog.Phase2Seconds > 0)
            {
                this.CurrentPhase = "3";
            }
            if (plotLog.Phase3Seconds > 0)
            {
                this.CurrentPhase = "4";
            }
            if (plotLog.Phase4Seconds > 0)
            {
                this.CurrentPhase = "";
            }
            else
            {
                this.CurrentPhase += "/4";
            }
            this.TimeRemaining = formatSeconds(plotLog.TimeRemaining);
            this.ETA = formatDateTime(plotLog.ETA);
            this.Phase1Seconds = formatSeconds(plotLog.Phase1Seconds);
            this.Phase2Seconds = formatSeconds(plotLog.Phase2Seconds);
            this.Phase3Seconds = formatSeconds(plotLog.Phase3Seconds);
            this.Phase4Seconds = formatSeconds(plotLog.Phase4Seconds);
            this.TotalSeconds = formatSeconds(plotLog.TotalSeconds);
            this.ApproximateWorkingSpace = plotLog.ApproximateWorkingSpace;
            this.FinalFileSize = plotLog.FinalFileSize;
            this.Buckets = plotLog.Buckets.ToString();
            this.Threads = plotLog.Threads.ToString();
            this.Buffer = plotLog.Buffer + " MB";
            this.CurrentBucket = plotLog.CurrentBucket + "/" + plotLog.Buckets.ToString();
            this.StartDate = plotLog.StartDate;
            this.PlotName = plotLog.PlotName;
            this.LogFolder = plotLog.LogFolder;
            this.LogFile = plotLog.LogFile.Substring(plotLog.LogFile.LastIndexOf("\\") + 1);
            switch (this.CurrentPhase)
            {
                case "1/4":
                    this.CurrentTable = plotLog.Phase1Table + "/7 ↑";
                    break;
                case "2/4":
                    this.CurrentTable = plotLog.Phase2Table + "/7 ↓";
                    break;
                case "3/4":
                    this.CurrentTable = plotLog.Phase3Table + "/7 ↑";
                    break;
                case "4/4":
                    this.CurrentTable = "4/4";
                    break;
                default:
                    this.CurrentTable ="";
                    break;
            }
        }

        private string formatSeconds(int seconds)
        {
            if (seconds == 0)
            {
                return "";

            }
            else if (seconds > 24 * 60 * 60)
            {
                return TimeSpan.FromSeconds(seconds).ToString(@"dd\d\ hh\h\ mm\m\ ss\s");
            }
            else if (seconds > 60 * 60)
            {
                return TimeSpan.FromSeconds(seconds).ToString(@"hh\h\ mm\m\ ss\s");
            }
            else if (seconds > 60)
            {
                return TimeSpan.FromSeconds(seconds).ToString(@"mm\m\ ss\s");

            }
            else
            {
                return TimeSpan.FromSeconds(seconds).ToString(@"ss\s");
            }
        }

        private string formatDateTime(DateTime? dateTime)
        {
            // decided agains local date format for now
            if (dateTime == null)
                return "";
            else
                // forced to make it non nullable or it does not find ToString(format)
                return ((DateTime)dateTime).ToString("MM/dd/yyyy H:mm");
        }
    }
}
