using ChiaPlotStatus;
using ChiaPlotStatus.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatusGUI.GUI.Models
{
    public class PlotCounts
    {
        public int PlotsInPhase1 { get; set; } = 0;
        public int PlotsInPhase2 { get; set; } = 0;
        public int PlotsInPhase3 { get; set; } = 0;
        public int PlotsInPhase4 { get; set; } = 0;
        public int PlotsInPhase5 { get; set; } = 0;
        public int Finished { get; set; } = 0;
        public int Running { get; set; } = 0;
        public int Concerning { get; set; } = 0;
        public int Failed { get; set; } = 0;

        public PlotCounts() { }

        public PlotCounts(List<(PlotLog, PlotLogReadable)> plotLogs)
        {
            foreach (var tuple in plotLogs)
            {
                var plotLog = tuple.Item1;
                var done = tuple.Item1.CurrentPhase == 6;
                switch (plotLog.Health)
                {
                    case Healthy:
                        if (done)
                            Finished++;
                        else
                            handlePhase(plotLog);
                        break;
                    case TempError:
                    case Concerning c:
                        Concerning++;
                        handlePhase(plotLog);
                        break;
                    case PossiblyDead p:
                        Concerning++;
                        Failed++;
                        handlePhase(plotLog);
                        break;
                    case ConfirmedDead c:
                        Failed++;
                        break;
                }
            }
        }

        private void handlePhase(PlotLog plotLog)
        {
            Running++;
            switch (plotLog.CurrentPhase)
            {
                case 1:
                    PlotsInPhase1++;
                    break;
                case 2:
                    PlotsInPhase2++;
                    break;
                case 3:
                    PlotsInPhase3++;
                    break;
                case 4:
                    PlotsInPhase4++;
                    break;
                case 5:
                    PlotsInPhase5++;
                    break;
            }
        }
    }
}
