using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatusLib.Logic.Statistics.Harvest
{
    public class Harvest
    {
        public string LogFolder { get; set; }
        public DateTime DateTime { get; set; }
        public int ElgiblePlots { get; set; }
        public int FoundProofs { get; set; }
        public double LookupTime { get; set; }
        public double FilterRatio { get; set; }
        public int TotalPlots { get; set; }
        public double Heat { get; set; }
    }
}
