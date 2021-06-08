using ChiaPlotStatus;
using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatusLib.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChiaPlotStatusLib.Logic.Models
{
    public class CPPlotLog: IsPlotLog
    {
        public static int P1PARTS = 42;
        public static int P2PARTS = 42;
        public static int P3PARTS = 42;
        public static int P4PARTS = 42;


        public int Threads { get; set; } = 0;
        public int Buckets { get; set; } = 0;
        public int PlotSize { get; set; } = 32;
        public string LogFile { get; set; } = "";
        public string LogFolder { get; set; } = "";
        public string Tmp1Drive { get; set; } = "";
        public string Tmp2Drive { get; set; } = "";
        public string DestDrive { get; set; } = "";
        public string LastLogLine { get; set; } = "";
        public string PlotName { get; set; } = "";
        public DateTime? FileLastWritten { get; set; }
        public DateTime? StartDate { get; set; }
        [JsonIgnore]
        public HealthIndicator Health { get; set; } = Healthy.Instance;
        public DateTime? ETA { get; set; }
        public int TimeRemaining { get; set; } = 0;
        public float Progress { get; set; } = 0.0f;
        public string Note { get; set; } = "";
        public int RunTimeSeconds { get; set; } = 0;


        public int CurrentPhase { get; set; } = 1;
        public int CurrentTable { get; set; } = 1;
        public int CurrentPhasePart { get; set; } = 1;

        public float P1 { get; set; } = 0.0f;
        public float P2 { get; set; } = 0.0f;
        public float P3 { get; set; } = 0.0f;
        public long P3Entries { get; set; } = 0L;
        public float P4 { get; set; } = 0.0f;
        public float Total { get; set; } = 0.0f;
        public long FinalFileSize { get; set; } = 0L;

        public float P1Table1 { get; set; } = 0.0f;
        public long P1Table1Found { get; set; } = 0L;
        public long P1Table1Lost { get; set; } = 0L;
        public float P1Table2 { get; set; } = 0.0f;
        public long P1Table2Found { get; set; } = 0L;
        public long P1Table2Lost { get; set; } = 0L;
        public float P1Table3 { get; set; } = 0.0f;
        public long P1Table3Found { get; set; } = 0L;
        public long P1Table3Lost { get; set; } = 0L;
        public float P1Table4 { get; set; } = 0.0f;
        public long P1Table4Found { get; set; } = 0L;
        public long P1Table4Lost { get; set; } = 0L;
        public float P1Table5 { get; set; } = 0.0f;
        public long P1Table5Found { get; set; } = 0L;
        public long P1Table5Lost { get; set; } = 0L;
        public float P1Table6 { get; set; } = 0.0f;
        public long P1Table6Found { get; set; } = 0L;
        public long P1Table6Lost { get; set; } = 0L;
        public float P1Table7 { get; set; } = 0.0f;
        public long P1Table7Found { get; set; } = 0L;
        public long P1Table7Lost { get; set; } = 0L;

        public long P2MaxTableSize { get; set; } = 0L;

        public float P2Table1Scan { get; set; } = 0.0f;
        public float P2Table2Scan { get; set; } = 0.0f;
        public float P2Table3Scan { get; set; } = 0.0f;
        public float P2Table4Scan { get; set; } = 0.0f;
        public float P2Table5Scan { get; set; } = 0.0f;
        public float P2Table6Scan { get; set; } = 0.0f;
        public float P2Table7Scan { get; set; } = 0.0f;

        public float P2Table1Rewrite { get; set; } = 0.0f;
        public float P2Table2Rewrite { get; set; } = 0.0f;
        public float P2Table3Rewrite { get; set; } = 0.0f;
        public float P2Table4Rewrite { get; set; } = 0.0f;
        public float P2Table5Rewrite { get; set; } = 0.0f;
        public float P2Table6Rewrite { get; set; } = 0.0f;
        public float P2Table7Rewrite { get; set; } = 0.0f;

        public long P2Table1Lost { get; set; } = 0L;
        public long P2Table2Lost { get; set; } = 0L;
        public long P2Table3Lost { get; set; } = 0L;
        public long P2Table4Lost { get; set; } = 0L;
        public long P2Table5Lost { get; set; } = 0L;
        public long P2Table6Lost { get; set; } = 0L;
        public long P2Table7Lost { get; set; } = 0L;

        public float P31Table2 { get; set; } = 0.0f;
        public long P31Table2Entries { get; set; } = 0L;
        public float P31Table3 { get; set; } = 0.0f;
        public long P31Table3Entries { get; set; } = 0L;
        public float P31Table4 { get; set; } = 0.0f;
        public long P31Table4Entries { get; set; } = 0L;
        public float P31Table5 { get; set; } = 0.0f;
        public long P31Table5Entries { get; set; } = 0L;
        public float P31Table6 { get; set; } = 0.0f;
        public long P31Table6Entries { get; set; } = 0L;
        public float P31Table7 { get; set; } = 0.0f;
        public long P31Table7Entries { get; set; } = 0L;

        public float P32Table1 { get; set; } = 0.0f;
        public long P32Table1Entries { get; set; } = 0L;
        public float P32Table2 { get; set; } = 0.0f;
        public long P32Table2Entries { get; set; } = 0L;
        public float P32Table3 { get; set; } = 0.0f;
        public long P32Table3Entries { get; set; } = 0L;
        public float P32Table4 { get; set; } = 0.0f;
        public long P32Table4Entries { get; set; } = 0L;
        public float P32Table5 { get; set; } = 0.0f;
        public long P32Table5Entries { get; set; } = 0L;
        public float P32Table6 { get; set; } = 0.0f;
        public long P32Table6Entries { get; set; } = 0L;
        public float P32Table7 { get; set; } = 0.0f;
        public long P32Table7Entries { get; set; } = 0L;


        public PlotLog AsPlotLog()
        {
            PlotLog plotLog = new();
            plotLog.UsedPlotter = "chia-plotter";
            plotLog.Tmp1Drive = this.Tmp1Drive;
            plotLog.Tmp2Drive = this.Tmp2Drive;
            plotLog.DestDrive = this.DestDrive;
            plotLog.Progress = this.Progress;
            plotLog.TimeRemaining = this.TimeRemaining;
            plotLog.ETA = this.ETA;
            plotLog.CurrentTable = this.CurrentTable;
            plotLog.CurrentPhase = this.CurrentPhase;
            plotLog.CurrentBucket = 0;
            plotLog.Phase1Seconds = (int) this.P1;
            plotLog.Phase2Seconds = (int) this.P2;
            plotLog.Phase3Seconds = (int) this.P3;
            plotLog.Phase4Seconds = (int) this.P4;
            // plotLog.CopyTimeSeconds = this.P5;
            plotLog.PlotSize = this.PlotSize;
            plotLog.Threads = this.Threads;
            // plotLog.Buffer = this.Buffer;
            plotLog.Buckets = this.Buckets;
            plotLog.StartDate = this.StartDate;
            // plotLog.FinishDate = this.FinishDate;
            plotLog.PlotName = this.PlotName;
            plotLog.LogFolder = this.LogFolder;
            plotLog.LogFile = this.LogFile;
            if (this.FinalFileSize == 0L)
                plotLog.FinalFileSize = "";
            else
                plotLog.FinalFileSize = (this.FinalFileSize / 1024 / 1024) + " MB";
            plotLog.Health = this.Health;
            plotLog.IsLastInLogFile = true;
            plotLog.PlaceInLogFile = 1;
            plotLog.IsLastLineTempError = false; // TODO
            plotLog.Health = this.Health;
            plotLog.RunTimeSeconds = this.RunTimeSeconds;
            plotLog.CaughtPlottingError = false; // TODO
            plotLog.LastLogLine = this.LastLogLine;
            plotLog.Note = this.Note;
            return plotLog;
        }

        public void EnterPhase(int phase)
        {
            this.CurrentPhase = phase;
            this.CurrentPhasePart = 0;
        }

        public void NextPhasePart()
        {
            this.CurrentPhasePart++;
        }

        public bool IsRunning()
        {
            if (this.CurrentPhase == 6)
                return true;
            switch (this.Health)
            {
                case ConfirmedDead:
                    return false;
                default:
                    return true;
            }
        }


        public override int GetCurrentPhase()
        {
            return CurrentPhase;
        }

        public override HealthIndicator GetHealth()
        {
            return Health;
        }

        public void UpdateEta(CPPlottingStatistics stats)
        {
            // TODO
        }

        public void UpdateHealth(CPPlottingStatistics stats)
        {
            // TODO
        }
    }
}
