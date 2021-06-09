using ChiaPlotStatus;
using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatusLib.Logic.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChiaPlotStatusLib.Logic.Models
{
    public class CPPlotLog: PlotLog
    {
        public static int P1PARTS = 12;
        public static int P2PARTS = 15;
        public static int P3PARTS = 14;
        public static int P4PARTS = 6;


        public int CurrentPhasePart { get; set; } = 1;

        public float P1 { get; set; } = 0.0f;
        public float P2 { get; set; } = 0.0f;
        public float P3 { get; set; } = 0.0f;
        public long P3Entries { get; set; } = 0L;
        public float P4 { get; set; } = 0.0f;
        public float Total { get; set; } = 0.0f;

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


        public void EnterPhase(int phase)
        {
            Debug.WriteLine("XXXXXX " + this.CurrentPhase + ": " + this.CurrentPhasePart);
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


        public void UpdateProgress()
        {
            float part = 0;

            switch (CurrentPhase)
            {
                case 6:
                    part = 1 + CPPlotLog.P4PARTS + CPPlotLog.P3PARTS + CPPlotLog.P2PARTS + CPPlotLog.P1PARTS;
                    break;
                case 5:
                    part = CPPlotLog.P4PARTS + CPPlotLog.P3PARTS + CPPlotLog.P2PARTS + CPPlotLog.P1PARTS;
                    break;
                case 4:
                    part = part = CPPlotLog.P3PARTS + CPPlotLog.P2PARTS + CPPlotLog.P1PARTS;
                    break;
                case 3:
                    int totalTablesIn3 = 7;
                    part = part = CPPlotLog.P2PARTS + CPPlotLog.P1PARTS;
                    break;
                case 2:
                    part = part = CPPlotLog.P1PARTS;
                    break;
                case 1:
                    part = 0;
                    break;
            }
            Progress = (part + CurrentPhasePart) / (1 + CPPlotLog.P4PARTS + CPPlotLog.P3PARTS + CPPlotLog.P2PARTS + CPPlotLog.P1PARTS) * 100;
            if (Double.IsNaN(Progress))
                Progress = 0;
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
