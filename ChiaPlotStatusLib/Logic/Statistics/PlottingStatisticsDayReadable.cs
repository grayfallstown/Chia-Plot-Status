using ChiaPlotStatusLib.Logic.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus
{
    public class PlottingStatisticsDayReadable
    {
        public string Day { get; set; }
        public int Phase1 { get; set; } = 0;
        public int Phase2 { get; set; } = 0;
        public int Phase3 { get; set; } = 0;
        public int Phase4 { get; set; } = 0;
        public int Phase5 { get; set; } = 0;
        public int Finished { get; set; } = 0;
        public int Died { get; set; } = 0;

        public PlottingStatisticsDayReadable(PlottingStatisticsDay psd)
        {
            this.Day = Formatter.formatDate(psd.Day);
            this.Phase1 = psd.Phase1;
            this.Phase2 = psd.Phase2;
            this.Phase3 = psd.Phase3;
            this.Phase4 = psd.Phase4;
            this.Phase5 = psd.Phase5;
            this.Finished = psd.Finished;
            this.Died = psd.Died;
        }
    }
}
