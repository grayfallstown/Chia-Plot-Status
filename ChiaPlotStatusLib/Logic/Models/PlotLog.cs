using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatusLib.Logic.Models;
using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ChiaPlotStatus
{
    /**
     * Stores informations about a plotting process.
     * This is the object shown in the ui table.
     */
    public class PlotLog
    {
        public string UsedPlotter { get; set; } = "chiapos";
        public string? Tmp1Drive { get; set; }
        public string? Tmp2Drive { get; set; }
        public string? DestDrive { get; set; }
        public int Errors { get; set; } = 0;
        public int PID { get; set; } = 0;
        public float Progress { get; set; } = 0;
        public int TimeRemaining { get; set; } = 0;
        public DateTime? ETA { get; set; }
        public int CurrentTable { get; set; } = 0;
        // Phase 6 is done
        public int CurrentPhase { get; set; } = 0;
        public int CurrentBucket { get; set; } = 0;
        public int Phase1Table { get; set; } = 0;
        public int Phase2Table { get; set; } = 0;
        public int Phase3Table { get; set; } = 0;
        public int Phase1Seconds { get; set; } = 0;
        public int Phase2Seconds { get; set; } = 0;
        public int Phase3Seconds { get; set; } = 0;
        public int Phase4Seconds { get; set; } = 0;
        public double Phase1Cpu { get; set; } = 0;
        public double Phase2Cpu { get; set; } = 0;
        public double Phase3Cpu { get; set; } = 0;
        public double Phase4Cpu { get; set; } = 0;
        public int CopyTimeSeconds { get; set; } = 0;
        public int TotalSeconds { get; set; } = 0;
        public int PlotSize { get; set; } = 0;
        public int Threads { get; set; } = 0;
        public int Buffer { get; set; } = 0;
        public int Buckets { get; set; } = 0;
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string PlotName { get; set; } = "";
        public string LogFolder { get; set; } = "";
        public string LogFile { get; set; } = "";
        public string ApproximateWorkingSpace { get; set; } = "";
        public string FinalFileSize { get; set; } = "";
        public DateTime? FileLastWritten { get; set; }
        [JsonIgnore]
        public HealthIndicator Health { get; set; } = Healthy.Instance;
        public bool IsLastInLogFile { get; set; } = true;
        public bool IsLastLineTempError { get; set; } = false;
        public int QueueSize { get; set; } = 1;
        public int PlaceInLogFile { get; set; } = 1;
        public int RunTimeSeconds { get; set; } = 0;
        public bool CaughtPlottingError { get; set; } = false;
        public string LastLogLine { get; set; } = "";
        public string Note { get; set; } = "Notice / Tags";

        public virtual void UpdateProgress()
        {
            float part = 0;
            // 22 parts total:
            // 7 tables in phase 1
            // 7 tables in phase 2
            // 7 tables in phase 3
            // 1 phase 4
            // 1 phase 5
            float subpart = 0;

            switch(CurrentPhase)
            {
                case 6:
                    part = 23;
                    break;
                case 5:
                    part = 22;
                    break;
                case 4:
                    part = 20;
                    subpart = (float)this.CurrentBucket / this.Buckets;
                    break;
                case 3:
                    int totalTablesIn3 = 7;
                    part = 20 - (totalTablesIn3 - Phase3Table);
                    subpart = (float)CurrentBucket / Buckets;
                    break;
                case 2:
                    part = 14 - Phase2Table;
                    subpart = (float)CurrentBucket / Buckets;
                    break;
                case 1:
                    int totalTablesIn1 = 7;
                    part = 7 - (totalTablesIn1 - Phase1Table) - 1;
                    subpart = (float)CurrentBucket / Buckets;
                    break;
            }
            Progress = (part + subpart) / 23 * 100;
            if (Double.IsNaN(Progress))
                Progress = 0;
        }

        /**
         * Run UpdateHealth before calling
         */
        public virtual bool IsRunning()
        {
            switch (Health)
            {
                case Healthy:
                case Concerning:
                case TempError:
                case PossiblyDead:
                    return CurrentPhase != 6;
                default:
                    return false;
            }
        }

        public virtual void UpdateEta(PlottingStatistics stats)
        {
            this.TimeRemaining = 0;
            if (this.Buckets == 0)
            {
                // log too short to know anything yet
                this.ETA = null;
                this.TimeRemaining = 0;
                return;
            }
            if (CurrentPhase == 6)
            {
                this.TimeRemaining = 0;
            }
            if (CurrentPhase <= 5)
            {
                float factor = 1;
                if (CurrentPhase == 5)
                {
                    DateTime copyStart = ((DateTime)this.StartDate)
                        .AddSeconds(this.Phase1Seconds)
                        .AddSeconds(this.Phase2Seconds)
                        .AddSeconds(this.Phase3Seconds)
                        .AddSeconds(this.Phase4Seconds);
                    int elapsed = (int)(DateTime.Now - copyStart).TotalSeconds;
                    factor = (float)1 - (float)((float)elapsed / stats.CopyTimeAvgTimeNeed);
                    if (factor < 0.01f)
                        factor = 0.01f;
                }
                this.TimeRemaining += (int)(factor * (float)stats.CopyTimeAvgTimeNeed);
            }
            if (CurrentPhase <= 4)
            {
                float factor = 1;
                if (CurrentPhase == 4)
                {
                    factor = (float)1 - ((float)((float)this.CurrentBucket / this.Buckets));
                }
                this.TimeRemaining += (int)(factor * stats.Phase4AvgTimeNeed);
            }
            if (CurrentPhase <= 3)
            {
                float factor = 1;
                if (CurrentPhase == 3)
                {
                    factor = (float)1 - ((float)(((float)this.Phase3Table - 1) + ((float)this.CurrentBucket / this.Buckets)) / 7);
                }
                this.TimeRemaining += (int)(factor * stats.Phase3AvgTimeNeed);
            }
            if (CurrentPhase <= 2)
            {
                float factor = 1;
                if (CurrentPhase == 2)
                {
                    factor = (float)1 - ((float)((float) 7 - this.Phase2Table) / 7);
                }
                this.TimeRemaining += (int)(factor * stats.Phase2AvgTimeNeed);
            }
            if (CurrentPhase == 1)
            {
                var factor = (float)1 - ((float)(((float)this.Phase3Table - 1) + ((float)this.CurrentBucket / this.Buckets)) / 7);
                this.TimeRemaining += (int)(factor * stats.Phase2AvgTimeNeed);
            }
            if (this.TimeRemaining > 0)
            {
                DateTime dt = DateTime.Now;
                dt = dt.AddSeconds(this.TimeRemaining);
                this.ETA = dt;
            }
            else
                this.ETA = null;
        }

        public virtual void UpdateHealth(PlottingStatistics stats)
        {
            int lastModifiedAtWarningThreashold = 0;
            int lastModifiedAtErrorThreashold = 0;

            switch (this.CurrentPhase)
            {
                case 1:
                    if (this.CurrentTable == 1)
                        lastModifiedAtWarningThreashold = (int)(((float)stats.Phase1AvgTimeNeed / 60 / 7) * 3);
                    else
                        lastModifiedAtWarningThreashold = (int)(((float)stats.Phase1AvgTimeNeed / 60 / 7 / this.Buckets) * 3);
                    if (lastModifiedAtWarningThreashold == 0)
                        lastModifiedAtWarningThreashold = 15;
                    break;
                case 2:
                    lastModifiedAtWarningThreashold = (int)((float)stats.Phase2AvgTimeNeed / 60 / 7 * 3);
                    if (lastModifiedAtWarningThreashold == 0)
                        lastModifiedAtWarningThreashold = 10;
                    break;
                case 3:
                    lastModifiedAtWarningThreashold = (int)(((float)stats.Phase3AvgTimeNeed / 60 / 7 / this.Buckets) * 3);
                    if (lastModifiedAtWarningThreashold == 0)
                        lastModifiedAtWarningThreashold = 15;
                    break;
                case 4:
                    lastModifiedAtWarningThreashold = (int)(((float)stats.Phase3AvgTimeNeed / 60 / this.Buckets) * 3);
                    if (lastModifiedAtWarningThreashold == 0)
                        lastModifiedAtWarningThreashold = 15;
                    if (this.CurrentBucket == this.Buckets)
                        lastModifiedAtWarningThreashold = 20;
                    break;
                case 5:
                case 6:
                    Health = Healthy.Instance;
                    return;
            }
            if (lastModifiedAtWarningThreashold < 10)
                lastModifiedAtWarningThreashold = 10;
            lastModifiedAtWarningThreashold = (int)((float)lastModifiedAtWarningThreashold * 1.2f);
            lastModifiedAtErrorThreashold = lastModifiedAtWarningThreashold * 4;
            // Debug.WriteLine("lastModifiedAtWarningThreashold: " + lastModifiedAtWarningThreashold);
            // Debug.WriteLine("lastModifiedAtErrorThreashold: " + lastModifiedAtErrorThreashold);

            bool notLastAndNotDone = this.Progress < 100d && !this.IsLastInLogFile;
            bool lastModfiedAtWarning = false;
            bool lastModfiedAtError = false;
            bool lastLineError = this.IsLastLineTempError;
            TimeSpan? fileLastWrittenAge = null;
            int lastWriteMinutesAgo = 0;
            if (this.FileLastWritten != null)
            {
                fileLastWrittenAge = DateTime.Now - ((DateTime)this.FileLastWritten);
                if (((TimeSpan)fileLastWrittenAge).TotalMinutes > (lastModifiedAtWarningThreashold + 1))
                    lastModfiedAtWarning = true;
                if (((TimeSpan)fileLastWrittenAge).TotalMinutes > (lastModifiedAtErrorThreashold + 1))
                    lastModfiedAtError = true;
                lastWriteMinutesAgo = (int)((TimeSpan)fileLastWrittenAge).TotalMinutes;
            }

            // manual is set to false for now, it will be overwritten in ChiaPlotStatus.cs if necessary
            bool manual = false;
            if (notLastAndNotDone || this.CaughtPlottingError)
                this.Health = new ConfirmedDead(manual);
            else if (lastModfiedAtError)
                this.Health = new PossiblyDead(lastWriteMinutesAgo, lastModifiedAtWarningThreashold);
            else if (lastModfiedAtWarning)
                this.Health = new Concerning(lastWriteMinutesAgo, lastModifiedAtWarningThreashold);
            else if (lastLineError)
                this.Health = TempError.Instance;
            else
                this.Health = Healthy.Instance;
        }
    }
}
