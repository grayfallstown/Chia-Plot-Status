using ChiaPlotStatus.Logic.Models;
using System;
using System.Diagnostics;

namespace ChiaPlotStatus
{
    /**
     * Stores readable informations about a plotting process.
     * This is the object shown in the ui table.
     *
     * Currently does the formatting as well.. should be separated
     */
    public class PlotLogReadable
    {
        public string Tmp1Drive { get; set; } = "";
        public string Tmp2Drive { get; set; } = "";
        public string DestDrive { get; set; }
        public string Errors { get; set; } = "";
        public string Progress { get; set; } = "";
        public string TimeRemaining { get; set; } = "";
        public string ETA { get; set; } = "";
        public string CurrentTable { get; set; } = "";
        public string CurrentBucket { get; set; } = "";
        public string CurrentPhase { get; set; } = "";
        public string Phase1Cpu { get; set; } = "";
        public string Phase2Cpu { get; set; } = "";
        public string Phase3Cpu { get; set; } = "";
        public string Phase4Cpu { get; set; } = "";
        public string Phase1Seconds { get; set; } = "";
        public string Phase2Seconds { get; set; } = "";
        public string Phase3Seconds { get; set; } = "";
        public string Phase4Seconds { get; set; } = "";
        public string CopyTimeSeconds { get; set; } = "";
        public string TotalSeconds { get; set; } = "";
        public string PlotSize { get; set; } = "";
        public string Threads { get; set; } = "";
        public string Buffer { get; set; } = "";
        public string Buckets { get; set; } = "";
        public string StartDate { get; set; } = "";
        public string FinishDate { get; set; } = "";
        public string PlotName { get; set; } = "";
        public string LogFolder { get; set; } = "";
        public string LogFile { get; set; } = "";
        public string ApproximateWorkingSpace { get; set; } = "";
        public string FinalFileSize { get; set; } = "";
        public string Health { get; set; } = "";
        public int PlaceInLogFile { get; set; } = -1;
        public string RunTimeSeconds { get; set; } = "";
        public string LastLogLine { get; set; } = "";
        public bool IsSelected { get; set; } = false;

        public PlotLogReadable(PlotLog plotLog)
        {
            this.Tmp1Drive = plotLog.Tmp1Drive;
            this.Tmp2Drive = plotLog.Tmp2Drive;
            if (plotLog.Errors > 0)
                this.Errors = plotLog.Errors.ToString();
            this.Progress = string.Format("{0:0.00}", plotLog.Progress) + "%";
            if (string.Equals(this.Progress, "NaN%")) this.Progress = "";
            switch(plotLog.CurrentPhase)
            {
                case 1:
                    this.CurrentPhase = "1/5";
                    this.CurrentTable = plotLog.Phase1Table + "/7 ↑";
                    break;
                case 2:
                    this.CurrentPhase = "2/5";
                    this.CurrentTable = plotLog.Phase2Table + "/7 ↓";
                    break;
                case 3:
                    this.CurrentPhase = "3/5";
                    this.CurrentTable = plotLog.Phase3Table + "/7 ↑";
                    break;
                case 4:
                    this.CurrentPhase = "4/5";
                    this.CurrentTable = "1/1";
                    break;
                case 5:
                    this.CurrentPhase = "5/5";
                    this.CurrentTable = "";
                    break;
                case 6: // done
                    this.CurrentPhase = "";
                    this.CurrentTable = "";
                    break;
            }
            this.ETA = formatDateTime(plotLog.ETA);
            this.StartDate = formatDateTime(plotLog.StartDate);
            this.FinishDate = formatDateTime(plotLog.FinishDate);
            this.TimeRemaining = formatSeconds(plotLog.TimeRemaining);
            this.Phase1Cpu = formatCpuUsage(plotLog.Phase1Cpu);
            this.Phase2Cpu = formatCpuUsage(plotLog.Phase2Cpu);
            this.Phase3Cpu = formatCpuUsage(plotLog.Phase3Cpu);
            this.Phase4Cpu = formatCpuUsage(plotLog.Phase4Cpu);
            this.Phase1Seconds = formatSeconds(plotLog.Phase1Seconds);
            this.Phase2Seconds = formatSeconds(plotLog.Phase2Seconds);
            this.Phase3Seconds = formatSeconds(plotLog.Phase3Seconds);
            this.Phase4Seconds = formatSeconds(plotLog.Phase4Seconds);
            this.CopyTimeSeconds = formatSeconds(plotLog.CopyTimeSeconds);
            this.TotalSeconds = formatSeconds(plotLog.TotalSeconds);
            this.RunTimeSeconds = formatSeconds(plotLog.RunTimeSeconds);
            this.ApproximateWorkingSpace = plotLog.ApproximateWorkingSpace;
            this.DestDrive = plotLog.DestDrive;
            this.FinalFileSize = plotLog.FinalFileSize;
            this.Buckets = plotLog.Buckets.ToString();
            this.Threads = plotLog.Threads.ToString();
            this.Buffer = plotLog.Buffer + " MB";
            this.LastLogLine = plotLog.LastLogLine;
            switch (plotLog.CurrentPhase)
            {
                case 1:
                case 3:
                case 4:
                    this.CurrentBucket = plotLog.CurrentBucket + "/" + plotLog.Buckets;
                    break;
                case 2:
                case 5:
                case 6:
                    this.CurrentBucket = "";
                    break;
            }
            this.PlotName = plotLog.PlotName;
            this.LogFolder = plotLog.LogFolder;
            this.LogFile = plotLog.LogFile.Substring(plotLog.LogFile.LastIndexOf("\\") + 1);
            this.Health = formatHealth(plotLog.Health);
            this.PlaceInLogFile = plotLog.PlaceInLogFile;
        }

        private string formatCpuUsage(double usage)
        {
            if (usage < 0.01d)
                return "";
            else
                return usage.ToString("0.0 '%'");
        }

        private string formatSeconds(int seconds)
        {
            if (seconds == 0)
                return "";
            else if (seconds > 24 * 60 * 60)
                return TimeSpan.FromSeconds(seconds).ToString(@"dd\d\ hh\h\ mm\m\ ss\s");
            else if (seconds > 60 * 60)
                return TimeSpan.FromSeconds(seconds).ToString(@"hh\h\ mm\m\ ss\s");
            else if (seconds > 60)
                return TimeSpan.FromSeconds(seconds).ToString(@"mm\m\ ss\s");
            else
                return TimeSpan.FromSeconds(seconds).ToString(@"ss\s");
        }

        private string formatDateTime(DateTime? dateTime)
        {
            // decided agains local date format for now
            if (dateTime == null)
                return "";
            else
                // forced to make it non nullable or it does not find ToString(format)
                return ((DateTime)dateTime).ToString("MM/dd HH:mm");
        }

        private string formatHealth(HealthIndicator health)
        {
            switch (health)
            {
                case Healthy:
                    return "✓";
                case TempError:
                    return "⚠ Temp Errors";
                case Concerning c:
                    return "⚠ Slow " + c.Minutes + " / " + c.ExpectedMinutes + "m";
                case PossiblyDead p:
                    return "⚠ Dead? " + p.Minutes + " / " + p.ExpectedMinutes + "m";
                case ConfirmedDead c:
                    return "✗ Dead" + (c.Manual ? " (m)" : "");
                default:
                    throw new NotImplementedException("formatHealth (HealthIndicator " + Health + ")");
            }
        }


    }
}
