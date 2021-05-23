using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatusLib.Logic.Utils;
using System;
using System.Diagnostics;

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
        public string PlaceInLogFile { get; set; } = "";
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
            this.ETA = Formatter.formatDateTime(plotLog.ETA);
            this.StartDate = Formatter.formatDateTime(plotLog.StartDate);
            this.FinishDate = Formatter.formatDateTime(plotLog.FinishDate);
            this.TimeRemaining = Formatter.formatSeconds(plotLog.TimeRemaining);
            this.Phase1Cpu = Formatter.formatCpuUsage(plotLog.Phase1Cpu);
            this.Phase2Cpu = Formatter.formatCpuUsage(plotLog.Phase2Cpu);
            this.Phase3Cpu = Formatter.formatCpuUsage(plotLog.Phase3Cpu);
            this.Phase4Cpu = Formatter.formatCpuUsage(plotLog.Phase4Cpu);
            this.Phase1Seconds = Formatter.formatSeconds(plotLog.Phase1Seconds);
            this.Phase2Seconds = Formatter.formatSeconds(plotLog.Phase2Seconds);
            this.Phase3Seconds = Formatter.formatSeconds(plotLog.Phase3Seconds);
            this.Phase4Seconds = Formatter.formatSeconds(plotLog.Phase4Seconds);
            this.CopyTimeSeconds = Formatter.formatSeconds(plotLog.CopyTimeSeconds);
            this.TotalSeconds = Formatter.formatSeconds(plotLog.TotalSeconds);
            this.RunTimeSeconds = Formatter.formatSeconds(plotLog.RunTimeSeconds);
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
            this.Health = Formatter.formatHealth(plotLog.Health);
            this.PlaceInLogFile = plotLog.PlaceInLogFile + "/" + plotLog.QueueSize;
        }
    }
}
