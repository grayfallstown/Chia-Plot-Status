using ChiaPlotStatusLib.Logic.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus
{

    /**
     * Collects Statistics on plotting processes such as avarage time spend on phases.
     * Used for ETA.
     */
    public class CPPlottingStatistics: PlottingStatistics
    {

        public CPPlottingStatistics(List<PlotLog> plotLogs): base(plotLogs)
        {
            // TODO: implement details like table times
        }
    }
}
