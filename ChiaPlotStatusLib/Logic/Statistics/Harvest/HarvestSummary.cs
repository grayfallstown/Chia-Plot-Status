using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatusLib.Logic.Statistics.Harvest
{
    public class HarvestSummary
    {
        public string LogFolder { get; set; }
        public int AvgEligiblePlots { get; set; }
        public int FoundProofs { get; set; }
        public double BestLookupTime { get; set; }
        public double WorstLookupTime { get; set; }
        public double AvgLookupTime { get; set; }
        public double FilterRatio { get; set; }
        public int TotalPlots { get; set; }
        public double ChallengesPerMinute { get; set; }
        public double AvgHeat { get; set; }
        public double MaxHeat { get; set; }
        public double MinHeat { get; set; }
    }
}
