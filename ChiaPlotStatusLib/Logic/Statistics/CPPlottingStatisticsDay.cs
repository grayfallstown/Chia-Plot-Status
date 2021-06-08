using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus
{
    public class CPPlottingStatisticsDay
    {
        public DateTime Day { get; set; }
        public int Phase1 { get; set; } = 0;
        public int Phase2 { get; set; } = 0;
        public int Phase3 { get; set; } = 0;
        public int Phase4 { get; set; } = 0;
        public int Phase5 { get; set; } = 0;
        public int Finished { get; set; } = 0;
        public int Died { get; set; } = 0;

        public CPPlottingStatisticsDay(DateTime day)
        {
            this.Day = day;
        }
    }
}
