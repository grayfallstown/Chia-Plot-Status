using ChiaPlotStatus.GUI.Models;
using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatus.Logic.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus
{
    /**
     * Core Object of this tool.
     * Knows everything, does everything.
     */
    public class ChiaPlotStatus
    {
        public Settings Settings { get; }
        private Dictionary<string, PlotLogFileParser> PlotLogFiles { get; } = new Dictionary<string, PlotLogFileParser>();
        public PlottingStatisticsIdRelevanceWeights Weights { get; } = new PlottingStatisticsIdRelevanceWeights();
        public PlottingStatisticsHolder Statistics { get; set; }

        public ChiaPlotStatus(Settings settings) {
            this.Settings = settings;
            Statistics = new PlottingStatisticsHolder(new List<PlotLog>(), Weights);
        }

        public void AddDefaultLogFolder()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                     + Path.DirectorySeparatorChar + ".chia" + Path.DirectorySeparatorChar + "mainnet" +
                     Path.DirectorySeparatorChar + "plotter";
            AddLogFolder(path);
        }

        public void AddLogFolder(string path)
        {
            Settings.LogDirectories.Add(path);
        }

        public void RemoveLogFolder(string path)
        {
            Settings.LogDirectories.Remove(path);
            // drop plotlogs from that folder
            foreach (var plotLogFile in PlotLogFiles.Values)
            {
                var folder = plotLogFile.LogFolder;
                if (string.Equals(folder, path))
                   PlotLogFiles.Remove(plotLogFile.LogFile);
                folder = folder + "\\";
                if (string.Equals(folder, path))
                    PlotLogFiles.Remove(plotLogFile.LogFile);
            }
        }

        public List<(PlotLog, PlotLogReadable)> PollPlotLogs(string sortPropertyName, bool sortAsc, string? searchString, Filter filter)
        {
            SearchForNewLogFiles();
            ConcurrentBag<PlotLog> plotLogs = ParseTheLogs();
            if (Settings.AlwaysDoFullRead == true) // nullable, so == true
                PlotLogFiles.Clear();
            HandleStatistics(plotLogs.ToList());
            List<(PlotLog, PlotLogReadable)> plusReadable = new();
            foreach (var plotLog in plotLogs)
            {
                foreach (var markOfDeath in Settings.MarksOfDeath)
                    if (markOfDeath.IsMatch(plotLog))
                        plotLog.Health = new ConfirmedDead(true);
                if (!plotLog.IsRunning())
                    plotLog.RunTimeSeconds = 0;
                else if (plotLog.StartDate != null)
                    plotLog.RunTimeSeconds = (int)((TimeSpan)(DateTime.Now - plotLog.StartDate)).TotalSeconds;
                plusReadable.Add((plotLog, new PlotLogReadable(plotLog)));
            }
            List<(PlotLog, PlotLogReadable)> result = Filter(searchString, filter, plusReadable);
            SortPlotLogs(sortPropertyName, sortAsc, result);
            return result;
        }

        private void SearchForNewLogFiles()
        {
            List<string> directoriesToDrop = new();
            foreach (var directory in Settings.LogDirectories)
            {
                if (Directory.Exists(directory))
                {
                    // when a logfolder gets deleted that was added to Settings Chia Plot Status failed to start
                    foreach (var filePath in Directory.GetFiles(directory))
                    {
                        if (!PlotLogFiles.ContainsKey(filePath) && LooksLikeAPlotLog(filePath))
                        {
                            PlotLogFiles[filePath] = new PlotLogFileParser(filePath, Settings.AlwaysDoFullRead == true);
                        }
                    }
                } else
                {
                    directoriesToDrop.Add(directory);
                }
            }
            foreach (var directory in directoriesToDrop)
            {
                Settings.LogDirectories.Remove(directory);
            }
        }

        private ConcurrentBag<PlotLog> ParseTheLogs()
        {
            ConcurrentBag<PlotLog> plotLogs = new ConcurrentBag<PlotLog>();
            Parallel.ForEach(PlotLogFiles.Values, (plotLogFile) =>
            {
                foreach (var plotLog in plotLogFile.ParsePlotLog())
                {
                    plotLogs.Add(plotLog);
                }
            });
            return plotLogs;
        }

        private List<(PlotLog, PlotLogReadable)> Filter(string? searchString, Filter filter, List<(PlotLog, PlotLogReadable)> plotLogs)
        {
            List<(PlotLog, PlotLogReadable)> searchResults = SearchFilter.Search<PlotLog, PlotLogReadable>(searchString, plotLogs);
            List<(PlotLog, PlotLogReadable)> filterResults = new();
            foreach (var tuple in searchResults)
            {
                if (filter.HideFinished && tuple.Item1.CurrentPhase == 6)
                    continue;
                switch (tuple.Item1.Health)
                {
                    case Healthy:
                        if (!filter.HideHealthy) filterResults.Add(tuple);
                        break;
                    case TempError:
                        // well, you have to take action here.
                        filterResults.Add(tuple);
                        break;
                    case Concerning:
                        // would one want to hide those?
                        filterResults.Add(tuple);
                        break;
                    case PossiblyDead:
                        if (!filter.HidePossiblyDead) filterResults.Add(tuple);
                        break;
                    case ConfirmedDead:
                        if (!filter.HideConfirmedDead) filterResults.Add(tuple);
                        break;
                }
            }
            return filterResults;
        }

        private static void SortPlotLogs(string propertyName, bool sortAsc, List<(PlotLog, PlotLogReadable)> plotLogs)
        {
            Sorter.Sort(propertyName, sortAsc, plotLogs);
        }

        private void HandleStatistics(List<PlotLog> plotLogs)
        {
            Statistics = new PlottingStatisticsHolder(plotLogs, Weights);
            //Parallel.ForEach(result, (plotLog) =>
            foreach (var plotLog in plotLogs)
            {
                PlottingStatistics stats = Statistics.GetMostRelevantStatistics(plotLog);
                plotLog.UpdateEta(stats);
                plotLog.UpdateHealth(stats);
            }
        }

        private bool LooksLikeAPlotLog(string file)
        {
            byte[] buffer = new byte[4096];
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var bytes_read = fs.Read(buffer, 0, buffer.Length);
                    fs.Close();
                    if (bytes_read > 63)
                    {
                        string asString = GetEncoding(file).GetString(buffer);
                        bool hasNewLine = asString.Contains("\n");
                        bool hasStartingSentence = asString.Contains("Starting plotting progress into temporary dirs");
                        bool hasHarvesterDuplicateWarning = asString.Contains("already exists for harvester, please remove it manually");
                        if (hasNewLine && (hasStartingSentence || hasHarvesterDuplicateWarning))
                            return true;
                    }
                }
            } catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
            Debug.WriteLine("File " + file + " was detected as a non plotlog file and will be ignored for now");
            return false;
        }

        private static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            // if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32; //UTF-32LE
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return new UTF32Encoding(true, true);  //UTF-32BE

            // We actually have no idea what the encoding is if we reach this point, so
            // you may wish to return null instead of defaulting to ASCII
            return Encoding.ASCII;
        }

    }

}
