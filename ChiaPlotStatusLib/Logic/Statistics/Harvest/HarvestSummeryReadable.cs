using ChiaPlotStatusLib.Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatusLib.Logic.Statistics.Harvest
{
    public class HarvestSummeryReadable
    {
        public string LogFolder { get; set; }
        public string AvgEligiblePlots { get; set; }
        public int FoundProofs { get; set; }
        public string BestLookupTime { get; set; }
        public string WorstLookupTime { get; set; }
        public string AvgLookupTime { get; set; }
        public string FilterRatio { get; set; }
        public int TotalPlots { get; set; }
        public string ChallengesPerMinute { get; set; }
        public string AvgHeat { get; set; }
        public string MaxHeat { get; set; }
        public string MinHeat { get; set; }

        public HarvestSummeryReadable(HarvestSummary summery)
        {
            this.LogFolder = summery.LogFolder;
            this.AvgEligiblePlots = Formatter.formatDouble(summery.AvgEligiblePlots, 2, null);
            this.FoundProofs = summery.FoundProofs;
            this.BestLookupTime = Formatter.formatDouble(summery.BestLookupTime, 5, "s");
            this.WorstLookupTime = Formatter.formatDouble(summery.WorstLookupTime, 5, "s");
            this.AvgLookupTime = Formatter.formatDouble(summery.AvgLookupTime, 5, "s");
            this.FilterRatio = Formatter.formatDouble(summery.FilterRatio, 5, null);
            this.TotalPlots = summery.TotalPlots;
            this.ChallengesPerMinute = Formatter.formatDouble(summery.ChallengesPerMinute, 5, null);
            this.AvgHeat = Formatter.formatDouble(summery.AvgHeat, 5, null);
            this.MaxHeat = Formatter.formatDouble(summery.MaxHeat, 5, null);
            this.MinHeat = Formatter.formatDouble(summery.MinHeat, 5, null);
        }
    }
}
