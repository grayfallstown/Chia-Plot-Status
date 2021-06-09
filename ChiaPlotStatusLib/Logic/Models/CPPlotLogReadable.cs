using ChiaPlotStatus;
using ChiaPlotStatusLib.Logic.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatusLib.Logic.Models
{
    public class CPPlotLogReadable: PlotLogReadable
    {
        public string CurrentPhasePart { get; set; } = "";

        public string P3Entries { get; set; } = "";

        public string P1Table1 { get; set; } = "";
        public string P1Table1Found { get; set; } = "";
        public string P1Table1Lost { get; set; } = "";
        public string P1Table2 { get; set; } = "";
        public string P1Table2Found { get; set; } = "";
        public string P1Table2Lost { get; set; } = "";
        public string P1Table3 { get; set; } = "";
        public string P1Table3Found { get; set; } = "";
        public string P1Table3Lost { get; set; } = "";
        public string P1Table4 { get; set; } = "";
        public string P1Table4Found { get; set; } = "";
        public string P1Table4Lost { get; set; } = "";
        public string P1Table5 { get; set; } = "";
        public string P1Table5Found { get; set; } = "";
        public string P1Table5Lost { get; set; } = "";
        public string P1Table6 { get; set; } = "";
        public string P1Table6Found { get; set; } = "";
        public string P1Table6Lost { get; set; } = "";
        public string P1Table7 { get; set; } = "";
        public string P1Table7Found { get; set; } = "";
        public string P1Table7Lost { get; set; } = "";

        public string P2MaxTableSize { get; set; } = "";

        public string P2Table1Scan { get; set; } = "";
        public string P2Table2Scan { get; set; } = "";
        public string P2Table3Scan { get; set; } = "";
        public string P2Table4Scan { get; set; } = "";
        public string P2Table5Scan { get; set; } = "";
        public string P2Table6Scan { get; set; } = "";
        public string P2Table7Scan { get; set; } = "";

        public string P2Table1Rewrite { get; set; } = "";
        public string P2Table2Rewrite { get; set; } = "";
        public string P2Table3Rewrite { get; set; } = "";
        public string P2Table4Rewrite { get; set; } = "";
        public string P2Table5Rewrite { get; set; } = "";
        public string P2Table6Rewrite { get; set; } = "";
        public string P2Table7Rewrite { get; set; } = "";

        public string P2Table1Lost { get; set; } = "";
        public string P2Table2Lost { get; set; } = "";
        public string P2Table3Lost { get; set; } = "";
        public string P2Table4Lost { get; set; } = "";
        public string P2Table5Lost { get; set; } = "";
        public string P2Table6Lost { get; set; } = "";
        public string P2Table7Lost { get; set; } = "";

        public string P31Table2 { get; set; } = "";
        public string P31Table2Entries { get; set; } = "";
        public string P31Table3 { get; set; } = "";
        public string P31Table3Entries { get; set; } = "";
        public string P31Table4 { get; set; } = "";
        public string P31Table4Entries { get; set; } = "";
        public string P31Table5 { get; set; } = "";
        public string P31Table5Entries { get; set; } = "";
        public string P31Table6 { get; set; } = "";
        public string P31Table6Entries { get; set; } = "";
        public string P31Table7 { get; set; } = "";
        public string P31Table7Entries { get; set; } = "";

        public string P32Table1 { get; set; } = "";
        public string P32Table1Entries { get; set; } = "";
        public string P32Table2 { get; set; } = "";
        public string P32Table2Entries { get; set; } = "";
        public string P32Table3 { get; set; } = "";
        public string P32Table3Entries { get; set; } = "";
        public string P32Table4 { get; set; } = "";
        public string P32Table4Entries { get; set; } = "";
        public string P32Table5 { get; set; } = "";
        public string P32Table5Entries { get; set; } = "";
        public string P32Table6 { get; set; } = "";
        public string P32Table6Entries { get; set; } = "";
        public string P32Table7 { get; set; } = "";
        public string P32Table7Entries { get; set; } = "";


        public CPPlotLogReadable(CPPlotLog cpPlotLog): base(cpPlotLog)
        {
            this.CurrentBucket = "";
            if (cpPlotLog.CurrentTable == 0)
                this.CurrentTable = "";
            else
                this.CurrentTable = cpPlotLog.CurrentTable + "/7";

            switch (cpPlotLog.CurrentPhase)
            {
                case 1:
                    this.CurrentPhase = "1/5";
                    this.CurrentPhasePart = cpPlotLog.CurrentPhasePart + "/" + CPPlotLog.P1PARTS;
                    break;
                case 2:
                    this.CurrentPhase = "2/5";
                    this.CurrentPhasePart = cpPlotLog.CurrentPhasePart + "/" + CPPlotLog.P2PARTS;
                    break;
                case 3:
                    this.CurrentPhase = "3/5";
                    this.CurrentPhasePart = cpPlotLog.CurrentPhasePart + "/" + CPPlotLog.P3PARTS;
                    break;
                case 4:
                    this.CurrentPhase = "4/5";
                    this.CurrentPhasePart = cpPlotLog.CurrentPhasePart + "/" + CPPlotLog.P4PARTS;
                    break;
                case 5:
                    this.CurrentPhase = "5/5";
                    this.CurrentPhasePart = "";
                    break;
                case 6:
                    this.CurrentPhase = "";
                    this.CurrentPhasePart = "";
                    break;
            }
        }
    }
}
