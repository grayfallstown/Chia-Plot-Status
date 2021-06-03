using ChiaPlotStatus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChiaPlotStatusLib.Logic.Statistics.Harvest
{

    // partially ported from https://github.com/MrPig91/PSChiaPlotter/blob/main/PSChiaPlotter/Public/Get-ChiaHarvesterActivity.ps1
    public class HarvestParser
    {
        public static string DefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                     + Path.DirectorySeparatorChar + ".chia" + Path.DirectorySeparatorChar + "mainnet" +
                     Path.DirectorySeparatorChar + "log";

        public List<Tuple<string, HarvestSummary, List<Harvest>>?> ParseLogs(List<string> paths, double maxAllowedLookupTime, int nrOfRecentEntries)
        {
            ConcurrentBag<Tuple<string, HarvestSummary, List<Harvest>>?> results = new();
            Parallel.ForEach(paths, (path) => results.Add(ParseLogs(path, maxAllowedLookupTime, nrOfRecentEntries)));
            return results.ToList();
        }

        public Tuple<string, HarvestSummary, List<Harvest>>? ParseLogs(string path, double maxAllowedLookupTime, int nrOfRecentEntries)
        {
            var debugLogFiles = Directory.GetFiles(path, "debug.log*");

            var regex = new Regex("([0-9:.\\-T]*) harvester chia.harvester.harvester: INFO\\s*([0-9]*) " +
                 "plots were eligible for farming ([a-z0-9.]*) Found ([0-9]*) proofs. " +
                 "Time: ([0-9.]*) s. Total ([0-9]*) plots", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            ConcurrentBag<Harvest> harvestsBag = new();

            Parallel.ForEach(debugLogFiles, (file) =>
            {
                try
                {
                    // for now close after each read as I am not sure how wallets / harvesters
                    // react if they run or get restarted while we keep a shared lock on those files
                    new TailLineEmitter(file, true, (line) =>
                    {
                        if (regex.IsMatch(line))
                        {
                            var matches = regex.Matches(line)[0];
                            var harvest = new Harvest
                            {
                                LogFolder = path,
                                DateTime = DateTime.Parse(matches.Groups[1].Value),
                                ElgiblePlots = int.Parse(matches.Groups[2].Value, CultureInfo.InvariantCulture),
                                FoundProofs = int.Parse(matches.Groups[4].Value, CultureInfo.InvariantCulture),
                                LookupTime = double.Parse(matches.Groups[5].Value, CultureInfo.InvariantCulture),
                                TotalPlots = int.Parse(matches.Groups[6].Value, CultureInfo.InvariantCulture),
                                FilterRatio = double.Parse(matches.Groups[2].Value) / int.Parse(matches.Groups[6].Value, CultureInfo.InvariantCulture),
                                Heat = double.Parse(matches.Groups[5].Value, CultureInfo.InvariantCulture) == 0 ? 0 :
                                    (double.Parse(matches.Groups[5].Value, CultureInfo.InvariantCulture) / maxAllowedLookupTime),
                            };
                            harvestsBag.Add(harvest);
                        }
                    }).ReadMore();
                } catch (Exception e)
                {
                    Debug.WriteLine("ERROR: parsing harvester log " + path + " failed. skipping it");
                }
            });

            List<Harvest> harvests = harvestsBag.ToList();
            if (harvests.Count == 0)
            {
                Debug.WriteLine("No harvests found! Set log level at least to info!");
                return null;
            }
            else if (harvests.Count == 1)
            {
                Debug.WriteLine("Only found a single harvest! Need at least two!");
                return null;
            }
            harvests.Sort((a, b) => a.DateTime.CompareTo(b.DateTime));
            harvests = new(harvests.Skip(Math.Max(0, harvests.Count() - nrOfRecentEntries)));
            var first = harvests[0];
            var last = harvests[harvests.Count - 1];
            var runtimeMinutes = (last.DateTime - first.DateTime).TotalMinutes;

            var summary = new HarvestSummary
            {
                LogFolder = path,
                WorstLookupTime = harvests.Aggregate((a, b) => a.LookupTime > b.LookupTime ? a : b).LookupTime,
                BestLookupTime = harvests.Aggregate((a, b) => a.LookupTime < b.LookupTime ? a : b).LookupTime,
                AvgLookupTime = harvests.Average(a => a.LookupTime),
                AvgEligiblePlots = (int)harvests.Average(a => a.ElgiblePlots),
                TotalPlots = last.TotalPlots,
                FoundProofs = (int)harvests.Sum(a => a.FoundProofs),
                FilterRatio = harvests.Average(a => a.FilterRatio),
                ChallengesPerMinute = harvests.Count / runtimeMinutes,
                AvgHeat = harvests.Average(a => a.Heat),
                MaxHeat = harvests.Aggregate((a, b) => a.LookupTime > b.LookupTime ? a : b).LookupTime,
                MinHeat = harvests.Aggregate((a, b) => a.LookupTime < b.LookupTime ? a : b).LookupTime,
            };

            return new(path, summary, harvests);
        }
    }
}
